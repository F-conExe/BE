using Business.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipPlanAsmController : ControllerBase
    {

        private readonly IMembershipPlanAsmBusiness imbsp;
        public MembershipPlanAsmController(IMembershipPlanAsmBusiness _imbsp)
        {
            imbsp = _imbsp;
        }

        [HttpGet("getAllBill")]
        public async Task<IActionResult> GetAllMemberships()
        {
            try
            {
                var result = await imbsp.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

    }
}
