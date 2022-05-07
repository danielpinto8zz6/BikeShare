import json
from pika import PlainCredentials
from masstransitpython import RabbitMQConfiguration
from masstransitpython import RabbitMQReceiver, RabbitMQSender
from json import JSONEncoder
import dock_manager
import RPi.GPIO as gpio



port_one = 11
port_two = 12

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

    rental['status'] = 6
    rental['bikeId'] = dock_manager.get_dock_by_id(dock_id)['bikeId']

    dock_manager.unlock_dock(dock_id)

    CardRead.write(msg)

    print(' [X] Notifying service of port unlocked')
    send_message(msg, correlation_id, rental, BIKE_UNLOCKED_EXCHANGE)


def on_bike_lock(dock_port, bike_id, msg):
    dock_manager.lock_dock(dock_port, bike_id)

    print(' [X] Notifying service of port locked')
    send_message(msg, msg['message']['correlationId'], msg['message']['rental'], BIKE_LOCKED_EXCHANGE)


# define receiver
receiver = RabbitMQReceiver(conf, 'bike-unlock')
receiver.add_on_message_callback(on_bike_unlock)
receiver.start_consuming()


print('Card Scanning')
print('for Cancel Press ctrl+c')
try:
    bike_port = 11
    while True:
        bike_id, msg = CardRead.read()
        print(msg)
        print(bike_id)
        if id == 465199183884:
            print(bike_id)
        if id == 13796710267:
            print(bike_id)
        on_bike_lock(bike_port, bike_id, msg)

except KeyboardInterrupt:
    gpio.cleanup()
