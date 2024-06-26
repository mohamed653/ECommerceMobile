﻿// <auto-generated />
using System;
using ECommereceApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ECommereceApi.Migrations
{
    [DbContext(typeof(ECommerceContext))]
    partial class ECommerceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ECommereceApi.Models.Admin", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("IsSuperAdmin")
                        .HasColumnType("bit");

                    b.HasKey("UserId");

                    b.ToTable("Admin");
                });

            modelBuilder.Entity("ECommereceApi.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("ImageId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CategoryId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("ECommereceApi.Models.CategorySubCategory", b =>
                {
                    b.Property<int>("CategorySubCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategorySubCategoryId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("SubCategoryId")
                        .HasColumnType("int");

                    b.HasKey("CategorySubCategoryId");

                    b.HasIndex("SubCategoryId");

                    b.HasIndex("CategoryId", "SubCategoryId")
                        .IsUnique();

                    b.ToTable("CategorySubCategory");
                });

            modelBuilder.Entity("ECommereceApi.Models.CategorySubCategoryValues", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategorySubCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("ImageId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("CategorySubCategoryId", "Value")
                        .IsUnique();

                    b.ToTable("CategorySubCategoryValues");
                });

            modelBuilder.Entity("ECommereceApi.Models.Customer", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("ECommereceApi.Models.NotificationMessage", b =>
                {
                    b.Property<int>("MsgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MsgId"));

                    b.Property<string>("HiddenLink")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("MsgContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Seen")
                        .HasColumnType("bit");

                    b.Property<DateTime>("SendingDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("MsgId");

                    b.HasIndex("UserId");

                    b.ToTable("NotificationMessage");
                });

            modelBuilder.Entity("ECommereceApi.Models.Offer", b =>
                {
                    b.Property<int>("OfferId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OfferId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateOnly>("OfferDate")
                        .HasColumnType("date");

                    b.Property<decimal?>("PackageDiscount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Title")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("OfferId");

                    b.HasIndex("Title")
                        .IsUnique()
                        .HasFilter("[Title] IS NOT NULL");

                    b.ToTable("Offer");
                });

            modelBuilder.Entity("ECommereceApi.Models.Order", b =>
                {
                    b.Property<Guid>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ArrivalInDays")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Governerate")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("OfferId")
                        .HasColumnType("int");

                    b.Property<int?>("PaymentMethod")
                        .HasColumnType("int");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal?>("Score")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateOnly?>("ShippingDate")
                        .HasColumnType("date");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("TotalAmount")
                        .HasColumnType("int");

                    b.Property<double?>("TotalPrice")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("OrderId");

                    b.HasIndex("OfferId");

                    b.HasIndex("UserId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("ECommereceApi.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Discount")
                        .HasColumnType("float");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("OriginalPrice")
                        .HasColumnType("float");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductCart", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ProductAmount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.HasKey("ProductId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ProductCart");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductCategorySubCategoryValues", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("CategorySubCategoryValuesId")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "CategorySubCategoryValuesId");

                    b.HasIndex("CategorySubCategoryValuesId");

                    b.ToTable("ProductCategorySubCategoryValues");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductImage", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ImageId")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("ProductId", "ImageId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductOffer", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("OfferId")
                        .HasColumnType("int");

                    b.Property<double?>("Discount")
                        .HasColumnType("float");

                    b.Property<int>("ProductAmount")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "OfferId");

                    b.HasIndex("OfferId");

                    b.ToTable("ProductOffer");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductOrder", b =>
                {
                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.HasKey("OrderId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductOrder");
                });

            modelBuilder.Entity("ECommereceApi.Models.Rate", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NumOfStars")
                        .HasColumnType("int");

                    b.Property<DateOnly>("RateDate")
                        .HasColumnType("date");

                    b.HasKey("ProductId", "CustomerId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Rates");
                });

            modelBuilder.Entity("ECommereceApi.Models.SubCategory", b =>
                {
                    b.Property<int>("SubCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubCategoryId"));

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("SubCategoryId");

                    b.ToTable("SubCategory");
                });

            modelBuilder.Entity("ECommereceApi.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("City")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("DateDeleted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Governorate")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("LName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PostalCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Street")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("VertificationCode")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.HasKey("UserId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ECommereceApi.Models.WebInfo", b =>
                {
                    b.Property<int>("WebInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WebInfoId"));

                    b.Property<string>("CustomerServicePhone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FacebookAccount")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("InstagramAccount")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("StoreAddress")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("WebLogoImageUrl")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("WebName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("WebPhone")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("WebInfoId");

                    b.ToTable("Web_Info");
                });

            modelBuilder.Entity("ECommereceApi.Models.WishList", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("WishList");
                });

            modelBuilder.Entity("ECommereceApi.Models.Admin", b =>
                {
                    b.HasOne("ECommereceApi.Models.User", "User")
                        .WithOne("Admin")
                        .HasForeignKey("ECommereceApi.Models.Admin", "UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Admin_User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ECommereceApi.Models.CategorySubCategory", b =>
                {
                    b.HasOne("ECommereceApi.Models.Category", "Category")
                        .WithMany("CategorySubCategory")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECommereceApi.Models.SubCategory", "SubCategory")
                        .WithMany("CategorySubCategories")
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("SubCategory");
                });

            modelBuilder.Entity("ECommereceApi.Models.CategorySubCategoryValues", b =>
                {
                    b.HasOne("ECommereceApi.Models.CategorySubCategory", "CategorySubCategory")
                        .WithMany("CategorySubCategoryValues")
                        .HasForeignKey("CategorySubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CategorySubCategory");
                });

            modelBuilder.Entity("ECommereceApi.Models.Customer", b =>
                {
                    b.HasOne("ECommereceApi.Models.User", "User")
                        .WithOne("Customer")
                        .HasForeignKey("ECommereceApi.Models.Customer", "UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Customer_User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ECommereceApi.Models.NotificationMessage", b =>
                {
                    b.HasOne("ECommereceApi.Models.User", "User")
                        .WithMany("NotificationMessages")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_NotificationMessage_User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ECommereceApi.Models.Order", b =>
                {
                    b.HasOne("ECommereceApi.Models.Offer", "Offer")
                        .WithMany("Orders")
                        .HasForeignKey("OfferId");

                    b.HasOne("ECommereceApi.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Order_User");

                    b.Navigation("Offer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ECommereceApi.Models.Product", b =>
                {
                    b.HasOne("ECommereceApi.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_Product_Category");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductCart", b =>
                {
                    b.HasOne("ECommereceApi.Models.Product", "Product")
                        .WithMany("ProductCarts")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductCart_Product1");

                    b.HasOne("ECommereceApi.Models.User", "User")
                        .WithMany("ProductCarts")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductCart_User");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductCategorySubCategoryValues", b =>
                {
                    b.HasOne("ECommereceApi.Models.CategorySubCategoryValues", "CategorySubCategoryValues")
                        .WithMany("ProductCategorySubCategoryValues")
                        .HasForeignKey("CategorySubCategoryValuesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECommereceApi.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CategorySubCategoryValues");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductImage", b =>
                {
                    b.HasOne("ECommereceApi.Models.Product", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductImages_Product");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductOffer", b =>
                {
                    b.HasOne("ECommereceApi.Models.Offer", "Offer")
                        .WithMany("ProductOffers")
                        .HasForeignKey("OfferId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductOffer_Offer");

                    b.HasOne("ECommereceApi.Models.Product", "Product")
                        .WithMany("ProductOffers")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductOffer_Product");

                    b.Navigation("Offer");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ECommereceApi.Models.ProductOrder", b =>
                {
                    b.HasOne("ECommereceApi.Models.Order", "Order")
                        .WithMany("ProductOrders")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECommereceApi.Models.Product", "Product")
                        .WithMany("ProductOrders")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ECommereceApi.Models.Rate", b =>
                {
                    b.HasOne("ECommereceApi.Models.Customer", "Customer")
                        .WithMany("Rates")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_Rates_Customer");

                    b.HasOne("ECommereceApi.Models.Product", "Product")
                        .WithMany("Rates")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_Rates_Product");

                    b.Navigation("Customer");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ECommereceApi.Models.WishList", b =>
                {
                    b.HasOne("ECommereceApi.Models.Product", "Product")
                        .WithMany("WishLists")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ECommereceApi.Models.User", "User")
                        .WithMany("WishLists")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ECommereceApi.Models.Category", b =>
                {
                    b.Navigation("CategorySubCategory");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("ECommereceApi.Models.CategorySubCategory", b =>
                {
                    b.Navigation("CategorySubCategoryValues");
                });

            modelBuilder.Entity("ECommereceApi.Models.CategorySubCategoryValues", b =>
                {
                    b.Navigation("ProductCategorySubCategoryValues");
                });

            modelBuilder.Entity("ECommereceApi.Models.Customer", b =>
                {
                    b.Navigation("Rates");
                });

            modelBuilder.Entity("ECommereceApi.Models.Offer", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("ProductOffers");
                });

            modelBuilder.Entity("ECommereceApi.Models.Order", b =>
                {
                    b.Navigation("ProductOrders");
                });

            modelBuilder.Entity("ECommereceApi.Models.Product", b =>
                {
                    b.Navigation("ProductCarts");

                    b.Navigation("ProductImages");

                    b.Navigation("ProductOffers");

                    b.Navigation("ProductOrders");

                    b.Navigation("Rates");

                    b.Navigation("WishLists");
                });

            modelBuilder.Entity("ECommereceApi.Models.SubCategory", b =>
                {
                    b.Navigation("CategorySubCategories");
                });

            modelBuilder.Entity("ECommereceApi.Models.User", b =>
                {
                    b.Navigation("Admin");

                    b.Navigation("Customer");

                    b.Navigation("NotificationMessages");

                    b.Navigation("Orders");

                    b.Navigation("ProductCarts");

                    b.Navigation("WishLists");
                });
#pragma warning restore 612, 618
        }
    }
}
