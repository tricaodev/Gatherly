using Gatherly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Persistent.EFCore;

public class GatherlyDbContext : DbContext
{
    public GatherlyDbContext(DbContextOptions options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gathering>(builder =>
        {
            builder.ToTable("Gatherings").HasKey(g => g.Id);

            builder.HasMany(g => g.Attendees)
                .WithOne()
                .HasForeignKey(a => a.GatheringId)
                .IsRequired();

            builder.HasOne(g => g.Creator)
                .WithMany()
                .HasForeignKey("CreatorId")
                .IsRequired();

            builder.HasMany(g => g.Invitations)
                .WithOne()
                .HasForeignKey(i => i.GatheringId)
                .IsRequired();
        });

        modelBuilder.Entity<Member>(builder =>
        {
            builder.ToTable("Members").HasKey(m => m.Id);
        });

        modelBuilder.Entity<Invitation>(builder =>
        {
            builder.ToTable("Invitations").HasKey(i => i.Id);

            builder.HasOne<Member>()
                .WithMany()
                .HasForeignKey(i => i.MemberId)
                .IsRequired();
        });

        modelBuilder.Entity<Attendee>(builder =>
        {
            builder.ToTable("Attendees").HasKey(a => a.Id);

            builder.HasOne<Member>()
                 .WithMany()
                 .HasForeignKey(a => a.MemberId)
                 .IsRequired();
        });
    }
}
