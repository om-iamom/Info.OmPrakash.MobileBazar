using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MobileBazar.Basket.API.Business.Repositories;
using MobileBazar.Basket.API.Domain.Entities;
using MobileBazar.EventBusRabbbitMq.Events;
using MobileBazar.EventBusRabbbitMq.Producer;
using MobileBazar.EventBusRabbbitMq.Utils;

namespace MobileBazar.Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketBusiness _basketBusiness;
        private readonly IMapper _mapper;
        private readonly EventBusRabbitMqProducer _eventBus;

        public BasketController(IBasketBusiness basketBusiness, IMapper mapper, EventBusRabbitMqProducer eventBus)
        {
            _basketBusiness = basketBusiness ?? throw new ArgumentNullException(nameof(basketBusiness));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
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

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CheckOut([FromBody] BasketCheckOut basketCheckOut)
        {
            //get total price of basket
            //remove the basket
            // send checkout event to rabbit
            var basket =await _basketBusiness.GetBasket(basketCheckOut.UserName);
            if(basket==null)
            {
                return BadRequest();
            }

            var basketRemoved =await _basketBusiness.DeleteBasket(basketCheckOut.UserName);
            if(!basketRemoved)
            {
                return BadRequest();
            }

            var eventMessage = _mapper.Map<BasketCheckOutEvent>(basketCheckOut);
            eventMessage.RequestId = Guid.NewGuid();
            eventMessage.TotalPrice = basket.TotalPrice;

            try
            {
                _eventBus.PublishBasketCheckout(EventBusConstants.BASKETCHECKOUTQUEUE, eventMessage);
            }
            catch (Exception)
            {
                throw;
            }

            return Accepted();
        }
    }
}
