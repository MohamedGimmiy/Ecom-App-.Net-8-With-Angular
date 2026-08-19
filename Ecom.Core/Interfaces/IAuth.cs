using Ecom.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IAuth
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);

        Task<string> LoginAsync(LoginDTO loginDTO);
        Task<bool> SendEmailForForgetPassword(string email);

        Task<string> ResetPassword(ResetPasswordDTO resetPasswordDTO);

        Task<bool> ActiveAccount(ActiveAccountDTO activeAccountDTO);
    }
}
