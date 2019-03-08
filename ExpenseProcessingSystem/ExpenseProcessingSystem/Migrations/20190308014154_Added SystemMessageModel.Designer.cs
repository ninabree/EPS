﻿// <auto-generated />
using System;
using ExpenseProcessingSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExpenseProcessingSystem.Migrations
{
    [DbContext(typeof(EPSDbContext))]
    [Migration("20190308014154_Added SystemMessageModel")]
    partial class AddedSystemMessageModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ExpenseProcessingSystem.Models.AccountModel", b =>
                {
                    b.Property<int>("Acc_UserID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Acc_Approver_ID");

                    b.Property<string>("Acc_Comment");

                    b.Property<DateTime>("Acc_Created_Date");

                    b.Property<int>("Acc_Creator_ID");

                    b.Property<int>("Acc_DeptID");

                    b.Property<string>("Acc_Email");

                    b.Property<string>("Acc_FName");

                    b.Property<bool>("Acc_InUse");

                    b.Property<string>("Acc_LName");

                    b.Property<DateTime>("Acc_Last_Updated");

                    b.Property<string>("Acc_Password");

                    b.Property<string>("Acc_Role");

                    b.Property<string>("Acc_Status");

                    b.Property<string>("Acc_UserName");

                    b.HasKey("Acc_UserID");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMDeptModel", b =>
                {
                    b.Property<int>("Dept_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Dept_Approver_ID");

                    b.Property<string>("Dept_Code");

                    b.Property<DateTime>("Dept_Created_Date");

                    b.Property<int>("Dept_Creator_ID");

                    b.Property<DateTime>("Dept_Last_Updated");

                    b.Property<string>("Dept_Name");

                    b.Property<string>("Dept_Status");

                    b.Property<bool>("Dept_isDeleted");

                    b.HasKey("Dept_ID");

                    b.ToTable("DMDept");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMPayeeModel", b =>
                {
                    b.Property<int>("Payee_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Payee_Address");

                    b.Property<int>("Payee_Approver_ID");

                    b.Property<DateTime>("Payee_Created_Date");

                    b.Property<int>("Payee_Creator_ID");

                    b.Property<DateTime>("Payee_Last_Updated");

                    b.Property<string>("Payee_Name");

                    b.Property<int>("Payee_No");

                    b.Property<string>("Payee_Status");

                    b.Property<string>("Payee_TIN");

                    b.Property<string>("Payee_Type");

                    b.Property<bool>("Payee_isDeleted");

                    b.HasKey("Payee_ID");

                    b.ToTable("DMPayee");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.SystemMessageModel", b =>
                {
                    b.Property<string>("Msg_Code")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Msg_Content");

                    b.Property<string>("Msg_Type");

                    b.HasKey("Msg_Code");

                    b.ToTable("SystemMessageModels");
                });
#pragma warning restore 612, 618
        }
    }
}
