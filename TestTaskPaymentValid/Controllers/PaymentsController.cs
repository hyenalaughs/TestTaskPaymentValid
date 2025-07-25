using TestTaskPaymentValid.Domain.Interfaces;
using TestTaskPaymentValid.Domain.Enums;
using TestTaskPaymentValid.Models;
using Microsoft.AspNetCore.Mvc;

namespace TestTaskPaymentValid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentsController(IPaymentProcessor paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        [HttpPost("/payments")]

        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto requst)
        {
            var result = await _paymentProcessor.ProcessAsync(requst);

            return result.Status switch
            {
                PaymentStatus.Processed => Ok(result),
                PaymentStatus.Pending => Accepted(result),
                PaymentStatus.Failed => StatusCode(500, result),
                _ => BadRequest("Unknown status")
            };
        }
    } 
}
