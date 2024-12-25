using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAPI2.Data.Migrations
{
    /// <inheritdoc />
    public partial class addNavigationField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePerson_Movie_MoivesId",
                table: "MoviePerson");

            migrationBuilder.RenameColumn(
                name: "MoivesId",
                table: "MoviePerson",
                newName: "MoviesId");

            migrationBuilder.RenameIndex(
                name: "IX_MoviePerson_MoivesId",
                table: "MoviePerson",
                newName: "IX_MoviePerson_MoviesId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Person",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePerson_Movie_MoviesId",
                table: "MoviePerson",
                column: "MoviesId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePerson_Movie_MoviesId",
                table: "MoviePerson");

            migrationBuilder.RenameColumn(
                name: "MoviesId",
                table: "MoviePerson",
                newName: "MoivesId");

            migrationBuilder.RenameIndex(
                name: "IX_MoviePerson_MoviesId",
                table: "MoviePerson",
                newName: "IX_MoviePerson_MoivesId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Person",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePerson_Movie_MoivesId",
                table: "MoviePerson",
                column: "MoivesId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
