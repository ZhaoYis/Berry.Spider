﻿// <auto-generated />
using System;
using Berry.Spider.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Volo.Abp.EntityFrameworkCore;

#nullable disable

namespace Berry.Spider.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(SpiderDbContext))]
    [Migration("20221107095111_Add_Content_Tables")]
    partial class Add_Content_Tables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.MySql)
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Berry.Spider.Domain.SpiderBasic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("CreationTime");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<int>("From")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("LastModificationTime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("SpiderBasic", (string)null);
                });

            modelBuilder.Entity("Berry.Spider.Domain.SpiderContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .HasColumnType("longtext")
                        .HasColumnName("作者");

                    b.Property<int>("Collected")
                        .HasColumnType("int")
                        .HasColumnName("已采");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("内容");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("出处");

                    b.Property<string>("GroupId")
                        .HasColumnType("longtext");

                    b.Property<string>("Keywords")
                        .HasColumnType("longtext")
                        .HasColumnName("关键字");

                    b.Property<string>("PageUrl")
                        .HasColumnType("longtext")
                        .HasColumnName("PageUrl");

                    b.Property<int>("Published")
                        .HasColumnType("int")
                        .HasColumnName("已发");

                    b.Property<string>("Tag")
                        .HasColumnType("longtext")
                        .HasColumnName("tag");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("时间");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("标题");

                    b.HasKey("Id");

                    b.ToTable("Content", (string)null);
                });

            modelBuilder.Entity("Berry.Spider.Domain.SpiderContent_Composition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .HasColumnType("longtext")
                        .HasColumnName("作者");

                    b.Property<int>("Collected")
                        .HasColumnType("int")
                        .HasColumnName("已采");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("内容");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("出处");

                    b.Property<string>("GroupId")
                        .HasColumnType("longtext");

                    b.Property<string>("Keywords")
                        .HasColumnType("longtext")
                        .HasColumnName("关键字");

                    b.Property<string>("PageUrl")
                        .HasColumnType("longtext")
                        .HasColumnName("PageUrl");

                    b.Property<int>("Published")
                        .HasColumnType("int")
                        .HasColumnName("已发");

                    b.Property<string>("Tag")
                        .HasColumnType("longtext")
                        .HasColumnName("tag");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("时间");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("标题");

                    b.HasKey("Id");

                    b.ToTable("Content_Composition", (string)null);
                });

            modelBuilder.Entity("Berry.Spider.Domain.SpiderContent_HighQualityQA", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .HasColumnType("longtext")
                        .HasColumnName("作者");

                    b.Property<int>("Collected")
                        .HasColumnType("int")
                        .HasColumnName("已采");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("内容");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("出处");

                    b.Property<string>("GroupId")
                        .HasColumnType("longtext");

                    b.Property<string>("Keywords")
                        .HasColumnType("longtext")
                        .HasColumnName("关键字");

                    b.Property<string>("PageUrl")
                        .HasColumnType("longtext")
                        .HasColumnName("PageUrl");

                    b.Property<int>("Published")
                        .HasColumnType("int")
                        .HasColumnName("已发");

                    b.Property<string>("Tag")
                        .HasColumnType("longtext")
                        .HasColumnName("tag");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("时间");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("标题");

                    b.HasKey("Id");

                    b.ToTable("Content_HighQualityQA", (string)null);
                });

            modelBuilder.Entity("Berry.Spider.Domain.SpiderTitleContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .HasColumnType("longtext");

                    b.Property<int>("Collected")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ExtraProperties")
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Published")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("TitleContent", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
