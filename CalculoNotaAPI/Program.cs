var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/login", (LoginRequest request) =>
{
    if (request.Email == "admin" && request.Senha == "admin")
    {
        return Results.Ok(new { mensagem = "Login realizado com sucesso!" });
    }

    return Results.Unauthorized();
});

app.Run();
