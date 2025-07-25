using TestTaskPaymentValid.Infrastructure.DAL.Interfaces;
using TestTaskPaymentValid.Domain.Interfaces;
using TestTaskPaymentValid.Domain.Entities;
using TestTaskPaymentValid.Domain.Enums;
using TestTaskPaymentValid.Models;

namespace TestTaskPaymentValid.Application.Services
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IIdempotencyService _idempotencyService;
        private readonly IPaymentValidator _paymentValidator;
        private readonly IGatewayRouter _gatewayRouter;
        private readonly IRetryService _retryService;

        public PaymentProcessor(ITransactionRepository transactionRepository,
                                IIdempotencyService idempotencyService,
                                IPaymentValidator paymentValidator,
                                IGatewayRouter gatewayRouter,
                                IRetryService retryService)
        {
            _transactionRepository = transactionRepository;
            _idempotencyService = idempotencyService;
            _paymentValidator = paymentValidator;
            _gatewayRouter = gatewayRouter;
            _retryService = retryService;
        }

        public async Task<PaymentResult> ProcessAsync(PaymentDto dto)
        {
            var paymentId = Guid.NewGuid();
            if(await _idempotencyService.IsDuplicateAsync(paymentId))
            {
                var transaction = await _transactionRepository
                    .GetByIdAsync(paymentId);

                return new PaymentResult
                {
                    PaymentId = paymentId,
                    Status = transaction?.Status ?? PaymentStatus.Failed,
                    Message = "Duplicate request",
                    CreatedAt = DateTime.UtcNow
                };
            }

            var request = new PaymentRequest
            {
                Id = paymentId,
                Amount = dto.Amount,
                Currency = Enum.Parse<Currency>(dto.Currency),
                SourceAccount = dto.SourceAccount,
                DestinationAccount = dto.DestinationAccount,
                Metadata = dto.Metadata
            };

            try
            {
                await _paymentValidator.ValidateAsync(request);

                var gateway = await _gatewayRouter.SelectGatewayAsync(request);

                var status = await _retryService.ExecuteWithRetryAsync(() =>
                    gateway.ProcessAsync(request));

                // FIXME: Исправить метод в соотвествии с новыми моделями sшльщзую транзакцию.
                var transaction = new PaymentTransaction
                {
                    Id = request.Id,
                    Request = request.Id,
                    Status = status.Status,
                    CreatedAt = DateTime.UtcNow,
                    ProcessedAt = status.Status == PaymentStatus.Processed ? DateTime.UtcNow : null
                };

                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();
                await _idempotencyService.MarkProcessedAsync(request.Id);

                return new PaymentResult
                {
                    PaymentId = request.Id,
                    Status = status.Status,
                    Message = "Processed",
                    CreatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    PaymentId = request.Id,
                    Status = PaymentStatus.Failed,
                    Message = $"Error: {ex.Message}",
                    CreatedAt = DateTime.UtcNow
                };
            }
        }   

        public async Task<PaymentResult?> GetStatusAsync(Guid paymentId)
        {
            var tx = await _transactionRepository.GetByIdAsync(paymentId);

            return tx == null ? null
                : new PaymentResult
                {
                    PaymentId = tx.Id,
                    Status = tx.Status,
                    CreatedAt = tx.ProcessedAt ?? tx.CreatedAt,
                    Message = $"Status: {tx.Status}"
                };
        }

        public async Task HandleNotificationaAsync(GatewayNotificationDto notification)
        {
            var tx = await _transactionRepository.GetByIdAsync(notification.PaymentId);
            if (tx != null)
            {
                tx.Status = notification.NewStatus;
                tx.ProcessedAt = DateTime.UtcNow;
                await _transactionRepository.SaveChangesAsync();
            }
        }
    }
}
