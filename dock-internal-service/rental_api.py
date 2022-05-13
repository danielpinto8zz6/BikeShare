from tinydb import TinyDB, Query


db = TinyDB('rental.json')

get_rental_data_by_bike_id(bike_id):
    Rental = Query()
    rental = db.search(Rental.bikeId == bike_id)[0]

save_rental_data(rental_data):
    db.save(rental_data)