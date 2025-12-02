using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using VIviendaYa.API.Shared.Domain.Model;
using VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace VIviendaYa.API.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Application database context
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // HealthyLife DbSets
  

    
    // Authentication & Reports DbSets
    public DbSet<User> Users { get; set; }
  

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Add created/updated interceptor
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //
        // ===== HEALTHY LIFE =====
     
      

      


        // User Entity Configuration
        builder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(u => u.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(500).IsRequired();
            entity.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired();
            
            // Unique constraint for email
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Report Entity Configuration
   

        //
        // ===== NOTIFICATIONS =====
        //
     

        //
        // ===== APPOINTMENTS =====
        //
      
      
        //
        // ===== GLUCOMETER =====
        //
    

        //
        // ===== COMMUNITY =====
        //
        // CommunityPost <-> Comments (1:N)
    

        // ValueObject conversions for CommunityPost
     



       


        //
        // ===== GLOBAL NAMING CONVENTION =====
        //
        builder.UseSnakeCaseNamingConvention();
    }
}
