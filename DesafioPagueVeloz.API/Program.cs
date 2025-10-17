using System.Text.Json.Serialization;
using DesafioPagueVeloz.Domain.Entities;
using DesafioPagueVeloz.Helpers;
using DesafioPagueVeloz.Persistense.Context;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Mantém nomes iguais aos do C# (opcional)
    });
builder.Services.AddSwaggerGen();
builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

    context.Database.EnsureCreated();
    if (!context.Set<Currency>().Any())
    {
        context.Set<Currency>().AddRange(
            new Currency("BRL", "Real", 1),
            new Currency("USD", "Dólar", 5),
            new Currency("EUR", "Euro", 6)
        );
        context.SaveChanges();
    }
}

app.Run();