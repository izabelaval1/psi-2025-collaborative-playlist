using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using MyApi.Services; // 👈 reikalinga, kad pasiektų tavo servisus (PlaylistService, SongService, SpotifyService)

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
builder.Services.AddScoped<PlaylistService>();
builder.Services.AddScoped<SongService>();
builder.Services.AddScoped<SpotifyService>();

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

app.MapControllers(); // susieja visus controllerius automatiškai

// ===================================================
// Paleidžiam programą
// ===================================================
app.Run();
