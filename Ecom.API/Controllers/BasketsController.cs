using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class BasketsController : BaseController
    {
        public BasketsController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("get-basket-item/{id}")]
        public async Task<IActionResult> get(string id)
        {
            var basket = await work.CustomerBasketRepository.GetBasketAsync(id);
            if (basket == null)
            {
                return Ok(new CustomerBasket());
            }
            return Ok(basket);
        }
        [HttpPost("update-basket")]
        public async Task<IActionResult> update(CustomerBasket basket)
        {
            var updatedBasket = await work.CustomerBasketRepository.UpdateBasketAsync(basket);
            if (updatedBasket == null)
            {
                return BadRequest();
            }
            return Ok(updatedBasket);
        }

        [HttpDelete("delete-basket-item/{id}")]
        public async Task<IActionResult> delete(string id)
        {
            var result = await work.CustomerBasketRepository.DeleteBasketAsync(id);
            if (!result)
            {
                return BadRequest(new ResponseAPI(400, "Failed to delete item"));
            }
            return Ok(new ResponseAPI(200, "item deleted"));

        }
    }
}
