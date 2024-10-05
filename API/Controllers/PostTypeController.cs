using Business.Category;
using Business.DTO.Create;
using Business.DTO.Update;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostTypeController : ControllerBase
    {
        private readonly IPostTypeBusiness _postTypeBusiness;

        public PostTypeController(IPostTypeBusiness postTypeBusiness)
        {
            _postTypeBusiness = postTypeBusiness;
        }

        [HttpGet("getAllPostType")]
        public async Task<IActionResult> GetAllPostType()
        {
            try
            {
                var result = await _postTypeBusiness.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetUSerById(int id)
        {
            var result = await _postTypeBusiness.GetById(id);
            return Ok(result);
        }

        [HttpPost("AddNewPostType")]
        public async Task<IActionResult> AddUser([FromBody] CreatePostTypeDTO type)
        {
            if (type == null)
            {
                return BadRequest("Post Type data is required.");
            }

            // Assuming you have a User entity class

            var result = await _postTypeBusiness.Save(type);
            return Ok(result);
        }

        [HttpPut("UpdatePostType")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdatePostTypeDTO typeDto)
        {
            if (typeDto == null)
            {
                return BadRequest("User data is required.");
            }

            // Assuming you have a User entity class

            var result = await _postTypeBusiness.Update(typeDto);
            return Ok(result);
        }

        [HttpDelete("deletePostType/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _postTypeBusiness.DeleteById(id);
                if (result == null)
                {
                    return NotFound(new { message = "Post Type not found" });
                }
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

    }
}
