using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBazar.Basket.API.DataAccess.Repositories
{
    public interface IBasketContext
    {
        IDatabase Redis { get; }
    }
}
