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
    public class MembershipPlanController : ControllerBase
    {
        private readonly IMembershipPlanBusiness _membershipPlanBusiness;

        public MembershipPlanController(IMembershipPlanBusiness membershipPlanBusiness)
        {
            _membershipPlanBusiness = membershipPlanBusiness;
        }

        [HttpGet("getAllMembershipPlans")]
        public async Task<IActionResult> GetAllMembershipPlans()
        {
            try
            {
                var result = await _membershipPlanBusiness.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetMembershipPlanById(int id)
        {
            var result = await _membershipPlanBusiness.GetById(id);
            return Ok(result);
        }

        [HttpPost("AddNewMembershipPlan")]
        public async Task<IActionResult> AddMembershipPlan([FromBody] CreateMembershipPlanDTO planDto)
        {
            if (planDto == null)
            {
                return BadRequest("Membership Plan data is required.");
            }

            var result = await _membershipPlanBusiness.Save(planDto);
            return Ok(result);
        }

        [HttpPut("UpdateMembershipPlan")]
        public async Task<IActionResult> UpdateMembershipPlan([FromBody] UpdateMemberhsipPlanDTO planDto)
        {
            if (planDto == null)
            {
                return BadRequest("Membership Plan data is required.");
            }

            var result = await _membershipPlanBusiness.Update(planDto);
            return Ok(result);
        }

        [HttpDelete("deleteMembershipPlan/{id}")]
        public async Task<IActionResult> DeleteMembershipPlan(int id)
        {
            try
            {
                var result = await _membershipPlanBusiness.DeleteById(id);
                if (result == null)
                {
                    return NotFound(new { message = "Membership Plan not found" });
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
