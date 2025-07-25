using TestTaskPaymentValid.Infrastructure.DAL.Interfaces;
using TestTaskPaymentValid.Infrastructure.DAL.Core;
using Microsoft.EntityFrameworkCore;
using TestTaskPaymentValid.Models;

namespace TestTaskPaymentValid.Infrastructure.DAL.Repositories
{
    public class KeyRepository : IKeyRepository
    {
        private readonly AppDbContext _context;

        public KeyRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<bool> IsExistAsync(Guid key)
        {
            return await _context.IdempotencyKeys
                .AnyAsync(x => x.TransactionId == key);
        }

        public async Task AddAsync(IdempotencyKey key)
        {
            await _context.AddAsync(key);
        }
    }
}
