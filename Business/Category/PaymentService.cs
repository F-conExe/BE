using Business.Base;
using Business.Library;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Category
{
    public interface IPayOSService
    {
        Task<IBusinessResult> RequestWithPayOsAsync(string accountId, decimal amount);
    }
    public class PaymentService : IPayOSService
    {
        private readonly PayOSService _payOSService;

        public PaymentService(PayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        async Task<IBusinessResult> IPayOSService.RequestWithPayOsAsync(string userId, decimal amount)
        {
            var items = new List<ItemData>
            {
                new ItemData("NẠP TIỀN VÀO HỆ THỐNG", 1, (int)amount)
            };

            long orderCode = long.Parse(DateTimeOffset.Now.ToString("yyMMddHHmmss"));
            //string returnUrl = $"https://elderconnection.vercel.app/success?transactionId={orderId}";
            //string cancelUrl = $"https://elderconnection.vercel.app/cancel?transactionId={orderId}";


            var payOSModel = new PaymentData(
                orderCode: orderCode,
                amount: (int)amount,
                description: "Thanh toan don hang",
                items: items,
                returnUrl: "https://f-con.id.vn/membership",
                cancelUrl: "https://f-con.id.vn/membership"
            );


            var paymentUrl = await _payOSService.CreatePaymentLink(payOSModel);


            if (paymentUrl != null)
            {
                return new BusinessResult
                {
                    Status = 201,
                    Message = "Create payment url success",
                    Data = paymentUrl.checkoutUrl
                };
            }
            return new BusinessResult
            {
                Status = 400,
                Message = "Create URL failed"
            };
        }
    }
}
