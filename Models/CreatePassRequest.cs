namespace WalletGoogle.Models
{
    public class CreatePassRequest
    {
        public string UserId { get; set; }                       // ej: "joel-35bd685521ec1" (dinamico)
        public string VerificationId { get; set; }               // ej: "6ecb3cd0-7b48-418d-bdac-a24e9599c704" (dinamico)
        public string Name { get; set; }                         // ej: "Joel Jimenez" (dinamico)
        public string Cedula { get; set; }                       // ej: "8-923-678" (dinamico)
        public string ProgramName { get; set; }                  // ej: "Empleo 2.0"
        public string AppointmentTimeRange { get; set; }         // ej: "12:00PM a 4:00PM" (dinámico)
        public DateOnly EventDate { get; set; }                  // ej: new DateOnly(2025, 9, 4)

        // Lugar
        public string VenueName { get; set; }                    // ej: "Panama Convention Center"
        public string VenueAddress { get; set; }                 // ej: "Amador, Panamá"
        public double? VenueLat { get; set; }                    // opcional
        public double? VenueLng { get; set; }                    // opcional

        // Opcional: puertas / inicio / fin si los tienes exactos
        public DateTimeOffset? DoorsOpen { get; set; }           // ej: 2025-09-04T08:00:00-05:00
        public DateTimeOffset? Start { get; set; }               // si no, se calcula del rango
        public DateTimeOffset? End { get; set; }
    }
}
