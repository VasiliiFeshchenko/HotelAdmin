﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MvcTest.Data;

#nullable disable

namespace MvcTest.Migrations
{
    [DbContext(typeof(MvcTestContext))]
    partial class MvcTestContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.BookableObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("AdditionalPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDogAllowed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int>("NumberOfAdditionalBeds")
                        .HasColumnType("int");

                    b.Property<string>("RoomUrl")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("BookableObject");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BookableObject");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("AdditionalPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("BasePriceNumberOfPeople")
                        .HasColumnType("int");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<bool>("IsOwnKitchen")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("NumberOfPeople")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("HotelId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("EMail")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.MoneyTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsRefund")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("TransactionMethodId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("TransactionMethodId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime?>("CancelationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ClientId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<bool>("IsCanceled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("LastUpdateDate")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("PayedPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("Prepayment")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime>("PrepaymentDeadline")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int?>("SourceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("SourceId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.OrderSource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("OrderSource");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.TransactionMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("TransactionMethod");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.RoomStateSetters.BookableObjectStateSetter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("BookableObjectId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BookableObjectId");

                    b.ToTable("BookableObjectStateSetter");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BookableObjectStateSetter");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("MvcTest.Models.Hotel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("ParametersImageUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("ShowPetCheckbox")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Hotel");
                });

            modelBuilder.Entity("MvcTest.Models.HotelManagerModels.EmailCredentials", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("EmailBody")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmailSubject")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("HotelId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Port")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("HotelId");

                    b.ToTable("EmailCredentials");
                });

            modelBuilder.Entity("MvcTest.Models.HotelManagerModels.KeyCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("keyCodes");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.Room", b =>
                {
                    b.HasBaseType("HotelAdmin.Data.Models.BookableObjects.BookableObject");

                    b.Property<int?>("RoomCombinationId")
                        .HasColumnType("int");

                    b.HasIndex("HotelId");

                    b.HasIndex("RoomCombinationId");

                    b.HasDiscriminator().HasValue("Room");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.RoomCombination", b =>
                {
                    b.HasBaseType("HotelAdmin.Data.Models.BookableObjects.BookableObject");

                    b.HasIndex("HotelId");

                    b.HasDiscriminator().HasValue("RoomCombination");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.RoomStateSetters.ClosureSet", b =>
                {
                    b.HasBaseType("HotelAdmin.Data.Models.RoomStateSetters.BookableObjectStateSetter");

                    b.HasDiscriminator().HasValue("ClosureSet");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.RoomStateSetters.PriceSet", b =>
                {
                    b.HasBaseType("HotelAdmin.Data.Models.RoomStateSetters.BookableObjectStateSetter");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.HasDiscriminator().HasValue("PriceSet");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.RoomStateSetters.Reservation", b =>
                {
                    b.HasBaseType("HotelAdmin.Data.Models.RoomStateSetters.BookableObjectStateSetter");

                    b.Property<int>("NumberofPeople")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.HasIndex("OrderId");

                    b.HasDiscriminator().HasValue("Reservation");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.BookableObject", b =>
                {
                    b.HasOne("HotelAdmin.Data.Models.BookableObjects.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.Category", b =>
                {
                    b.HasOne("MvcTest.Models.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.MoneyTransaction", b =>
                {
                    b.HasOne("HotelAdmin.Data.Models.Order.Order", "Order")
                        .WithMany("MoneyTransactions")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelAdmin.Data.Models.Order.TransactionMethod", "TransactionMethod")
                        .WithMany()
                        .HasForeignKey("TransactionMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("TransactionMethod");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.Order", b =>
                {
                    b.HasOne("HotelAdmin.Data.Models.Order.Client", "Client")
                        .WithMany("Orders")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelAdmin.Data.Models.Order.OrderSource", "Source")
                        .WithMany()
                        .HasForeignKey("SourceId");

                    b.Navigation("Client");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.RoomStateSetters.BookableObjectStateSetter", b =>
                {
                    b.HasOne("HotelAdmin.Data.Models.BookableObjects.BookableObject", "BookableObject")
                        .WithMany("RoomStateSetters")
                        .HasForeignKey("BookableObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookableObject");
                });

            modelBuilder.Entity("MvcTest.Models.HotelManagerModels.EmailCredentials", b =>
                {
                    b.HasOne("MvcTest.Models.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.Room", b =>
                {
                    b.HasOne("MvcTest.Models.Hotel", "Hotel")
                        .WithMany("Rooms")
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HotelAdmin.Data.Models.BookableObjects.RoomCombination", "RoomCombination")
                        .WithMany("Rooms")
                        .HasForeignKey("RoomCombinationId");

                    b.Navigation("Hotel");

                    b.Navigation("RoomCombination");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.RoomCombination", b =>
                {
                    b.HasOne("MvcTest.Models.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hotel");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.RoomStateSetters.Reservation", b =>
                {
                    b.HasOne("HotelAdmin.Data.Models.Order.Order", "Order")
                        .WithMany("Reservations")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.BookableObject", b =>
                {
                    b.Navigation("RoomStateSetters");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.Client", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.Order.Order", b =>
                {
                    b.Navigation("MoneyTransactions");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("MvcTest.Models.Hotel", b =>
                {
                    b.Navigation("Rooms");
                });

            modelBuilder.Entity("HotelAdmin.Data.Models.BookableObjects.RoomCombination", b =>
                {
                    b.Navigation("Rooms");
                });
#pragma warning restore 612, 618
        }
    }
}
