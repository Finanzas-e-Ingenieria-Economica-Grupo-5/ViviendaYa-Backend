using VIviendaYa.API.Authentication.interfaces.REST.DTOs;
using VIviendaYa.API.Shared.Domain.Model;

namespace VIviendaYa.API.Authentication.application.@internal.queryservices;

public interface IAuthQueryService
{
    Task<(string token, User user)> LoginAsync(LoginRequestDto request);
}