using System.Numerics;

namespace BankAPI.Helpers
{
    public static class IbanValidator
    {
        public static bool IsValidIban(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return false;

            iban = iban.Replace(" ", string.Empty).ToUpper();

            // Verificar la longitud mínima y máxima del IBAN
            if (iban.Length < 15 || iban.Length > 34)
                return false;

            // Mover los cuatro primeros caracteres al final del IBAN
            string rearrangedIban = iban.Substring(4) + iban.Substring(0, 4);

            // Reemplazar letras por números (A=10, B=11, ..., Z=35)
            string numericIban = string.Concat(rearrangedIban.Select(c =>
                char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

            // Convertir el IBAN a un número entero y realizar la operación de módulo 97
            BigInteger ibanNumber;
            if (!BigInteger.TryParse(numericIban, out ibanNumber))
                return false;

            return ibanNumber % 97 == 1;
        }
    }
}
