using Bcp.Domain.Enums;
using Bcp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Bcp.Infrastructure.Persistence;

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
            _ = entity.HasAlternateKey(b => new { b.StoreName, b.OwnerName }); // Composite uniqueness
            _ = entity.Property(b => b.StoreId).ValueGeneratedOnAdd();
        });

        _ = modelBuilder.Entity<Beneficiary>(entity =>
    {
        _ = entity.HasKey(b => b.BeneficiaryId);
        _ = entity.Property(b => b.Cpf).IsRequired().HasMaxLength(11);
        _ = entity.Property(b => b.Card).IsRequired().HasMaxLength(12);
        _ = entity.HasAlternateKey(b => new { b.Cpf, b.Card }); // Composite uniqueness
        _ = entity.Property(b => b.BeneficiaryId).ValueGeneratedOnAdd();
    });

        _ = modelBuilder.Entity<Bcp.Domain.Models.File>(entity =>
        {
            _ = entity.HasKey(f => f.FileId);
            _ = entity.HasMany(f => f.Transactions)
                  .WithOne(fn => fn.File)
                  .HasForeignKey(fn => fn.FileId)
                  .OnDelete(DeleteBehavior.Cascade);
            _ = entity.HasMany(f => f.Error)
                  .WithOne(fn => fn.File)
                  .HasForeignKey(fn => fn.FileId)
                  .OnDelete(DeleteBehavior.Cascade);
            _ = entity.Property(b => b.FileId).ValueGeneratedOnAdd();
        });

        _ = modelBuilder.Entity<Transaction>(entity =>
        {
            _ = entity.HasKey(t => new { t.TransactionId });
            _ = entity.HasOne(t => t.File).WithMany(f => f.Transactions).HasForeignKey(t => t.FileId).IsRequired();
            _ = entity.HasOne(t => t.Beneficiary).WithMany().HasForeignKey(t => t.BeneficiaryId).IsRequired();
            _ = entity.HasOne(t => t.Store).WithMany().HasForeignKey(t => t.StoreId).IsRequired();
            _ = entity.HasOne(t => t.TransactionType).WithMany().HasForeignKey(t => t.TransactionTypeId).IsRequired();
            _ = entity.Property(b => b.TransactionId).ValueGeneratedOnAdd();
        });

        _ = modelBuilder.Entity<FileError>(entity =>
        {
            _ = entity.HasKey(t => new { t.ErrorId });
            _ = entity.HasOne(t => t.File).WithMany(f => f.Error).HasForeignKey(t => t.FileId).IsRequired();
            _ = entity.Property(b => b.Error).IsRequired();
            _ = entity.Property(b => b.ErrorId).ValueGeneratedOnAdd();
        });

        _ = modelBuilder.Entity<FileNotification>(entity =>
        {
            _ = entity.HasKey(b => b.FileNotificationId);
            _ = entity.Property(b => b.FileName).IsRequired().HasMaxLength(255);
            _ = entity.Property(b => b.CreatedAt).IsRequired();
            _ = entity.Property(b => b.Status).IsRequired();
            _ = entity.HasIndex(b => b.Status);
            _ = entity.Property(b => b.FileNotificationId).ValueGeneratedOnAdd();
        });
    }

    public DbSet<Bcp.Domain.Models.File> Files { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Beneficiary> Beneficiaries { get; set; }
    public DbSet<FileNotification> FileNotifications { get; set; }
    public DbSet<FileError> FileError { get; set; }
}