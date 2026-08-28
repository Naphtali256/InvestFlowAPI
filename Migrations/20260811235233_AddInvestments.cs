using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestFlowAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddInvestments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvestmentPlanPlanID",
                table: "Investments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Investments_InvestmentPlanPlanID",
                table: "Investments",
                column: "InvestmentPlanPlanID");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_UserID",
                table: "Investments",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_InvestmentPlans_InvestmentPlanPlanID",
                table: "Investments",
                column: "InvestmentPlanPlanID",
                principalTable: "InvestmentPlans",
                principalColumn: "PlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_Users_UserID",
                table: "Investments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_InvestmentPlans_InvestmentPlanPlanID",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_Users_UserID",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Investments_InvestmentPlanPlanID",
                table: "Investments");

            migrationBuilder.DropIndex(
                name: "IX_Investments_UserID",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "InvestmentPlanPlanID",
                table: "Investments");
        }
    }
}
