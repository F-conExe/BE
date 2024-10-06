using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using DATA;
using Common;
using DATA.Models;

namespace Business.Base
{
    public class BaseBusiness
    {
        protected readonly UnitOfWork _unitOfWork;
        protected readonly IConfiguration _configuration;

        public BaseBusiness(IConfiguration configuration)
        {
            _unitOfWork = new UnitOfWork();
            _configuration = configuration;
        }

        public async Task<IBusinessResult> GetCurrentUser(string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                if (principal == null)
                {
                    return new BusinessResult(Const.FAIL_VALIDATION_CODE, "Invalid token");
                }

                var username = principal.Identity.Name;
                var user = await _unitOfWork.UserRepository.FindByUsernameAsync(username);

                if (user == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, "User not found");
                }

                // Don't return the password hash
                user.PasswordHash = null;

                return new BusinessResult(Const.SUCCESS_READ_CODE, "Current user retrieved successfully", user);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        protected ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

    }
}