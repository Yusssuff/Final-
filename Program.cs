using Final_Task.Data;
using SalesBuzz.Shared.Authorization;
using SalesBuzz.Shared.Data;
using SalesBuzz.Shared.Middleware;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddSalesBuzzCurrentBU();

builder.Services.AddSalesBuzzDb<AppDbContext>(
    builder.Configuration
);

builder.Services.AddSalesBuzzJwt(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

// Rejects any [Authorize] request whose JWT jti isn't a live row in SA_Sessions
// (i.e. logged out / expired / never-issued-via-our-Login sessions).
app.UseSalesBuzzTokenValidation();

app.UseAuthorization();

app.MapControllers();

app.Run();