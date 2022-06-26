from tinydb import TinyDB, Query

db = TinyDB('dock.json')

db.insert({'id': '3fa85f64-5717-4562-b3fc-2c963f66afa6', 'port': 11, 'bikeId': '3fa85f64-5717-4562-b3fc-2c963f66afa6'})

Dock = Query()
dock = db.search(Dock.id == '3fa85f64-5717-4562-b3fc-2c963f66afa6')[0]
print(dock)