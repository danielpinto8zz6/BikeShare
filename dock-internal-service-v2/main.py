import RPi.GPIO as gpio
import bike_reader
from multiprocessing import Process
import requests


def send_bike_lock_request(dock_id, bike_id):
    json_data = {
        'dockId': dock_id,
        'bikeId': bike_id,
    }

    # TODO: request is insecure
    response = requests.post('http://192.168.1.199:8099/api/Docks/attach-bike', json=json_data)
    print(response)

def start_reader():
    reader = bike_reader.start_readers()
    print(' [X] Reading...')
    for data in reader:
        print(' [X] Read data: ' + data)
        send_bike_lock_request(data['dockId'], data['data'])


if __name__ == '__main__':
    try:
        reader = Process(target=start_reader)
        reader.start()

    except KeyboardInterrupt:
        print('Cleaning gpio before exit.')
        gpio.cleanup()
