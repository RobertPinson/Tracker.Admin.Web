using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Tracker.Admin.Web.Data;

namespace Tracker.Admin.Web.Migrations
{
    [DbContext(typeof(TrackerDbContext))]
    [Migration("20160913135720_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Tracker.Admin.Web.Models.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Uid")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<int>("LocationId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.Movement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CardId");

                    b.Property<int?>("CardId1");

                    b.Property<int>("DeviceId");

                    b.Property<int>("LocationId");

                    b.Property<DateTime>("SwipeTime");

                    b.HasKey("Id");

                    b.HasIndex("CardId1");

                    b.HasIndex("DeviceId");

                    b.HasIndex("LocationId");

                    b.ToTable("Movement");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<byte[]>("Image");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.PersonCard", b =>
                {
                    b.Property<int>("PersonId");

                    b.Property<int>("CardId");

                    b.HasKey("PersonId", "CardId");

                    b.HasIndex("CardId");

                    b.HasIndex("PersonId");

                    b.ToTable("PersonCard");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.Device", b =>
                {
                    b.HasOne("Tracker.Admin.Web.Models.Location", "Location")
                        .WithMany("Devices")
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.Movement", b =>
                {
                    b.HasOne("Tracker.Admin.Web.Models.Card")
                        .WithMany("Movements")
                        .HasForeignKey("CardId1");

                    b.HasOne("Tracker.Admin.Web.Models.Device", "Device")
                        .WithMany("Movements")
                        .HasForeignKey("DeviceId");

                    b.HasOne("Tracker.Admin.Web.Models.Location", "Location")
                        .WithMany("Movements")
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("Tracker.Admin.Web.Models.PersonCard", b =>
                {
                    b.HasOne("Tracker.Admin.Web.Models.Card", "Card")
                        .WithMany("PersonCards")
                        .HasForeignKey("CardId");

                    b.HasOne("Tracker.Admin.Web.Models.Person", "Person")
                        .WithMany("PersonCards")
                        .HasForeignKey("PersonId");
                });
        }
    }
}
