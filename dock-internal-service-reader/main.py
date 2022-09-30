from calendar import c
from pydoc import cli
from NFC import NFC
from multiprocessing import Process
from SimpleMFRC522 import SimpleMFRC522
import time
import RPi.GPIO as gpio
from client import Client
import json


def send_bike_lock_request(client, dock_id, bike_id):
    request = {
        'message': {
            'dockId': dock_id,
            'bikeId': bike_id,
        },
        'messageType': [
            "urn:message:DockService.Models.Dtos:BikeLockRequest"
        ]
    }

    client.send_message('BikeLockRequest', dock_id, json.dumps(request))
    print(json.dumps(request))


if __name__ == '__main__':
    docks = {
        '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf': 22,
        '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c': 18
    }

    try:
        client = Client()
        nfc = NFC()

        for dock_id in docks:
            nfc.addBoard(dock_id, docks[dock_id])

        while True:
            for dock_id in docks:
                data = nfc.read(dock_id)
                if data is not None:
                    print(data)
                    send_bike_lock_request(client, dock_id, data.strip())
                time.sleep(0.2)

    except KeyboardInterrupt:
        print('Cleaning gpio before exit.')
        client.close()
        GPIO.cleanup()
