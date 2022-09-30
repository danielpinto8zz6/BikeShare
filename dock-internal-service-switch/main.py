#!/usr/bin/env python
import json
import relay_controller
from paho.mqtt import client as mqtt_client
import uuid

port = 31883
broker = "192.168.1.199"
client_id = f'dock-internal-service-switch-{uuid.uuid4()}'
topic = 'dock-state-change'

DOCK_PORTS = (11, 37)

dock_one_id = '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf'
dock_two_id = '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c'

relay_controller.init_relay(DOCK_PORTS)


def handle_dock_state_change_request(msg):
    dock_id = msg['DockId']
    action = msg['Action']

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


def connect_mqtt() -> mqtt_client:
    def on_connect(client, userdata, flags, rc):
        if rc == 0:
            print("Connected to MQTT Broker!")
        else:
            print("Failed to connect, return code %d\n", rc)

    client = mqtt_client.Client(client_id)
    client.on_connect = on_connect
    client.connect(broker, port)
    return client


def subscribe(client: mqtt_client):
    def on_message(client, userdata, msg):
        message = json.loads(msg.payload.decode())

        print(f"Received `{message}` from `{msg.topic}` topic")
        handle_dock_state_change_request(message)
        
    client.subscribe(topic)
    client.on_message = on_message


def run():
    client = connect_mqtt()
    subscribe(client)
    client.loop_forever()

if __name__ == '__main__':
    run()