using System;
using GroceryStore.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.DAL
{
    public class GroceryStoreDbContext : DbContext
    {
        public GroceryStoreDbContext()
        {
        }

        public GroceryStoreDbContext(DbContextOptions<GroceryStoreDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Basket> Basket { get; set; }
        public virtual DbSet<BasketOwn> BasketOwn { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Consignment> Consignment { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Delivery> Delivery { get; set; }
        public virtual DbSet<DeliveryContents> DeliveryContents { get; set; }
        public virtual DbSet<DeliveryShipment> DeliveryShipment { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Goods> Goods { get; set; }
        public virtual DbSet<GoodsInMarket> GoodsInMarket { get; set; }
        public virtual DbSet<GoodsInMarketOwn> GoodsInMarketOwn { get; set; }
        public virtual DbSet<GoodsOwn> GoodsOwn { get; set; }
        public virtual DbSet<GoodsWriteOff> GoodsWriteOff { get; set; }
        public virtual DbSet<GoodsWriteOffOwn> GoodsWriteOffOwn { get; set; }
        public virtual DbSet<Market> Market { get; set; }
        public virtual DbSet<Producer> Producer { get; set; }
        public virtual DbSet<Production> Production { get; set; }
        public virtual DbSet<ProductionContents> ProductionContents { get; set; }
        public virtual DbSet<Provider> Provider { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }
        public virtual DbSet<VProductProducersv2> VProductProducersv2 { get; set; }
        public virtual DbSet<VProduction> VProduction { get; set; }
        public virtual DbSet<VProductsInfo> VProductsInfo { get; set; }
        public virtual DbSet<VWritingOffOwnProducts> VWritingOffOwnProducts { get; set; }
        public virtual DbSet<VWritingOffProducts> VWritingOffProducts { get; set; }
        public virtual DbSet<WriteOffReason> WriteOffReason { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(
                    "Data Source=LAPTOP-H0Q68PRE\\LIUBOMYRSERV;Initial Catalog=GroceryStore;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Basket>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKBasket");

                entity.Property(e => e.Id).HasColumnName("Id_Basket");

                entity.Property(e => e.IdGoodsInMarket).HasColumnName("Id_Goods_in_Market");

                entity.Property(e => e.IdSale).HasColumnName("Id_Sale");

                entity.HasOne(d => d.IdGoodsInMarketNavigation)
                    .WithMany(p => p.Basket)
                    .HasForeignKey(d => d.IdGoodsInMarket)
                    .HasConstraintName("R_35");

                entity.HasOne(d => d.IdSaleNavigation)
                    .WithMany(p => p.Basket)
                    .HasForeignKey(d => d.IdSale)
                    .HasConstraintName("R_38");
            });

            modelBuilder.Entity<BasketOwn>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKBasket_Own");

                entity.ToTable("Basket_Own");

                entity.Property(e => e.Id).HasColumnName("Id_Basket_Own");

                entity.Property(e => e.IdGoodsInMarketOwn).HasColumnName("Id_Goods_in_Market_Own");

                entity.Property(e => e.IdSale).HasColumnName("Id_Sale");

                entity.HasOne(d => d.IdGoodsInMarketOwnNavigation)
                    .WithMany(p => p.BasketOwn)
                    .HasForeignKey(d => d.IdGoodsInMarketOwn)
                    .HasConstraintName("R_36");

                entity.HasOne(d => d.IdSaleNavigation)
                    .WithMany(p => p.BasketOwn)
                    .HasForeignKey(d => d.IdSale)
                    .HasConstraintName("R_37");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKCategory");

                entity.Property(e => e.Id).HasColumnName("Id_Category");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKCity");

                entity.Property(e => e.Id).HasColumnName("Id_City");

                entity.Property(e => e.IdCountry).HasColumnName("Id_Country");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCountryNavigation)
                    .WithMany(p => p.City)
                    .HasForeignKey(d => d.IdCountry)
                    .HasConstraintName("R_14");
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKClient");

                entity.Property(e => e.Id).HasColumnName("Id_Client");

                entity.Property(e => e.AccountNumber)
                    .HasColumnName("Account_Number")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasColumnName("First_Name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IdCity).HasColumnName("Id_City");

                entity.Property(e => e.LastName)
                    .HasColumnName("Last_Name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("Phone_Number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCityNavigation)
                    .WithMany(p => p.Client)
                    .HasForeignKey(d => d.IdCity)
                    .HasConstraintName("R_33");
            });

            modelBuilder.Entity<Consignment>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKConsignment");

                entity.Property(e => e.Id).HasColumnName("Id_Consignment");

                entity.Property(e => e.BestBefore)
                    .HasColumnName("Best_Before")
                    .HasColumnType("date");

                entity.Property(e => e.ConsignmentNumber)
                    .HasColumnName("Consignment_Number")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.IdGoods).HasColumnName("Id_Goods");

                entity.Property(e => e.IncomePrice).HasColumnName("Income_Price");

                entity.Property(e => e.ManufactureDate)
                    .HasColumnName("Manufacture_Date")
                    .HasColumnType("date");

                entity.HasOne(d => d.IdGoodsNavigation)
                    .WithMany(p => p.Consignment)
                    .HasForeignKey(d => d.IdGoods)
                    .HasConstraintName("R_11");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKCountry");

                entity.Property(e => e.Id).HasColumnName("Id_Country");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKDelivery");

                entity.Property(e => e.Id).HasColumnName("Id_Delivery");

                entity.Property(e => e.DeliveryDate)
                    .HasColumnName("Delivery_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DeliveryNumber)
                    .HasColumnName("Delivery_Number")
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.IdProvider).HasColumnName("Id_Provider");

                entity.HasOne(d => d.IdProviderNavigation)
                    .WithMany(p => p.Delivery)
                    .HasForeignKey(d => d.IdProvider)
                    .HasConstraintName("R_2");
            });

            modelBuilder.Entity<DeliveryContents>(entity =>
            {
                entity.HasKey(e => new {e.IdConsignment, e.IdDelivery})
                    .HasName("XPKDelivery_Contents");

                entity.ToTable("Delivery_Contents");

                entity.Property(e => e.IdConsignment).HasColumnName("Id_Consignment");

                entity.Property(e => e.IdDelivery).HasColumnName("Id_Delivery");

                entity.Property(e => e.Id)
                    .HasColumnName("Id_Delivery_Contents")
                    .ValueGeneratedOnAdd();

                entity.HasOne(d => d.IdConsignmentNavigation)
                    .WithMany(p => p.DeliveryContents)
                    .HasForeignKey(d => d.IdConsignment)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("R_10");

                entity.HasOne(d => d.IdDeliveryNavigation)
                    .WithMany(p => p.DeliveryContents)
                    .HasForeignKey(d => d.IdDelivery)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("R_67");
            });

            modelBuilder.Entity<DeliveryShipment>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKDelivery_Shipment");

                entity.ToTable("Delivery_Shipment");

                entity.Property(e => e.Id).HasColumnName("Id_Delivery_Shipment");

                entity.Property(e => e.IdConsignment).HasColumnName("Id_Consignment");

                entity.Property(e => e.IdGoodsInMarket).HasColumnName("Id_Goods_in_Market");

                entity.Property(e => e.ShipmentDateTime)
                    .HasColumnName("Shipment_Date_Time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdConsignmentNavigation)
                    .WithMany(p => p.DeliveryShipment)
                    .HasForeignKey(d => d.IdConsignment)
                    .HasConstraintName("R_47");

                entity.HasOne(d => d.IdGoodsInMarketNavigation)
                    .WithMany(p => p.DeliveryShipment)
                    .HasForeignKey(d => d.IdGoodsInMarket)
                    .HasConstraintName("R_48");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKEmployee");

                entity.Property(e => e.Id).HasColumnName("Id_Employee");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasColumnName("First_Name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.IdCity).HasColumnName("Id_City");

                entity.Property(e => e.IdMarket).HasColumnName("Id_Market");

                entity.Property(e => e.IdRole).HasColumnName("Id_Role");

                entity.Property(e => e.LastName)
                    .HasColumnName("Last_Name")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Login)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("Phone_Number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WorkExperience).HasColumnName("Work_Experience");

                entity.HasOne(d => d.IdCityNavigation)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.IdCity)
                    .HasConstraintName("R_27");

                entity.HasOne(d => d.IdMarketNavigation)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.IdMarket)
                    .HasConstraintName("R_24");

                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.IdRole)
                    .HasConstraintName("R_25");
            });

            modelBuilder.Entity<Goods>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKGoods");

                entity.Property(e => e.Id).HasColumnName("Id_Goods");

                entity.Property(e => e.Components)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.IdCategory).HasColumnName("Id_Category");

                entity.Property(e => e.IdProducer).HasColumnName("Id_Producer");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("Product_Code")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.Goods)
                    .HasForeignKey(d => d.IdCategory)
                    .HasConstraintName("R_13");

                entity.HasOne(d => d.IdProducerNavigation)
                    .WithMany(p => p.Goods)
                    .HasForeignKey(d => d.IdProducer)
                    .HasConstraintName("R_16");
            });

            modelBuilder.Entity<GoodsInMarket>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKGoods_in_Market");

                entity.ToTable("Goods_in_Market");

                entity.Property(e => e.Id).HasColumnName("Id_Goods_in_Market");

                entity.Property(e => e.IdGoods).HasColumnName("Id_Goods");

                entity.Property(e => e.IdMarket).HasColumnName("Id_Market");

                entity.HasOne(d => d.IdGoodsNavigation)
                    .WithMany(p => p.GoodsInMarket)
                    .HasForeignKey(d => d.IdGoods)
                    .HasConstraintName("R_32");

                entity.HasOne(d => d.IdMarketNavigation)
                    .WithMany(p => p.GoodsInMarket)
                    .HasForeignKey(d => d.IdMarket)
                    .HasConstraintName("R_19");
            });

            modelBuilder.Entity<GoodsInMarketOwn>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKGoods_in_Market_Own");

                entity.ToTable("Goods_in_Market_Own");

                entity.Property(e => e.Id).HasColumnName("Id_Goods_in_Market_Own");

                entity.Property(e => e.IdMarket).HasColumnName("Id_Market");

                entity.Property(e => e.IdProduction).HasColumnName("Id_Production");

                entity.HasOne(d => d.IdMarketNavigation)
                    .WithMany(p => p.GoodsInMarketOwn)
                    .HasForeignKey(d => d.IdMarket)
                    .HasConstraintName("R_31");

                entity.HasOne(d => d.IdProductionNavigation)
                    .WithMany(p => p.GoodsInMarketOwn)
                    .HasForeignKey(d => d.IdProduction)
                    .HasConstraintName("R_30");
            });

            modelBuilder.Entity<GoodsOwn>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKGoods_Own");

                entity.ToTable("Goods_Own");

                entity.Property(e => e.Id).HasColumnName("Id_Goods_Own");

                entity.Property(e => e.Components)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.IdCategory).HasColumnName("Id_Category");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("Product_Code")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.GoodsOwn)
                    .HasForeignKey(d => d.IdCategory)
                    .HasConstraintName("R_20");
            });

            modelBuilder.Entity<GoodsWriteOff>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKGoods_Write_off");

                entity.ToTable("Goods_Write_off");

                entity.Property(e => e.Id).HasColumnName("Id_Goods_Write_off");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.IdDeliveryShipment).HasColumnName("Id_Delivery_Shipment");

                entity.Property(e => e.IdEmployee).HasColumnName("Id_Employee");

                entity.Property(e => e.IdGoodsInMarket).HasColumnName("Id_Goods_in_Market");

                entity.Property(e => e.IdWriteOffReason).HasColumnName("Id_Write_off_Reason");

                entity.HasOne(d => d.IdDeliveryShipmentNavigation)
                    .WithMany(p => p.GoodsWriteOff)
                    .HasForeignKey(d => d.IdDeliveryShipment)
                    .HasConstraintName("R_49");

                entity.HasOne(d => d.IdEmployeeNavigation)
                    .WithMany(p => p.GoodsWriteOff)
                    .HasForeignKey(d => d.IdEmployee)
                    .HasConstraintName("R_41");

                entity.HasOne(d => d.IdGoodsInMarketNavigation)
                    .WithMany(p => p.GoodsWriteOff)
                    .HasForeignKey(d => d.IdGoodsInMarket)
                    .HasConstraintName("R_50");

                entity.HasOne(d => d.IdWriteOffReasonNavigation)
                    .WithMany(p => p.GoodsWriteOff)
                    .HasForeignKey(d => d.IdWriteOffReason)
                    .HasConstraintName("R_42");
            });

            modelBuilder.Entity<GoodsWriteOffOwn>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKGoods_Write_off_Own");

                entity.ToTable("Goods_Write_off_Own");

                entity.Property(e => e.Id).HasColumnName("Id_Goods_Write_off_Own");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.IdEmployee).HasColumnName("Id_Employee");

                entity.Property(e => e.IdGoodsInMarketOwn).HasColumnName("Id_Goods_in_Market_Own");

                entity.Property(e => e.IdProduction).HasColumnName("Id_Production");

                entity.Property(e => e.IdWriteOffReason).HasColumnName("Id_Write_off_Reason");

                entity.HasOne(d => d.IdEmployeeNavigation)
                    .WithMany(p => p.GoodsWriteOffOwn)
                    .HasForeignKey(d => d.IdEmployee)
                    .HasConstraintName("R_53");

                entity.HasOne(d => d.IdGoodsInMarketOwnNavigation)
                    .WithMany(p => p.GoodsWriteOffOwn)
                    .HasForeignKey(d => d.IdGoodsInMarketOwn)
                    .HasConstraintName("R_52");

                entity.HasOne(d => d.IdProductionNavigation)
                    .WithMany(p => p.GoodsWriteOffOwn)
                    .HasForeignKey(d => d.IdProduction)
                    .HasConstraintName("R_55");

                entity.HasOne(d => d.IdWriteOffReasonNavigation)
                    .WithMany(p => p.GoodsWriteOffOwn)
                    .HasForeignKey(d => d.IdWriteOffReason)
                    .HasConstraintName("R_54");
            });

            modelBuilder.Entity<Market>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKMarket");

                entity.Property(e => e.Id).HasColumnName("Id_Market");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdCity).HasColumnName("Id_City");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("Phone_Number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCityNavigation)
                    .WithMany(p => p.Market)
                    .HasForeignKey(d => d.IdCity)
                    .HasConstraintName("R_18");
            });

            modelBuilder.Entity<Producer>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKProducer");

                entity.Property(e => e.Id).HasColumnName("Id_Producer");

                entity.Property(e => e.IdCountry).HasColumnName("Id_Country");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCountryNavigation)
                    .WithMany(p => p.Producer)
                    .HasForeignKey(d => d.IdCountry)
                    .HasConstraintName("R_15");
            });

            modelBuilder.Entity<Production>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKProduction");

                entity.Property(e => e.Id).HasColumnName("Id_Production");

                entity.Property(e => e.BestBefore)
                    .HasColumnName("Best_Before")
                    .HasColumnType("date");

                entity.Property(e => e.IdEmployee).HasColumnName("Id_Employee");

                entity.Property(e => e.IdGoodsOwn).HasColumnName("Id_Goods_Own");

                entity.Property(e => e.ManufactureDate)
                    .HasColumnName("Manufacture_Date")
                    .HasColumnType("date");

                entity.Property(e => e.ProductionCode).HasColumnName("Production_Code");

                entity.Property(e => e.TotalCost).HasColumnName("Total_Cost");

                entity.HasOne(d => d.IdEmployeeNavigation)
                    .WithMany(p => p.Production)
                    .HasForeignKey(d => d.IdEmployee)
                    .HasConstraintName("R_26");

                entity.HasOne(d => d.IdGoodsOwnNavigation)
                    .WithMany(p => p.Production)
                    .HasForeignKey(d => d.IdGoodsOwn)
                    .HasConstraintName("R_22");
            });

            modelBuilder.Entity<ProductionContents>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKProduction_Contents");

                entity.ToTable("Production_Contents");

                entity.Property(e => e.Id).HasColumnName("Id_Production_Contents");

                entity.Property(e => e.IdGoodsInMarket).HasColumnName("Id_Goods_in_Market");

                entity.Property(e => e.IdProduction).HasColumnName("Id_Production");

                entity.HasOne(d => d.IdGoodsInMarketNavigation)
                    .WithMany(p => p.ProductionContents)
                    .HasForeignKey(d => d.IdGoodsInMarket)
                    .HasConstraintName("R_46");

                entity.HasOne(d => d.IdProductionNavigation)
                    .WithMany(p => p.ProductionContents)
                    .HasForeignKey(d => d.IdProduction)
                    .HasConstraintName("R_45");
            });

            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKProvider");

                entity.Property(e => e.Id).HasColumnName("Id_Provider");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyTitle)
                    .HasColumnName("Company_Title")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasColumnName("Contact_Person")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IdCity).HasColumnName("Id_City");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("Phone_Number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCityNavigation)
                    .WithMany(p => p.Provider)
                    .HasForeignKey(d => d.IdCity)
                    .HasConstraintName("R_4");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKRole");

                entity.Property(e => e.Id).HasColumnName("Id_Role");

                entity.Property(e => e.Title)
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKSale");

                entity.Property(e => e.Id).HasColumnName("Id_Sale");

                entity.Property(e => e.CheckNumber)
                    .HasColumnName("Check_Number")
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.IdClient).HasColumnName("Id_Client");

                entity.Property(e => e.IdEmployee).HasColumnName("Id_Employee");

                entity.Property(e => e.Total).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdClientNavigation)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.IdClient)
                    .HasConstraintName("R_39");

                entity.HasOne(d => d.IdEmployeeNavigation)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.IdEmployee)
                    .HasConstraintName("R_44");
            });

            modelBuilder.Entity<VProductProducersv2>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vProductProducersv2");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasColumnName("Product_Code")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.TitleCountry)
                    .HasColumnName("Title_Country")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TitleВиробника)
                    .HasColumnName("Title_виробника")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TitleТовару)
                    .HasColumnName("Title_товару")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.ЗагальнаВартість).HasColumnName("Загальна_вартість");
            });

            modelBuilder.Entity<VProduction>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vProduction");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ManufactureDate)
                    .HasColumnName("Manufacture_Date")
                    .HasColumnType("date");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("Phone_Number")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ProductCode)
                    .HasColumnName("Product_Code")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VProductsInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vProductsInfo");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyTitle)
                    .HasColumnName("Company_Title")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConsignmentNumber)
                    .HasColumnName("Consignment_Number")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DeliveryDate)
                    .HasColumnName("Delivery_Date")
                    .HasColumnType("datetime");

                entity.Property(e => e.ManufactureDate)
                    .HasColumnName("Manufacture_Date")
                    .HasColumnType("date");

                entity.Property(e => e.ProductCode)
                    .HasColumnName("Product_Code")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TitleCity)
                    .HasColumnName("Title_City")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VWritingOffOwnProducts>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vWritingOffOwnProducts");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.IdGoodsInMarketOwn).HasColumnName("Id_Goods_in_Market_Own");

                entity.Property(e => e.ManufactureDate)
                    .HasColumnName("Manufacture_Date")
                    .HasColumnType("date");

                entity.Property(e => e.TotalCost).HasColumnName("Total_Cost");
            });

            modelBuilder.Entity<VWritingOffProducts>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vWritingOffProducts");

                entity.Property(e => e.DateСписування)
                    .HasColumnName("Date_списування")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.IdGoodsInMarket).HasColumnName("Id_Goods_in_Market");

                entity.Property(e => e.ShipmentDateTime)
                    .HasColumnName("Shipment_Date_Time")
                    .HasColumnType("datetime");

                entity.Property(e => e.КСтьСписаногоТовару).HasColumnName("К_сть_списаного_товару");
            });

            modelBuilder.Entity<WriteOffReason>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("XPKWrite_off_Reason");

                entity.ToTable("Write_off_Reason");

                entity.Property(e => e.Id).HasColumnName("Id_Write_off_Reason");

                entity.Property(e => e.Description)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        private void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            
        }
    }
}