using Microsoft.EntityFrameworkCore;

namespace bankAccounts.Models
{
    public class BankAccountsContext : DbContext
    {
        public BankAccountsContext(DbContextOptions options) : base(options) {}

        public DbSet<User> Users {get; set;}

        public DbSet<Transaction> Transactions {get; set;}
    }
}