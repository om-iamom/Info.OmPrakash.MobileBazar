using RabbitMQ.Client;
using System;

namespace MobileBazar.EventBusRabbbitMq
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel(); // create Queue operation
    }
}
