using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollectionsPortal.Migrations
{
    public partial class seed : Migration
    {
        private string UserRoleId = Guid.NewGuid().ToString();
        private string AdminRoleId = Guid.NewGuid().ToString();
        private string userId = Guid.NewGuid().ToString();
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedRolesSQL(migrationBuilder);

            SeedUser(migrationBuilder);

            SeedUserRoles(migrationBuilder);

            seedTags(migrationBuilder);

            seedTopics(migrationBuilder);

        }

        private void SeedUserRoles(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
            INSERT INTO [dbo].[AspNetUserRoles]
                ([UserId]
                ,[RoleId])
            VALUES
                ('{userId}', '{AdminRoleId}');");

            migrationBuilder.Sql($@"
            INSERT INTO [dbo].[AspNetUserRoles]
                ([UserId]
                ,[RoleId])
            VALUES
                ('{userId}', '{UserRoleId}');");
        }

        private void SeedUser(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"INSERT [dbo].[AspNetUsers] ([Id], [FirstName], [LastName], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed],
            [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount])
            VALUES
            (N'{userId}', N'Admin', N'Admin', N'admin@gmail.com', N'ADMIN@GMAIL.COM', N'admin@gmail.com', N'ADMIN@GMAIL.COM', 1, 
            N'AQAAAAEAACcQAAAAEM9bd2+tdwpkGD4E9QEIYvo/uI5ThjzRnRvTo0kjNES3lKF722gdXyRIHyjelgnAhg==',
            N'3ZMWQLTNBURJMN6KK3CRCMJ3KTGOHML2', N'd511593e-d565-4135-a19c-ac2eda0058f7', NULL, 0, 0, NULL, 1, 0)");
        }

        private void SeedRolesSQL(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{AdminRoleId}', 'Administrator', 'ADMINISTRATOR', null);");
            migrationBuilder.Sql($@"INSERT INTO [dbo].[AspNetRoles] ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
            VALUES ('{UserRoleId}', 'User', 'USER', null);");
        }

        private void seedTags(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"INSERT INTO [dbo].[Tags] ([Name]) VALUES ('Whiskey');");
            migrationBuilder.Sql($@"INSERT INTO [dbo].[Tags] ([Name]) VALUES ('Ferrari');");
            migrationBuilder.Sql($@"INSERT INTO [dbo].[Tags] ([Name]) VALUES ('Fantasy');");
        }

        private void seedTopics(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"INSERT INTO [dbo].[Topics] ([Name]) VALUES ('Books');");
            migrationBuilder.Sql($@"INSERT INTO [dbo].[Topics] ([Name]) VALUES ('Cars');");
            migrationBuilder.Sql($@"INSERT INTO [dbo].[Topics] ([Name]) VALUES ('Alcohol');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
