﻿// <auto-generated />
using System;
using LogProcessor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LogProcessor.Migrations
{
    [DbContext(typeof(LogContext))]
    [Migration("20210513144517_RemoveFileAddTextColumn")]
    partial class RemoveFileAddTextColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.LogMessageEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LogMessages");
                });

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.PoisonedMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("LogMessageId")
                        .HasColumnType("integer");

                    b.Property<int>("ViolatingMessageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LogMessageId");

                    b.HasIndex("ViolatingMessageId");

                    b.ToTable("PoisonedMessages");
                });

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.SessionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string[]>("ExternalParticipants")
                        .HasColumnType("jsonb");

                    b.Property<string[]>("InternalParticipants")
                        .HasColumnType("jsonb");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("ShouldLoad")
                        .HasColumnType("boolean");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SessionTypes");
                });

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.ViolatingMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("LogMessageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LogMessageId");

                    b.ToTable("ViolatingMessages");
                });

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.LogMessageEntity", b =>
                {
                    b.OwnsOne("Services.Shared.Models.LogMessage", "Message", b1 =>
                        {
                            b1.Property<int>("LogMessageEntityId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer")
                                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                            b1.Property<string>("Destination")
                                .HasColumnType("text");

                            b1.Property<int>("Direction")
                                .HasColumnType("integer");

                            b1.Property<string>("Origin")
                                .HasColumnType("text");

                            b1.Property<string>("SessionId")
                                .HasColumnType("text");

                            b1.Property<string>("TargetApi")
                                .HasColumnType("text");

                            b1.Property<DateTime>("Time")
                                .HasColumnType("timestamp without time zone");

                            b1.HasKey("LogMessageEntityId");

                            b1.HasIndex("SessionId");

                            b1.ToTable("LogMessages");

                            b1.WithOwner()
                                .HasForeignKey("LogMessageEntityId");
                        });

                    b.Navigation("Message");
                });

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.PoisonedMessage", b =>
                {
                    b.HasOne("LogProcessor.Models.Infrastructure.LogMessageEntity", "LogMessage")
                        .WithMany()
                        .HasForeignKey("LogMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LogProcessor.Models.Infrastructure.ViolatingMessage", "ViolatingMessage")
                        .WithMany("PoisonedMessages")
                        .HasForeignKey("ViolatingMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LogMessage");

                    b.Navigation("ViolatingMessage");
                });

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.ViolatingMessage", b =>
                {
                    b.HasOne("LogProcessor.Models.Infrastructure.LogMessageEntity", "LogMessage")
                        .WithMany()
                        .HasForeignKey("LogMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LogMessage");
                });

            modelBuilder.Entity("LogProcessor.Models.Infrastructure.ViolatingMessage", b =>
                {
                    b.Navigation("PoisonedMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
