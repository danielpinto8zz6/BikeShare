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

BIKE_UNLOCKED_EXCHANGE = 'Common.Models.Events.Rental:IBikeUnlocked'


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


if __name__ == '__main__':
    lock = Lock()

    open_ports = dock_manager.set_docks_state()

    # define receiver
    receiver = RabbitMQReceiver(conf, 'bike-unlock')
    receiver.add_on_message_callback(on_bike_unlock)
    receiver.start_consuming()
