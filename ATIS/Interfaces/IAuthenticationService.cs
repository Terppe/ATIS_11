using ATIS.Models;

namespace ATIS.Interfaces;

public interface IAuthenticationService
{
    Task<TblUserProfile?> AuthenticateUserAsync(string email, string password, CancellationToken ct = default);

    Task<TblUserProfile> RegisterAsync(string email, string password, string? role = null, CancellationToken ct = default);

    Task<bool> RequestPasswordResetAsync(string email, CancellationToken ct = default);
    Task ResetPasswordWithCodeAsync(string email, string code, string newPassword, CancellationToken ct = default);

    Task ChangePasswordAsync(int userProfileId, string currentPassword, string newPassword, CancellationToken ct = default);

    Task ChangePasswordByEmailAsync(string email, string currentPassword, string newPassword, CancellationToken ct = default);

    Task DeleteOwnAccountAsync(string email, string currentPassword, CancellationToken ct = default);
}
