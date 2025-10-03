using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ihjezly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sentSMTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailVerificationCodes");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EmailVerificationCodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "EmailVerificationCodes");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "EmailVerificationCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
