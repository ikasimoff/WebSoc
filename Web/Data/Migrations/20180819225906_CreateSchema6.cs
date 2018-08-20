using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class CreateSchema6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LoginsGroupId",
                table: "Logins",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoginsGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<long>(nullable: false),
                    TaskId = table.Column<long>(nullable: false),
                    LoginsGroupId = table.Column<long>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginsGroups", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginsGroups");

            migrationBuilder.DropColumn(
                name: "LoginsGroupId",
                table: "Logins");
        }
    }
}
