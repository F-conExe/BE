using Business.Category;
using Business.DTO.Create;
using Business.DTO.Update;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewBusiness _reviewBusiness;

        public ReviewController(IReviewBusiness reviewBusiness)
        {
            _reviewBusiness = reviewBusiness;
        }

        [HttpGet("getAllReviews")]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var result = await _reviewBusiness.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            try
            {
                var result = await _reviewBusiness.GetById(id);
                if (result == null)
                {
                    return NotFound(new { message = "Review not found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Review data is required.");
            }

            // Get the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var result = await _reviewBusiness.Save(reviewDto, token);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpPut("UpdateReview")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDTO reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest("Review data is required.");
            }

            // Get the token from the Authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var result = await _reviewBusiness.Update(reviewDto, token);

            if (result.Success)
            {
                return Ok(result);
            }
          

            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpDelete("deleteReview/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var result = await _reviewBusiness.DeleteById(id);
                if (result == null)
                {
                    return NotFound(new { message = "Review not found" });
                }
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
