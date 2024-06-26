using BankAPI.Data;
using BankAPI.Helpers;
using BankAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

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
            var account = await _context.Accounts.FindAsync(accountId);

            OperationValidator.IsValidOperation(account);

            return await _context.Transactions
                                 .Where(t => t.AccountId == accountId)
                                 .ToListAsync();
        }

        public async Task WithdrawAsync(int accountId, decimal amount, bool isExternalBank)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            OperationValidator.IsValidOperation(account);

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

            OperationValidator.IsValidOperation(account);

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

            OperationValidator.IsValidOperation(sourceAccount);

            if (IbanValidator.IsValidIban(destinationIBAN))
            {
                throw new Exception("IBAN not valid");
            }

            var destinationAccount = await _context.Accounts
                                                   .FirstOrDefaultAsync(a => a.IBAN == destinationIBAN);

            if (destinationAccount == null)
            {
                decimal fee = 1.2m; // Example fee
                amount += fee;
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

        public async Task ActivateAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if (account.Card.IsActive)
            {
                throw new Exception("Card is already active");
            }
            else
            {
                account.Card.IsActive = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ChangePINAsync(int accountId, string newPIN)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            account.PIN = newPIN;
            await _context.SaveChangesAsync();
        }
    }
}
