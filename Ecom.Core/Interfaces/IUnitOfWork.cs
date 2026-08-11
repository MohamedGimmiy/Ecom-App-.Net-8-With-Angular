using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IUnitOfWork
    {
        public ICategoryRepositry CategoryRepositry { get; }

        public IProductRepositry ProductRepositry { get; }
        public IPhotoRepositry PhotoRepositry { get; }

        public ICustomerBasketRepository CustomerBasketRepository { get; }

        public IAuth Auth { get; }
    }
}
