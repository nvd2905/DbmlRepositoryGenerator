//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Domain.Entities;

public class PostsConfiguration : IEntityTypeConfiguration<Posts>
{
    public void Configure(EntityTypeBuilder<Posts> builder)
    {

        builder.Property(x => x.Id);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title);

        builder.Property(x => x.Content);

        builder.Property(x => x.UserId);

        builder.Property(x => x.PublishedAt);

        builder.Property(x => x.CreatedAt);

        builder.Property(x => x.UpdatedAt);

    }
}
