#!/usr/bin/env python

from ast import arg
import RPi.GPIO as GPIO
from SimpleMFRC522 import SimpleMFRC522
from multiprocessing import Process
from iterable_queue import IterableQueue


reader_one = SimpleMFRC522(bus=0)
reader_two = SimpleMFRC522(bus=1)

def reader_port_one(queue):
    try:
            while True:
                    id, text = reader_one.read()
                    #print('(1) [' + str(id) + '] ' + text)
                    queue.put((id, text, 1))
                    #queue.close()
    finally:
            GPIO.cleanup()


def reader_port_two(queue):
    try:
            while True:
                    id, text = reader_two.read()
                    #print('(2) [' + str(id) + '] ' + text)
                    queue.put((id, text, 2))
                    #queue.close()
    finally:
            GPIO.cleanup()


def consumer_func(queue):
    print('consuming')
    for item in queue:
        print(item)


def start_readers():
    iq = IterableQueue()

    proc1 = Process(target=reader_port_one, args=(iq.get_producer(),))
    proc1.start()

    proc2 = Process(target=reader_port_two, args=(iq.get_producer(),))
    proc2.start()

    consumer_endpoint = iq.get_consumer()

    iq.close()

    return consumer_endpoint
    
if __name__ == '__main__':
    iq = IterableQueue()

    proc1 = Process(target=reader_port_one, args=(iq.get_producer(),))
    proc1.start()

    proc2 = Process(target=reader_port_two, args=(iq.get_producer(),))
    proc2.start()

    consumer = Process(target=consumer_func, args=(iq.get_consumer(),))
    consumer.start()

    iq.close()

    proc1.join()
    proc2.join()
    consumer.join()