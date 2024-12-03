using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitPostCategoriesRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Interactions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayPosition",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PostCategories",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCategories", x => new { x.PostId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_PostCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostCategories_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostCategories_CategoryId",
                table: "PostCategories",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostCategories");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DisplayPosition",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Categories",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Interactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
