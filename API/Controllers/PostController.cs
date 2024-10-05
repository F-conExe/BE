﻿using Business.Category;
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

            var result = await _postBusiness.Save(postDto);
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
    }
}
