using bcp.Core.Enums;
using bcp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace bcp.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.Entity<TransactionType>(entity =>
        {
            _ = entity.HasKey(t => t.TransactionTypeId);
            _ = entity.Property(t => t.Nature).IsRequired();
        });

        _ = modelBuilder.Entity<TransactionType>().HasData(
            new TransactionType { TransactionTypeId = 1, Description = "Debit", Nature = TransactionNature.Income },
            new TransactionType { TransactionTypeId = 2, Description = "Boleto", Nature = TransactionNature.Expense },
            new TransactionType { TransactionTypeId = 3, Description = "Financing", Nature = TransactionNature.Expense },
            new TransactionType { TransactionTypeId = 4, Description = "Credit", Nature = TransactionNature.Income },
            new TransactionType { TransactionTypeId = 5, Description = "Loan Receipt", Nature = TransactionNature.Income },
            new TransactionType { TransactionTypeId = 6, Description = "Sales", Nature = TransactionNature.Income },
            new TransactionType { TransactionTypeId = 7, Description = "TED Receipt", Nature = TransactionNature.Income },
            new TransactionType { TransactionTypeId = 8, Description = "DOC Receipt", Nature = TransactionNature.Income },
            new TransactionType { TransactionTypeId = 9, Description = "Rent", Nature = TransactionNature.Expense }
        );

        _ = modelBuilder.Entity<Store>(entity =>
        {
            _ = entity.HasKey(s => s.StoreId);
            _ = entity.Property(s => s.StoreName).IsRequired().HasMaxLength(19);
            _ = entity.Property(s => s.OwnerName).IsRequired().HasMaxLength(14);
        });

        _ = modelBuilder.Entity<Beneficiary>(entity =>
        {
            _ = entity.HasKey(b => b.BeneficiaryId);
            _ = entity.Property(b => b.Cpf).IsRequired().HasMaxLength(11);
            _ = entity.Property(b => b.Card).IsRequired().HasMaxLength(12);
            _ = entity.HasAlternateKey(b => new { b.Cpf, b.Card }); // Composite uniqueness
        });

        _ = modelBuilder.Entity<Core.Models.File>(entity =>
        {
            _ = entity.HasKey(f => f.FileId);
            _ = entity.Property(f => f.FileSize).IsRequired();
            _ = entity.Property(f => f.FileHash).IsRequired().HasMaxLength(32); // Based on MD5 hash format 32 in length
            _ = entity.HasIndex(f => new { f.FileSize, f.FileHash }).IsUnique(); // Enforces deduplication
            _ = entity.HasMany(f => f.FileNames)
                  .WithOne(fn => fn.File)
                  .HasForeignKey(fn => fn.FileId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);
            _ = entity.HasMany(f => f.Transactions)
                  .WithOne(fn => fn.File)
                  .HasForeignKey(fn => fn.FileId)
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);
        });

        _ = modelBuilder.Entity<FileName>(entity =>
        {
            _ = entity.HasKey(fn => fn.FileNameId);
            _ = entity.Property(fn => fn.Name).IsRequired().HasMaxLength(255); // Adjust based on filename limits
            _ = entity.Property(fn => fn.FileId).IsRequired();
            _ = entity.HasIndex(fn => new { fn.FileId, fn.Name }).IsUnique(); // Prevent duplicate names for the same file
        });

        _ = modelBuilder.Entity<Transaction>(entity =>
        {
            _ = entity.HasKey(t => new { t.TransactionId });
            _ = entity.HasOne(t => t.File).WithMany(f => f.Transactions).HasForeignKey(t => t.FileId).IsRequired();
            _ = entity.HasOne(t => t.Beneficiary).WithMany().HasForeignKey(t => t.BeneficiaryId).IsRequired();
            _ = entity.HasOne(t => t.Store).WithMany().HasForeignKey(t => t.StoreId).IsRequired();
            _ = entity.HasOne(t => t.TransactionType).WithMany().HasForeignKey(t => t.TransactionTypeId).IsRequired();
        });
    }

    public DbSet<Core.Models.File> Files { get; set; }
    public DbSet<FileName> FileNames { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Store> Stores { get; set; }
}