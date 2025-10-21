using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StupidChat.Domain.Entities;

namespace StupidChat.Infrastructure.Persistence.EntityConfigurations;

public class ChatMessageEntityTypeConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasOne<Chat>().WithMany().HasForeignKey(x => x.ChatId).IsRequired();
        builder.OwnsOne<MessageContent>(x => x.Question, cfg =>
        {
            cfg.Property(x => x.Content).IsRequired().HasMaxLength(1024);
        });
        builder.OwnsOne<MessageContent>(x => x.Answer, cfg =>
        {
            cfg.Property(x => x.Content).IsRequired().HasMaxLength(1024);
        });
    }
}