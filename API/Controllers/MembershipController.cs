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
    public class MembershipController : ControllerBase
    {
        private readonly IMemberBusiness _memberBusiness;

        public MembershipController(IMemberBusiness memberBusiness)
        {
            _memberBusiness = memberBusiness;
        }

        [HttpGet("getAllMemberships")]
        public async Task<IActionResult> GetAllMemberships()
        {
            try
            {
                var result = await _memberBusiness.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetMembershipById(int id)
        {
            var result = await _memberBusiness.GetById(id);
            if (result == null)
            {
                return NotFound(new { message = "Membership not found" });
            }
            return Ok(result);
        }

        [HttpPost("AddNewMembership")]
        public async Task<IActionResult> AddMembership([FromBody] CreateMembershipDTO membershipDto)
        {
            if (membershipDto == null)
            {
                return BadRequest("Membership data is required.");
            }

            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _memberBusiness.Save(membershipDto, token);
            return Ok(result);
        }

        [HttpPut("UpdateMembership")]
        public async Task<IActionResult> UpdateMembership([FromBody] UpdateMembershipDTO membershipDto)
        {
            if (membershipDto == null)
            {
                return BadRequest("Membership data is required.");
            }

            var result = await _memberBusiness.Update(membershipDto);
            return Ok(result);
        }

        [HttpDelete("deleteMembership/{id}")]
        public async Task<IActionResult> DeleteMembership(int id)
        {
            try
            {
                var result = await _memberBusiness.DeleteById(id);
                if (result == null)
                {
                    return NotFound(new { message = "Membership not found" });
                }
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getMembershipPlansByUser")]
        public async Task<IActionResult> GetMembershipPlansByUser()
        {
            try
            {
                string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _memberBusiness.GetMembershipPlanByUser(token);

                if (result == null || result.Data == null)
                {
                    return NotFound(new { message = "No membership plans found for this user." });
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
