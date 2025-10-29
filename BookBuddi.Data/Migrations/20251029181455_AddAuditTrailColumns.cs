using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookBuddi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTrailColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add audit trail columns to Authors table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Authors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Authors",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Authors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Authors",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to Categories table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to Genres table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Genres",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Genres",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Genres",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Genres",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to Members table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Members",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Members",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Members",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to Books table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Books",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Books",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Books",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Books",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to BookAuthors table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BookAuthors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "BookAuthors",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "BookAuthors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "BookAuthors",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to BorrowTransactions table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BorrowTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "BorrowTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "BorrowTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "BorrowTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to Fines table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Fines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Fines",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Fines",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Fines",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to BookRequests table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BookRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "BookRequests",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "BookRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "BookRequests",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to Notifications table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Notifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            // Add audit trail columns to AspNetUsers (Admin) table
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "System");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove audit trail columns from all tables
            migrationBuilder.DropColumn(name: "CreatedBy", table: "Authors");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "Authors");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Authors");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "Authors");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "Categories");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "Categories");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Categories");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "Categories");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "Genres");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "Genres");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Genres");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "Genres");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "Members");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "Members");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Members");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "Members");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "Books");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "Books");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Books");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "Books");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "BookAuthors");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "BookAuthors");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "BookAuthors");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "BookAuthors");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "BorrowTransactions");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "BorrowTransactions");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "BorrowTransactions");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "BorrowTransactions");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "Fines");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "Fines");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Fines");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "Fines");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "BookRequests");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "BookRequests");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "BookRequests");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "BookRequests");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "Notifications");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "Notifications");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "Notifications");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "Notifications");

            migrationBuilder.DropColumn(name: "CreatedBy", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "CreatedTime", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "UpdatedBy", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "UpdatedTime", table: "AspNetUsers");
        }
    }
}
