using MyApi.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

//register ef core class ApplicationDbContext, use npgsql (postgresql) provider to connect to db and read default connection string from appsettings.Development.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// ✅ Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") // your frontend origin
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();

app.UseRouting();

// ✅ Enable CORS
app.UseCors("AllowLocalhost5173");

app.UseAuthorization();

app.MapControllers();

app.Run();
