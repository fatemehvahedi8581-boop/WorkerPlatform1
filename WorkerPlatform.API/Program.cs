using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Users.Commands.Register.RegisterUserCommand).Assembly));
builder.Services.AddDbContext<Infrastructure.Persistence.AppDbContext>(options =>
    options.UseSqlite("Data Source=workerplatform.db"));
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<Infrastructure.Persistence.AppDbContext>());
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
