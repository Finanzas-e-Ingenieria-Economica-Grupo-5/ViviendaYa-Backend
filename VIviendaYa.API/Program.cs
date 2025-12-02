

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

// ----------------- Configure PORT -----------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ----------------- Add Services -----------------
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Configuration.AddEnvironmentVariables();

// ----------------- Swagger -----------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DiabeLife API",
        Version = "v1",
        Description = "API for diabetes health metrics, recommendations, food, authentication, community, and reports.",
        Contact = new OpenApiContact
        {
            Name = "DevsPros Team",
            Email = "devspros@diabelife.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token without 'Bearer' prefix.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    c.EnableAnnotations();
});

// ----------------- CORS -----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalAndNetlify", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://diabelife-frontend.netlify.app",
                "https://diabelife-application.netlify.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ----------------- DB -----------------
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Server=localhost;Database=diabelife;Uid=root;Pwd=password;";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySQL(connectionString);
});

// ----------------- JWT -----------------
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-long-key";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "DiabeLifeAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "DiabeLifeClient";

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

// ----------------- Repositories -----------------

builder.Services.AddScoped<IUserRepository, UserRepository>();


// ----------------- Command Services -----------------

builder.Services.AddScoped<IAuthCommandService, AuthCommandService>();


// ----------------- Query Services -----------------

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DiabeLife API v1");
    c.RoutePrefix = "swagger";
});

// CORS - aplicar **una vez** y antes de Authentication/Authorization// ----------------- Middleware -----------------
// CORS: aplicar **una sola vez** y antes de Authentication/Authorization
app.UseCors("AllowLocalAndNetlify");

// HTTPS redirection: solo en producción
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// ----------------- Ensure DB -----------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
