import etcd3
import json


def persist_message(bike_id, message):
    client = etcd3.client(host='192.168.1.199', port=2379)
    client.put(bike_id, json.dumps(message))

def get_message(bike_id):
    client = etcd3.client(host='192.168.1.199', port=2379)
    (value, metadata) = client.get(bike_id.strip())
    return json.loads(value)
