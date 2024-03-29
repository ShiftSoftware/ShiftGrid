﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Test.NETCore.EF;

#nullable disable

namespace Test.NETCore.PostgresMigrations
{
    [DbContext(typeof(PostgresDB))]
    partial class PostgresDBModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ShiftGrid.Test.Shared.Models.TestItem", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("ParentTestItemId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<long?>("TypeId")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("ParentTestItemId");

                    b.HasIndex("TypeId");

                    b.ToTable("TestItems");
                });

            modelBuilder.Entity("ShiftGrid.Test.Shared.Models.Type", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Types");
                });

            modelBuilder.Entity("ShiftGrid.Test.Shared.Models.TestItem", b =>
                {
                    b.HasOne("ShiftGrid.Test.Shared.Models.TestItem", "ParentTestItem")
                        .WithMany("ChildTestItems")
                        .HasForeignKey("ParentTestItemId");

                    b.HasOne("ShiftGrid.Test.Shared.Models.Type", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");

                    b.Navigation("ParentTestItem");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("ShiftGrid.Test.Shared.Models.TestItem", b =>
                {
                    b.Navigation("ChildTestItems");
                });
#pragma warning restore 612, 618
        }
    }
}
