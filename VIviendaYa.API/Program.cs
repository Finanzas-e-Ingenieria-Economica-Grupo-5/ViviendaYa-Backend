using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VIviendaYa.API.Authentication.application.@internal.commandservices;
using VIviendaYa.API.Authentication.application.@internal.queryservices;
using VIviendaYa.API.Shared.Domain.Repositories;
using VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ----------------- PORT -----------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ----------------- Services -----------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Configuration.AddEnvironmentVariables();

// ----------------- Swagger -----------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ViviendaYa API",
        Version = "v1",
        Description = "API for finance",
        Contact = new OpenApiContact { Name = "DevsPros Team", Email = "devspros@diabelife.com" }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });

    c.EnableAnnotations();
});

// ----------------- CORS -----------------
// Esta política permite tu front en Netlify y localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "https://kaleidoscopic-starship-6792b4.netlify.app",
                "https://vocal-monstera-ef4f92.netlify.app",
                "https://vivienda-ya-frontend-git-main-gabriels-projects-0a95c3fe.vercel.app"
                
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ----------------- DB -----------------
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "tramway.proxy.rlwy.net";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "11099";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
var dbPass = Environment.GetEnvironmentVariable("DB_PASS") ?? "MRFurSUPJTEZtEMklzxGiGDDbOEPKVNo";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "railway";

var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPass};";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString)
);

// ----------------- JWT -----------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-long-key";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ViviendaYaAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ViviendaYaClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ----------------- Repositories & Services -----------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthCommandService, AuthCommandService>();
builder.Services.AddScoped<IAuthQueryService, AuthQueryService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ----------------- Logging -----------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();

// ----------------- Middleware -----------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ViviendaYa API v1");
    c.RoutePrefix = "swagger";
});

// **CORS primero**
app.UseCors("AllowFrontend");

// HTTPS redirection solo en producción
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ----------------- Migraciones al inicio -----------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Esto aplica todas las migraciones pendientes
}

app.Run();
