using AutoMapper;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
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
        private readonly UserManager<AppUser> userManager;
        private readonly IConnectionMultiplexer redis;
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IGenerateToken token;
        public ICategoryRepositry CategoryRepositry { get; }

        public IProductRepositry ProductRepositry { get; }

        public IPhotoRepositry PhotoRepositry { get; }

        public ICustomerBasketRepository CustomerBasketRepository { get; }


        public IAuth Auth { get; }

        public UnitOfWork(AppDbContext context, IImageManagementService imageManagementService, IMapper mapper,
            IConnectionMultiplexer redis, UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken token)
        {
            _context = context;
            this.imageManagementService = imageManagementService;
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.redis = redis;
            this.emailService = emailService;
            this.token = token;
            CategoryRepositry = new CategoryRepositry(_context);
            ProductRepositry = new ProductRepositry(_context, mapper, imageManagementService);
            PhotoRepositry = new PhotoRepositry(_context);
            CustomerBasketRepository = new CustomerBasketRepository(redis);
            Auth = new AuthRepository(userManager, emailService, signInManager, token);
        }
    }
}
