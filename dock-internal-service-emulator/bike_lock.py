import json
from multiprocessing import Lock
from pika import PlainCredentials
from masstransitpython import RabbitMQConfiguration
from masstransitpython import RabbitMQReceiver, RabbitMQSender
from json import JSONEncoder
import dock_manager
import message_cache
from datetime import datetime


RABBITMQ_USERNAME = 'guest'
RABBITMQ_PASSWORD = 'guest'
RABBITMQ_HOST = '192.168.1.199'
RABBITMQ_PORT = 5672
RABBITMQ_VIRTUAL_HOST = '/'

BIKE_ATTACHED_EXCHANGE = 'Common.Models.Events.Rental:IBikeAttached'


credentials = PlainCredentials(RABBITMQ_USERNAME, RABBITMQ_PASSWORD)
conf = RabbitMQConfiguration(credentials,
                             queue='IBikeUnlocked',
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

    send_message(msg, correlation_id, rental, BIKE_ATTACHED_EXCHANGE)

if __name__ == '__main__':
    lock = Lock()

    bike_id = input("Enter bike id to lock: ")
    port = input("Enter the port")

    data = {
        'data': bike_id,
        'port': int(port)
    }

    on_bike_lock(data, lock)
