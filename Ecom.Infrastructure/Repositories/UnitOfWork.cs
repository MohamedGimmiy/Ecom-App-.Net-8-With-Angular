using Ecom.Core.Interfaces;
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
        public ICategoryRepositry CategoryRepositry { get; }

        public IProductRepositry ProductRepositry { get; }

        public IPhotoRepositry PhotoRepositry { get; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            CategoryRepositry = new CategoryRepositry(_context);
            ProductRepositry = new ProductRepositry(_context);
            PhotoRepositry = new PhotoRepositry(_context);
        }
    }
}
