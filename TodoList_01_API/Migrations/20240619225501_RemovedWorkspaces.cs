using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoList_01_API.Migrations
{
    /// <inheritdoc />
    public partial class RemovedWorkspaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoLists_Workspaces_WorkspaceId",
                table: "TodoLists");

            migrationBuilder.DropTable(
                name: "Workspaces");

            migrationBuilder.RenameColumn(
                name: "WorkspaceId",
                table: "TodoLists",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoLists_WorkspaceId",
                table: "TodoLists",
                newName: "IX_TodoLists_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLists_AspNetUsers_OwnerId",
                table: "TodoLists",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoLists_AspNetUsers_OwnerId",
                table: "TodoLists");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "TodoLists",
                newName: "WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_TodoLists_OwnerId",
                table: "TodoLists",
                newName: "IX_TodoLists_WorkspaceId");

            migrationBuilder.CreateTable(
                name: "Workspaces",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_OwnerId",
                table: "Workspaces",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLists_Workspaces_WorkspaceId",
                table: "TodoLists",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
