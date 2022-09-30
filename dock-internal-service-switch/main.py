#!/usr/bin/env python
import re
import pika
import json
import relay_controller
from masstransitpython import RabbitMQConfiguration
from masstransitpython import RabbitMQReceiver, RabbitMQSender
from pika import PlainCredentials


RABBITMQ_USERNAME = 'guest'
RABBITMQ_PASSWORD = 'guest'
RABBITMQ_HOST = '192.168.1.199'
RABBITMQ_PORT = 5672
RABBITMQ_VIRTUAL_HOST = '/'

EXCHANGE_NAME = 'Common.Models.Dtos.DockStateChangeRequest'
QUEUE_NAME = 'dock-internal-service-switch'

credentials = PlainCredentials(RABBITMQ_USERNAME, RABBITMQ_PASSWORD)
conf = RabbitMQConfiguration(credentials,
                             queue=QUEUE_NAME,
                             host=RABBITMQ_HOST,
                             port=RABBITMQ_PORT,
                             virtual_host=RABBITMQ_VIRTUAL_HOST)

DOCK_PORTS = (11, 37)

dock_one_id = '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf'
dock_two_id = '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c'

relay_controller.init_relay(DOCK_PORTS)


def callback(ch, method, properties, body):
    msg = json.loads(body.decode())
    print(msg)
    dock_id = msg['message']['dockId']
    action = msg['message']['action']

    print(' [x] Dock id: %r' % dock_id)
    print(' [x] Action: %r' % action)

    global relay_controller
    if (dock_id == dock_one_id):
        if (action == 1):
            relay_controller.relay_on(1)
        elif (action == 2):
            relay_controller.relay_off(1)
        else:
            print(' [x] Invalid action')
    elif (dock_id == dock_two_id):
        if (action == 1):
            relay_controller.relay_on(2)
        elif (action == 2):
            relay_controller.relay_off(2)
    else:
        print(' [x] Dock id not recognized')


if __name__ == '__main__':
    # define receiver
    receiver = RabbitMQReceiver(conf, EXCHANGE_NAME)
    receiver.add_on_message_callback(callback)
    receiver.start_consuming()
