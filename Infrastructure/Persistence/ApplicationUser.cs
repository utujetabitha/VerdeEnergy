using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence;

public sealed class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}