using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Paymentprocess.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRepaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RepaymentStatus",
                table: "Repayments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepaymentStatus",
                table: "Repayments");
        }
    }
}
