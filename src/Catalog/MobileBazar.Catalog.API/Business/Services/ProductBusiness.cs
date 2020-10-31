using MobileBazar.Catalog.API.Business.Repositories;
using MobileBazar.Catalog.API.DataAccess.Repositories;
using MobileBazar.Catalog.API.Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBazar.Catalog.API.Business.Services
{
    public class ProductBusiness : IProductBusiness
    {
        public readonly ICatalogContext _catalogContext;

        public ProductBusiness(ICatalogContext catalogContext)
        {
            _catalogContext = catalogContext??throw new ArgumentNullException(nameof(catalogContext));
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _catalogContext
                            .Products
                            .Find(p => true)
                            .ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _catalogContext
                                .Products
                                .Find(p => p.Id == id)
                                .FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);

            return await _catalogContext
                                .Products
                                .Find(filter)
                                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Category, categoryName);
            return await _catalogContext
                                .Products
                                .Find(filter)
                                .ToListAsync();
        }

        public async Task Create(Product product)
        {
            await _catalogContext.Products.InsertOneAsync(product);
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var isDeleteSuccessful = await _catalogContext.Products.DeleteOneAsync(filter);

            return isDeleteSuccessful.IsAcknowledged && isDeleteSuccessful.DeletedCount > 0;

        }

        public async Task<bool> Update(Product product)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
            var updateResult = await _catalogContext.Products.ReplaceOneAsync(filter, product);

            return (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0);
        }
    }
}
