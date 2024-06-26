namespace BankAPI.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string PIN { get; set; }
        public string IBAN { get; set; }
        public decimal Balance { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual Card Card { get; set; }
    }
}
