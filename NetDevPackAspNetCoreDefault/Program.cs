
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using NetDevPackAspNetCoreDefault;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services
    .AddJwksManager() // <- Use component
    .UseJwtValidation(); // <- This will instruct ASP.NET to validate the JWT token using JwksManager component


// Here we're setting a secure validation of token. Like issuer, audience.
// But instead setting a custom key, this validation was overrided by `.UseJwtValidation()`
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://localhost:7180", // <- Your website
        ValidAudience = "NetDevPack.Security.Jwt.AspNet"
    };
});

builder.Services.AddAuthorization();

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapGet("/random-jws", async (IJwtService service) =>
{
    var handler = new JsonWebTokenHandler();
    var now = DateTime.Now;
    var descriptor = new SecurityTokenDescriptor
    {
        Issuer = "https://localhost:7180", // <- Your website
        Audience = "NetDevPack.Security.Jwt.AspNet",
        IssuedAt = now,
        NotBefore = now,
        Expires = now.AddMinutes(60),
        Subject = new ClaimsIdentity(FakeClaims.GenerateClaim().Generate(5)),
        SigningCredentials = await service.GetCurrentSigningCredentials()
    };

    return handler.CreateToken(descriptor);
})
    .WithName("Generate random JWS")
    .WithTags("JWS");

app.MapGet("/random-jwe", async (IJwtService service) =>
{
    var handler = new JsonWebTokenHandler();
    var now = DateTime.Now;
    var descriptor = new SecurityTokenDescriptor
    {
        Issuer = "https://localhost:7180",
        Audience = "NetDevPack.Security.Jwt.AspNet",
        IssuedAt = now,
        NotBefore = now,
        Expires = now.AddMinutes(5),
        Subject = new ClaimsIdentity(FakeClaims.GenerateClaim().Generate(5)),
        EncryptingCredentials = await service.GetCurrentEncryptingCredentials()
    };

    return handler.CreateToken(descriptor);
})
    .WithName("Generate random JWE")
    .WithTags("JWE");

app.MapGet("/validate-jws/{jws}", async (IJwtService service, string jws) =>
{
    var handler = new JsonWebTokenHandler();

    var result = handler.ValidateToken(jws,
        new TokenValidationParameters
        {
            ValidIssuer = "https://localhost:7180",
            ValidAudience = "NetDevPack.Security.Jwt.AspNet",
            RequireSignedTokens = false,
            IssuerSigningKey = await service.GetCurrentSecurityKey(),
        });

    return result.Claims;
})
.WithName("Validate JWT (In fact jws, but no one cares)")
.WithTags("Validate");


app.MapGet("/validate-jwe/{jwe}", async (IJwtService service, string jwe) =>
{
    var handler = new JsonWebTokenHandler();

    var result = handler.ValidateToken(jwe,
        new TokenValidationParameters
        {
            ValidIssuer = "https://localhost:7180",
            ValidAudience = "NetDevPack.Security.Jwt.AspNet",
            RequireSignedTokens = false,
            TokenDecryptionKey = await service.GetCurrentSecurityKey(),
        });

    return result.Claims;
})
    .WithName("Validate JWE")
    .WithTags("Validate");

app.MapGet("/protected-endpoint", [Authorize] ([FromServices] IHttpContextAccessor context) =>
{
    return Results.Ok(context.HttpContext?.User.Claims.Select(s => new { s.Type, s.Value }));
}).WithName("Protected Endpoint")
    .WithTags("Validate");

app.Run();
