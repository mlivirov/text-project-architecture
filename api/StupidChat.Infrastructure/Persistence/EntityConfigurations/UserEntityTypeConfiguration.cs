using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StupidChat.Domain.Entities;

namespace StupidChat.Infrastructure.Persistence.EntityConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserName)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(x => x.UserName).IsUnique();
    }
}