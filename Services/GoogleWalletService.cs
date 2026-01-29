using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Walletobjects.v1;
using Google.Apis.Walletobjects.v1.Data;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WalletGoogle.Models;
using UriData = Google.Apis.Walletobjects.v1.Data.Uri;

namespace WalletGoogle.Services
{


    public class GoogleWalletService
    {

        private readonly WalletOptions _opt;
        private readonly WalletobjectsService _svc;
        private readonly JwtSigner _jwt;
        private string FullClassId => $"{_opt.IssuerId}.{_opt.ClassId}";

        public GoogleWalletService(IOptions<WalletOptions> opt)
        {
            _opt = opt.Value;
            var credential = GoogleCredential.FromFile(_opt.ServiceAccountJsonPath)
                .CreateScoped(WalletobjectsService.Scope.WalletObjectIssuer);

            _svc = new WalletobjectsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

            _jwt = new JwtSigner(_opt.ServiceAccountJsonPath);
        }

        private static LocalizedString L(string value) => new()
        {
            DefaultValue = new TranslatedString { Language = "es", Value = value }
        };

        // Crear Class si no existe
        public async Task<object> EnsureEventClassAsync()
        {
            try
            {
                // Verifica si ya existe
                await _svc.Eventticketclass.Get(FullClassId).ExecuteAsync();
                return new { classId = FullClassId, created = false };
            }
            catch
            {
                var eClass = new Google.Apis.Walletobjects.v1.Data.EventTicketClass
                {
                    Id = FullClassId, 

                    // Localización del evento (latitud, longitud, país)
                    Locations = new List<Google.Apis.Walletobjects.v1.Data.LatLongPoint>
            {
                new Google.Apis.Walletobjects.v1.Data.LatLongPoint
                {
                    Latitude = 8.937890517,
                    Longitude = -79.548051
                }
            },
                    CountryCode = "PA",

                    // Estilo de la tarjeta
                    Logo = new Google.Apis.Walletobjects.v1.Data.Image
                    {
                        SourceUri = new Google.Apis.Walletobjects.v1.Data.ImageUri
                        {
                            Uri = "https://torredecontrol.mupa.gob.pa/assets/logo.png"
                        }
                    },
                    HeroImage = new Google.Apis.Walletobjects.v1.Data.Image
                    {
                        SourceUri = new Google.Apis.Walletobjects.v1.Data.ImageUri
                        {
                            Uri = "https://vacantes.mupa.gob.pa/banner01-escritorio.webp"
                        }
                    },
                    HexBackgroundColor = "#0938c4",

                    // Mensajes
                    Messages = new List<Google.Apis.Walletobjects.v1.Data.Message>
            {
                new Google.Apis.Walletobjects.v1.Data.Message
                {
                    Id = "msg_cita_previa",
                    Header = "CV Virtual",
                    Body = "Este pase contiene un código QR que enlaza a tu CV Virtual en la plataforma de la Alcaldía de Panamá",
                    MessageType = "MESSAGE_TYPE_UNSPECIFIED"
                }
            },

                    // Información del evento
                    EventId = "empleo-feria-2025",
                    EventName = new Google.Apis.Walletobjects.v1.Data.LocalizedString
                    {
                        DefaultValue = new Google.Apis.Walletobjects.v1.Data.TranslatedString
                        {
                            Language = "es",
                            Value = "Alcaldía de Panamá"
                        }
                    },
                    Venue = new Google.Apis.Walletobjects.v1.Data.EventVenue
                    {
                        Name = new Google.Apis.Walletobjects.v1.Data.LocalizedString
                        {
                            DefaultValue = new Google.Apis.Walletobjects.v1.Data.TranslatedString
                            {
                                Language = "es",
                                Value = "Panama Convention Center"
                            }
                        },
                        Address = new Google.Apis.Walletobjects.v1.Data.LocalizedString
                        {
                            DefaultValue = new Google.Apis.Walletobjects.v1.Data.TranslatedString
                            {
                                Language = "es",
                                Value = "Calle Gral. Juan D. Peron, Panamá, Provincia de Panamá"
                            }
                        }
                    }
                };

                await _svc.Eventticketclass.Insert(eClass).ExecuteAsync();
                return new { classId = FullClassId, created = true };
            }
        }


        // Crear/obtener pase por persona
        public async Task<object> CreateOrUpdatePassAsync(CreatePassRequest req)
        {
            try
            {
                var fullClassId = $"{FullClassId}";
                var objectId = $"{_opt.IssuerId}.{req.UserId}";
                var verifyUrl = $"https://verificar.mupa.gob.pa/cvstar/{req.VerificationId}";

                EventTicketObject eObj;

                try
                {
                    // ✅ Si ya existe, lo obtenemos
                    eObj = await _svc.Eventticketobject.Get(objectId).ExecuteAsync();

                    // --- TITULAR (Nombre y Cédula) como módulo de texto grande ---
                    eObj.TextModulesData = new List<TextModuleData>
            {
                new TextModuleData { Header = "Titular", Body = $"{req.Name} – {req.Cedula}" },
                new TextModuleData { Header = "Horario", Body = req.AppointmentTimeRange }
            };

                    // --- EVENTO (más pequeño con InfoModuleData) ---
                    eObj.InfoModuleData = new InfoModuleData
                    {
                        LabelValueRows = new List<LabelValueRow>
                {
                    new LabelValueRow
                    {
                        Columns = new List<LabelValue>
                        {
                            new LabelValue { Label = "Evento", Value = req.ProgramName }
                        }
                    }
                }
                    };

                    // --- Enlaces útiles ---
                    eObj.LinksModuleData = new LinksModuleData
                    {
                        Uris = new List<UriData>
                {
                    new UriData
                    {
                        Id = "verificacion",
                        Description = "Verificar pase",
                        UriValue = verifyUrl
                    },
                    new UriData
                    {
                        Id = "sitio_oficial",
                        Description = "Sitio oficial",
                        UriValue = "https://vacantes.mupa.gob.pa/"
                    }
                }
                    };

                    // --- Ubicación opcional ---
                    eObj.Locations = (req.VenueLat.HasValue && req.VenueLng.HasValue)
                        ? new List<LatLongPoint>
                        {
                    new LatLongPoint
                    {
                        Latitude = req.VenueLat.Value,
                        Longitude = req.VenueLng.Value
                    }
                        }
                        : null;

                    // --- Intervalo de validez ---
                    eObj.ValidTimeInterval = new TimeInterval
                    {
                        Start = req.Start.HasValue
                            ? new Google.Apis.Walletobjects.v1.Data.DateTime
                            { Date = req.Start.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") }
                            : null,
                        End = req.End.HasValue
                            ? new Google.Apis.Walletobjects.v1.Data.DateTime
                            { Date = req.End.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") }
                            : null
                    };

                    // 🔄 Actualizamos
                    eObj = await _svc.Eventticketobject.Update(eObj, objectId).ExecuteAsync();

                    var tokenUpd = _jwt.BuildSaveToWalletJwt(_opt.Origins, new[] { objectId });
                    var urlUpd = $"https://pay.google.com/gp/v/save/{tokenUpd}";

                    return new { objectId, updated = true, addToWalletUrl = urlUpd };
                }
                catch
                {
                    // 🚀 Si no existe, lo creamos
                    eObj = new EventTicketObject
                    {
                        Id = objectId,
                        ClassId = fullClassId,
                        State = "ACTIVE",
                        Barcode = new Barcode
                        {
                            Type = "QR_CODE",
                            Value = verifyUrl,
                            AlternateText = $"{req.Name} – {req.Cedula}"
                        },
                        TextModulesData = new List<TextModuleData>
                {
                    new TextModuleData { Header = "Titular", Body = $"{req.Name} – {req.Cedula}" },
                    new TextModuleData { Header = "Horario", Body = req.AppointmentTimeRange }
                },
                        InfoModuleData = new InfoModuleData
                        {
                            LabelValueRows = new List<LabelValueRow>
                    {
                        new LabelValueRow
                        {
                            Columns = new List<LabelValue>
                            {
                                new LabelValue { Label = "Evento", Value = req.ProgramName }
                            }
                        }
                    }
                        },
                        LinksModuleData = new LinksModuleData
                        {
                            Uris = new List<UriData>
                    {
                        new UriData
                        {
                            Id = "verificacion",
                            Description = "Verificar pase",
                            UriValue = verifyUrl
                        },
                        new UriData
                        {
                            Id = "sitio_oficial",
                            Description = "Sitio oficial",
                            UriValue = "https://vacantes.mupa.gob.pa/"
                        }
                    }
                        },
                        Locations = (req.VenueLat.HasValue && req.VenueLng.HasValue)
                            ? new List<LatLongPoint>
                            {
                        new LatLongPoint
                        {
                            Latitude = req.VenueLat.Value,
                            Longitude = req.VenueLng.Value
                        }
                            }
                            : null,
                        ValidTimeInterval = new TimeInterval
                        {
                            Start = req.Start.HasValue
                                ? new Google.Apis.Walletobjects.v1.Data.DateTime
                                { Date = req.Start.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") }
                                : null,
                            End = req.End.HasValue
                                ? new Google.Apis.Walletobjects.v1.Data.DateTime
                                { Date = req.End.Value.UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'") }
                                : null
                        }
                    };

                    // Insertamos el pase
                    eObj = await _svc.Eventticketobject.Insert(eObj).ExecuteAsync();

                    var tokenNew = _jwt.BuildSaveToWalletJwt(_opt.Origins, new[] { objectId });
                    var urlNew = $"https://pay.google.com/gp/v/save/{tokenNew}";

                    return new { objectId, created = true, addToWalletUrl = urlNew };
                }
            }
            catch (Exception ex)
            {
                return new { error = true, message = ex.Message, stackTrace = ex.StackTrace };
            }
        }




        // Actualizar pase
        public async Task<bool> UpdatePassAsync(UpdatePassRequest req)
        {
             
            return true;
        }



    }
}
