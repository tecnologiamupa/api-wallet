using Microsoft.AspNetCore.Mvc;
using WalletGoogle.Models;
using WalletGoogle.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.Configure<WalletOptions>(builder.Configuration.GetSection("Wallet"));
builder.Services.AddSingleton<GoogleWalletService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 

var app = builder.Build();

 
    app.UseSwagger();
    app.UseSwaggerUI();
 

// Endpoints Wallet
app.MapPost("/wallet/class/init", async ([FromServices] GoogleWalletService svc) =>
{
    var info = await svc.EnsureEventClassAsync();
    return Results.Ok(info);
});

app.MapPost("/wallet/create", async ([FromBody] CreatePassRequest req, [FromServices] GoogleWalletService svc) =>
{
    var result = await svc.CreateOrUpdatePassAsync(req);

    return Results.Ok(result);
});

app.MapPost("/wallet/update", async ([FromBody] UpdatePassRequest req, [FromServices] GoogleWalletService svc) =>
{
    var ok = await svc.UpdatePassAsync(req);
    return Results.Ok(new { updated = ok });
});

app.Run();