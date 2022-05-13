import json
import re
from pika import PlainCredentials
from masstransitpython import RabbitMQConfiguration
from masstransitpython import RabbitMQReceiver, RabbitMQSender
from json import JSONEncoder
import dock_manager
import RPi.GPIO as gpio
import bike_reader
from multiprocessing import Process
import rental_api


lamp_one = 37
lamp_two = 11

RABBITMQ_USERNAME = 'guest'
RABBITMQ_PASSWORD = 'guest'
RABBITMQ_HOST = '192.168.1.199'
RABBITMQ_PORT = 5672
RABBITMQ_VIRTUAL_HOST = '/'

BIKE_UNLOCKED_EXCHANGE = 'Common.Models.Events.Rental:IBikeUnlocked'
BIKE_LOCKED_EXCHANGE = 'Common.Models.Events.Rental:IBikeLocked'

credentials = PlainCredentials(RABBITMQ_USERNAME, RABBITMQ_PASSWORD)
conf = RabbitMQConfiguration(credentials,
                             queue='PythonServiceQueue',
                             host=RABBITMQ_HOST,
                             port=RABBITMQ_PORT,
                             virtual_host=RABBITMQ_VIRTUAL_HOST)


def send_message(body, correlation_id, rental, exchange):
    with RabbitMQSender(conf) as sender:
        sender.set_exchange(exchange)
        encoded_msg = JSONEncoder().encode({
            "correlationId": correlation_id,
            "rental": rental
        })

        response = sender.create_masstransit_response(
            json.loads(encoded_msg), body)
        sender.publish(message=response)


def on_bike_unlock(ch, method, properties, body):
    msg = json.loads(body.decode())
    dock_id = msg['message']['rental']['originDockId']
    print(" [X] Received unlock command for dock with id: " + dock_id)
    rental = msg['message']['rental']
    correlation_id = msg['message']['correlationId']

    bike_id = dock_manager.get_dock_by_id(dock_id)['bikeId']
    rental['status'] = 6
    rental['bikeId'] = bike_id

    dock_manager.unlock_dock(dock_id)

    msg['bikeId'] = bike_id

    #CardRead.write(msg)
    rental_api.save_rental_data(save_rental_data)

    print(' [X] Notifying service of port unlocked')
    send_message(msg, correlation_id, rental, BIKE_UNLOCKED_EXCHANGE)


def on_bike_lock(data):
    (id, text, port) = data
    dock_port = port
    bike_id = id
    
    dock = dock_manager.lock_dock(dock_port, bike_id)

    rental_data = rental_api.get_rental_data_by_bike_id(bike_id)
    rental_data['message']['destinationDockId'] = dock['id']

    print(' [X] Notifying service of port locked')
    send_message(rental_data, rental_data['message']['correlationId'], rental_data['message']['rental'], BIKE_LOCKED_EXCHANGE)


def start_receiver():
    # define receiver
    receiver = RabbitMQReceiver(conf, 'bike-unlock')
    receiver.add_on_message_callback(on_bike_unlock)
    receiver.start_consuming()

receiver = Process(target=start_receiver)
receiver.start()

reader = bike_reader.start_readers()
print('reading')
for data in reader:
    (id, text, port) = data
    print('[' + str(id) + '] ' + text)
    on_bike_lock(data)
