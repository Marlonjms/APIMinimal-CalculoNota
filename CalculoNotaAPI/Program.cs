using System.Xml.Serialization;
using CalculoNotaAPI.Models;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173",                                  // Frontend local
                    "https://dda5d6517c3c.ngrok-free.app",                   // URL pública ngrok
                    "chrome-extension://kllanimkmjifhopglllfckdploiijcgh"    // Sua extensão Chrome
                )
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware CORS
app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/calcular", async (IFormFile xmlFile, DateTime dataRecebimento) =>
{
    if (xmlFile == null)
        return Results.BadRequest(new { erro = "Arquivo XML é obrigatório." });

    try
    {
        using var stream = xmlFile.OpenReadStream();
        var serializer = new XmlSerializer(typeof(NotaFiscal));
        var nota = (NotaFiscal)serializer.Deserialize(stream);

        if (nota?.Totais == null)
            return Results.BadRequest(new { erro = "Totais não encontrados no XML." });

        var hoje = DateTime.Today;
        int periodoDias = (dataRecebimento - hoje).Days;

        if (periodoDias < 0)
            return Results.BadRequest(new { erro = "Data de recebimento não pode ser no passado." });

        decimal taxaDiaria = 0.00066m;
        decimal valorBruto = nota.Totais.ValorTotalNota;
        decimal fator = (decimal)Math.Pow((double)(1 - taxaDiaria), periodoDias);
        decimal valorLiquido = valorBruto * fator;

        return Results.Ok(new
        {
            ValorBruto = valorBruto,
            ValorLiquido = Math.Round(valorLiquido, 2),
            TaxaDiaria = taxaDiaria,
            PeriodoDias = periodoDias
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { erro = "Erro ao processar o XML", detalhes = ex.Message });
    }
})
.DisableAntiforgery();

app.Run();
