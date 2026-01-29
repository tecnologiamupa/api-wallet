namespace WalletGoogle.Models
{
    public class WalletOptions
    {
        public string IssuerId { get; set; } = default!;
        public string ClassId { get; set; } = default!;
        public string[] Origins { get; set; } = Array.Empty<string>();
        public string ServiceAccountJsonPath { get; set; } = default!;
    }
}
