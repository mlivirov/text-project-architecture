using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StupidChat.Domain.Entities;

namespace StupidChat.Infrastructure.Persistence.EntityConfigurations;

public class ChatEntityTypeConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(chat => chat.Id);
        builder.Property(chat => chat.CreatedAt).IsRequired();

        builder.Property(chat => chat.UserId).IsRequired();
        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).IsRequired();
    }
}