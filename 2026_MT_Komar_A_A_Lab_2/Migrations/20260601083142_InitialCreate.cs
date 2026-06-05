using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _2026_MT_Komar_A_A_Lab_2.Migrations
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
                    CpuModelId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PhysicalCoreCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LogicalThreadCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CpuModels", x => x.CpuModelId);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionStatuses",
                columns: table => new
                {
                    ExecutionStatusId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionStatuses", x => x.ExecutionStatusId);
                });

            migrationBuilder.CreateTable(
                name: "IssueCodes",
                columns: table => new
                {
                    IssueCodeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueCodes", x => x.IssueCodeId);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceTests",
                columns: table => new
                {
                    PerformanceTestId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceTests", x => x.PerformanceTestId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FolderPath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "SeverityTypes",
                columns: table => new
                {
                    SeverityTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeverityTypes", x => x.SeverityTypeId);
                });

            migrationBuilder.CreateTable(
                name: "StageTypes",
                columns: table => new
                {
                    StageTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTypes", x => x.StageTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Hosts",
                columns: table => new
                {
                    HostId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CpuModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    RamGb = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    OperatingSystem = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hosts", x => x.HostId);
                    table.ForeignKey(
                        name: "FK_Hosts_CpuModels_CpuModelId",
                        column: x => x.CpuModelId,
                        principalTable: "CpuModels",
                        principalColumn: "CpuModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PipelineStepExecutions",
                columns: table => new
                {
                    PipelineStepExecutionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    StageTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutionStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false),
                    ExitCode = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PipelineStepExecutions", x => x.PipelineStepExecutionId);
                    table.ForeignKey(
                        name: "FK_PipelineStepExecutions_ExecutionStatuses_ExecutionStatusId",
                        column: x => x.ExecutionStatusId,
                        principalTable: "ExecutionStatuses",
                        principalColumn: "ExecutionStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineStepExecutions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PipelineStepExecutions_StageTypes_StageTypeId",
                        column: x => x.StageTypeId,
                        principalTable: "StageTypes",
                        principalColumn: "StageTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IssueLogs",
                columns: table => new
                {
                    IssueLogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PipelineStepExecutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SeverityTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IssueCodeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueLogs", x => x.IssueLogId);
                    table.ForeignKey(
                        name: "FK_IssueLogs_IssueCodes_IssueCodeId",
                        column: x => x.IssueCodeId,
                        principalTable: "IssueCodes",
                        principalColumn: "IssueCodeId");
                    table.ForeignKey(
                        name: "FK_IssueLogs_PipelineStepExecutions_PipelineStepExecutionId",
                        column: x => x.PipelineStepExecutionId,
                        principalTable: "PipelineStepExecutions",
                        principalColumn: "PipelineStepExecutionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueLogs_SeverityTypes_SeverityTypeId",
                        column: x => x.SeverityTypeId,
                        principalTable: "SeverityTypes",
                        principalColumn: "SeverityTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThreadSpeedMetrics",
                columns: table => new
                {
                    ThreadSpeedMetricId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PerformanceTestId = table.Column<int>(type: "INTEGER", nullable: false),
                    HostId = table.Column<int>(type: "INTEGER", nullable: false),
                    PipelineStepExecutionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SequentialTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    ParallelTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DurationMs = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadSpeedMetrics", x => x.ThreadSpeedMetricId);
                    table.ForeignKey(
                        name: "FK_ThreadSpeedMetrics_Hosts_HostId",
                        column: x => x.HostId,
                        principalTable: "Hosts",
                        principalColumn: "HostId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadSpeedMetrics_PerformanceTests_PerformanceTestId",
                        column: x => x.PerformanceTestId,
                        principalTable: "PerformanceTests",
                        principalColumn: "PerformanceTestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadSpeedMetrics_PipelineStepExecutions_PipelineStepExecutionId",
                        column: x => x.PipelineStepExecutionId,
                        principalTable: "PipelineStepExecutions",
                        principalColumn: "PipelineStepExecutionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExecutionStatuses",
                columns: new[] { "ExecutionStatusId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Step completed successfully", "Success" },
                    { 2, "Step failed", "Failed" },
                    { 3, "Step was skipped", "Skipped" },
                    { 4, "Step is in progress", "Running" }
                });

            migrationBuilder.InsertData(
                table: "SeverityTypes",
                columns: new[] { "SeverityTypeId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Compilation or runtime error", "Error" },
                    { 2, "Non-fatal issue", "Warning" },
                    { 3, "Informational message", "Info" }
                });

            migrationBuilder.InsertData(
                table: "StageTypes",
                columns: new[] { "StageTypeId", "Name" },
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
                name: "IX_ExecutionStatuses_Name",
                table: "ExecutionStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hosts_CpuModelId",
                table: "Hosts",
                column: "CpuModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueCodes_Code",
                table: "IssueCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueLogs_IssueCodeId",
                table: "IssueLogs",
                column: "IssueCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLogs_PipelineStepExecutionId",
                table: "IssueLogs",
                column: "PipelineStepExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLogs_SeverityTypeId",
                table: "IssueLogs",
                column: "SeverityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceTests_Description",
                table: "PerformanceTests",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PipelineStepExecutions_ExecutionStatusId",
                table: "PipelineStepExecutions",
                column: "ExecutionStatusId");

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
                name: "IX_SeverityTypes_Name",
                table: "SeverityTypes",
                column: "Name",
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
                name: "IssueCodes");

            migrationBuilder.DropTable(
                name: "SeverityTypes");

            migrationBuilder.DropTable(
                name: "Hosts");

            migrationBuilder.DropTable(
                name: "PerformanceTests");

            migrationBuilder.DropTable(
                name: "PipelineStepExecutions");

            migrationBuilder.DropTable(
                name: "CpuModels");

            migrationBuilder.DropTable(
                name: "ExecutionStatuses");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "StageTypes");
        }
    }
}
