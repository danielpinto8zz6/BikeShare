from tinydb import TinyDB, Query
import relay_controller

DOCK_PORTS = (11, 12)

relay_controller.init_relay(DOCK_PORTS)

db = TinyDB('dock.json')

def get_dock_by_id(dock_id):
    Dock = Query()
    dock = db.search(Dock.id == dock_id)[0]

def get_dock_by_port(port):    
    Dock = Query()
    dock = db.search(Dock.port == port)[0]
    
    return dock

def unlock_dock(dock_id):
    dock = get_dock_by_id(dock_id)
    dock_port = dock['port']

    if dock['bikeId'] is None:
        raise ValueError('Dock has not bike attached')

    print('Unlocking dock on port ' + dock_port)

    relay_controller.relay_off(dock_port)

    dock['bikeId'] = None
    db.update(dock)

def lock_dock(dock_port, bike_id):
    dock = get_dock_by_port(dock_port)

    if dock['bikeId'] is not None:
        raise ValueError('Dock already occupied')

    print('Locking dock on port ' + dock_port)

    relay_controller.relay_on(dock_port)

    dock['bikeId'] = bike_id
    db.update(dock)

    return dock
