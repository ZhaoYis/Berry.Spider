﻿// <auto-generated />
using System;
using Berry.Spider.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Volo.Abp.EntityFrameworkCore;

#nullable disable

namespace Berry.Spider.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(SpiderDbContext))]
    partial class SpiderDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.MySql)
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Berry.Spider.Domain.SpiderBasicInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("CreationTime");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
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

                    b.ToTable("spider_basic_info", (string)null);
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
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("内容");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("出处");

                    b.Property<string>("GroupId")
                        .HasColumnType("longtext");

                    b.Property<string>("IdentityId")
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

                    b.Property<string>("TraceCode")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("spider_content", (string)null);
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
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("内容");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("出处");

                    b.Property<string>("GroupId")
                        .HasColumnType("longtext");

                    b.Property<string>("IdentityId")
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

                    b.Property<string>("TraceCode")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("spider_content_composition", (string)null);
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
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("内容");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("出处");

                    b.Property<string>("GroupId")
                        .HasColumnType("longtext");

                    b.Property<string>("IdentityId")
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

                    b.Property<string>("TraceCode")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("spider_content_high_quality_qa", (string)null);
                });

            modelBuilder.Entity("Berry.Spider.Domain.SpiderContent_Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("出处");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("时间");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("标题");

                    b.Property<string>("TraceCode")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("spider_content_keyword", (string)null);
                });

            modelBuilder.Entity("Berry.Spider.Domain.SpiderContent_Title", b =>
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
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
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

                    b.Property<string>("TraceCode")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("spider_content_title", (string)null);
                });

            modelBuilder.Entity("Berry.Spider.Domain.WeatherForecast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Adcode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("DayPower")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("DayTempFloat")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("DayWeather")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DayWind")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ExtraProperties")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("ExtraProperties");

                    b.Property<string>("NightPower")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("NightTempFloat")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("NightWeather")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NightWind")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ReportTime")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("ReportTimeTicks")
                        .HasColumnType("bigint");

                    b.Property<int>("Week")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("spider_weather_forecast", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
