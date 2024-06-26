namespace BankAPI.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Type { get; set; } // Debit or Credit
        public decimal Limit { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public string PIN { get; set; }
        public int AccountId { get; set; }
    }
}
