using VIviendaYa.API.Authentication.interfaces.REST.DTOs;

namespace VIviendaYa.API.Authentication.application.@internal.commandservices;

public interface IAuthCommandService
{
    Task RegisterAsync(RegisterRequestDto request);
}