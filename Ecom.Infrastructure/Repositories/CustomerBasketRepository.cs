using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public class CustomerBasketRepository : ICustomerBasketRepository
    {
        private readonly IDatabase _database;
        public CustomerBasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();

        }
        public Task<bool> DeleteBasketAsync(string Id)
        {
            return _database.KeyDeleteAsync(Id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string Id)
        {
            var result = await _database.StringGetAsync(Id);
            if (!string.IsNullOrEmpty(result))
            {
                return JsonSerializer.Deserialize<CustomerBasket>(result);
            }
            return null;
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var _basket = await _database
                .StringSetAsync(basket.Id, JsonSerializer.Serialize(basket),
                TimeSpan.FromDays(3));

            if (_basket)
            {
                return await GetBasketAsync(basket.Id);
            }
            return null;
        }
    }
}
