from paho.mqtt import client as mqtt_client

port = 31883
broker = "192.168.1.199"
client_id = 'dock-internal-service-reader'


class Client():
    def __init__(self):
        self.client = connect_mqtt()

    def close(self):
        self.connection.close()

    def send_message(self, topic, message):
        result = self.client.publish(topic, message)
        status = result[0]
        if status == 0:
            print(f"Send `{message}` to topic `{topic}`")
        else:
            print(f"Failed to send message to topic {topic}")


def connect_mqtt():
    def on_connect(client, userdata, flags, rc):
        if rc == 0:
            print("Connected to MQTT Broker!")
        else:
            print("Failed to connect, return code %d\n", rc)

    client = mqtt_client.Client(client_id, clean_session=False)
    client.on_connect = on_connect
    client.connect(broker, port)

    client.loop_start()
    
    return client
