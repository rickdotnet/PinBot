using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PinBot.Data.Entities;

#nullable disable

namespace PinBot.Data
{
    public partial class PinBotContext : DbContext
    {
        public PinBotContext(DbContextOptions<PinBotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Authorization> Authorizations { get; set; }
        public virtual DbSet<Pin> Pins { get; set; }
        public virtual DbSet<PinBoardMapping> PinBoardMappings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_unicode_ci");

            modelBuilder.Entity<Authorization>(entity =>
            {
                entity.Property(e => e.AuthorizationId).HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.GuildOrChannelId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasComment("The Guild or ChannelId to be authorized to");

                entity.Property(e => e.UserOrRoleId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasComment("The UserId or RoleId to be authorized");
            });

            modelBuilder.Entity<Pin>(entity =>
            {
                entity.Property(e => e.PinId).HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.ChannelId).HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.GuildId).HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.IsMessageDeleted)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.IsPinRemoved)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'");

                entity.Property(e => e.MessageId).HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.PinnedUserId).HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Timestamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<PinBoardMapping>(entity =>
            {
                entity.Property(e => e.PinBoardMappingId).HasColumnType("smallint(5) unsigned");

                entity.Property(e => e.IsGlobalBoard)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'")
                    .HasComment("All pinned messages go to this channel");

                entity.Property(e => e.PinBoardChannelId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasComment("The channel that pinned messages go to");

                entity.Property(e => e.PinnedMessageChannelId)
                    .HasColumnType("bigint(20) unsigned")
                    .HasComment("The channel that messages are pinned in");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
