from flask import Flask, request, jsonify
from tinydb import TinyDB, Query

db = TinyDB('db.json')

app = Flask(__name__)


@app.route('/api/docks', methods=['GET'])
def get_docks():
    return jsonify(db.all())


@app.route('/api/docks', methods=['POST'])
def save():
    # Bad request
    if not request.headers['Content-Type'] == 'application/json':
        return jsonify(res='failure'), 400

    data = request.json
    db.insert(data)
    return jsonify(res='success', **data)


if __name__ ==  '__main__':
    app.run(host='0.0.0.0', port=8080)  