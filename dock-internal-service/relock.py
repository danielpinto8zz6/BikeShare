#!/usr/bin/env python

from mfrc522 import SimpleMFRC522

reader = SimpleMFRC522()

try:
        id, dock_id = reader.read()
        print(id)
        print(dock_id)
finally:
        GPIO.cleanup()