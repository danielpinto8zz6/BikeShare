import RPi.GPIO as gpio
from multiprocessing import Process
import requests
from SimpleMFRC522 import SimpleMFRC522
import time


reader_one = SimpleMFRC522(bus=0)
reader_two = SimpleMFRC522(bus=1)

dock_one_id = '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf'
dock_two_id = '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c'


def send_bike_lock_request(dock_id, bike_id):
    json_data = {
        'dockId': dock_id,
        'bikeId': bike_id,
    }

    # TODO: request is insecure
    response = requests.post('http://192.168.1.199:8099/api/Docks/attach-bike', json=json_data)
    print(response)


def consume(dock_id):
    try:
        reader = reader_one if dock_id == dock_one_id else reader_two
        while True:
                id, data = reader.read()
                print(data)
                data = {
                    "id": id,
                    "data": data.strip(),
                    "dockId": dock_id
                }
                #queue.put(data)
                # Sleep 5 sec to avoid multiple reads
                time.sleep(5)
    finally:
            GPIO.cleanup()


if __name__ == '__main__':
    try:
        dock_ids = [dock_one_id, dock_two_id]
        for dock_id in dock_ids:
            proc = Process(target=consume, args=(dock_id,))
            proc.start()

    except KeyboardInterrupt:
        print('Cleaning gpio before exit.')
        gpio.cleanup()
