using Business.Base;
using Business.DTO.Auth;
using Business.DTO.Create;
using Business.DTO.Update;
using Common;
using DATA;
using DATA.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Category
{
    public interface IUserBusiness
    {
        Task<IBusinessResult> GetAll();
        Task<IBusinessResult> GetById(int id);
        Task<IBusinessResult> Save(CreateUserDTO user);
        Task<IBusinessResult> Update(UpdateUserDTO user);
        Task<IBusinessResult> DeleteById(int id);
        Task<IBusinessResult> Login(LoginUserDTO loginUserDTO); // Thêm phương thức login
        Task<IBusinessResult> Register(CreateUserDTO userDTO);
        Task<IBusinessResult> RefreshToken(string token);
    }

    public class UserBusiness : IUserBusiness
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public UserBusiness(IConfiguration configuration)
        {
            _unitOfWork ??= new UnitOfWork();
            _configuration = configuration;
        }

        public async Task<IBusinessResult> GetAll()
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetAllAsync();

                if (users == null || !users.Any())
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, users);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> GetById(int id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(id);

                if (user == null)
                {
                    return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
                }

                return new BusinessResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, user);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<IBusinessResult> Save(CreateUserDTO userDTO)
        {
            try
            {
                var user = new User
                {
                    Username = userDTO.Username,
                    Email = userDTO.Email,
                    PasswordHash = userDTO.PasswordHash,
                    UserType = userDTO.UserType,
                    ContactInfo = userDTO.ContactInfo,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                int result = await _unitOfWork.UserRepository.CreateAsync(user);
                if (result > 0)
                {
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG);
                }

                return new BusinessResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<IBusinessResult> Update(UpdateUserDTO updateUserDTO)
        {
            try
            {
                User existingUser = await _unitOfWork.UserRepository.GetByIdAsync(updateUserDTO.UserID);

                if(existingUser != null)
                {
                    existingUser.Username = updateUserDTO.Username ?? existingUser.Username;
                    existingUser.Email = updateUserDTO.Email ?? existingUser.Email;
                    existingUser.PasswordHash = updateUserDTO.PasswordHash ?? existingUser.PasswordHash;
                    existingUser.ContactInfo = updateUserDTO.ContactInfo ?? existingUser.ContactInfo;
                    existingUser.UserType = updateUserDTO.UserType ?? existingUser.UserType;
                    existingUser.CreatedAt = updateUserDTO.CreatedAt;
                    existingUser.UpdatedAt = DateTime.UtcNow;

                    int result = await _unitOfWork.UserRepository.UpdateAsync(existingUser);
                    if(result > 0)
                    {
                        return new BusinessResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG);
                    }
                    else
                    {
                        return new BusinessResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
                    }
                }
                else
                {
                    return new BusinessResult(Const.FAIL_UPDATE_CODE, "User not found");
                }
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }
       

        public async Task<IBusinessResult> DeleteById(int id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
                if (user != null)
                {
                    bool result = await _unitOfWork.UserRepository.RemoveAsync(user);
                    if (result)
                    {
                        return new BusinessResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG);
                    }

                    return new BusinessResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                }

                return new BusinessResult(Const.WARNING_NO_DATA_CODE, Const.WARNING_NO_DATA_MSG);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<IBusinessResult> Register(CreateUserDTO userDTO)
        {
            try
            {
                var existingUser = await _unitOfWork.UserRepository.FindByUsernameAsync(userDTO.Username);
                if (existingUser != null)
                {
                    return new BusinessResult(Const.FAIL_CREATE_CODE, "Username already exists");
                }

                var user = new User
                {
                    Username = userDTO.Username,
                    Email = userDTO.Email,
                    // Hash the password before saving
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.PasswordHash),
                    UserType = userDTO.UserType,
                    ContactInfo = userDTO.ContactInfo,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                int result = await _unitOfWork.UserRepository.CreateAsync(user);
                if (result > 0)
                {
                    // Create a JWT token
                    var token = GenerateJwtToken(user);
                    return new BusinessResult(Const.SUCCESS_CREATE_CODE, "User registered successfully", token);
                }

                return new BusinessResult(Const.FAIL_CREATE_CODE, "Failed to register user");
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }


        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Lấy khóa bí mật từ cấu hình

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(1), // Token hết hạn sau 1 giờ
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<IBusinessResult> Login(LoginUserDTO loginUserDTO)
{
    try
    {
        var user = await _unitOfWork.UserRepository.FindByUsernameAsync(loginUserDTO.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginUserDTO.Password, user.PasswordHash))
        {
            return new BusinessResult(Const.FAIL_VALIDATION_CODE, "Invalid username or password");
        }

        // Tạo JWT Token
        var token = GenerateJwtToken(user);

        return new BusinessResult(Const.SUCCESS_AUTHENTICATION_CODE, "Login successful", token);
    }
    catch (Exception ex)
    {
        return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
    }
}

        public async Task<IBusinessResult> RefreshToken(string token)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(token);
                var username = principal.Identity.Name;

                var user = await _unitOfWork.UserRepository.FindByUsernameAsync(username);
                if (user == null)
                {
                    return new BusinessResult(Const.FAIL_VALIDATION_CODE, "Invalid token");
                }

                // Tạo một token mới
                var newToken = GenerateJwtToken(user);

                return new BusinessResult(Const.SUCCESS_AUTHENTICATION_CODE, "Token refreshed successfully", newToken);
            }
            catch (Exception ex)
            {
                return new BusinessResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]); // Lấy khóa bí mật từ cấu hình

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // Chúng ta sẽ không kiểm tra thời hạn ở đây
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtToken = securityToken as JwtSecurityToken;
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }


    }
}
