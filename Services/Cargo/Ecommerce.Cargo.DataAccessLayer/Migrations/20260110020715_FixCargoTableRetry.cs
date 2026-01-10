using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Cargo.DataAccessLayer.Migrations
{
    
    public partial class FixCargoTableRetry : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CargoCompanies]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[CargoCompanies](
                        [CargoCompanyId] [int] IDENTITY(1,1) NOT NULL,
                        [CargoCompanyName] [nvarchar](max) NOT NULL,
                     CONSTRAINT [PK_CargoCompanies] PRIMARY KEY CLUSTERED 
                    (
                        [CargoCompanyId] ASC
                    )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CargoCustomers]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[CargoCustomers](
                        [CargoCustomerId] [int] IDENTITY(1,1) NOT NULL,
                        [Name] [nvarchar](max) NOT NULL,
                        [Surname] [nvarchar](max) NOT NULL,
                        [Email] [nvarchar](max) NOT NULL,
                        [Phone] [nvarchar](max) NOT NULL,
                        [District] [nvarchar](max) NOT NULL,
                        [City] [nvarchar](max) NOT NULL,
                        [Address] [nvarchar](max) NOT NULL,
                        [UserCustomerId] [nvarchar](max) NULL,
                        [IsDefault] [bit] NOT NULL DEFAULT 0,
                        [Title] [nvarchar](max) NULL,
                     CONSTRAINT [PK_CargoCustomers] PRIMARY KEY CLUSTERED 
                    (
                        [CargoCustomerId] ASC
                    )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CargoOperations]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[CargoOperations](
                        [CargoOperationId] [int] IDENTITY(1,1) NOT NULL,
                        [Barcode] [nvarchar](max) NOT NULL,
                        [Description] [nvarchar](max) NOT NULL,
                        [OperationDate] [datetime2](7) NOT NULL,
                     CONSTRAINT [PK_CargoOperations] PRIMARY KEY CLUSTERED 
                    (
                        [CargoOperationId] ASC
                    )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CargoDetails]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[CargoDetails](
                        [CargoDetailId] [int] IDENTITY(1,1) NOT NULL,
                        [SenderCustomer] [nvarchar](max) NOT NULL,
                        [ReceiverCustomer] [nvarchar](max) NOT NULL,
                        [Barcode] [int] NOT NULL,
                        [CargoCompanyId] [int] NOT NULL,
                     CONSTRAINT [PK_CargoDetails] PRIMARY KEY CLUSTERED 
                    (
                        [CargoDetailId] ASC
                    )
                    )
                    
                    ALTER TABLE [dbo].[CargoDetails]  WITH CHECK ADD  CONSTRAINT [FK_CargoDetails_CargoCompanies_CargoCompanyId] FOREIGN KEY([CargoCompanyId])
                    REFERENCES [dbo].[CargoCompanies] ([CargoCompanyId])
                    ON DELETE CASCADE

                    ALTER TABLE [dbo].[CargoDetails] CHECK CONSTRAINT [FK_CargoDetails_CargoCompanies_CargoCompanyId]

                    CREATE NONCLUSTERED INDEX [IX_CargoDetails_CargoCompanyId] ON [dbo].[CargoDetails]
                    (
                        [CargoCompanyId] ASC
                    )
                END
            ");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS [dbo].[CargoDetails];
                DROP TABLE IF EXISTS [dbo].[CargoOperations];
                DROP TABLE IF EXISTS [dbo].[CargoCustomers];
                DROP TABLE IF EXISTS [dbo].[CargoCompanies];
            ");
        }
    }
}
