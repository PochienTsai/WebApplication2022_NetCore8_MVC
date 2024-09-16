using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2022_NetCore8_MVC.Models;

public partial class MvcUserDbContext : DbContext
{
    public MvcUserDbContext()
    {
    }

    public MvcUserDbContext(DbContextOptions<MvcUserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserTable> UserTables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=MVC_UserDB;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true");
    }

    //*** 請自己加上  {   }  大刮號（花刮號）***

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserTable");

            entity.Property(e => e.UserBirthDay).HasColumnType("datetime");
            entity.Property(e => e.UserMobilePhone).HasMaxLength(15);
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.UserSex)
                .HasMaxLength(1)
                .HasDefaultValueSql("(N'M')")
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
