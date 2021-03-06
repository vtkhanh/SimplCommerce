﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class initialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLog_ActivityType",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog_ActivityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_Brand",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SeoTitle = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 5000, nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttributeGroup",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttributeGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductOption",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductOption", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cms_Menu",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsSystem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cms_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts_ContactArea",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts_ContactArea", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_AppSetting",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Module = table.Column<string>(nullable: true),
                    IsVisibleInCommonSettingPage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_AppSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Country",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Code2 = table.Column<string>(nullable: true),
                    Code3 = table.Column<string>(nullable: true),
                    IsBillingEnabled = table.Column<bool>(nullable: false),
                    IsShippingEnabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_CustomerGroup",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_CustomerGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_EntityType",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsMenuable = table.Column<bool>(nullable: false),
                    RoutingController = table.Column<string>(nullable: true),
                    RoutingAction = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_EntityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Media",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Caption = table.Column<string>(nullable: true),
                    FileSize = table.Column<int>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    MediaType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Role",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Vendor",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SeoTitle = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Vendor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Widget",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ViewComponentName = table.Column<string>(nullable: true),
                    CreateUrl = table.Column<string>(nullable: true),
                    EditUrl = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Widget", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_WidgetZone",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_WidgetZone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inventory_Stock",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<long>(nullable: false),
                    WarehouseId = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_Stock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localization_Culture",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_Culture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "News_NewsCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SeoTitle = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 5000, nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News_NewsCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments_PaymentProvider",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    ConfigureUrl = table.Column<string>(nullable: true),
                    LandingViewComponentName = table.Column<string>(nullable: true),
                    AdditionalSettings = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments_PaymentProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CartRule",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StartOn = table.Column<DateTimeOffset>(nullable: true),
                    EndOn = table.Column<DateTimeOffset>(nullable: true),
                    IsCouponRequired = table.Column<bool>(nullable: false),
                    RuleToApply = table.Column<string>(nullable: true),
                    DiscountAmount = table.Column<decimal>(nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(nullable: true),
                    DiscountStep = table.Column<int>(nullable: true),
                    UsageLimitPerCoupon = table.Column<int>(nullable: true),
                    UsageLimitPerCustomer = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CartRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CatalogRule",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StartOn = table.Column<DateTimeOffset>(nullable: true),
                    EndOn = table.Column<DateTimeOffset>(nullable: true),
                    RuleToApply = table.Column<string>(nullable: true),
                    DiscountAmount = table.Column<decimal>(nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CatalogRule", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductRecentlyViewed_RecentlyViewedProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    LatestViewedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecentlyViewed_RecentlyViewedProduct", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Search_Query",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QueryText = table.Column<string>(nullable: true),
                    ResultsCount = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Search_Query", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shipping_ShippingProvider",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    ConfigureUrl = table.Column<string>(nullable: true),
                    ToAllShippingEnabledCountries = table.Column<bool>(nullable: false),
                    OnlyCountryIdsString = table.Column<string>(nullable: true),
                    ToAllShippingEnabledStatesOrProvinces = table.Column<bool>(nullable: false),
                    OnlyStateOrProvinceIdsString = table.Column<string>(nullable: true),
                    AdditionalSettings = table.Column<string>(nullable: true),
                    ShippingPriceServiceTypeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipping_ShippingProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tax_TaxClass",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax_TaxClass", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLog_Activity",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityTypeId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    EntityId = table.Column<long>(nullable: false),
                    EntityTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLog_Activity_ActivityLog_ActivityType_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalTable: "ActivityLog_ActivityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttribute",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    GroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttribute_Catalog_ProductAttributeGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Catalog_ProductAttributeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts_Contact",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    ContactAreaId = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Contact_Contacts_ContactArea_ContactAreaId",
                        column: x => x.ContactAreaId,
                        principalTable: "Contacts_ContactArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_StateOrProvince",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountryId = table.Column<long>(nullable: false),
                    CountryCode = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_StateOrProvince", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_StateOrProvince_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_Entity",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Slug = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    EntityId = table.Column<long>(nullable: false),
                    EntityTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Entity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_Entity_Core_EntityType_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "Core_EntityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_Category",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SeoTitle = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 5000, nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    IsPublished = table.Column<bool>(nullable: false),
                    IncludeInMenu = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ParentId = table.Column<long>(nullable: true),
                    ThumbnailImageId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_Category_Catalog_Category_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Catalog_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Category_Core_Media_ThumbnailImageId",
                        column: x => x.ThumbnailImageId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<long>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_RoleClaim_Core_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Core_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_WidgetInstance",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    PublishStart = table.Column<DateTimeOffset>(nullable: true),
                    PublishEnd = table.Column<DateTimeOffset>(nullable: true),
                    WidgetId = table.Column<long>(nullable: false),
                    WidgetZoneId = table.Column<long>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    HtmlData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_WidgetInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_WidgetInstance_Core_Widget_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Core_Widget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Core_WidgetInstance_Core_WidgetZone_WidgetZoneId",
                        column: x => x.WidgetZoneId,
                        principalTable: "Core_WidgetZone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Localization_Resource",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    CultureId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Localization_Resource_Localization_Culture_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Localization_Culture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CartRuleCustomerGroup",
                columns: table => new
                {
                    CartRuleId = table.Column<long>(nullable: false),
                    CustomerGroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CartRuleCustomerGroup", x => new { x.CartRuleId, x.CustomerGroupId });
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleCustomerGroup_Pricing_CartRule_CartRuleId",
                        column: x => x.CartRuleId,
                        principalTable: "Pricing_CartRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleCustomerGroup_Core_CustomerGroup_CustomerGroupId",
                        column: x => x.CustomerGroupId,
                        principalTable: "Core_CustomerGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_Coupon",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CartRuleId = table.Column<long>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_Coupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pricing_Coupon_Pricing_CartRule_CartRuleId",
                        column: x => x.CartRuleId,
                        principalTable: "Pricing_CartRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CatalogRuleCustomerGroup",
                columns: table => new
                {
                    CatalogRuleId = table.Column<long>(nullable: false),
                    CustomerGroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CatalogRuleCustomerGroup", x => new { x.CatalogRuleId, x.CustomerGroupId });
                    table.ForeignKey(
                        name: "FK_Pricing_CatalogRuleCustomerGroup_Pricing_CatalogRule_CatalogRuleId",
                        column: x => x.CatalogRuleId,
                        principalTable: "Pricing_CatalogRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pricing_CatalogRuleCustomerGroup_Core_CustomerGroup_CustomerGroupId",
                        column: x => x.CustomerGroupId,
                        principalTable: "Core_CustomerGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductTemplateProductAttribute",
                columns: table => new
                {
                    ProductTemplateId = table.Column<long>(nullable: false),
                    ProductAttributeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductTemplateProductAttribute", x => new { x.ProductTemplateId, x.ProductAttributeId });
                    table.ForeignKey(
                        name: "FK_Catalog_ProductTemplateProductAttribute_Catalog_ProductAttribute_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalTable: "Catalog_ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductTemplateProductAttribute_Catalog_ProductTemplate_ProductTemplateId",
                        column: x => x.ProductTemplateId,
                        principalTable: "Catalog_ProductTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_District",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StateOrProvinceId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_District", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_District_Core_StateOrProvince_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingTableRate_PriceAndDestination",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountryId = table.Column<long>(nullable: true),
                    StateOrProvinceId = table.Column<long>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    MinOrderSubtotal = table.Column<decimal>(nullable: false),
                    ShippingPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingTableRate_PriceAndDestination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingTableRate_PriceAndDestination_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingTableRate_PriceAndDestination_Core_StateOrProvince_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tax_TaxRate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TaxClassId = table.Column<long>(nullable: false),
                    CountryId = table.Column<long>(nullable: false),
                    StateOrProvinceId = table.Column<long>(nullable: true),
                    Rate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax_TaxRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tax_TaxRate_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tax_TaxRate_Core_StateOrProvince_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tax_TaxRate_Tax_TaxClass_TaxClassId",
                        column: x => x.TaxClassId,
                        principalTable: "Tax_TaxClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cms_MenuItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<long>(nullable: true),
                    MenuId = table.Column<long>(nullable: false),
                    EntityId = table.Column<long>(nullable: true),
                    CustomLink = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cms_MenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cms_MenuItem_Core_Entity_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Core_Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cms_MenuItem_Cms_Menu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Cms_Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cms_MenuItem_Cms_MenuItem_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Cms_MenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CartRuleCategory",
                columns: table => new
                {
                    CategoryId = table.Column<long>(nullable: false),
                    CartRuleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CartRuleCategory", x => new { x.CartRuleId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleCategory_Pricing_CartRule_CartRuleId",
                        column: x => x.CartRuleId,
                        principalTable: "Pricing_CartRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleCategory_Catalog_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Catalog_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_Address",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    DistrictId = table.Column<long>(nullable: true),
                    StateOrProvinceId = table.Column<long>(nullable: false),
                    CountryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_Address_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_Address_Core_District_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Core_District",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_Address_Core_StateOrProvince_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders_OrderAddress",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactName = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    DistrictId = table.Column<long>(nullable: true),
                    StateOrProvinceId = table.Column<long>(nullable: false),
                    CountryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_OrderAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderAddress_Core_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Core_Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderAddress_Core_District_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Core_District",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderAddress_Core_StateOrProvince_StateOrProvinceId",
                        column: x => x.StateOrProvinceId,
                        principalTable: "Core_StateOrProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory_Warehouse",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    AddressId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Warehouse_Core_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Core_Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_Product",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SeoTitle = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    PublishedOn = table.Column<DateTimeOffset>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<long>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedById = table.Column<long>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Specification = table.Column<string>(nullable: true),
                    Cost = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    OldPrice = table.Column<decimal>(nullable: true),
                    SpecialPrice = table.Column<decimal>(nullable: true),
                    SpecialPriceStart = table.Column<DateTimeOffset>(nullable: true),
                    SpecialPriceEnd = table.Column<DateTimeOffset>(nullable: true),
                    HasOptions = table.Column<bool>(nullable: false),
                    IsVisibleIndividually = table.Column<bool>(nullable: false),
                    IsFeatured = table.Column<bool>(nullable: false),
                    IsCallForPricing = table.Column<bool>(nullable: false),
                    IsAllowToOrder = table.Column<bool>(nullable: false),
                    Stock = table.Column<int>(nullable: false),
                    Sku = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    VendorId = table.Column<long>(nullable: true),
                    HitCount = table.Column<int>(nullable: false),
                    ThumbnailImageId = table.Column<long>(nullable: true),
                    ReviewsCount = table.Column<int>(nullable: false),
                    RatingAverage = table.Column<double>(nullable: true),
                    BrandId = table.Column<long>(nullable: true),
                    TaxClassId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Catalog_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Catalog_Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Tax_TaxClass_TaxClassId",
                        column: x => x.TaxClassId,
                        principalTable: "Tax_TaxClass",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_Product_Core_Media_ThumbnailImageId",
                        column: x => x.ThumbnailImageId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductAttributeValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttributeValue_Catalog_ProductAttribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Catalog_ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductAttributeValue_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsFeaturedProduct = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    CategoryId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductCategory_Catalog_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Catalog_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductCategory_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductLink",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<long>(nullable: false),
                    LinkedProductId = table.Column<long>(nullable: false),
                    LinkType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductLink_Catalog_Product_LinkedProductId",
                        column: x => x.LinkedProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductLink_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductMedia",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<long>(nullable: false),
                    MediaId = table.Column<long>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductMedia_Core_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductMedia_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductOptionCombination",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<long>(nullable: false),
                    OptionId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    SortIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductOptionCombination", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionCombination_Catalog_ProductOption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Catalog_ProductOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionCombination_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Catalog_ProductOptionValue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OptionId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    DisplayType = table.Column<string>(nullable: true),
                    SortIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog_ProductOptionValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionValue_Catalog_ProductOption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Catalog_ProductOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalog_ProductOptionValue_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CartRuleProduct",
                columns: table => new
                {
                    ProductId = table.Column<long>(nullable: false),
                    CartRuleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CartRuleProduct", x => new { x.CartRuleId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleProduct_Pricing_CartRule_CartRuleId",
                        column: x => x.CartRuleId,
                        principalTable: "Pricing_CartRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleProduct_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders_OrderItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<long>(nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    ProductPrice = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TaxAmount = table.Column<decimal>(nullable: false),
                    TaxPercent = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderItem_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductComparison_ComparingProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductComparison_ComparingProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductComparison_ComparingProduct_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart_CartItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    CartId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart_CartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_CartItem_Catalog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Catalog_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserAddress",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    AddressId = table.Column<long>(nullable: false),
                    AddressType = table.Column<int>(nullable: false),
                    LastUsedOn = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_UserAddress_Core_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Core_Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_User",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    UserGuid = table.Column<Guid>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    VendorId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    DefaultShippingAddressId = table.Column<long>(nullable: true),
                    DefaultBillingAddressId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_User_Core_UserAddress_DefaultBillingAddressId",
                        column: x => x.DefaultBillingAddressId,
                        principalTable: "Core_UserAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_User_Core_UserAddress_DefaultShippingAddressId",
                        column: x => x.DefaultShippingAddressId,
                        principalTable: "Core_UserAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Core_User_Core_Vendor_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Core_Vendor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cms_Page",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SeoTitle = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    PublishedOn = table.Column<DateTimeOffset>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<long>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedById = table.Column<long>(nullable: true),
                    Body = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cms_Page", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cms_Page_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cms_Page_Core_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_UserClaim_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserCustomerGroup",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    CustomerGroupId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserCustomerGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_UserCustomerGroup_Core_CustomerGroup_CustomerGroupId",
                        column: x => x.CustomerGroupId,
                        principalTable: "Core_CustomerGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Core_UserCustomerGroup_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_Core_UserLogin_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserRole",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Core_UserRole_Core_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Core_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Core_UserRole_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserToken",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_Core_UserToken_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "News_NewsItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SeoTitle = table.Column<string>(nullable: true),
                    MetaTitle = table.Column<string>(nullable: true),
                    MetaKeywords = table.Column<string>(nullable: true),
                    MetaDescription = table.Column<string>(nullable: true),
                    IsPublished = table.Column<bool>(nullable: false),
                    PublishedOn = table.Column<DateTimeOffset>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedById = table.Column<long>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedById = table.Column<long>(nullable: true),
                    ShortContent = table.Column<string>(nullable: true),
                    FullContent = table.Column<string>(nullable: true),
                    ThumbnailImageId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News_NewsItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_NewsItem_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_News_NewsItem_Core_Media_ThumbnailImageId",
                        column: x => x.ThumbnailImageId,
                        principalTable: "Core_Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_News_NewsItem_Core_User_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders_Order",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: true),
                    CreatedById = table.Column<long>(nullable: false),
                    CustomerId = table.Column<long>(nullable: false),
                    VendorId = table.Column<long>(nullable: true),
                    CouponCode = table.Column<string>(nullable: true),
                    CouponRuleName = table.Column<string>(nullable: true),
                    Discount = table.Column<decimal>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false),
                    SubTotalWithDiscount = table.Column<decimal>(nullable: false),
                    ShippingAddressId = table.Column<long>(nullable: true),
                    BillingAddressId = table.Column<long>(nullable: true),
                    OrderStatus = table.Column<int>(nullable: false),
                    ParentId = table.Column<long>(nullable: true),
                    ShippingMethod = table.Column<string>(nullable: true),
                    ShippingAmount = table.Column<decimal>(nullable: false),
                    TaxAmount = table.Column<decimal>(nullable: false),
                    OrderTotal = table.Column<decimal>(nullable: false),
                    PaymentMethod = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Orders_OrderAddress_BillingAddressId",
                        column: x => x.BillingAddressId,
                        principalTable: "Orders_OrderAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Core_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Core_User_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Orders_Order_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Orders_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Order_Orders_OrderAddress_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Orders_OrderAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CartRuleUsage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CartRuleId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    OrderId = table.Column<long>(nullable: false),
                    UsedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CartRuleUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleUsage_Pricing_CartRule_CartRuleId",
                        column: x => x.CartRuleId,
                        principalTable: "Pricing_CartRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pricing_CartRuleUsage_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pricing_CouponUsage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CouponId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    UsedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pricing_CouponUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pricing_CouponUsage_Pricing_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Pricing_Coupon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pricing_CouponUsage_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews_Review",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    ReviewerName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    EntityTypeId = table.Column<long>(nullable: false),
                    EntityId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Review_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCart_Cart",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CouponCode = table.Column<string>(nullable: true),
                    CouponRuleName = table.Column<string>(nullable: true),
                    ShippingMethod = table.Column<string>(nullable: true),
                    ShippingAmount = table.Column<decimal>(nullable: true),
                    TaxAmount = table.Column<decimal>(nullable: true),
                    ShippingData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart_Cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_Cart_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "News_NewsItemCategory",
                columns: table => new
                {
                    CategoryId = table.Column<long>(nullable: false),
                    NewsItemId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News_NewsItemCategory", x => new { x.CategoryId, x.NewsItemId });
                    table.ForeignKey(
                        name: "FK_News_NewsItemCategory_News_NewsCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "News_NewsCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_News_NewsItemCategory_News_NewsItem_NewsItemId",
                        column: x => x.NewsItemId,
                        principalTable: "News_NewsItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments_Payment",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    PaymentMethod = table.Column<string>(nullable: true),
                    GatewayTransactionId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Payment_Orders_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shipments_Shipment",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    TrackingNumber = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(nullable: false),
                    UpdatedOn = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments_Shipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_Shipment_Orders_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shipments_ShipmentItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ShipmentId = table.Column<long>(nullable: false),
                    OrderItemId = table.Column<long>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments_ShipmentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_ShipmentItem_Shipments_Shipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments_Shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_Activity_ActivityTypeId",
                table: "ActivityLog_Activity",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Category_ParentId",
                table: "Catalog_Category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Category_ThumbnailImageId",
                table: "Catalog_Category",
                column: "ThumbnailImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_BrandId",
                table: "Catalog_Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_CreatedById",
                table: "Catalog_Product",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_TaxClassId",
                table: "Catalog_Product",
                column: "TaxClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_ThumbnailImageId",
                table: "Catalog_Product",
                column: "ThumbnailImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_Product_UpdatedById",
                table: "Catalog_Product",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttribute_GroupId",
                table: "Catalog_ProductAttribute",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttributeValue_AttributeId",
                table: "Catalog_ProductAttributeValue",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductAttributeValue_ProductId",
                table: "Catalog_ProductAttributeValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductCategory_CategoryId",
                table: "Catalog_ProductCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductCategory_ProductId",
                table: "Catalog_ProductCategory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductLink_LinkedProductId",
                table: "Catalog_ProductLink",
                column: "LinkedProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductLink_ProductId",
                table: "Catalog_ProductLink",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductMedia_MediaId",
                table: "Catalog_ProductMedia",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductMedia_ProductId",
                table: "Catalog_ProductMedia",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionCombination_OptionId",
                table: "Catalog_ProductOptionCombination",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionCombination_ProductId",
                table: "Catalog_ProductOptionCombination",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionValue_OptionId",
                table: "Catalog_ProductOptionValue",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductOptionValue_ProductId",
                table: "Catalog_ProductOptionValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_ProductTemplateProductAttribute_ProductAttributeId",
                table: "Catalog_ProductTemplateProductAttribute",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cms_MenuItem_EntityId",
                table: "Cms_MenuItem",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Cms_MenuItem_MenuId",
                table: "Cms_MenuItem",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Cms_MenuItem_ParentId",
                table: "Cms_MenuItem",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Cms_Page_CreatedById",
                table: "Cms_Page",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Cms_Page_UpdatedById",
                table: "Cms_Page",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Contact_ContactAreaId",
                table: "Contacts_Contact",
                column: "ContactAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_Address_CountryId",
                table: "Core_Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_Address_DistrictId",
                table: "Core_Address",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_Address_StateOrProvinceId",
                table: "Core_Address",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_CustomerGroup_Name",
                table: "Core_CustomerGroup",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Core_District_StateOrProvinceId",
                table: "Core_District",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_Entity_EntityTypeId",
                table: "Core_Entity",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Core_Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Core_RoleClaim_RoleId",
                table: "Core_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_StateOrProvince_CountryId",
                table: "Core_StateOrProvince",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_DefaultBillingAddressId",
                table: "Core_User",
                column: "DefaultBillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_DefaultShippingAddressId",
                table: "Core_User",
                column: "DefaultShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Core_User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Core_User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_VendorId",
                table: "Core_User",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserAddress_AddressId",
                table: "Core_UserAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserAddress_UserId",
                table: "Core_UserAddress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserClaim_UserId",
                table: "Core_UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserCustomerGroup_CustomerGroupId",
                table: "Core_UserCustomerGroup",
                column: "CustomerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserCustomerGroup_UserId",
                table: "Core_UserCustomerGroup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserLogin_UserId",
                table: "Core_UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserRole_RoleId",
                table: "Core_UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_WidgetInstance_WidgetId",
                table: "Core_WidgetInstance",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_WidgetInstance_WidgetZoneId",
                table: "Core_WidgetInstance",
                column: "WidgetZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Warehouse_AddressId",
                table: "Inventory_Warehouse",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Localization_Resource_CultureId",
                table: "Localization_Resource",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsItem_CreatedById",
                table: "News_NewsItem",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsItem_ThumbnailImageId",
                table: "News_NewsItem",
                column: "ThumbnailImageId");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsItem_UpdatedById",
                table: "News_NewsItem",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_News_NewsItemCategory_NewsItemId",
                table: "News_NewsItemCategory",
                column: "NewsItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_BillingAddressId",
                table: "Orders_Order",
                column: "BillingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_CreatedById",
                table: "Orders_Order",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_CustomerId",
                table: "Orders_Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_ParentId",
                table: "Orders_Order",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Order_ShippingAddressId",
                table: "Orders_Order",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderAddress_CountryId",
                table: "Orders_OrderAddress",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderAddress_DistrictId",
                table: "Orders_OrderAddress",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderAddress_StateOrProvinceId",
                table: "Orders_OrderAddress",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderItem_OrderId",
                table: "Orders_OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderItem_ProductId",
                table: "Orders_OrderItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Payment_OrderId",
                table: "Payments_Payment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CartRuleCategory_CategoryId",
                table: "Pricing_CartRuleCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CartRuleCustomerGroup_CustomerGroupId",
                table: "Pricing_CartRuleCustomerGroup",
                column: "CustomerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CartRuleProduct_ProductId",
                table: "Pricing_CartRuleProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CartRuleUsage_CartRuleId",
                table: "Pricing_CartRuleUsage",
                column: "CartRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CartRuleUsage_UserId",
                table: "Pricing_CartRuleUsage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CatalogRuleCustomerGroup_CustomerGroupId",
                table: "Pricing_CatalogRuleCustomerGroup",
                column: "CustomerGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_Coupon_CartRuleId",
                table: "Pricing_Coupon",
                column: "CartRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CouponUsage_CouponId",
                table: "Pricing_CouponUsage",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_Pricing_CouponUsage_UserId",
                table: "Pricing_CouponUsage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductComparison_ComparingProduct_ProductId",
                table: "ProductComparison_ComparingProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductComparison_ComparingProduct_UserId",
                table: "ProductComparison_ComparingProduct",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Review_UserId",
                table: "Reviews_Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_Shipment_OrderId",
                table: "Shipments_Shipment",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ShipmentItem_ShipmentId",
                table: "Shipments_ShipmentItem",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingTableRate_PriceAndDestination_CountryId",
                table: "ShippingTableRate_PriceAndDestination",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingTableRate_PriceAndDestination_StateOrProvinceId",
                table: "ShippingTableRate_PriceAndDestination",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_Cart_UserId",
                table: "ShoppingCart_Cart",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CartItem_CartId",
                table: "ShoppingCart_CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_CartItem_ProductId",
                table: "ShoppingCart_CartItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_TaxRate_CountryId",
                table: "Tax_TaxRate",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_TaxRate_StateOrProvinceId",
                table: "Tax_TaxRate",
                column: "StateOrProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tax_TaxRate_TaxClassId",
                table: "Tax_TaxRate",
                column: "TaxClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_Product_Core_User_CreatedById",
                table: "Catalog_Product",
                column: "CreatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_Product_Core_User_UpdatedById",
                table: "Catalog_Product",
                column: "UpdatedById",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderItem_Orders_Order_OrderId",
                table: "Orders_OrderItem",
                column: "OrderId",
                principalTable: "Orders_Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductComparison_ComparingProduct_Core_User_UserId",
                table: "ProductComparison_ComparingProduct",
                column: "UserId",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_CartItem_ShoppingCart_Cart_CartId",
                table: "ShoppingCart_CartItem",
                column: "CartId",
                principalTable: "ShoppingCart_Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Core_UserAddress_Core_User_UserId",
                table: "Core_UserAddress",
                column: "UserId",
                principalTable: "Core_User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Core_UserAddress_Core_User_UserId",
                table: "Core_UserAddress");

            migrationBuilder.DropTable(
                name: "ActivityLog_Activity");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "Catalog_ProductCategory");

            migrationBuilder.DropTable(
                name: "Catalog_ProductLink");

            migrationBuilder.DropTable(
                name: "Catalog_ProductMedia");

            migrationBuilder.DropTable(
                name: "Catalog_ProductOptionCombination");

            migrationBuilder.DropTable(
                name: "Catalog_ProductOptionValue");

            migrationBuilder.DropTable(
                name: "Catalog_ProductTemplateProductAttribute");

            migrationBuilder.DropTable(
                name: "Cms_MenuItem");

            migrationBuilder.DropTable(
                name: "Cms_Page");

            migrationBuilder.DropTable(
                name: "Contacts_Contact");

            migrationBuilder.DropTable(
                name: "Core_AppSetting");

            migrationBuilder.DropTable(
                name: "Core_RoleClaim");

            migrationBuilder.DropTable(
                name: "Core_UserClaim");

            migrationBuilder.DropTable(
                name: "Core_UserCustomerGroup");

            migrationBuilder.DropTable(
                name: "Core_UserLogin");

            migrationBuilder.DropTable(
                name: "Core_UserRole");

            migrationBuilder.DropTable(
                name: "Core_UserToken");

            migrationBuilder.DropTable(
                name: "Core_WidgetInstance");

            migrationBuilder.DropTable(
                name: "Inventory_Stock");

            migrationBuilder.DropTable(
                name: "Inventory_Warehouse");

            migrationBuilder.DropTable(
                name: "Localization_Resource");

            migrationBuilder.DropTable(
                name: "News_NewsItemCategory");

            migrationBuilder.DropTable(
                name: "Orders_OrderItem");

            migrationBuilder.DropTable(
                name: "Payments_Payment");

            migrationBuilder.DropTable(
                name: "Payments_PaymentProvider");

            migrationBuilder.DropTable(
                name: "Pricing_CartRuleCategory");

            migrationBuilder.DropTable(
                name: "Pricing_CartRuleCustomerGroup");

            migrationBuilder.DropTable(
                name: "Pricing_CartRuleProduct");

            migrationBuilder.DropTable(
                name: "Pricing_CartRuleUsage");

            migrationBuilder.DropTable(
                name: "Pricing_CatalogRuleCustomerGroup");

            migrationBuilder.DropTable(
                name: "Pricing_CouponUsage");

            migrationBuilder.DropTable(
                name: "ProductComparison_ComparingProduct");

            migrationBuilder.DropTable(
                name: "ProductRecentlyViewed_RecentlyViewedProduct");

            migrationBuilder.DropTable(
                name: "Reviews_Review");

            migrationBuilder.DropTable(
                name: "Search_Query");

            migrationBuilder.DropTable(
                name: "Shipments_ShipmentItem");

            migrationBuilder.DropTable(
                name: "Shipping_ShippingProvider");

            migrationBuilder.DropTable(
                name: "ShippingTableRate_PriceAndDestination");

            migrationBuilder.DropTable(
                name: "ShoppingCart_CartItem");

            migrationBuilder.DropTable(
                name: "Tax_TaxRate");

            migrationBuilder.DropTable(
                name: "ActivityLog_ActivityType");

            migrationBuilder.DropTable(
                name: "Catalog_ProductOption");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttribute");

            migrationBuilder.DropTable(
                name: "Catalog_ProductTemplate");

            migrationBuilder.DropTable(
                name: "Core_Entity");

            migrationBuilder.DropTable(
                name: "Cms_Menu");

            migrationBuilder.DropTable(
                name: "Contacts_ContactArea");

            migrationBuilder.DropTable(
                name: "Core_Role");

            migrationBuilder.DropTable(
                name: "Core_Widget");

            migrationBuilder.DropTable(
                name: "Core_WidgetZone");

            migrationBuilder.DropTable(
                name: "Localization_Culture");

            migrationBuilder.DropTable(
                name: "News_NewsCategory");

            migrationBuilder.DropTable(
                name: "News_NewsItem");

            migrationBuilder.DropTable(
                name: "Catalog_Category");

            migrationBuilder.DropTable(
                name: "Pricing_CatalogRule");

            migrationBuilder.DropTable(
                name: "Core_CustomerGroup");

            migrationBuilder.DropTable(
                name: "Pricing_Coupon");

            migrationBuilder.DropTable(
                name: "Shipments_Shipment");

            migrationBuilder.DropTable(
                name: "ShoppingCart_Cart");

            migrationBuilder.DropTable(
                name: "Catalog_Product");

            migrationBuilder.DropTable(
                name: "Catalog_ProductAttributeGroup");

            migrationBuilder.DropTable(
                name: "Core_EntityType");

            migrationBuilder.DropTable(
                name: "Pricing_CartRule");

            migrationBuilder.DropTable(
                name: "Orders_Order");

            migrationBuilder.DropTable(
                name: "Catalog_Brand");

            migrationBuilder.DropTable(
                name: "Tax_TaxClass");

            migrationBuilder.DropTable(
                name: "Core_Media");

            migrationBuilder.DropTable(
                name: "Orders_OrderAddress");

            migrationBuilder.DropTable(
                name: "Core_User");

            migrationBuilder.DropTable(
                name: "Core_UserAddress");

            migrationBuilder.DropTable(
                name: "Core_Vendor");

            migrationBuilder.DropTable(
                name: "Core_Address");

            migrationBuilder.DropTable(
                name: "Core_District");

            migrationBuilder.DropTable(
                name: "Core_StateOrProvince");

            migrationBuilder.DropTable(
                name: "Core_Country");
        }
    }
}
