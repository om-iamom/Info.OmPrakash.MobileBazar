using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileBazar.Catalog.API.Business.Repositories;
using MobileBazar.Catalog.API.Domain.Entities;

namespace MobileBazar.Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IProductBusiness _productBusiness;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductBusiness productBusiness, ILogger<CatalogController> logger)
        {
            _productBusiness = productBusiness ?? throw new ArgumentNullException(nameof(productBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productBusiness.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productBusiness.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"{id} not found");
                return NotFound();
            }

            return Ok(product);
        }

        [Route("[action]/{categoryName}")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string categoryName)
        {
            var products = await _productBusiness.GetProductByCategory(categoryName);
            if (products == null)
            {
                _logger.LogError($"No product found under {categoryName} category");
                return NotFound();
            }

            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _productBusiness.Create(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<ActionResult<bool>> UpdateProduct([FromBody] Product product)
        {
            var isUpdateSuccessfull = await _productBusiness.Update(product);

            return Ok(isUpdateSuccessfull);
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<ActionResult<bool>> DeleteProductById(string id)
        {
            return Ok(await _productBusiness.Delete(id));
        }
    }
}
