using Business.Category;
using Business.DTO.Create;
using Business.DTO.Update;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostBusiness _postBusiness;

        public PostController(IPostBusiness postBusiness)
        {
            _postBusiness = postBusiness;
        }
        [HttpGet("getAllPosts")]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var result = await _postBusiness.GetAll();
                if (result == null ) 
                {
                    return NotFound(new { message = "No posts found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }


        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var result = await _postBusiness.GetById(id);
            if (result == null)
            {
                return NotFound(new { message = "Post not found" });
            }
            return Ok(result);
        }

        [HttpPost("AddNewPost")]
        public async Task<IActionResult> AddPost([FromBody] CreatePostDTO postDto)
        {
            if (postDto == null)
            {
                return BadRequest("Post data is required.");
            }

            // Get the token from the Authorization header
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var result = await _postBusiness.Save(postDto, token);
            return Ok(result);
        }

        [HttpPut("UpdatePost")]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostDTO postDto)
        {
            if (postDto == null)
            {
                return BadRequest("Post data is required.");
            }

            var result = await _postBusiness.Update(postDto);
            return Ok(result);
        }

        [HttpDelete("deletePost/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var result = await _postBusiness.DeleteById(id);
                if (result == null)
                {
                    return NotFound(new { message = "Post not found" });
                }
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        [HttpGet("search")] // Changed from "/search" to "search"
        public async Task<IActionResult> Search(string skill = null, decimal? minSalary = null, decimal? maxSalary = null, int? postTypeId = null)
        {
            try
            {
                var result = await _postBusiness.Search(skill, minSalary, maxSalary, postTypeId);
                if (result == null )
                {
                    return NotFound(new { message = "No posts found matching the criteria" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getPostCount")]
        public async Task<IActionResult> GetPostCount()
        {
            try
            {
                var count = await _postBusiness.GetPostCount();
                return Ok(new { Count = count });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getPostsByType/{typeName}")]
        public async Task<IActionResult> GetPostsByType(string typeName)
        {
            try
            {
                // Ensure only "Freelancer" and "Enterprise" types are allowed
                if (typeName != "Freelancer" && typeName != "Enterprise")
                {
                    return BadRequest(new { message = "Invalid post type. Only 'Freelancer' and 'Enterprise' are allowed." });
                }

                var result = await _postBusiness.GetPostsByType(typeName);
                if (result == null)
                {
                    return NotFound(new { message = "No posts found for the specified type" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }




    }
}
