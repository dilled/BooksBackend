using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendDeveloperTask.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_books_title_author_year",
                table: "books",
                columns: new[] { "title", "author", "year" },
                unique: true);
        }
    }
}
