from SimpleMFRC522 import SimpleMFRC522
import RPi.GPIO as GPIO


def reader_port_one():
    reader_one = SimpleMFRC522(bus=0)

    try:
        print('Reading on port one...')
        while True:
            id, text = reader_one.read()
            print(id)
            print(text)
    finally:
        GPIO.cleanup()


def reader_port_two():
    reader_two = SimpleMFRC522(bus=1)

    try:
        print('Reading on port two...')
        while True:
            id, text = reader_two.read()
            print(id)
            print(text)
    finally:
        GPIO.cleanup()
