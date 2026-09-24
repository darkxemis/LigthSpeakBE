using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightSpeak.Backend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoiseSuppressionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EchoCancellationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AutoGainControlEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SfxEnabled = table.Column<bool>(type: "bit", nullable: false),
                    OutputVolume = table.Column<int>(type: "int", nullable: false),
                    StartMuted = table.Column<bool>(type: "bit", nullable: false),
                    PushToTalkEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PushToTalkKey = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    DesktopNotificationsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MessageSoundEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EnterToSendEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ShowTimestampsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CompactMessagesEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ReducedMotionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.CheckConstraint("CK_UserSettings_OutputVolume", "[OutputVolume] BETWEEN 0 AND 100");
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
