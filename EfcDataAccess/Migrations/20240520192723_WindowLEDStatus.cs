using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfcDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class WindowLEDStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WindowStatus = table.Column<byte>(nullable: false, defaultValue: (byte)0),
                    LedStatus = table.Column<byte>(nullable: false, defaultValue: (byte)0),
                    CommandCode = table.Column<byte>(nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceStatuses", x => x.Id);
                });

            // Insert default device status
            migrationBuilder.InsertData(
                table: "DeviceStatuses",
                columns: new[] { "WindowStatus", "LedStatus", "CommandCode" },
                values: new object[] { (byte)0, (byte)0, (byte)0 }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceStatuses");
        }
    }
}
