using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "WorkerPlatform",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "WorkerPlatformUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "ThisIsASuperSecretKeyForWorkerPlatform123456!"))
        };
    });

builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Users.Commands.Register.RegisterUserCommand).Assembly));
builder.Services.AddDbContext<Infrastructure.Persistence.AppDbContext>(options =>
    options.UseSqlite("Data Source=workerplatform.db"));
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<Infrastructure.Persistence.AppDbContext>());
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "WorkerPlatform API";
        document.Info.Version = "v1";
        document.Info.Description = "API برای اتصال کارگران و کارفرمایان";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
