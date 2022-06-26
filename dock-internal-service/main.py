import json
import re
from pika import PlainCredentials
from masstransitpython import RabbitMQConfiguration
from masstransitpython import RabbitMQReceiver, RabbitMQSender
from json import JSONEncoder
import dock_manager
import RPi.GPIO as gpio
import bike_reader
from multiprocessing import Process, Manager, Lock
import message_cache
from datetime import datetime

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
    global lock
    msg = json.loads(body.decode())
    dock_id = msg['message']['rental']['originDockId']
    print(" [X] Trying to unlock bike on dock: " + dock_id)
    rental = msg['message']['rental']
    correlation_id = msg['message']['correlationId']

    bike_id = dock_manager.unlock_dock(lock, dock_id)

    print(' [X] Bike: ' + str(bike_id) + ' unlocked')

    rental['status'] = 6
    rental['startDate'] = datetime.now().strftime('%Y-%m-%dT%H:%M:%SZ')

    print(' [X] Persisting in cache')

    message_cache.persist_message(bike_id, msg)

    print(' [X] Notifying service')
    print('')
    send_message(msg, correlation_id, rental, BIKE_UNLOCKED_EXCHANGE)

def on_bike_lock(data, lock):
    dock_port = data['port']
    bike_id = data['data'].strip()
    
    print(' [X] Locking bike: ' + bike_id + ' on port ' + str(dock_port))

    dock = dock_manager.lock_dock(lock, dock_port, bike_id)

    print(' [X] Bike locked, notifying service...')

    msg = message_cache.get_message(dock['bikeId'])

    correlation_id = msg['message']['correlationId']
    
    rental = msg['message']['rental']
    rental['destinationDockId'] = dock['id']
    rental['endDate'] = datetime.now().strftime('%Y-%m-%dT%H:%M:%SZ')

    print(rental)

    send_message(msg, correlation_id, rental, BIKE_LOCKED_EXCHANGE)


def start_reader(lock):
    reader = bike_reader.start_readers()
    print(' [X] Reading...')
    for data in reader:
        port = data['port']
        print(' [X] Bike trying to attach to port: ' + str(port))
        is_port_open = dock_manager.is_port_open(lock, port)
        if is_port_open:
            print(' [X] Port is open, attaching...')
            on_bike_lock(data, lock)
            print('')
        else:
            print(' [X] Port: ' + str(port) + ' occupied, ignoring')
            print('')


if __name__ == '__main__':
    try:
        lock = Lock()
        open_ports = dock_manager.set_docks_state()

        print(" [X] Open ports: ", open_ports)

        reader = Process(target=start_reader, args=(lock, ))
        reader.start()


        # define receiver
        receiver = RabbitMQReceiver(conf, 'bike-unlock')
        receiver.add_on_message_callback(on_bike_unlock)
        receiver.start_consuming()
    except KeyboardInterrupt:
        print('Cleaning gpio before exit.')
        gpio.cleanup()
