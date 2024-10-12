using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPath.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_AspNetUsers_TeacherId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_AspNetUsers_parentId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_parentId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_TeacherId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "ClassAttendance",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "parentId",
                table: "notifications");

            migrationBuilder.AlterColumn<string>(
                name: "MorningAttendance",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusNumber",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AfternoonAttendance",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_StudentId",
                table: "notifications",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_students_StudentId",
                table: "notifications",
                column: "StudentId",
                principalTable: "students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_students_StudentId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_StudentId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "notifications");

            migrationBuilder.AlterColumn<string>(
                name: "MorningAttendance",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BusNumber",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AfternoonAttendance",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ClassAttendance",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NotificationType",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "notifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "parentId",
                table: "notifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_parentId",
                table: "notifications",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_TeacherId",
                table: "notifications",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_AspNetUsers_TeacherId",
                table: "notifications",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_AspNetUsers_parentId",
                table: "notifications",
                column: "parentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
