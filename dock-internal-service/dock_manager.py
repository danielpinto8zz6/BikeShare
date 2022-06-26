from tinydb import TinyDB, Query
import relay_controller

DOCK_PORTS = (11, 37)

relay_controller.init_relay(DOCK_PORTS)

def get_dock_by_id(lock, dock_id):
    with lock:
        db = TinyDB('dock.json')
        Dock = Query()
        dock = db.search(Dock.id == dock_id)[0]

    return dock

def get_dock_by_port(lock, port):
    with lock:    
        db = TinyDB('dock.json')
        Dock = Query()
        dock = db.search(Dock.port == port)[0]
        
        return dock

def unlock_dock(lock, dock_id):
    dock = get_dock_by_id(lock, dock_id)
    dock_port = dock['port']

    bike_id = dock['bikeId']
    
    if bike_id is None:
        raise ValueError('Dock has not bike attached')

    relay_controller.relay_off(dock_port)

    dock['bikeId'] = None
        
    with lock: 
        db = TinyDB('dock.json')
        Dock = Query()
        db.update(dock, Dock.id == dock_id)

        return bike_id

def lock_dock(lock, dock_port, bike_id):
    dock = get_dock_by_port(lock, dock_port)

    if dock['bikeId'] is not None:
        raise ValueError('Dock already occupied')

    relay_controller.relay_on(dock_port)

    dock['bikeId'] = bike_id

    with lock:  
        db = TinyDB('dock.json')  
        Dock = Query()
        db.update(dock, Dock.port == dock_port)

    return dock

def set_docks_state():
    db = TinyDB('dock.json')
    docks = db.all()
    open_ports = []

    for dock in docks:
        bike_id = dock['bikeId']
        port = dock['port']

        if bike_id is None:
            relay_controller.relay_off(port)
            open_ports.append(port)
        else:
            relay_controller.relay_on(port)
    return open_ports


def is_port_open(lock, port):
    dock = get_dock_by_port(lock, port)
    return dock['bikeId'] is None