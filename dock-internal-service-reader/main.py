import RPi.GPIO as gpio
from multiprocessing import Process
import requests
from SimpleMFRC522 import SimpleMFRC522
import time
import sys

def send_bike_lock_request(dock_id, bike_id):
    json_data = {
        'dockId': dock_id,
        'bikeId': bike_id,
    }

    # TODO: request is insecure
    response = requests.post('http://192.168.1.199:8099/api/Docks/attach-bike', json=json_data)
    print(response)

if __name__ == '__main__':
    
    #dock_id = '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf'
    #dock_two_id = '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c'

    dock_id = str(sys.argv[1]).strip()
    dock_port = int(sys.argv[2])

    print(dock_id)
    print(dock_port)

    reader = SimpleMFRC522(bus=dock_port)

    try:
        while True:
            id, data = reader.read()
            print(data)
            # sleep 5 sec to avoid multiple readings
            time.sleep(5)


    except KeyboardInterrupt:
        print('Cleaning gpio before exit.')
        gpio.cleanup()
