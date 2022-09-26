from ast import For, arg
import RPi.GPIO as GPIO
from SimpleMFRC522 import SimpleMFRC522
from multiprocessing import Process
from iterable_queue import IterableQueue


reader_one = SimpleMFRC522(bus=0)
reader_two = SimpleMFRC522(bus=1)

dock_one_id = '53ce1a33-2b6a-4a3a-a4d3-95555ab60edf'
dock_two_id = '7b28d2e9-8c4b-4b2c-b695-ab5b4215c98c'


def consume(queue, dock_id):
    try:
            reader = reader_one if dock_id == dock_one_id else reader_two
            while True:
                    id, data = reader.read()
                    data = {
                        "id": id,
                        "data": data,
                        "dockId": dock_id
                    }
                    queue.put(data)
    finally:
            GPIO.cleanup()


def start_readers():
    iq = IterableQueue()

    dock_ids = [dock_one_id, dock_two_id]
    procs = []
    for dock_id in dock_ids:
        proc = Process(target=consume, args=(iq.get_producer(), dock_id))
        proc.start()
        procs.append(proc)

    consumer_endpoint = iq.get_consumer()

    iq.close()

    return consumer_endpoint