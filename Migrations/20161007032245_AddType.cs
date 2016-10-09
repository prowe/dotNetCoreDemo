using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace demo.Migrations
{
    public partial class AddType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Pokemon",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Pokemon");
        }
    }
}
