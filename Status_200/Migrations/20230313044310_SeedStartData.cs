using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Status_200.Migrations
{
    /// <inheritdoc />
    public partial class SeedStartData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
           table: "Roles",
           columns: new[] { "Id", "RoleName" },
           values: new object[,]
           {
                    { 1, "Admin" },
                    { 2, "Developer" }
           });

            migrationBuilder.InsertData(
            table: "UsersStatus",
            columns: new[] { "Id", "NameStatus" },
            values: new object[,]
            {
                                { 1, "Active" },
                                { 2, "Blocked" },
                                { 3, "Removed" }

            });

            migrationBuilder.InsertData(
            table: "StatusesTask",
            columns: new[] { "Id", "StatusName" },
            values: new object[,]
            {
                                { 1, "In developing" },
                                { 2, "Under review" },
                                { 3, "On completion" },
                                { 4, "Completed" }

            });

            migrationBuilder.InsertData(
            table: "Users",
            columns: new[] { "FirstName", "SecondName", "StatusId", "RoleId", "Email", "Password" },
            values: new object[,]
            {
                    { "Status", "200", 1, 1, "sstatus200@mail.ru", "password1" },
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
