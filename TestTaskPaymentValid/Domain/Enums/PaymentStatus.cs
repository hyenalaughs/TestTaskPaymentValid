namespace TestTaskPaymentValid.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,      // Платёж принят, но не обработан
        Processed,    // Успешно завершён
        Failed,       // Ошибка при обработке 
        Refunded      // Возврт
    }
}
