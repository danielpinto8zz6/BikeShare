from tinydb import TinyDB, Query

db = TinyDB('dock.json')

db.update({'id': '3fa85f64-5717-4562-b3fc-2c963f66afa6', 'port': None})