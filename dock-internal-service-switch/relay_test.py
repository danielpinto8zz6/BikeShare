#!/usr/bin/env python
import relay_controller
import time

DOCK_PORTS = (11, 37)

relay_controller.init_relay(DOCK_PORTS)

relay_controller.relay_on(1)
relay_controller.relay_on(2)
time.sleep(3)
relay_controller.relay_off(1)
relay_controller.relay_off(2)