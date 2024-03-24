﻿// <auto-generated />
using DominosStockOrder.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DominosStockOrder.Server.Migrations
{
    [DbContext(typeof(StockOrderContext))]
    [Migration("20240324044717_Removed-InventoryItem-PortalItemId")]
    partial class RemovedInventoryItemPortalItemId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

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

                    b.Property<bool>("ManualCount")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Multiplier")
                        .HasColumnType("REAL");

                    b.Property<float>("PackSize")
                        .HasColumnType("REAL");

                    b.HasKey("Code");

                    b.ToTable("InventoryItems");
                });

            modelBuilder.Entity("DominosStockOrder.Server.Models.ItemInitialFoodTheo", b =>
                {
                    b.Property<string>("PulseCode")
                        .HasColumnType("TEXT");

                    b.Property<float>("InitialFoodTheo")
                        .HasColumnType("REAL");

                    b.HasKey("PulseCode");

                    b.ToTable("InitialFoodTheos");
                });

            modelBuilder.Entity("DominosStockOrder.Server.Models.ItemInitialFoodTheo", b =>
                {
                    b.HasOne("DominosStockOrder.Server.Models.InventoryItem", "Item")
                        .WithMany()
                        .HasForeignKey("PulseCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });
#pragma warning restore 612, 618
        }
    }
}
