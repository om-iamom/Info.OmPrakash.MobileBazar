using AutoMapper;
using MobileBazar.Basket.API.Domain.Entities;
using MobileBazar.EventBusRabbbitMq.Events;

namespace MobileBazar.Basket.API.Mapping
{
    public class BasketMapping: Profile
    {
        public BasketMapping()
        {
            CreateMap<BasketCheckOut, BasketCheckOutEvent>().ReverseMap();
        }
    }
}
