﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Migrations
{
    [DbContext(typeof(ReuseventoryDbContext))]
    [Migration("20210502141845_ListingPrice")]
    partial class ListingPrice
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("ReuseventoryApi.Models.Listing", b =>
                {
                    b.Property<Guid?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("description")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("modified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<double?>("price")
                        .HasColumnType("double precision");

                    b.Property<Guid?>("userId")
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("userId");

                    b.ToTable("Listings");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.ListingImage", b =>
                {
                    b.Property<Guid?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("contentType")
                        .HasColumnType("text");

                    b.Property<string>("fileName")
                        .HasColumnType("text");

                    b.Property<byte[]>("image")
                        .HasColumnType("bytea");

                    b.Property<Guid?>("listingId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.HasKey("id");

                    b.HasIndex("listingId");

                    b.ToTable("ListingImages");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.ListingTag", b =>
                {
                    b.Property<Guid?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("listingId")
                        .HasColumnType("uuid");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("id");

                    b.HasIndex("listingId");

                    b.ToTable("ListingTags");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.User", b =>
                {
                    b.Property<Guid?>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("email")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("phone")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.Listing", b =>
                {
                    b.HasOne("ReuseventoryApi.Models.User", "user")
                        .WithMany("listings")
                        .HasForeignKey("userId");

                    b.Navigation("user");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.ListingImage", b =>
                {
                    b.HasOne("ReuseventoryApi.Models.Listing", "listing")
                        .WithMany()
                        .HasForeignKey("listingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("listing");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.ListingTag", b =>
                {
                    b.HasOne("ReuseventoryApi.Models.Listing", "listing")
                        .WithMany("tags")
                        .HasForeignKey("listingId");

                    b.Navigation("listing");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.Listing", b =>
                {
                    b.Navigation("tags");
                });

            modelBuilder.Entity("ReuseventoryApi.Models.User", b =>
                {
                    b.Navigation("listings");
                });
#pragma warning restore 612, 618
        }
    }
}
