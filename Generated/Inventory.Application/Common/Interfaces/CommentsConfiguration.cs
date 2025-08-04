//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Domain.Entities;

public class CommentsConfiguration : IEntityTypeConfiguration<Comments>
{
    public void Configure(EntityTypeBuilder<Comments> builder)
    {

        builder.Property(x => x.Id);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content);

        builder.Property(x => x.PostId);

        builder.Property(x => x.UserId);

        builder.Property(x => x.CreatedAt);

    }
}
