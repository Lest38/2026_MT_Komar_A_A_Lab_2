using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _2026_MT_Komar_A_A_Lab_2.Migrations
{
    /// <inheritdoc />
    public partial class AddCpuModelSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CpuModels",
                columns: new[] { "CpuModelId", "LogicalThreadCount", "ModelName", "PhysicalCoreCount" },
                values: new object[,]
                {
                    { 1, 32, "AMD Ryzen 9 7950X", 16 },
                    { 2, 32, "Intel Core i9-13900K", 24 },
                    { 3, 12, "AMD Ryzen 5 5600X", 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CpuModels",
                keyColumn: "CpuModelId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CpuModels",
                keyColumn: "CpuModelId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CpuModels",
                keyColumn: "CpuModelId",
                keyValue: 3);
        }
    }
}
