using AutoMapper;
using Ecom.Core.DTO;
using Ecom.Core.Entities.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public class ProductRepositry :GenericRepository<Product>, IProductRepositry
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imageManagementService;
        public ProductRepositry(AppDbContext context, IMapper mapper, IImageManagementService imageManagementService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagementService = imageManagementService;
        }

        public async Task<ReturnProductDTO> GetAllAsync(ProductParams productParams)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Include(m => m.Photos)
                .AsNoTracking();

            // filtering by search term if Search is provided

            if (!string.IsNullOrEmpty(productParams.Search))
            {
                //var searchTerm = productParams.Search.ToLower();
                //query = query.Where(p => p.Name.ToLower().Contains(searchTerm) 
                //||
                //p.Description.ToLower().Contains(searchTerm));

                var searchWords = productParams.Search.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = query.Where(p => searchWords.Any(word =>
                    p.Name.ToLower().Contains(word) ||
                    p.Description.ToLower().Contains(word)));
            }
            // Filtering by category if CategoryID is provided
            if (productParams.CategoryID.HasValue)
            {
                query = query.Where(p => p.CategoryId == productParams.CategoryID.Value);

            }
                if (!string.IsNullOrEmpty(productParams.Sort))
            {
                query = productParams.Sort switch
                {
                    "name" => query.OrderBy(p => p.Name),
                    "priceAsn" => query.OrderBy(p => p.NewPrice),
                    "priceDsn" => query.OrderByDescending(p => p.NewPrice),
                    _ => query.OrderBy(p => p.Name)
                };
            }

            ReturnProductDTO returnProductDTO = new ReturnProductDTO();
            returnProductDTO.TotalCount = await query.CountAsync();

            query = query
                .Skip((productParams.PageNumber - 1) * productParams.pageSize)
                .Take(productParams.pageSize);
            returnProductDTO.Products = mapper.Map<List<ProductDTO>>(await query.ToListAsync());
            return returnProductDTO;
        }
        public async Task<bool> AddAsync(AddProductDTO productDTO)
        {
            if (productDTO == null) return false;
            var product = mapper.Map<Product>(productDTO);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var imagePath = 
                await imageManagementService
                .AddImageAsync(productDTO.Photo, productDTO.Name);

            var photo = imagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id
            }).ToList();

            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();
            return true;
        }

        

        public async Task<bool> UpdateAsync(UpdateProductDTO updateProductDTO)
        {
            if(updateProductDTO is null)
            {
                return false;
            }
            var findProduct = await context.Products
                .Include(p => p.Category)
                .Include(m => m.Photos)
                .FirstOrDefaultAsync(m => m.Id == updateProductDTO.Id);

            if(findProduct is null)
            {
                return false;
            }
            mapper.Map(updateProductDTO, findProduct);


            var FindPhoto = findProduct.Photos;
            foreach (var item in FindPhoto)
            {
                imageManagementService.DeleteImageAsync(item.ImageName);
            }
            context.Photos.RemoveRange(FindPhoto);
            await context.SaveChangesAsync();

            var ImagePath = await imageManagementService
                .AddImageAsync(updateProductDTO.Photo, updateProductDTO.Name);

            var updatedProductId = updateProductDTO.Id;
            var photo = ImagePath.Select(path => new Photo
            {
                ImageName = path,
                ProductId = updatedProductId,   
            });

            await context.Photos.AddRangeAsync(photo);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task DeleteAsync(Product product)
        {
            var photo =product.Photos;

            foreach (var item in photo)
            {
                imageManagementService.DeleteImageAsync(item.ImageName);
            }
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
