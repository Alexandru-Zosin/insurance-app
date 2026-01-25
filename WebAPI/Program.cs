using WebApi.Extensions;
using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWeb(); // Controllers + web concerns
builder.Services.AddApplication(); // Application usecases
builder.Services.AddInfrastructure(builder.Configuration); // (EF + repos)

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
