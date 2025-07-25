using TestTaskPaymentValid.Infrastructure.DAL.Interfaces;
using TestTaskPaymentValid.Domain.Interfaces;
using TestTaskPaymentValid.Models;

namespace TestTaskPaymentValid.Application.Services
{
    public class IdempotencyService : IIdempotencyService
    {
        private readonly IKeyRepository _keyRepository;

        public IdempotencyService(IKeyRepository keyRepository)
        {
            _keyRepository = keyRepository;
        }

        public async Task<bool> IsDuplicateAsync(Guid transId)
        {
            return await _keyRepository.IsExistAsync(transId);
        }

        public async Task MarkProcessedAsync(Guid transId)
        {
            await _keyRepository
                .AddAsync(new IdempotencyKey { TransactionId = transId });
        }
    }
}
