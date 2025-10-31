using System.Reflection;
using System.Text;
using AlguienDijoChamba.Api.IAM.Application;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.IAM.Infrastructure.Authentication;
using AlguienDijoChamba.Api.IAM.Infrastructure.Repositories;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Professionals.Infrastructure.ExternalServices.Reniec;
using AlguienDijoChamba.Api.Professionals.Infrastructure.Repositories;
using AlguienDijoChamba.Api.Shared.ASP.Configuration.Extensions;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN DE SERVICIOS ---
var config = builder.Configuration;
config["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
config["ReniecApiKey"] = Environment.GetEnvironmentVariable("RENIEC_API_KEY");
config["Jwt:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
config["Jwt:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
config["Jwt:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

// 1. Añade Controllers y KebabCase Routing
builder.Services.AddKebabCaseRouting();
builder.Services.AddEndpointsApiExplorer();

// 2. Añade Swagger con configuración JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AlguienDijoChamba API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme { Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.Http, Scheme = "Bearer", BearerFormat = "JWT", Description = "Introduce 'Bearer' [espacio] y tu token." });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme } }, Array.Empty<string>() } });
});

// 3. Añade Persistencia (DB Context)
builder.Services.AddPersistence(config);

// 4. Añade MediatR para CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// 5. Añade Repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();

// 6. Añade Servicios Externos (Reniec)
builder.Services.AddHttpClient("ReniecApiClient", client =>
{
    client.BaseAddress = new Uri("https://api.decolecta.com/v1/reniec/");
    client.DefaultRequestHeaders.Authorization = new("Bearer", config["ReniecApiKey"]);
});
builder.Services.AddScoped<IReniecService, ReniecService>();

// 7. Añade Autenticación JWT
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.Configure<JwtOptions>(config.GetSection("Jwt"));
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!))
        };
    });

var app = builder.Build();

// --- CONFIGURACIÓN DEL PIPELINE HTTP ---
// Habilitar Swagger (puedes limitar a Development si prefieres)
app.UseSwagger();
app.UseSwaggerUI(c => 
{ 
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AlguienDijoChamba API v1"); 
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();
app.UseStaticFiles();
// IMPORTANTE: Estos deben ir en este orden
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();