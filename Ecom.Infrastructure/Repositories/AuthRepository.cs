using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Core.Sharing;
using Ecom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories
{
    public  class AuthRepository : IAuth
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IGenerateToken token;
        private readonly AppDbContext context;
        public AuthRepository(UserManager<AppUser> userManager,
            IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken token, AppDbContext context)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.token = token;
            this.context = context;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO == null) {
                return null;
            }
            if (await userManager.FindByNameAsync(registerDTO.UserName) != null)
            {
                return "This userName is Already registered";
            }

            if (await userManager.FindByEmailAsync(registerDTO.Email) != null)
            {
                return "This email is Already registered";
            }
            var user = new AppUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                DisplayName = registerDTO.DisplayName
            };
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                //send Active email
                string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendEmail(user.Email, token,"active","ActiveEmail", "Please Active your email, click on button to active");
                return "Done";
            }
            return result.Errors.ToList()[0].Description;
        }

        public async Task SendEmail(string email, string code, string component, string subject, string message)
        {
            var result = new EmailDTO(email, "mohamedhussin07@gmail.com", subject,
                EmailStringBody.send(email, code , component, message));

            await emailService.sendEmail(result);
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            if(loginDTO == null)
            {
                return null;
            }
            var findUser = await userManager.FindByEmailAsync(loginDTO.Email);

            if (!findUser.EmailConfirmed)
            {
                string token = await userManager.GenerateEmailConfirmationTokenAsync(findUser);
                await SendEmail(findUser.Email, token, "active", "ActiveEmail", "Please Active your email, click on button to active");
                return "Please confirm your email first, we have sent activation token to your email";
            }

            var result = await signInManager.CheckPasswordSignInAsync(findUser, loginDTO.Password, true);
            if (result.Succeeded)
            {
                return token.GetAndCreateToken(findUser);
            }
            return "Please check your email or password, and try again";
        }

        public async Task<bool> SendEmailForForgetPassword(string email)
        {
            var findUser = await userManager.FindByEmailAsync(email);
            if(findUser is null)
            {
                return false;
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(findUser);
            await SendEmail(findUser.Email, token, "Reset-Password", "Reset Password", " click on button to reset your password");
            return true;
        }

        public async Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var findUser = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (findUser is null)
            {
                return null;
            }
            var result = await userManager.ResetPasswordAsync(findUser, resetPasswordDTO.Token, resetPasswordDTO.Password);
            if (result.Succeeded)
            {
                return "done";
            }
            return result.Errors.ToList()[0].Description;
        }

        public async Task<bool> ActiveAccount(ActiveAccountDTO activeAccountDTO)
        {
            var findUser = await userManager.FindByEmailAsync(activeAccountDTO.Email);
            if (findUser is null)
            {
                return false;
            }
            var result = await userManager.ConfirmEmailAsync(findUser, activeAccountDTO.Token);
            if (result.Succeeded)
            {
                return true;
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(findUser);

            await SendEmail(findUser.Email, token, "active", "ActiveEmail", "Please Active your email, click on button to active");
            return false;

        }

        public async Task<bool> UpdateAddress(string email, Address address)
        {
            var findUser = await userManager.FindByEmailAsync(email);
            if(findUser is null)
            {
                return false;
            }
            var Myaddress = await context.Addresses.FirstOrDefaultAsync(m => m.AppUserId  == findUser.Id);

            if(Myaddress is null)
            {
                address.AppUserId = findUser.Id;
                await context.Addresses.AddAsync(address);
            } else
            {
                address.Id = Myaddress.Id;
                address.AppUserId = Myaddress.AppUserId;
                context.Addresses.Update(address);
            }
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Address> GetUserAddress(string email)
        {
            var User = await userManager.FindByEmailAsync(email);
            var address = await context.Addresses.FirstOrDefaultAsync(m => m.AppUserId == User.Id);

            return address;
        }
    }
}
