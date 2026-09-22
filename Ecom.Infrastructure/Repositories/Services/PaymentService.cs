using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork work;
        private readonly IConfiguration configuration;
        private readonly AppDbContext appDbContext;
        public PaymentService(IUnitOfWork work, AppDbContext appDbContext, IConfiguration configuration)
        {
            this.work = work;
            this.appDbContext = appDbContext;
            this.configuration = configuration;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethodId)
        {
            var basket = await work.CustomerBasketRepository.GetBasketAsync(basketId);
            StripeConfiguration.ApiKey = configuration["StripeSetting:secretKey"];
            decimal shippingPrice = 0m;
            if (deliveryMethodId.HasValue)
            {
                var delivery = await appDbContext.DeliveryMethods.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == deliveryMethodId.Value);
                shippingPrice = delivery.Price;
            }
            foreach (var item in basket.basketItems)
            {
                var product = await work.ProductRepositry.GetByIdAsync(item.Id);
                item.Price = product.NewPrice;
            }
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent _intent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var option = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.basketItems.Sum(m => m.Quantity * (m.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "USD",
                    PaymentMethodTypes = new List<string> { "card"}
                };
                _intent = await paymentIntentService.CreateAsync(option);
                basket.PaymentIntentId = _intent.Id;
                basket.ClientSecret = _intent.ClientSecret;
            } else
            {
                var option = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.basketItems.Sum(m => m.Quantity * (m.Price * 100) ) +(long) (shippingPrice * 100),
                };

                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, option);
            }
            await work.CustomerBasketRepository.UpdateBasketAsync(basket);
            return basket;
        }
    }
}
