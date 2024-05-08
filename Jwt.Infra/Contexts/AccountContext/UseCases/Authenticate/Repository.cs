using Jwt.Core.Contexts.AccountContext.Entities;
using Jwt.Core.Contexts.AccountContext.UseCases.Authenticate.Contracts;
using Jwt.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Infra.Contexts.AccountContext.UseCases.Authenticate;

public class Repository : IRepository
{
    private readonly JwtContext _context;

    public Repository(JwtContext context) => _context = context;

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken) =>
        await _context
            .Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email.Address == email, cancellationToken);
}