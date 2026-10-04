using System.Security.Claims;
using IS7012_FinalProject.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace IS7012_FinalProject.Services
{
    public class AppRoleClaimsTransformation : IClaimsTransformation
    {
        private readonly ApplicationDbContext _context;

        public AppRoleClaimsTransformation(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ClaimsPrincipal> TransformAsync(
            ClaimsPrincipal principal)
        {
            if (principal.Identity is not ClaimsIdentity identity ||
                !identity.IsAuthenticated)
            {
                return principal;
            }

            if (identity.HasClaim(c => c.Type == "AppRole"))
            {
                return principal;
            }

            var email =
                principal.FindFirst("email")?.Value;

            if (string.IsNullOrWhiteSpace(email))
            {
                return principal;
            }

            var employee = await _context.Employees
                .Include(e => e.JobRole)
                .FirstOrDefaultAsync(e =>
                    e.Email.ToLower() == email.ToLower());

            if (employee?.JobRole != null)
            {
                identity.AddClaim(
                    new Claim(
                        "AppRole",
                        employee.JobRole.Title));
            }

            return principal;
        }
    }
}