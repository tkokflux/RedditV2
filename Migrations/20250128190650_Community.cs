using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reddit.Migrations
{
    public partial class Community : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Remove the old 'CommunityName' column from the Posts table
            migrationBuilder.DropColumn(
                name: "CommunityName",
                table: "Posts");

            // Step 2: Add the 'CommunityId' column to the Posts table
            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "Posts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            // Step 3: Create the 'Communities' table
            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communities_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // Step 4: Create the 'UserCommunitySubscriptions' table
            migrationBuilder.CreateTable(
                name: "UserCommunitySubscriptions",
                columns: table => new
                {
                    SubscribedCommunitiesId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscribersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommunitySubscriptions", x => new { x.SubscribedCommunitiesId, x.SubscribersId });
                    table.ForeignKey(
                        name: "FK_UserCommunitySubscriptions_Communities_SubscribedCommunitiesId",
                        column: x => x.SubscribedCommunitiesId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCommunitySubscriptions_Users_SubscribersId",
                        column: x => x.SubscribersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Step 5: Create necessary indexes
            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityId",
                table: "Posts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_OwnerId",
                table: "Communities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommunitySubscriptions_SubscribersId",
                table: "UserCommunitySubscriptions",
                column: "SubscribersId");

            // Step 6: Add the foreign key constraint after ensuring 'CommunityId' is in place
            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                table: "Posts",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "UserCommunitySubscriptions");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CommunityId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "CommunityName",
                table: "Posts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}

