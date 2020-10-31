using MobileBazar.Catalog.API.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBazar.Catalog.API.DataAccess.Repositories
{
    public interface ICatalogContext
    {
        IMongoCollection<Product> Products { get; }
    }
}
