using System.Reflection;
using System.Text;
using AlguienDijoChamba.Api.Customers.Application;
using AlguienDijoChamba.Api.Customers.Domain;
using AlguienDijoChamba.Api.Customers.Infrastructure.Authentication;
using AlguienDijoChamba.Api.Customers.Infrastructure.Repositories;
using AlguienDijoChamba.Api.IAM.Application;
using AlguienDijoChamba.Api.IAM.Domain;
using AlguienDijoChamba.Api.IAM.Infrastructure.Authentication;
using AlguienDijoChamba.Api.IAM.Infrastructure.Repositories;
using AlguienDijoChamba.Api.Notifications.Domain;
using AlguienDijoChamba.Api.Notifications.Infrastructure.Repositories;
using AlguienDijoChamba.Api.Professionals.Domain;
using AlguienDijoChamba.Api.Professionals.Infrastructure.ExternalServices.Reniec;
using AlguienDijoChamba.Api.Professionals.Infrastructure.Repositories;
using AlguienDijoChamba.Api.Reputation.Application.Queries;
using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Infrastructure.Repositories;
using AlguienDijoChamba.Api.Shared.ASP.Configuration.Extensions;
using AlguienDijoChamba.Api.Shared.Domain.Repositories;
using AlguienDijoChamba.Api.Shared.Infrastructure.Extensions;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC;
using AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using AlguienDijoChamba.Api.Reputation.Application;
using AlguienDijoChamba.Api.Jobs.Domain;  
using AlguienDijoChamba.Api.Jobs.Infrastructure.Repositories;
using AlguienDijoChamba.Api.Hubs; 
using Microsoft.AspNetCore.SignalR; 




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
builder.Services.AddMediatR(cfg => 
{
    // Escanea el ensamblado de la API (por si hay Handlers allí)
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); 
    
    // 🚀 AÑADIR: Escanea el ensamblado de la capa de Application donde reside el Handler
    // Usamos el 'typeof' de la nueva Query para obtener su ensamblado.
    cfg.RegisterServicesFromAssembly(typeof(SearchReputationsQuery).Assembly); 
    
    // 🔵 AGREGA TU PROPIO ASSEMBLY PARA TU HANDLER NUEVO:
    cfg.RegisterServicesFromAssembly(typeof(AlguienDijoChamba.Api.Reputation.Application.CreateReputationFromJobCommandHandler).Assembly);
});
builder.Services.AddInfrastructureServices();

// 5. Añade Repositorios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IReputationRepository, ReputationRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IJobRequestRepository, JobRequestRepository>();


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
builder.Services.AddSingleton<ICustomerJwtProvider, CustomerJwtProvider>();


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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!)),
            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Los clientes SignalR envían el token en el query string "access_token"
                var accessToken = context.Request.Query["access_token"];

                // Si hay token y la ruta es la del Hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/hubs/servicerequests")))
                {
                    // Leemos el token del Query String y lo asignamos al contexto
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
        
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddSignalR();


var app = builder.Build();

// --- 1. AÑADE ESTE BLOQUE PARA CREAR LA CARPETA 'UPLOADS' ---
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
    Console.WriteLine($"[INFO] Directorio de subidas creado: {uploadsPath}");
}

// Para aceptar peticiones de otros origenes 
app.UseCors("AllowAll");

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

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    // Usa ContentRootPath para encontrar la carpeta 'uploads'
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "uploads")),
    // Mapea la URL /uploads para que apunte a esa carpeta
    RequestPath = "/uploads" 
});
// IMPORTANTE: Estos deben ir en este orden
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Esta es la ruta que usarán los clientes Flutter y Android
// Asegúrate de que apunte al namespace correcto de tu Hub
app.MapHub<AlguienDijoChamba.Api.Hubs.ServiceRequestHub>("/hubs/servicerequests");
app.Run(); 