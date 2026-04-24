using Entities;
using Factories;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<StageType> StageTypes { get; set; }

        public DbSet<PipelineStepExecution> PipelineStepExecutions { get; set; }

        public DbSet<IssueLog> IssueLogs { get; set; }

        public DbSet<CpuModel> CpuModels { get; set; }

        public DbSet<Host> Hosts { get; set; }

        public DbSet<PerformanceTest> PerformanceTests { get; set; }

        public DbSet<ThreadSpeedMetric> ThreadSpeedMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder?.Entity<Project>()
                .HasIndex(p => p.FolderPath)
                .IsUnique();

            modelBuilder?.Entity<StageType>()
                .HasIndex(st => st.Name)
                .IsUnique();

            modelBuilder?.Entity<PerformanceTest>()
                .HasIndex(pt => pt.Description)
                .IsUnique();

            modelBuilder?.Entity<CpuModel>()
                .HasIndex(cm => cm.ModelName)
                .IsUnique();

            modelBuilder?.Entity<StageType>().HasData(
                new StageType { Id = 1, Name = "Build" },
                new StageType { Id = 2, Name = "Test" },
                new StageType { Id = 3, Name = "Clean" },
                new StageType { Id = 4, Name = "Run" });

            modelBuilder?.Entity<CpuModel>().HasData(
                new CpuModel { Id = 1, ModelName = "Intel Core i7-12700K", PhysicalCoreCount = 12, LogicalThreadCount = 20 },
                new CpuModel { Id = 2, ModelName = "AMD Ryzen 9 5900X", PhysicalCoreCount = 12, LogicalThreadCount = 24 },
                new CpuModel { Id = 3, ModelName = "Intel Core i9-13900K", PhysicalCoreCount = 24, LogicalThreadCount = 32 });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!(optionsBuilder?.IsConfigured ?? false))
            {
                optionsBuilder.UseSqlite("Data Source=app.db");
            }
        }
    }
}