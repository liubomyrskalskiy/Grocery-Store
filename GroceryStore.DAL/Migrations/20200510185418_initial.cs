using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GroceryStore.DAL.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id_Category = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(unicode: false, maxLength: 40, nullable: true),
                    Description = table.Column<string>(unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKCategory", x => x.Id_Category);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id_Country = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKCountry", x => x.Id_Country);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id_Role = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
                    Salary = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKRole", x => x.Id_Role);
                });

            migrationBuilder.CreateTable(
                name: "Write_off_Reason",
                columns: table => new
                {
                    Id_Write_off_Reason = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKWrite_off_Reason", x => x.Id_Write_off_Reason);
                });

            migrationBuilder.CreateTable(
                name: "Goods_Own",
                columns: table => new
                {
                    Id_Goods_Own = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Weight = table.Column<double>(nullable: true),
                    Components = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Price = table.Column<double>(nullable: true),
                    Id_Category = table.Column<int>(nullable: true),
                    Product_Code = table.Column<string>(unicode: false, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKGoods_Own", x => x.Id_Goods_Own);
                    table.ForeignKey(
                        name: "R_20",
                        column: x => x.Id_Category,
                        principalTable: "Category",
                        principalColumn: "Id_Category",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id_City = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Id_Country = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKCity", x => x.Id_City);
                    table.ForeignKey(
                        name: "R_14",
                        column: x => x.Id_Country,
                        principalTable: "Country",
                        principalColumn: "Id_Country",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Producer",
                columns: table => new
                {
                    Id_Producer = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Id_Country = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKProducer", x => x.Id_Producer);
                    table.ForeignKey(
                        name: "R_15",
                        column: x => x.Id_Country,
                        principalTable: "Country",
                        principalColumn: "Id_Country",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id_Client = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Last_Name = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    First_Name = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    Bonuses = table.Column<double>(nullable: true),
                    Address = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Phone_Number = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Id_City = table.Column<int>(nullable: true),
                    Account_Number = table.Column<string>(unicode: false, maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKClient", x => x.Id_Client);
                    table.ForeignKey(
                        name: "R_33",
                        column: x => x.Id_City,
                        principalTable: "City",
                        principalColumn: "Id_City",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Market",
                columns: table => new
                {
                    Id_Market = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Phone_Number = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Id_City = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKMarket", x => x.Id_Market);
                    table.ForeignKey(
                        name: "R_18",
                        column: x => x.Id_City,
                        principalTable: "City",
                        principalColumn: "Id_City",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    Id_Provider = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Company_Title = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Contact_Person = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Phone_Number = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Address = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Id_City = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKProvider", x => x.Id_Provider);
                    table.ForeignKey(
                        name: "R_4",
                        column: x => x.Id_City,
                        principalTable: "City",
                        principalColumn: "Id_City",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id_Goods = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    Weight = table.Column<double>(nullable: true),
                    Components = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    Price = table.Column<double>(nullable: true),
                    Id_Category = table.Column<int>(nullable: true),
                    Id_Producer = table.Column<int>(nullable: true),
                    Product_Code = table.Column<string>(unicode: false, maxLength: 5, nullable: true),
                    Title = table.Column<string>(unicode: false, maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKGoods", x => x.Id_Goods);
                    table.ForeignKey(
                        name: "R_13",
                        column: x => x.Id_Category,
                        principalTable: "Category",
                        principalColumn: "Id_Category",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_16",
                        column: x => x.Id_Producer,
                        principalTable: "Producer",
                        principalColumn: "Id_Producer",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id_Employee = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Last_Name = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    First_Name = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    Phone_Number = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    Work_Experience = table.Column<int>(nullable: true),
                    Address = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Id_Market = table.Column<int>(nullable: true),
                    Id_Role = table.Column<int>(nullable: true),
                    Id_City = table.Column<int>(nullable: true),
                    Login = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Password = table.Column<string>(unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKEmployee", x => x.Id_Employee);
                    table.ForeignKey(
                        name: "R_27",
                        column: x => x.Id_City,
                        principalTable: "City",
                        principalColumn: "Id_City",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_24",
                        column: x => x.Id_Market,
                        principalTable: "Market",
                        principalColumn: "Id_Market",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_25",
                        column: x => x.Id_Role,
                        principalTable: "Role",
                        principalColumn: "Id_Role",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    Id_Delivery = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Delivery_Number = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Delivery_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Id_Provider = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKDelivery", x => x.Id_Delivery);
                    table.ForeignKey(
                        name: "R_2",
                        column: x => x.Id_Provider,
                        principalTable: "Provider",
                        principalColumn: "Id_Provider",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Consignment",
                columns: table => new
                {
                    Id_Consignment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Consignment_Number = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
                    Manufacture_Date = table.Column<DateTime>(type: "date", nullable: true),
                    Best_Before = table.Column<DateTime>(type: "date", nullable: true),
                    Amount = table.Column<double>(nullable: true),
                    Id_Goods = table.Column<int>(nullable: true),
                    Income_Price = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKConsignment", x => x.Id_Consignment);
                    table.ForeignKey(
                        name: "R_11",
                        column: x => x.Id_Goods,
                        principalTable: "Goods",
                        principalColumn: "Id_Goods",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goods_in_Market",
                columns: table => new
                {
                    Id_Goods_in_Market = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(nullable: true),
                    Id_Market = table.Column<int>(nullable: true),
                    Id_Goods = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKGoods_in_Market", x => x.Id_Goods_in_Market);
                    table.ForeignKey(
                        name: "R_32",
                        column: x => x.Id_Goods,
                        principalTable: "Goods",
                        principalColumn: "Id_Goods",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_19",
                        column: x => x.Id_Market,
                        principalTable: "Market",
                        principalColumn: "Id_Market",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Production",
                columns: table => new
                {
                    Id_Production = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Goods_Own = table.Column<int>(nullable: true),
                    Production_Code = table.Column<string>(nullable: true),
                    Manufacture_Date = table.Column<DateTime>(type: "date", nullable: true),
                    Best_Before = table.Column<DateTime>(type: "date", nullable: true),
                    Amount = table.Column<int>(nullable: true),
                    Id_Employee = table.Column<int>(nullable: true),
                    Total_Cost = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKProduction", x => x.Id_Production);
                    table.ForeignKey(
                        name: "R_26",
                        column: x => x.Id_Employee,
                        principalTable: "Employee",
                        principalColumn: "Id_Employee",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_22",
                        column: x => x.Id_Goods_Own,
                        principalTable: "Goods_Own",
                        principalColumn: "Id_Goods_Own",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sale",
                columns: table => new
                {
                    Id_Sale = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Total = table.Column<double>(nullable: true, defaultValueSql: "((0))"),
                    Check_Number = table.Column<string>(unicode: false, maxLength: 16, nullable: true),
                    Id_Client = table.Column<int>(nullable: true),
                    Id_Employee = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKSale", x => x.Id_Sale);
                    table.ForeignKey(
                        name: "R_39",
                        column: x => x.Id_Client,
                        principalTable: "Client",
                        principalColumn: "Id_Client",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_44",
                        column: x => x.Id_Employee,
                        principalTable: "Employee",
                        principalColumn: "Id_Employee",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Delivery_Contents",
                columns: table => new
                {
                    Id_Consignment = table.Column<int>(nullable: false),
                    Id_Delivery = table.Column<int>(nullable: false),
                    Id_Delivery_Contents = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKDelivery_Contents", x => new { x.Id_Consignment, x.Id_Delivery });
                    table.ForeignKey(
                        name: "R_10",
                        column: x => x.Id_Consignment,
                        principalTable: "Consignment",
                        principalColumn: "Id_Consignment",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_67",
                        column: x => x.Id_Delivery,
                        principalTable: "Delivery",
                        principalColumn: "Id_Delivery",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Delivery_Shipment",
                columns: table => new
                {
                    Id_Delivery_Shipment = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Consignment = table.Column<int>(nullable: true),
                    Id_Goods_in_Market = table.Column<int>(nullable: true),
                    Amount = table.Column<double>(nullable: true),
                    Shipment_Date_Time = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKDelivery_Shipment", x => x.Id_Delivery_Shipment);
                    table.ForeignKey(
                        name: "R_47",
                        column: x => x.Id_Consignment,
                        principalTable: "Consignment",
                        principalColumn: "Id_Consignment",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_48",
                        column: x => x.Id_Goods_in_Market,
                        principalTable: "Goods_in_Market",
                        principalColumn: "Id_Goods_in_Market",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goods_in_Market_Own",
                columns: table => new
                {
                    Id_Goods_in_Market_Own = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(nullable: true),
                    Id_Production = table.Column<int>(nullable: true),
                    Id_Market = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKGoods_in_Market_Own", x => x.Id_Goods_in_Market_Own);
                    table.ForeignKey(
                        name: "R_31",
                        column: x => x.Id_Market,
                        principalTable: "Market",
                        principalColumn: "Id_Market",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_30",
                        column: x => x.Id_Production,
                        principalTable: "Production",
                        principalColumn: "Id_Production",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Production_Contents",
                columns: table => new
                {
                    Id_Production_Contents = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(nullable: true),
                    Id_Production = table.Column<int>(nullable: true),
                    Id_Goods_in_Market = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKProduction_Contents", x => x.Id_Production_Contents);
                    table.ForeignKey(
                        name: "R_46",
                        column: x => x.Id_Goods_in_Market,
                        principalTable: "Goods_in_Market",
                        principalColumn: "Id_Goods_in_Market",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_45",
                        column: x => x.Id_Production,
                        principalTable: "Production",
                        principalColumn: "Id_Production",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Basket",
                columns: table => new
                {
                    Id_Basket = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(nullable: true),
                    Id_Goods_in_Market = table.Column<int>(nullable: true),
                    Id_Sale = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKBasket", x => x.Id_Basket);
                    table.ForeignKey(
                        name: "R_35",
                        column: x => x.Id_Goods_in_Market,
                        principalTable: "Goods_in_Market",
                        principalColumn: "Id_Goods_in_Market",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_38",
                        column: x => x.Id_Sale,
                        principalTable: "Sale",
                        principalColumn: "Id_Sale",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goods_Write_off",
                columns: table => new
                {
                    Id_Goods_Write_off = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Id_Employee = table.Column<int>(nullable: true),
                    Id_Write_off_Reason = table.Column<int>(nullable: true),
                    Id_Delivery_Shipment = table.Column<int>(nullable: true),
                    Id_Goods_in_Market = table.Column<int>(nullable: true),
                    Amount = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKGoods_Write_off", x => x.Id_Goods_Write_off);
                    table.ForeignKey(
                        name: "R_49",
                        column: x => x.Id_Delivery_Shipment,
                        principalTable: "Delivery_Shipment",
                        principalColumn: "Id_Delivery_Shipment",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_41",
                        column: x => x.Id_Employee,
                        principalTable: "Employee",
                        principalColumn: "Id_Employee",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_50",
                        column: x => x.Id_Goods_in_Market,
                        principalTable: "Goods_in_Market",
                        principalColumn: "Id_Goods_in_Market",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_42",
                        column: x => x.Id_Write_off_Reason,
                        principalTable: "Write_off_Reason",
                        principalColumn: "Id_Write_off_Reason",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Basket_Own",
                columns: table => new
                {
                    Id_Basket_Own = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(nullable: true),
                    Id_Goods_in_Market_Own = table.Column<int>(nullable: true),
                    Id_Sale = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKBasket_Own", x => x.Id_Basket_Own);
                    table.ForeignKey(
                        name: "R_36",
                        column: x => x.Id_Goods_in_Market_Own,
                        principalTable: "Goods_in_Market_Own",
                        principalColumn: "Id_Goods_in_Market_Own",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_37",
                        column: x => x.Id_Sale,
                        principalTable: "Sale",
                        principalColumn: "Id_Sale",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goods_Write_off_Own",
                columns: table => new
                {
                    Id_Goods_Write_off_Own = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_Goods_in_Market_Own = table.Column<int>(nullable: true),
                    Id_Employee = table.Column<int>(nullable: true),
                    Id_Write_off_Reason = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    Amount = table.Column<double>(nullable: true),
                    Id_Production = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("XPKGoods_Write_off_Own", x => x.Id_Goods_Write_off_Own);
                    table.ForeignKey(
                        name: "R_53",
                        column: x => x.Id_Employee,
                        principalTable: "Employee",
                        principalColumn: "Id_Employee",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_52",
                        column: x => x.Id_Goods_in_Market_Own,
                        principalTable: "Goods_in_Market_Own",
                        principalColumn: "Id_Goods_in_Market_Own",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_55",
                        column: x => x.Id_Production,
                        principalTable: "Production",
                        principalColumn: "Id_Production",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "R_54",
                        column: x => x.Id_Write_off_Reason,
                        principalTable: "Write_off_Reason",
                        principalColumn: "Id_Write_off_Reason",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Basket_Id_Goods_in_Market",
                table: "Basket",
                column: "Id_Goods_in_Market");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_Id_Sale",
                table: "Basket",
                column: "Id_Sale");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_Own_Id_Goods_in_Market_Own",
                table: "Basket_Own",
                column: "Id_Goods_in_Market_Own");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_Own_Id_Sale",
                table: "Basket_Own",
                column: "Id_Sale");

            migrationBuilder.CreateIndex(
                name: "IX_City_Id_Country",
                table: "City",
                column: "Id_Country");

            migrationBuilder.CreateIndex(
                name: "IX_Client_Id_City",
                table: "Client",
                column: "Id_City");

            migrationBuilder.CreateIndex(
                name: "IX_Consignment_Id_Goods",
                table: "Consignment",
                column: "Id_Goods");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_Id_Provider",
                table: "Delivery",
                column: "Id_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_Contents_Id_Delivery",
                table: "Delivery_Contents",
                column: "Id_Delivery");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_Shipment_Id_Consignment",
                table: "Delivery_Shipment",
                column: "Id_Consignment");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_Shipment_Id_Goods_in_Market",
                table: "Delivery_Shipment",
                column: "Id_Goods_in_Market");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Id_City",
                table: "Employee",
                column: "Id_City");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Id_Market",
                table: "Employee",
                column: "Id_Market");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_Id_Role",
                table: "Employee",
                column: "Id_Role");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Id_Category",
                table: "Goods",
                column: "Id_Category");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Id_Producer",
                table: "Goods",
                column: "Id_Producer");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_in_Market_Id_Goods",
                table: "Goods_in_Market",
                column: "Id_Goods");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_in_Market_Id_Market",
                table: "Goods_in_Market",
                column: "Id_Market");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_in_Market_Own_Id_Market",
                table: "Goods_in_Market_Own",
                column: "Id_Market");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_in_Market_Own_Id_Production",
                table: "Goods_in_Market_Own",
                column: "Id_Production");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Own_Id_Category",
                table: "Goods_Own",
                column: "Id_Category");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Id_Delivery_Shipment",
                table: "Goods_Write_off",
                column: "Id_Delivery_Shipment");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Id_Employee",
                table: "Goods_Write_off",
                column: "Id_Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Id_Goods_in_Market",
                table: "Goods_Write_off",
                column: "Id_Goods_in_Market");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Id_Write_off_Reason",
                table: "Goods_Write_off",
                column: "Id_Write_off_Reason");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Own_Id_Employee",
                table: "Goods_Write_off_Own",
                column: "Id_Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Own_Id_Goods_in_Market_Own",
                table: "Goods_Write_off_Own",
                column: "Id_Goods_in_Market_Own");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Own_Id_Production",
                table: "Goods_Write_off_Own",
                column: "Id_Production");

            migrationBuilder.CreateIndex(
                name: "IX_Goods_Write_off_Own_Id_Write_off_Reason",
                table: "Goods_Write_off_Own",
                column: "Id_Write_off_Reason");

            migrationBuilder.CreateIndex(
                name: "IX_Market_Id_City",
                table: "Market",
                column: "Id_City");

            migrationBuilder.CreateIndex(
                name: "IX_Producer_Id_Country",
                table: "Producer",
                column: "Id_Country");

            migrationBuilder.CreateIndex(
                name: "IX_Production_Id_Employee",
                table: "Production",
                column: "Id_Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Production_Id_Goods_Own",
                table: "Production",
                column: "Id_Goods_Own");

            migrationBuilder.CreateIndex(
                name: "IX_Production_Contents_Id_Goods_in_Market",
                table: "Production_Contents",
                column: "Id_Goods_in_Market");

            migrationBuilder.CreateIndex(
                name: "IX_Production_Contents_Id_Production",
                table: "Production_Contents",
                column: "Id_Production");

            migrationBuilder.CreateIndex(
                name: "IX_Provider_Id_City",
                table: "Provider",
                column: "Id_City");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_Id_Client",
                table: "Sale",
                column: "Id_Client");

            migrationBuilder.CreateIndex(
                name: "IX_Sale_Id_Employee",
                table: "Sale",
                column: "Id_Employee");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.DropTable(
                name: "Basket_Own");

            migrationBuilder.DropTable(
                name: "Delivery_Contents");

            migrationBuilder.DropTable(
                name: "Goods_Write_off");

            migrationBuilder.DropTable(
                name: "Goods_Write_off_Own");

            migrationBuilder.DropTable(
                name: "Production_Contents");

            migrationBuilder.DropTable(
                name: "Sale");

            migrationBuilder.DropTable(
                name: "Delivery");

            migrationBuilder.DropTable(
                name: "Delivery_Shipment");

            migrationBuilder.DropTable(
                name: "Goods_in_Market_Own");

            migrationBuilder.DropTable(
                name: "Write_off_Reason");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropTable(
                name: "Consignment");

            migrationBuilder.DropTable(
                name: "Goods_in_Market");

            migrationBuilder.DropTable(
                name: "Production");

            migrationBuilder.DropTable(
                name: "Goods");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Goods_Own");

            migrationBuilder.DropTable(
                name: "Producer");

            migrationBuilder.DropTable(
                name: "Market");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Country");
        }
    }
}
