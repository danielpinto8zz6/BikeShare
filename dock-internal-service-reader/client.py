import pika


class Client():
    def __init__(self):
        self.connection = pika.BlockingConnection(pika.ConnectionParameters(
            '192.168.1.199', 5672, '/', pika.PlainCredentials('guest', 'guest')))
        self.channel = self.connection.channel()

    def close(self):
        self.connection.close()

    def send_message(self, endpoint, key, message):
        self.channel.basic_publish(
            exchange=endpoint, routing_key=key, body=message)
