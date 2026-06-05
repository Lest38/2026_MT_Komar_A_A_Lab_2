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

        public DbSet<IssueCode> IssueCodes { get; set; }

        public DbSet<SeverityType> SeverityTypes { get; set; }

        public DbSet<ExecutionStatus> ExecutionStatuses { get; set; }

        public DbSet<CpuModel> CpuModels { get; set; }

        public DbSet<Host> Hosts { get; set; }

        public DbSet<OperatingSystemType> OperatingSystemTypes { get; set; }

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

            modelBuilder?.Entity<SeverityType>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder?.Entity<ExecutionStatus>()
                .HasIndex(es => es.Name)
                .IsUnique();

            modelBuilder?.Entity<IssueCode>()
                .HasIndex(ic => ic.Code)
                .IsUnique();

            modelBuilder?.Entity<OperatingSystemType>()
                .HasIndex(os => os.Name).IsUnique();

            modelBuilder?.Entity<StageType>().HasData(
                new StageType { StageTypeId = 1, Name = "Build" },
                new StageType { StageTypeId = 2, Name = "Test" },
                new StageType { StageTypeId = 3, Name = "Clean" },
                new StageType { StageTypeId = 4, Name = "Run" });

            modelBuilder?.Entity<CpuModel>().HasData(
                new CpuModel { CpuModelId = 1, ModelName = "AMD Ryzen 9 7950X", PhysicalCoreCount = 16, LogicalThreadCount = 32 },
                new CpuModel { CpuModelId = 2, ModelName = "Intel Core i9-13900K", PhysicalCoreCount = 24, LogicalThreadCount = 32 },
                new CpuModel { CpuModelId = 3, ModelName = "AMD Ryzen 5 5600X", PhysicalCoreCount = 6, LogicalThreadCount = 12 });

            modelBuilder?.Entity<SeverityType>().HasData(
                new SeverityType { SeverityTypeId = 1, Name = "Error", Description = "Compilation or runtime error" },
                new SeverityType { SeverityTypeId = 2, Name = "Warning", Description = "Non-fatal issue" },
                new SeverityType { SeverityTypeId = 3, Name = "Info", Description = "Informational message" });

            modelBuilder?.Entity<ExecutionStatus>().HasData(
                new ExecutionStatus { ExecutionStatusId = 1, Name = "Success", Description = "Step completed successfully" },
                new ExecutionStatus { ExecutionStatusId = 2, Name = "Failed", Description = "Step failed" },
                new ExecutionStatus { ExecutionStatusId = 3, Name = "Skipped", Description = "Step was skipped" },
                new ExecutionStatus { ExecutionStatusId = 4, Name = "Running", Description = "Step is in progress" });

            modelBuilder?.Entity<OperatingSystemType>().HasData(
                new OperatingSystemType { OperatingSystemTypeId = 1, Name = "Windows 11 Pro (64-bit)" },
                new OperatingSystemType { OperatingSystemTypeId = 2, Name = "Windows 10 Pro (64-bit)" },
                new OperatingSystemType { OperatingSystemTypeId = 3, Name = "Ubuntu 24.04 LTS (64-bit)" },
                new OperatingSystemType { OperatingSystemTypeId = 4, Name = "macOS Sequoia 15" });
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