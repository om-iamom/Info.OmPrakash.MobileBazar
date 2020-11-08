using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MobileBazar.Basket.API.Business.Repositories;
using MobileBazar.Basket.API.Business.Services;
using MobileBazar.Basket.API.DataAccess.Repositories;
using MobileBazar.Basket.API.DataAccess.Services;
using MobileBazar.EventBusRabbbitMq;
using MobileBazar.EventBusRabbbitMq.Producer;
using MobileBazar.EventBusRabbbitMq.Services;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace MobileBazar.Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ConnectionMultiplexer>(sp =>
               {
                   var configuration = ConfigurationOptions.Parse(Configuration.GetConnectionString("Redis"), true);
                   return ConnectionMultiplexer.Connect(configuration);
               });
            services.AddScoped<IBasketContext, BasketContext>();
            services.AddScoped<IBasketBusiness, BasketBusiness>();
            services.AddAutoMapper(typeof(Startup)); // for current assembly

            services.AddSingleton<IRabbitMqConnection>(sp =>
               {
                   var factory = new ConnectionFactory()
                   {
                       HostName = Configuration["EventBus:HostName"]
                   };
                   
                   if(!string.IsNullOrEmpty(Configuration["EventBus:UserName"]))
                   {
                       factory.UserName = Configuration["EventBus:UserName"];
                   }

                   if(!string.IsNullOrEmpty(Configuration["EventBus:Password"]))
                   {
                       factory.Password = Configuration["EventBus:Password"];
                   }

                   return new RabbitMqConnection(factory);
               });

            services.AddSingleton<EventBusRabbitMqProducer>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API V1");
            });
        }
    }
}
