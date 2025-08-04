using Dispatcher.Api.Middlewares;
using Dispatcher.Application.Extensions;
using Dispatcher.Domain;
using Dispatcher.Domain.Extensions;
using Dispatcher.Infrastructure;
using Dispatcher.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddDomain(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

