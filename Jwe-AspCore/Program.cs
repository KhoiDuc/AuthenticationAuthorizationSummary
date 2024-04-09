using Jwe_AspCore.Models;
using Jwe_AspCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

static string GenerateRandomEncryptKey(int length)
{
    const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"; // Valid characters for the key
    Random random = new Random();
    char[] keyChars = new char[length];

    for (int i = 0; i < length; i++)
    {
        keyChars[i] = validChars[random.Next(validChars.Length)];
    }

    return new string(keyChars);
}
static string GenerateRandomSecretKey(int length)
{
    const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=[]{}|;:,.<>?";
    StringBuilder stringBuilder = new StringBuilder();

    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
        byte[] randomBytes = new byte[length];
        rng.GetBytes(randomBytes);

        for (int i = 0; i < length; i++)
        {
            int index = randomBytes[i] % validChars.Length;
            stringBuilder.Append(validChars[index]);
        }
    }

    return stringBuilder.ToString();
}

string encryptKey = GenerateRandomEncryptKey(16);
string secretKey = GenerateRandomSecretKey(64);

var jwtSettings = new JwtSettings();
configuration.Bind("JwtSettings", jwtSettings);

builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

// Get a read-only snapshot of the configuration
var jwtSettingsSnapshot = builder.Services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<JwtSettings>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretkey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var encryptionkey = Encoding.UTF8.GetBytes(jwtSettings.Encryptkey);

        var validationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero, // default: 5 min
            RequireSignedTokens = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretkey),

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ValidateAudience = true, //default : false
            ValidAudience = jwtSettings.Audience,

            ValidateIssuer = true, //default : false
            ValidIssuer = jwtSettings.Issuer,

            TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
        };

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = validationParameters;
    }
);

builder.Services.AddTransient<IJwtService, JwtService>();  // Assuming JwtService is still needed
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
