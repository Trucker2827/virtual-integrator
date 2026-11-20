using Microsoft.EntityFrameworkCore;
using VirtualIntegrator.Data.Entities;

namespace VirtualIntegrator.Data;

public class VirtualIntegratorDbContext(DbContextOptions<VirtualIntegratorDbContext> options) : DbContext(options)
{
    public DbSet<LonDomain> Domains => Set<LonDomain>();
    public DbSet<Subnet> Subnets => Set<Subnet>();
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<NetworkVariable> NetworkVariables => Set<NetworkVariable>();
    public DbSet<Binding> Bindings => Set<Binding>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LonDomain>()
            .HasIndex(d => d.DomainId).IsUnique();

        modelBuilder.Entity<Subnet>()
            .HasIndex(s => new { s.DomainId, s.SubnetId }).IsUnique();

        modelBuilder.Entity<Node>()
            .HasIndex(n => new { n.SubnetId, n.NodeId }).IsUnique();

        modelBuilder.Entity<Node>()
            .HasIndex(n => n.NeuronId).IsUnique()
            .HasFilter("\"NeuronId\" IS NOT NULL");

        modelBuilder.Entity<NetworkVariable>()
            .HasIndex(nv => new { nv.NodeId, nv.Index }).IsUnique();

        modelBuilder.Entity<Binding>()
            .HasOne(b => b.SourceNv).WithMany().HasForeignKey(b => b.SourceNvId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Binding>()
            .HasOne(b => b.TargetNv).WithMany().HasForeignKey(b => b.TargetNvId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
