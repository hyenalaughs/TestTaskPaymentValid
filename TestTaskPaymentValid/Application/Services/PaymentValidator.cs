using TestTaskPaymentValid.Infrastructure.Interfaces;
using TestTaskPaymentValid.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using TestTaskPaymentValid.Domain.Entities;
using TestTaskPaymentValid.Domain.Enums;
using System.Text.RegularExpressions;

namespace TestTaskPaymentValid.Application.Services
{
    public class PaymentValidator : IPaymentValidator
    {
        private readonly IBalanceService _balanceService;
        private readonly ILogger<IPaymentValidator> _loggerService;

        public PaymentValidator(IBalanceService balanceService,
                                ILogger<IPaymentValidator> loggerService)
        {
            _balanceService = balanceService;
            _loggerService = loggerService;
        }

        public async Task ValidateAsync(PaymentRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            _loggerService.LogInformation("Transaction validation has begun. " +
                "TransId: {TransactionId}, Amount: {Amount}, Currency: {Currency}",
                 request.Id, request.Amount, request.Currency);

            ValidateAcc(request.SourceAccount, request.Currency, "Source account");
            ValidateAcc(request.DestinationAccount, request.Currency, "Destination account");

            ValidateLimit(request.Currency, request.Amount);

            await HasSufficientBalanceAsync(request.SourceAccount, 
                                            request.Currency, 
                                            request.Amount);
        }

        private void ValidateAcc(string acc, Currency currency, string? type)
        {
            var isValid = currency switch
            {
                Currency.USD => Regex.IsMatch(acc, @"^USD\d{10,12}$"),
                Currency.EUR => Regex.IsMatch(acc, @"^EUR\d{10,12}$"),
                Currency.GBP => Regex.IsMatch(acc, @"^GBP\d{10,12}$"),
                Currency.JPY => Regex.IsMatch(acc, @"^JPY\d{10,12}$"),
                Currency.CAD => Regex.IsMatch(acc, @"^CAD\d{10,12}$"),
                Currency.CHF => Regex.IsMatch(acc, @"^CHF\d{10,12}$"),
                Currency.AUD => Regex.IsMatch(acc, @"^AUD\d{10,12}$"),
                Currency.RUB => Regex.IsMatch(acc, @"^RUB\d{10,12}$"),
                _ => throw new ValidationException("Unknown format")
            };

            if (!isValid)
            {
                _loggerService.LogWarning("Unknown format: {Currency} {Acc} {type}.",
                    currency, acc, type);

                throw new ValidationException($"Unknown format: {currency} {acc} {type}");
            }
        }

        private void ValidateLimit(Currency currency, decimal amount)
        {
            var (min, max) = currency switch
            {
                Currency.USD => (1m, 10000m),
                Currency.EUR => (5m, 8000m),
                Currency.GBP => (10m, 10000m),
                Currency.JPY => (5m, 500000m),
                Currency.CAD => (10m, 10000m),
                Currency.CHF => (5m, 1000000m),
                Currency.AUD => (10m, 10000m),
                Currency.RUB => (100m, 1000000m),
                _ => throw new ValidationException("Unknown format")
            };

            if (amount < min || amount > max)
            {
                _loggerService.LogWarning("Amount out of range: {Currency} {Amount}." +
                    " Range: {Min} - {Max}", currency, amount, min, max);

                throw new ValidationException($"Amount out of range {currency} {amount}. " +
                    $"Range: {min} - {max}");
            }
        }

        private async Task HasSufficientBalanceAsync(string account, 
                                                      Currency currency, 
                                                      decimal amount)
        {
            var isEnought =  await _balanceService
                .HasSufficientBalanceAsync(account, currency, amount);

            if (!isEnought)
            {
                _loggerService.LogWarning("There are insufficient funds in the account. " +
                    "{Account} {Currency} {Amount}", account, currency, amount);

                throw new ValidationException($"There are insufficient funds in the account. " +
                    $"{account} {currency} {amount}");
            }
        }
    }
}
