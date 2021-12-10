using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tokens.Domain.Entities;

namespace Tokens.Infrastructure.Persistence.Database.EntityConfigurations;

public class TokenEntityTypeConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasIndex(r => r.CreatedBy);

        builder.Property(x => x.Id).HasColumnType($"char({TokenId.MAX_LENGTH})");
        builder.Property(x => x.CreatedBy).HasColumnType($"char({IdentityAddress.MAX_LENGTH})");
        builder.Property(x => x.CreatedByDevice).HasColumnType($"char({DeviceId.MAX_LENGTH})");

        builder.Ignore(a => a.Content);
    }
}
