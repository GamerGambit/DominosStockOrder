﻿// <auto-generated />
using System;
using DominosStockOrder.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    [DbContext(typeof(StockOrderContext))]
    [Migration("20240427045452_Added-Settings")]
    partial class AddedSettings
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("DominosStockOrder.Server.Models.InventoryItem", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("DoubleCheck")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("IgnoreFoodTheoBefore")
                        .HasColumnType("TEXT");

                    b.Property<float?>("InitialFoodTheo")
                        .HasColumnType("REAL");

                    b.Property<bool>("ManualCount")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Multiplier")
                        .HasColumnType("REAL");

                    b.Property<float>("PackSize")
                        .HasColumnType("REAL");

                    b.HasKey("Code");

                    b.ToTable("InventoryItems");
                });

            modelBuilder.Entity("DominosStockOrder.Server.Models.Settings", b =>
                {
                    b.Property<int>("NumFoodTheoWeeks")
                        .HasColumnType("INTEGER");

                    b.ToTable("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
