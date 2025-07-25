namespace TestTaskPaymentValid.Domain.Interfaces
{
    public interface IIdempotencyService
    {
        Task<bool> IsDuplicateAsync(Guid transId);
        Task MarkProcessedAsync(Guid transId);
    }
}
