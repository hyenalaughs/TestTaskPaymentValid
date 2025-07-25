using TestTaskPaymentValid.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TestTaskPaymentValid.Models;

namespace TestTaskPaymentValid.Infrastructure.DAL.Core
{
    public class AppDbContext : DbContext
    {
        public DbSet<PaymentTransaction> Transactions { get; set; }
        public DbSet<IdempotencyKey> IdempotencyKeys { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
