using MoutsTI.Domain.Entities.Interfaces;

namespace MoutsTI.Domain.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IEmployeeModel?> AuthenticateAsync(string email, string password);
        string GenerateJwtToken(IEmployeeModel employee);
        string HashPassword(string password);
    }
}