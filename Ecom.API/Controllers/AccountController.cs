using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entities;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecom.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-address-for-user")]
        public async Task<IActionResult> getAddress()
        {
            var address = await work.Auth.GetUserAddress(User.FindFirst(ClaimTypes.Email).Value);
            var result = mapper.Map<shipAddressDTO>(address);
            return Ok(result);
        }



        [Authorize]  // ✅ ADD THIS
        [HttpPut("update-address")]
        public async Task<IActionResult> updateAddress(shipAddressDTO shipAddressDTO)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var address = mapper.Map<Address>(shipAddressDTO);
            var result = await work.Auth.UpdateAddress(email, address);
            return result ? Ok() : BadRequest();
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            var Result = await work.Auth.RegisterAsync(registerDto);
            if(Result != "Done")
            {
                return BadRequest(new ResponseAPI(400, Result));
            }

            return Ok(new ResponseAPI(200, Result));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var Result = await work.Auth.LoginAsync(loginDto);

            if (Result.StartsWith("Please"))
            {
                return BadRequest(new ResponseAPI(400, Result));
            }
            // Set cookie options to work in development (only secure when request is HTTPS)

            Response.Cookies.Append("token", Result, new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(1),
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new ResponseAPI(200));
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> active(ActiveAccountDTO accountDTO)
        {
            var result = await work.Auth.ActiveAccount(accountDTO);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(400));
        }

        [HttpGet("send-email-forget-password")]
        public async Task<IActionResult> forget(string email)
        {
            var result = await work.Auth.SendEmailForForgetPassword(email);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(400));
        }

        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> reset(ResetPasswordDTO resetPasswordDTO)
        {
            var result = await work.Auth.ResetPassword(resetPasswordDTO);
            if(result == "done")
            {
                return Ok(new ResponseAPI(200));
            }
            return BadRequest(new ResponseAPI(400));
        }
    }
}
