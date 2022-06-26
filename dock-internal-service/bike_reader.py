from ast import For, arg
import RPi.GPIO as GPIO
from SimpleMFRC522 import SimpleMFRC522
from multiprocessing import Process
from iterable_queue import IterableQueue


reader_one = SimpleMFRC522(bus=0)
reader_two = SimpleMFRC522(bus=1)


def consume(queue, port):
    try:
            reader = reader_one if port == 1 else reader_two
            while True:
                    id, data = reader.read()
                    data = {
                        "id": id,
                        "data": data,
                        "port": port
                    }
                    queue.put(data)
    finally:
            GPIO.cleanup()


def start_readers():
    iq = IterableQueue()

    ports = [1, 2]
    procs = []
    for port in ports:
        proc = Process(target=consume, args=(iq.get_producer(), port))
        proc.start()
        procs.append(proc)

    consumer_endpoint = iq.get_consumer()

    iq.close()

    return consumer_endpoint