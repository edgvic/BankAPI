using BankAPI.Models;

namespace BankAPI.Helpers
{
    public static class OperationValidator
    {
        public static bool IsValidOperation(Account account)
        {
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            if (!account.Card.IsActive)
            {
                throw new Exception("Card is not active");
            }

            if (account.PIN == "")
            {
                throw new Exception("PIN not set");
            }

            return true;
        }
    }
}
