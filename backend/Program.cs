using Microsoft.EntityFrameworkCore;
using MyApi.Models;

var builder = WebApplication.CreateBuilder(args); // 🚀✨

// Add HttpClient 🌐
builder.Services.AddHttpClient();

// Read directly from appsettings.Development.json 📖🔧
builder.Services.AddDbContext<PlaylistAppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());


builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteDev", policy =>
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

// Add controllers with JSON options to prevent circular references 🔄🛡️
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

//app.UseHttpsRedirection(); // ❌🔒

app.UseRouting();

app.UseCors("ViteDev");

app.UseAuthorization();

app.MapControllers();

app.Run();
