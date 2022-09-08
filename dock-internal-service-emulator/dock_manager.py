from tinydb import TinyDB, Query


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

    bike_id = dock['bikeId']
    
    if bike_id is None:
        raise ValueError('Dock has not bike attached')

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
            open_ports.append(port)

    return open_ports


def is_port_open(lock, port):
    dock = get_dock_by_port(lock, port)
    return dock['bikeId'] is None