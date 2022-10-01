#!/usr/bin/env python
import json
import relay_controller
from paho.mqtt import client as mqtt_client
import uuid

port = 31883
broker = "192.168.1.196"
client_id = f'dock-internal-service-switch-{uuid.uuid4()}'
topic = 'dock-state-change'

DOCK_PORTS = (11, 37)

docks = {
    '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf': 1,
    '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c': 2
}

relay_controller.init_relay(DOCK_PORTS)


def set_relay(dock_id, state):
    if (state == 1):
        relay_controller.relay_on(docks[dock_id])
    elif (state == 2):
        relay_controller.relay_off(docks[dock_id])
    else:
        print(' [x] Invalid action')


def restore_states():
    for dock_id in docks.keys():
        try:
            f = open(dock_id, "r")
            state = int(f.read())
            set_relay(dock_id, state)
        except IOError:
            print(f'Dock with id {dock_id} doesnt have a state to restore, ignoring...')

def save_state(dock_id, state):
    file = open(dock_id, 'w')
    file.write(str(state))
    file.close()


def handle_dock_state_change_request(msg):
    dock_id = msg['DockId']
    state = msg['State']

    print(' [x] Dock id: %r' % dock_id)
    print(' [x] State: %r' % state)

    global relay_controller
    if (dock_id in docks.keys()):
        set_relay(dock_id, state)
        save_state(dock_id, state)
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
    restore_states()
    client = connect_mqtt()
    subscribe(client)
    client.loop_forever()


if __name__ == '__main__':
    run()
