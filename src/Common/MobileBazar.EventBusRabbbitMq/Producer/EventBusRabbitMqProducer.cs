using MobileBazar.EventBusRabbbitMq.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MobileBazar.EventBusRabbbitMq.Producer
{
    public class EventBusRabbitMqProducer
    {
        private readonly IRabbitMqConnection _connection;

        public EventBusRabbitMqProducer(IRabbitMqConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// create queue
        /// create basicpublish
        /// create basic aks
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="publishModel"></param>
        public void PublishBasketCheckout(string queueName, BasketCheckOutEvent publishModel)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                var message = JsonConvert.SerializeObject(publishModel);
                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     mandatory: true,
                                     basicProperties: properties,
                                     body: body);

                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                 {
                     Console.WriteLine("Sent RabbitMq");
                 };
                channel.ConfirmSelect();
            }
        }

    }
}
