namespace BankAPI.Models
{
    public class TransferRequest
    {
        public string DestinationIBAN { get; set; }
        public decimal Amount { get; set; }
    }
}
