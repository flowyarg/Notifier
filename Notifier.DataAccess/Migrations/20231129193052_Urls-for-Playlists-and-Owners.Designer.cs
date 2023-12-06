﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Notifier.DataAccess;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Notifier.DataAccess.Migrations
{
    [DbContext(typeof(NotifierDbContext))]
    [Migration("20231129193052_Urls-for-Playlists-and-Owners")]
    partial class UrlsforPlaylistsandOwners
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Notifier.DataAccess.Model.AccessToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("IV")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("ValidThrough")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("AccessTokens");
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Owner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Owners");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsBeingTracked")
                        .HasColumnType("boolean");

                    b.Property<string>("PlaylistId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id", "OwnerId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Duration")
                        .IsRequired()
                        .HasColumnType("character varying(48)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<int>("PlaylistId")
                        .HasColumnType("integer");

                    b.Property<string>("PreviewUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("PublicationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("VideoId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PlaylistId", "OwnerId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Group", b =>
                {
                    b.HasBaseType("Notifier.DataAccess.Model.Owner");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.User", b =>
                {
                    b.HasBaseType("Notifier.DataAccess.Model.Owner");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Playlist", b =>
                {
                    b.HasOne("Notifier.DataAccess.Model.Owner", "Owner")
                        .WithMany("Playlists")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Video", b =>
                {
                    b.HasOne("Notifier.DataAccess.Model.Playlist", "Playlist")
                        .WithMany()
                        .HasForeignKey("PlaylistId", "OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Playlist");
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Group", b =>
                {
                    b.HasOne("Notifier.DataAccess.Model.Owner", null)
                        .WithOne()
                        .HasForeignKey("Notifier.DataAccess.Model.Group", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.User", b =>
                {
                    b.HasOne("Notifier.DataAccess.Model.Owner", null)
                        .WithOne()
                        .HasForeignKey("Notifier.DataAccess.Model.User", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Notifier.DataAccess.Model.Owner", b =>
                {
                    b.Navigation("Playlists");
                });
#pragma warning restore 612, 618
        }
    }
}
