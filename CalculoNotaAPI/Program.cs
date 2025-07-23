using CalculoNotaAPI.Models;
using System.Xml.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/login", (LoginRequest login) =>
{
    if (login.Email == "admin" && login.Senha == "admin")
        return Results.Ok(new { mensagem = "Login realizado com sucesso!" });

    return Results.Unauthorized();
});

app.MapPost("/calcular", (XmlInput input) =>
{
    try
    {
        var serializer = new XmlSerializer(typeof(NotaFiscal));
        using var reader = new StringReader(input.Xml);
        var nota = (NotaFiscal)serializer.Deserialize(reader);

        decimal fator = (decimal)Math.Pow((double)(1 - nota.Taxa), nota.Periodos);
        decimal valorLiquido = nota.ValorRecebivel * fator;

        return Results.Ok(new
        {
            ValorBruto = nota.ValorRecebivel,
            ValorLiquido = valorLiquido,
            Taxa = nota.Taxa,
            Periodos = nota.Periodos
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { erro = "Erro ao processar o XML", detalhes = ex.Message });
    }
});

app.Run();
