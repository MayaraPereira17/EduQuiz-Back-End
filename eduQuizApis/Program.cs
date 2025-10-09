using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// Domain Layer
using eduQuizApis.Domain.Interfaces;

// Application Layer
using eduQuizApis.Application.Interfaces;
using eduQuizApis.Application.Services;
using eduQuizApis.Application.UseCases.Auth;

// Infrastructure Layer
using eduQuizApis.Infrastructure.Data;
using eduQuizApis.Infrastructure.Repositories;
using eduQuizApis.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "EduQuiz API - Clean Architecture", 
        Version = "v1",
        Description = "API de autenticação para o sistema EduQuiz implementada com Clean Architecture"
    });
    
    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// ===== CONFIGURAÇÃO DO BANCO DE DADOS =====
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<EduQuizContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), 
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// ===== CONFIGURAÇÃO JWT =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey not found in configuration.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Remove tolerância de tempo
    };

    // Configurar eventos
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

// Configurar autorização
builder.Services.AddAuthorization();

// ===== DEPENDENCY INJECTION - DOMAIN LAYER =====
// Interfaces do domínio são registradas aqui, mas implementadas na Infrastructure

// ===== DEPENDENCY INJECTION - APPLICATION LAYER =====
// Use Cases
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RegisterUseCase>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

// ===== DEPENDENCY INJECTION - INFRASTRUCTURE LAYER =====
// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IJwtService, JwtService>();

// ===== CONFIGURAÇÃO CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    // Política mais restritiva para produção
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== CONFIGURAÇÃO DE LOGGING =====
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ===== CONFIGURAÇÃO DE HEALTH CHECKS =====
builder.Services.AddHealthChecks();

var app = builder.Build();

// ===== CONFIGURAÇÃO DO PIPELINE HTTP =====

// Health Checks
app.MapHealthChecks("/health");

// Swagger apenas em desenvolvimento
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduQuiz API v1");
//        c.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz
//        c.DisplayRequestDuration();
//    });
//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduQuiz API v1");
    c.RoutePrefix = string.Empty; // Abre o Swagger direto na raiz
    c.DisplayRequestDuration();
});
// HTTPS Redirection
app.UseHttpsRedirection();

// CORS
if (app.Environment.IsDevelopment())
{
     app.UseCors("AllowAll");
}
else
{
    app.UseCors("Production");
}

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// ===== CONFIGURAÇÃO DE BANCO DE DADOS =====
// O banco de dados deve ser criado manualmente usando o script SQL
// Execute o arquivo database_script.sql no seu banco MySQL do Railway

// Log de inicialização
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("EduQuiz API ");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Swagger disponível em: {SwaggerUrl}", app.Environment.IsDevelopment() ? "http://localhost:5034" : "Produção");

app.Run();