//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Domain.Entities;

public class UsersConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {

        builder.Property(x => x.Id);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username);

        builder.Property(x => x.Email);

        builder.Property(x => x.PasswordHash);

        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.IsActive);

    }
}
