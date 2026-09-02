using InvestFlowAPI.Data;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// 1. Read the PORT environment variable provided by the hosting platform
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";

// 2. Tell the web host to listen on all network interfaces using that port
builder.WebHost.UseUrls($"http://0.0.0:{port}");

// ... Keep all your existing service configurations below (Controllers, Swagger, etc.)
builder.Services.AddControllers();
// ==========================================
// DATABASE
// ==========================================
builder.Services.AddDbContext<InvestFlowDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
// ==========================================
// CORS
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowInvestFlowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// ==========================================
// CONTROLLERS
// ==========================================
builder.Services.AddControllers();
// ==========================================
// SWAGGER
// ==========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
// ==========================================
// SWAGGER
// ==========================================
app.UseSwagger();
app.UseSwaggerUI();
// ==========================================
// CORS
// ==========================================
app.UseCors("AllowInvestFlowFrontend");
// ==========================================
// AUTHORIZATION
// ==========================================
app.UseAuthorization();
// ==========================================
// CONTROLLERS
// ==========================================
app.MapControllers();
app.Run();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InvestFlowDbContext>();
    db.Database.Migrate();
}