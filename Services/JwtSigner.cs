using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Linq;
using System.Security.Claims;                 // <-- importante

public class JwtSigner
{
    private readonly string _clientEmail;
    private readonly RSA _rsa;

    public JwtSigner(string serviceAccountJsonPath)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(serviceAccountJsonPath));
        var root = doc.RootElement;

        _clientEmail = root.GetProperty("client_email").GetString()!;
        var privateKeyPem = root.GetProperty("private_key").GetString()!;

        _rsa = RSA.Create();
        _rsa.ImportFromPem(privateKeyPem.ToCharArray());
    }

    public string BuildSaveToWalletJwt(string[] origins, string[] genericObjectIds)
    {
        // Construir la lista [{"id": "<issuerId>.<algo>"} , ...]
        var genericObjects = genericObjectIds
            .Select(id => new Dictionary<string, object> { { "id", id } })
            .ToList();

        var body = new JwtPayload
        {
            { "iss", _clientEmail },
            { "aud", "google" },
            { "typ", "savetowallet" },
            { "origins", origins }, // string[]
            { "payload", new Dictionary<string, object> {
                { "genericObjects", genericObjects }
            }}
        };

        // Firmar RS256
        var creds = new SigningCredentials(new RsaSecurityKey(_rsa), SecurityAlgorithms.RsaSha256);
        var header = new JwtHeader(creds);
        var token = new JwtSecurityToken(header, body);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}