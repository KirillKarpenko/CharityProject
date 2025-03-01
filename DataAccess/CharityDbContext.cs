using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;
public class CharityDbContext : DbContext
{
    public DbSet<Donor> Donors { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Funding> Fundings { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CharityDb;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Donation>()
            .HasKey(d => d.Id);

        modelBuilder.Entity<Donation>()
            .HasOne(d => d.Donor)
            .WithMany(d => d.Donations)
            .HasForeignKey(d => d.DonorId);

        modelBuilder.Entity<Donation>()
            .HasOne(d => d.Organization)
            .WithMany(o => o.Donations)
            .HasForeignKey(d => d.OrganizationId);

        modelBuilder.Entity<Organization>()
            .HasKey(o => o.Id);

        modelBuilder.Entity<Funding>()
             .HasKey(d => d.Id);

        modelBuilder.Entity<Funding>()
            .HasOne(f => f.Organization)
            .WithMany(o => o.Fundings)
            .HasForeignKey(f => f.OrganizationId);

        modelBuilder.Entity<Funding>()
            .HasOne(f => f.Project)
            .WithMany(p => p.Fundings)
            .HasForeignKey(f => f.ProjectId);

        modelBuilder.Entity<Project>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Report>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Project)
            .WithOne(p => p.Report)
            .HasForeignKey<Report>(r => r.ProjectId);

        DbSeeder.SeedDb(modelBuilder);
    }
}
