﻿using Gatherly.Domain.Entities;
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
            builder.ToTable("Gatherings");
                

        });

        modelBuilder.Entity<Member>(builder =>
        {

        });

        modelBuilder.Entity<Invitation>(builder =>
        {

        });

        modelBuilder.Entity<Attendee>(builder =>
        {

        });
    }
}
