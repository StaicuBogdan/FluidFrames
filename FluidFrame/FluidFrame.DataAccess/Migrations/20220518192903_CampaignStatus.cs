using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluidFrame.DataAccess.Migrations
{
    public partial class CampaignStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CampaignStatus",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignStatus",
                table: "Campaigns");
        }
    }
}
