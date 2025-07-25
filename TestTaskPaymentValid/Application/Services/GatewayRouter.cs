using TestTaskPaymentValid.Infrastructure.Interfaces;
using TestTaskPaymentValid.Domain.Interfaces;
using TestTaskPaymentValid.Domain.Entities;

namespace TestTaskPaymentValid.Application.Services
{
    public class GatewayRouter : IGatewayRouter
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;
        private readonly ILogger<GatewayRouter> _logger;

        public GatewayRouter(IEnumerable<IPaymentGateway> gateways,
                             ILogger<GatewayRouter> logger) 
        {
            _gateways = gateways;
            _logger = logger;
        }

        public async Task<IPaymentGateway> SelectGatewayAsync(PaymentRequest request)
        {
            var availablesGateways = await SelectAvailablesGatewaysAsync(request);

            var bestGateway = availablesGateways.OrderBy(x => x.Comission).First();
            _logger.LogWarning("Gateway {Gateway} selected for transaction {Transaction}",
                bestGateway.Gateway.Name, request.Id);

            return bestGateway.Gateway;
        }

        private async Task<List<(IPaymentGateway Gateway, decimal Comission)>> 
            SelectAvailablesGatewaysAsync(PaymentRequest request)
        {

            var availablesGateways = new List<(IPaymentGateway Gateway, decimal Comission)>();

            foreach (var gateway in _gateways)
            {
                if (!gateway.AvailableCurrency.Contains(request.Currency))
                    continue;

                if (!await gateway.IsAvailableAsync())
                    continue;

                var fee = await gateway.GetCommissionAsync(request);
                availablesGateways.Add((gateway, fee));
            }

            if (!availablesGateways.Any())
            {
                _logger.LogWarning("No gateways available for currency {Currency}.",
                    request.Currency);
                throw new InvalidOperationException("No gateways available.");
            }

            return availablesGateways;
        }
    }
}
