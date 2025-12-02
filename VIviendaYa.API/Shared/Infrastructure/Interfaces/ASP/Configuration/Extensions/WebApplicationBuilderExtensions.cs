using VIviendaYa.API.Shared.Domain.Repositories;
using VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace VIviendaYa.API.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddSharedContextServices(this WebApplicationBuilder builder)
    {
        // Profiles Bounded Context Dependency Injection Configuration
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}