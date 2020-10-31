using MobileBazar.Basket.API.Business.Repositories;
using MobileBazar.Basket.API.DataAccess.Repositories;
using MobileBazar.Basket.API.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MobileBazar.Basket.API.Business.Services
{
    public class BasketBusiness : IBasketBusiness
    {
        private readonly IBasketContext _basketContext;

        public BasketBusiness(IBasketContext basketContext)
        {
            _basketContext = basketContext ?? throw new ArgumentNullException(nameof(basketContext));
        }

        public async Task<BasketCart> GetBasket(string userName)
        {
            var basket =await _basketContext
                             .Redis
                             .StringGetAsync(userName);

            if(basket.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basket)
        {
            var updatedBasket = await _basketContext
                                        .Redis
                                        .StringSetAsync(basket.UserName, JsonConvert.SerializeObject(basket));

            if(!updatedBasket)
            {
                return null;
            }

            return await GetBasket(basket.UserName);
            
        }

        public async Task<bool> DeleteBasket(string userName)
        {
            return await _basketContext
                            .Redis
                            .KeyDeleteAsync(userName);
        }

       
    }
}
