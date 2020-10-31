using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileBazar.Basket.API.Business.Repositories;
using MobileBazar.Basket.API.Domain.Entities;

namespace MobileBazar.Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketBusiness _basketBusiness;

        public BasketController(IBasketBusiness basketBusiness)
        {
            _basketBusiness = basketBusiness ?? throw new ArgumentNullException(nameof(basketBusiness));
        }

        [HttpGet]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> GetBasket(string userName)
        {
            return Ok((await _basketBusiness.GetBasket(userName))??new BasketCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(BasketCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BasketCart>> UpdateBasket([FromBody] BasketCart basketCart)
        {
            return Ok(await _basketBusiness.UpdateBasket(basketCart));
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            
            return Ok(await _basketBusiness.DeleteBasket(userName));
        }
    }
}
