﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(IronGymContext))]
    partial class IronGymContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Exercise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ExerciseDayId")
                        .HasColumnType("int");

                    b.Property<string>("ExerciseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Repetitions")
                        .HasColumnType("int");

                    b.Property<int>("Series")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ExerciseDayId");

                    b.ToTable("Exercise");
                });

            modelBuilder.Entity("Domain.Entities.ExerciseDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WeekDay")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ExerciseDay");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsEmailVerified")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VerificationCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.Exercise", b =>
                {
                    b.HasOne("Domain.Entities.ExerciseDay", null)
                        .WithMany("Exercises")
                        .HasForeignKey("ExerciseDayId");
                });

            modelBuilder.Entity("Domain.Entities.ExerciseDay", b =>
                {
                    b.HasOne("Domain.Entities.User", null)
                        .WithMany("Schedule")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Domain.Entities.ExerciseDay", b =>
                {
                    b.Navigation("Exercises");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("Schedule");
                });
#pragma warning restore 612, 618
        }
    }
}
