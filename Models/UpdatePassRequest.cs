namespace WalletGoogle.Models
{
    public class UpdatePassRequest
    {
        public string UserId { get; set; } = default!;
        public string? Name { get; set; }
        public string? Cedula { get; set; }
        public string? AppointmentTime { get; set; }
        public string? VerificationId { get; set; }
        public bool? EmbedDataInQr { get; set; }
        public string? NotifyTitle { get; set; }   // ej. "Actualización de cita"
        public string? NotifyBody { get; set; }   // ej. "Tu hora cambió a 10:30 AM."
    }
}
