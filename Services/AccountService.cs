using BankAPI.Data;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly BankingContext _context;

        public AccountService(BankingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(int accountId)
        {
            return await _context.Transactions
                                 .Where(t => t.AccountId == accountId)
                                 .ToListAsync();
        }

        public async Task WithdrawAsync(int accountId, decimal amount, bool isExternalBank)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            decimal fee = isExternalBank ? 1.5m : 0; // Example fee
            decimal totalAmount = amount + fee;

            if (totalAmount > account.Card.Limit)
            {
                throw new Exception("Withdrawal amount exceeds card limit");
            }

            if (account.Balance < totalAmount && account.Card.Type == "Debit")
            {
                throw new Exception("Insufficient funds");
            }

            account.Balance -= totalAmount;
            _context.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Amount = -amount,
                Date = DateTime.Now,
                Type = "Withdrawal",
            });

            if (isExternalBank)
            {
                _context.Transactions.Add(new Transaction
                {
                    AccountId = accountId,
                    Amount = -fee,
                    Date = DateTime.Now,
                    Type = "Fee",
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DepositAsync(int accountId, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            account.Balance += amount;
            _context.Transactions.Add(new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                Date = DateTime.Now,
                Type = "Deposit",
            });

            await _context.SaveChangesAsync();
        }

        public async Task TransferAsync(int sourceAccountId, string destinationIBAN, decimal amount)
        {
            var sourceAccount = await _context.Accounts.FindAsync(sourceAccountId);
            if (sourceAccount == null)
            {
                throw new Exception("Source account not found");
            }

            var destinationAccount = await _context.Accounts
                                                   .FirstOrDefaultAsync(a => a.IBAN == destinationIBAN);
            if (destinationAccount == null)
            {
                throw new Exception("Destination account not found");
            }

            if (sourceAccount.Balance < amount)
            {
                throw new Exception("Insufficient funds");
            }

            sourceAccount.Balance -= amount;
            destinationAccount.Balance += amount;

            _context.Transactions.Add(new Transaction
            {
                AccountId = sourceAccountId,
                Amount = -amount,
                Date = DateTime.Now,
                Type = "Transfer",
            });

            _context.Transactions.Add(new Transaction
            {
                AccountId = destinationAccount.Id,
                Amount = amount,
                Date = DateTime.Now,
                Type = "Transfer",
            });

            await _context.SaveChangesAsync();
        }
    }
}
