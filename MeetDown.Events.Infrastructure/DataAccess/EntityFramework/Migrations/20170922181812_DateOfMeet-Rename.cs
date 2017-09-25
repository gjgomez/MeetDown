using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MeetDown.Events.Infrastructure.DataAccess.EntityFramework.Migrations
{
    public partial class DateOfMeetRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateofMeet",
                table: "Meet",
                newName: "DateOfMeet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateOfMeet",
                table: "Meet",
                newName: "DateofMeet");
        }
    }
}
