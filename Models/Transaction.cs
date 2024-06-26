namespace BankAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // Deposit, Withdrawal, Transfer, Fee
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
