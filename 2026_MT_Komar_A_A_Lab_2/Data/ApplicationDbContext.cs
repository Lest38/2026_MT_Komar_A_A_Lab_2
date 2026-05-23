using Entities;
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

        public DbSet<Project> Projects { get; set; } = null!;

        public DbSet<StageType> StageTypes { get; set; } = null!;

        public DbSet<PipelineStepExecution> PipelineStepExecutions { get; set; } = null!;

        public DbSet<IssueLog> IssueLogs { get; set; } = null!;

        public DbSet<CpuModel> CpuModels { get; set; } = null!;

        public DbSet<Host> Hosts { get; set; } = null!;

        public DbSet<PerformanceTest> PerformanceTests { get; set; } = null!;

        public DbSet<ThreadSpeedMetric> ThreadSpeedMetrics { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder?.Entity<CpuModel>()
                .Property(e => e.CpuModelId).HasColumnName("Id");

            modelBuilder.Entity<Host>()
                .Property(e => e.HostId).HasColumnName("Id");

            modelBuilder.Entity<Project>()
                .Property(e => e.ProjectId).HasColumnName("Id");

            modelBuilder.Entity<StageType>()
                .Property(e => e.StageTypeId).HasColumnName("Id");

            modelBuilder.Entity<PerformanceTest>()
                .Property(e => e.PerformanceTestId).HasColumnName("Id");

            modelBuilder.Entity<PipelineStepExecution>()
                .Property(e => e.PipelineStepExecutionId).HasColumnName("Id");

            modelBuilder.Entity<IssueLog>()
                .Property(e => e.IssueLogId).HasColumnName("Id");

            modelBuilder.Entity<ThreadSpeedMetric>()
                .Property(e => e.ThreadSpeedMetricId).HasColumnName("Id");

            modelBuilder.Entity<Project>()
                .HasIndex(p => p.FolderPath).IsUnique();

            modelBuilder.Entity<StageType>()
                .HasIndex(st => st.Name).IsUnique();

            modelBuilder.Entity<PerformanceTest>()
                .HasIndex(pt => pt.Description).IsUnique();

            modelBuilder.Entity<CpuModel>()
                .HasIndex(cm => cm.ModelName).IsUnique();

            modelBuilder.Entity<StageType>().HasData(
                new StageType { StageTypeId = 1, Name = "Build" },
                new StageType { StageTypeId = 2, Name = "Test" },
                new StageType { StageTypeId = 3, Name = "Clean" },
                new StageType { StageTypeId = 4, Name = "Run" });

            modelBuilder.Entity<CpuModel>().HasData(
                new CpuModel { CpuModelId = 1, ModelName = "Intel Core i7-12700K", PhysicalCoreCount = 12, LogicalThreadCount = 20 },
                new CpuModel { CpuModelId = 2, ModelName = "AMD Ryzen 9 5900X", PhysicalCoreCount = 12, LogicalThreadCount = 24 },
                new CpuModel { CpuModelId = 3, ModelName = "Intel Core i9-13900K", PhysicalCoreCount = 24, LogicalThreadCount = 32 });

            modelBuilder.Entity<ExecutionStatus>().HasData(
    new ExecutionStatus { ExecutionStatusId = 1, Name = "Running" },
    new ExecutionStatus { ExecutionStatusId = 2, Name = "Success" },
    new ExecutionStatus { ExecutionStatusId = 3, Name = "Failed" },
    new ExecutionStatus { ExecutionStatusId = 4, Name = "Cancelled" });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!(optionsBuilder?.IsConfigured ?? false))
            {
                optionsBuilder!.UseSqlite("Data Source=app.db");
            }
        }
    }
}