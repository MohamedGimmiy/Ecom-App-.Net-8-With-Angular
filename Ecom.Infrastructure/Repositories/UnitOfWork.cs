using AutoMapper;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imageManagementService;
        public ICategoryRepositry CategoryRepositry { get; }

        public IProductRepositry ProductRepositry { get; }

        public IPhotoRepositry PhotoRepositry { get; }
        public UnitOfWork(AppDbContext context, IImageManagementService imageManagementService, IMapper mapper)
        {
            _context = context;
            this.imageManagementService = imageManagementService;
            this.mapper = mapper;
            CategoryRepositry = new CategoryRepositry(_context);
            ProductRepositry = new ProductRepositry(_context,mapper,imageManagementService);
            PhotoRepositry = new PhotoRepositry(_context);
        }
    }
}
