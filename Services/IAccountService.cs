using BankAPI.Models;

namespace BankAPI.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync(int accountId);
        Task WithdrawAsync(int accountId, decimal amount, bool isExternalBank);
        Task DepositAsync(int accountId, decimal amount);
        Task TransferAsync(int sourceAccountId, string destinationIBAN, decimal amount);
        Task ActivateAsync(int accountId);
        Task ChangePINAsync(int accountId, string newPIN);
    }

}
