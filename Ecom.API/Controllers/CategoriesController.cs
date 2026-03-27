using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await work.CategoryRepositry.GetAllAsync();
                if (categories is null)
                {
                    return BadRequest(new ResponseAPI(400));
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var category = await work.CategoryRepositry.GetByIdAsync(id);
                if (category is null)
                {
                    return BadRequest(new ResponseAPI(400,"Not found category"));
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("add-category")]
        public async Task<IActionResult> Add(CategoryDTO categoryDTO)
        {
            try
            {
                var category =  mapper.Map<Category>(categoryDTO);
                await work.CategoryRepositry.AddAsync(category);
                return Ok(new ResponseAPI(200, "Item has been Added"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("update-category")]
        public async Task<IActionResult> Update(UpdateCategoryDTO categoryDTO)
        {
            try
            {
                Category category = mapper.Map<Category>(categoryDTO);
                await work.CategoryRepositry.UpdateAsync(category);
                return Ok(new ResponseAPI(200, "Item has been Updated"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await work.CategoryRepositry.DeleteAsync(id);
                return Ok(new ResponseAPI(200, "Item has been Deleted"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
