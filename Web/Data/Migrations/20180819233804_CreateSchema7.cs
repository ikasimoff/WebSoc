using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class CreateSchema7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "Tasks",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CommentText",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentedPostId",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Tasks",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DirectText",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SQL",
                table: "Tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SelectedCount",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SelectedIds",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedIdsComment",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedIdsDirect",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedIdsDirectLikes",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedIdsFollow",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Tasks",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop",
                table: "Tasks",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentText",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentedPostId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DirectText",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SQL",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SelectedCount",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SelectedIds",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SelectedIdsComment",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SelectedIdsDirect",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SelectedIdsDirectLikes",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SelectedIdsFollow",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Stop",
                table: "Tasks");
        }
    }
}
