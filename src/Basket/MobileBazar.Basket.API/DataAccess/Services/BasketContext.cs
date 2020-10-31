using MobileBazar.Basket.API.DataAccess.Repositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBazar.Basket.API.DataAccess.Services
{
    public class BasketContext : IBasketContext
    {
        private readonly ConnectionMultiplexer _redisConnection;

        public BasketContext(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
            Redis = redisConnection.GetDatabase();
        }

        public IDatabase Redis { get; }
    }
}
