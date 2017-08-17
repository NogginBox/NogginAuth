﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Noggin.SampleSite.Data;

namespace Noggin.SampleSite.Migrations
{
    [DbContext(typeof(SampleSimpleDbContext))]
    [Migration("20170817160603_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Noggin.SampleSite.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Noggin.SampleSite.Data.UserAuthAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Provider");

                    b.Property<int?>("UserId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserAuthAccount");
                });

            modelBuilder.Entity("Noggin.SampleSite.Data.UserAuthAccount", b =>
                {
                    b.HasOne("Noggin.SampleSite.Data.User")
                        .WithMany("AuthAccounts")
                        .HasForeignKey("UserId");
                });
        }
    }
}
