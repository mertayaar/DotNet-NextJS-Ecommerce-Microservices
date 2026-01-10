using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Order.Persistance.Migrations
{
    
    public partial class FixOrderTableRetry : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Addresses]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Addresses](
                        [AddressId] [int] IDENTITY(1,1) NOT NULL,
                        [UserId] [nvarchar](max) NOT NULL,
                        [District] [nvarchar](max) NOT NULL,
                        [City] [nvarchar](max) NOT NULL,
                        [Detail] [nvarchar](max) NOT NULL,
                     CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED 
                    (
                        [AddressId] ASC
                    )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orderings]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Orderings](
                        [OrderingId] [int] IDENTITY(1,1) NOT NULL,
                        [UserId] [nvarchar](max) NOT NULL,
                        [TotalPrice] [decimal](18, 2) NOT NULL,
                        [OrderDate] [datetime2](7) NOT NULL,
                        [DiscountCode] [nvarchar](max) NULL,
                        [DiscountRate] [int] NULL,
                     CONSTRAINT [PK_Orderings] PRIMARY KEY CLUSTERED 
                    (
                        [OrderingId] ASC
                    )
                    )
                END

                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[OrderDetails](
                        [OrderDetailId] [int] IDENTITY(1,1) NOT NULL,
                        [ProductId] [nvarchar](max) NOT NULL,
                        [ProductName] [nvarchar](max) NOT NULL,
                        [ProductPrice] [decimal](18, 2) NOT NULL,
                        [ProductAmount] [int] NOT NULL,
                        [ProductTotalPrice] [decimal](18, 2) NOT NULL,
                        [OrderingId] [int] NOT NULL,
                        [ProductImageUrl] [nvarchar](max) NOT NULL DEFAULT '',
                     CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
                    (
                        [OrderDetailId] ASC
                    )
                    )

                    ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orderings_OrderingId] FOREIGN KEY([OrderingId])
                    REFERENCES [dbo].[Orderings] ([OrderingId])
                    ON DELETE CASCADE

                    ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orderings_OrderingId]

                    CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderingId] ON [dbo].[OrderDetails]
                    (
                        [OrderingId] ASC
                    )
                END
            ");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS [dbo].[OrderDetails];
                DROP TABLE IF EXISTS [dbo].[Orderings];
                DROP TABLE IF EXISTS [dbo].[Addresses];
            ");
        }
    }
}
