﻿// <auto-generated />
using Internal.Services.Movements.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Internal.Services.Movements.Data.Migrations
{
    [DbContext(typeof(MovementsDataContext))]
    [Migration("20220509112447_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("Internal.Services.Movements.Data.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerEmail")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerFirstName")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerLastName")
                        .HasColumnType("TEXT");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Internal.Services.Movements.Data.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExternalAccount")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProductType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ProductId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Internal.Services.Movements.Data.Models.ProductCustomer", b =>
                {
                    b.Property<int>("ProductCustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ProductCustomerId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductsCustomers");
                });

            modelBuilder.Entity("Internal.Services.Movements.Data.Models.ProductCustomer", b =>
                {
                    b.HasOne("Internal.Services.Movements.Data.Models.Customer", "Customer")
                        .WithMany("ProductCustomers")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Internal.Services.Movements.Data.Models.Product", "Product")
                        .WithMany("ProductCustomers")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Internal.Services.Movements.Data.Models.Customer", b =>
                {
                    b.Navigation("ProductCustomers");
                });

            modelBuilder.Entity("Internal.Services.Movements.Data.Models.Product", b =>
                {
                    b.Navigation("ProductCustomers");
                });
#pragma warning restore 612, 618
        }
    }
}
