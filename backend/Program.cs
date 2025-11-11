using Microsoft.EntityFrameworkCore;
using MyApi.Interfaces;
using MyApi.Models;
using MyApi.Data;
using MyApi.Services; //  reikalinga, kad pasiektų tavo servisus (PlaylistService, SongService, SpotifyService)
using Microsoft.AspNetCore.Authetication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args); // 🚀 Programos paleidimo taškas

// ===================================================
//  HttpClient — naudojamas Spotify API paieškai
// ===================================================
builder.Services.AddHttpClient();

// ===================================================
//  Duomenų bazės konfigūracija (PostgreSQL per EF Core)
// ===================================================
builder.Services.AddDbContext<PlaylistAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// ===================================================
//  Servisų registravimas (Dependency Injection)
// ===================================================
// Kiekvienas servisų instance bus sukurtas per užklausą (Scoped)
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<ISpotifyService, SpotifyService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // login
builder.Services.AddScoped<IUserService, UserService>(); //login

// ===================================================
//  CORS — leidžiam frontend'ui jungtis prie API
// ===================================================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod(); // GET, POST, PUT, DELETE
    });
});

// ===================================================
// Controllers + JSON nustatymai
// ===================================================
// Kad nebūtų ciklinių nuorodų (pvz. Playlist -> Songs -> Playlist)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// ===================================================
//  Sukuriam WebApplication objektą
// ===================================================
var app = builder.Build();

// ===================================================
//  Middleware pipeline (užklausų apdorojimo seka)
// ===================================================
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors(); // leidžia frontend’ui pasiekti API

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers(); // susieja visus controllerius automatiškai

// ===================================================
// Paleidžiam programą
// ===================================================
app.Run();
