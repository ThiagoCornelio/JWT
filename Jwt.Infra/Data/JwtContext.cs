using Jwt.Core.Contexts.AccountContext.Entities;
using Jwt.Infra.Context.AccountContext.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Infra.Data;

public class JwtContext : DbContext
{
    public JwtContext(DbContextOptions<JwtContext> options)
    : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMap());
        modelBuilder.ApplyConfiguration(new RoleMap());
    }
}
