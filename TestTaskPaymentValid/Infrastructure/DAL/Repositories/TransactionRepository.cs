using TestTaskPaymentValid.Infrastructure.DAL.Interfaces;
using TestTaskPaymentValid.Infrastructure.DAL.Core;
using TestTaskPaymentValid.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TestTaskPaymentValid.Infrastructure.DAL.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        // FIXME: Исправить метод в соотвествии с новыми моделями.
        public async Task<PaymentTransaction?> GetByIdAsync(Guid id)
        {
            return await _context.Transactions
                .Include(t => t.Request) 
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(PaymentTransaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public Task UpdateAsync(PaymentTransaction transaction)
        {
            _context.Transactions.Update(transaction);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
