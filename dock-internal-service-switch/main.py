#!/usr/bin/env python
import pika
import json
import relay_controller


DOCK_PORTS = (11, 37)

dock_one_id = '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf'
dock_two_id = '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c'

relay_controller.init_relay(DOCK_PORTS)

def callback(ch, method, properties, body):
    msg = json.loads(body.decode())
    dock_id = msg['dockId']
    action = msg['action']

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
    connection = pika.BlockingConnection(pika.ConnectionParameters('192.168.1.199', 5672, '/', pika.PlainCredentials('guest', 'guest')))
    channel = connection.channel()

    channel.exchange_declare(exchange='DockStateChangeRequest', exchange_type='fanout')

    result = channel.queue_declare(queue='', exclusive=True)
    queue_name = result.method.queue

    channel.queue_bind(exchange='DockStateChangeRequest', queue=queue_name)

    print(' [*] Waiting for logs. To exit press CTRL+C')

    channel.basic_consume(
        queue=queue_name, on_message_callback=callback, auto_ack=True)

    channel.start_consuming()