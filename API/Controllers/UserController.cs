using Business.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DATA.Models;
using System;
using System.Threading.Tasks;
using Business.DTO.Create;
using Business.DTO.Update;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;

        public UserController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var result = await _userBusiness.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var result = await _userBusiness.GetById(id);
                if (result == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "User data cannot be null." });
            }

            try
            {
                var result = await _userBusiness.Save(user);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating user" });
                }

                if (result.Success) // Adjust based on your actual result object properties
                {
                    return Ok(new { message = "User created successfully" });
                }
                else
                {
                    return BadRequest(new { message = result.Message }); // Adjust based on your actual result object
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                // _logger.LogError(ex, "An error occurred while creating a user.");

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the user." });
            }
        }



        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUserDto)
        {
            if (updateUserDto == null)
            {
                return BadRequest("User data is required.");
            }

            // Assuming you have a User entity class

            var result = await _userBusiness.Update(updateUserDto);
            return Ok(result);
        }

      

        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userBusiness.DeleteById(id);
                if (result == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
