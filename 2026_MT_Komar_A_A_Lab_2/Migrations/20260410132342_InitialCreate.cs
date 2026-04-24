using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DesignTimeDbContextFactory.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CpuModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PhysicalCoreCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LogicalThreadCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpuModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FolderPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StageTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CpuModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    RamGb = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    OperatingSystem = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hosts_CpuModels_CpuModelId",
                        column: x => x.CpuModelId,
                        principalTable: "CpuModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineStepExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    StageTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    ExitCode = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalErrors = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalWarnings = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineStepExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PipelineStepExecutions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineStepExecutions_StageTypes_StageTypeId",
                        column: x => x.StageTypeId,
                        principalTable: "StageTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PipelineStepExecutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueLogs_PipelineStepExecutions_PipelineStepExecutionId",
                        column: x => x.PipelineStepExecutionId,
                        principalTable: "PipelineStepExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThreadSpeedMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PerformanceTestId = table.Column<int>(type: "INTEGER", nullable: false),
                    HostId = table.Column<int>(type: "INTEGER", nullable: false),
                    PipelineStepExecutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SequentialTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    ParallelTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    EfficiencyCoefficient = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadSpeedMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreadSpeedMetrics_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadSpeedMetrics_PerformanceTests_PerformanceTestId",
                        column: x => x.PerformanceTestId,
                        principalTable: "PerformanceTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadSpeedMetrics_PipelineStepExecutions_PipelineStepExecutionId",
                        column: x => x.PipelineStepExecutionId,
                        principalTable: "PipelineStepExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StageTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Build" },
                    { 2, "Test" },
                    { 3, "Clean" },
                    { 4, "Run" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CpuModels_ModelName",
                table: "CpuModels",
                column: "ModelName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_CpuModelId",
                table: "Hosts",
                column: "CpuModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLogs_PipelineStepExecutionId",
                table: "IssueLogs",
                column: "PipelineStepExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceTests_Description",
                table: "PerformanceTests",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStepExecutions_ProjectId",
                table: "PipelineStepExecutions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStepExecutions_StageTypeId",
                table: "PipelineStepExecutions",
                column: "StageTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FolderPath",
                table: "Projects",
                column: "FolderPath",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StageTypes_Name",
                table: "StageTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThreadSpeedMetrics_HostId",
                table: "ThreadSpeedMetrics",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadSpeedMetrics_PerformanceTestId",
                table: "ThreadSpeedMetrics",
                column: "PerformanceTestId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadSpeedMetrics_PipelineStepExecutionId",
                table: "ThreadSpeedMetrics",
                column: "PipelineStepExecutionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueLogs");

            migrationBuilder.DropTable(
                name: "ThreadSpeedMetrics");

            migrationBuilder.DropTable(
                name: "Hosts");

            migrationBuilder.DropTable(
                name: "PerformanceTests");

            migrationBuilder.DropTable(
                name: "PipelineStepExecutions");

            migrationBuilder.DropTable(
                name: "CpuModels");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "StageTypes");
        }
    }
}
