using Business.Base;
using Business.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayOSService _ipayOSService;

        public PaymentController(IPayOSService ipayOSService)
        {
            _ipayOSService = ipayOSService;
        }

        [HttpPost("request-top-up-wallet-with-payos")]
        public async Task<IActionResult> RequestTopUpWalletWithPayOs(string userId,decimal amount)
        {
            try
            {
                var paymenturl = await _ipayOSService.RequestWithPayOsAsync(userId, amount);
                return Ok(paymenturl);
            }
            catch (Exception ex)
            {
                return BadRequest(new BusinessResult
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Invalid parameters."
                });
            }

        }

        
    }
}
