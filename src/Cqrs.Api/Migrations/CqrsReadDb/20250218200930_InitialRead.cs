using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cqrs.Api.Migrations.CqrsReadDb
{
    /// <inheritdoc />
    public partial class InitialRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "articles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    article_number = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    characteristic_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_articles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attribute_mappings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    attribute_reference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    mapping = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_mappings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "root_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    locale_code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_root_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    value_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    min_values = table.Column<int>(type: "int", nullable: false),
                    max_values = table.Column<int>(type: "int", nullable: false),
                    marketplace_attribute_ids = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    allowed_values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    min_length = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    max_length = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: true),
                    product_type = table.Column<string>(type: "nvarchar(450)", nullable: false, computedColumnSql: "SUBSTRING([marketplace_attribute_ids], 1, \r\n                    CAST((CHARINDEX(',', [marketplace_attribute_ids]) - 1 \r\n                    + (1 - ROUND(CAST(CHARINDEX(',', [marketplace_attribute_ids]) AS FLOAT) \r\n                    / (1.0 * CHARINDEX(',', [marketplace_attribute_ids]) + 1), 0))) \r\n                    * (LEN([marketplace_attribute_ids]) + 1) AS INT))", stored: true),
                    is_editable = table.Column<bool>(type: "bit", nullable: false),
                    example_values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    parent_attribute_id = table.Column<int>(type: "int", nullable: true),
                    root_category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attributes", x => x.id);
                    table.ForeignKey(
                        name: "fk_attributes_attributes_parent_attribute_id",
                        column: x => x.parent_attribute_id,
                        principalTable: "attributes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_attributes_root_categories_root_category_id",
                        column: x => x.root_category_id,
                        principalTable: "root_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_number = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_leaf = table.Column<bool>(type: "bit", nullable: false),
                    root_category_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_categories_categories_parent_id",
                        column: x => x.parent_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_categories_root_categories_root_category_id",
                        column: x => x.root_category_id,
                        principalTable: "root_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attribute_boolean_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<bool>(type: "bit", nullable: false),
                    article_id = table.Column<int>(type: "int", nullable: false),
                    attribute_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_boolean_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_attribute_boolean_values_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attribute_boolean_values_attributes_attribute_id",
                        column: x => x.attribute_id,
                        principalTable: "attributes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attribute_decimal_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<decimal>(type: "numeric(9,3)", nullable: false),
                    article_id = table.Column<int>(type: "int", nullable: false),
                    attribute_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_decimal_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_attribute_decimal_values_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attribute_decimal_values_attributes_attribute_id",
                        column: x => x.attribute_id,
                        principalTable: "attributes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attribute_int_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<int>(type: "int", nullable: false),
                    article_id = table.Column<int>(type: "int", nullable: false),
                    attribute_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_int_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_attribute_int_values_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attribute_int_values_attributes_attribute_id",
                        column: x => x.attribute_id,
                        principalTable: "attributes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attribute_string_values",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    article_id = table.Column<int>(type: "int", nullable: false),
                    attribute_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_string_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_attribute_string_values_articles_article_id",
                        column: x => x.article_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attribute_string_values_attributes_attribute_id",
                        column: x => x.attribute_id,
                        principalTable: "attributes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "article_category",
                columns: table => new
                {
                    articles_id = table.Column<int>(type: "int", nullable: false),
                    categories_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_article_category", x => new { x.articles_id, x.categories_id });
                    table.ForeignKey(
                        name: "fk_article_category_articles_articles_id",
                        column: x => x.articles_id,
                        principalTable: "articles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_article_category_categories_categories_id",
                        column: x => x.categories_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attributes_categories",
                columns: table => new
                {
                    attributes_id = table.Column<int>(type: "int", nullable: false),
                    categories_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attributes_categories", x => new { x.attributes_id, x.categories_id });
                    table.ForeignKey(
                        name: "fk_attributes_categories_attributes_attributes_id",
                        column: x => x.attributes_id,
                        principalTable: "attributes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_attributes_categories_categories_categories_id",
                        column: x => x.categories_id,
                        principalTable: "categories",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                table: "attribute_mappings",
                columns: new[] { "id", "attribute_reference", "mapping" },
                values: new object[,]
                {
                    { 1, "condition_type,value", "new_new" },
                    { 2, "externally_assigned_product_identifier,type", "ean" },
                    { 3, "manufacturer,value", "CompanyName" },
                    { 4, "brand,value", "CompanyName" },
                    { 5, "country_of_origin,value", "DE" },
                    { 6, "item_package_weight,unit", "kilograms" },
                    { 7, "item_weight,unit", "kilograms" },
                    { 8, "item_package_dimensions,length,unit", "centimeters" },
                    { 9, "item_package_dimensions,height,unit", "centimeters" },
                    { 10, "item_package_dimensions,width,unit", "centimeters" },
                    { 11, "item_dimensions,length,unit", "centimeters" },
                    { 12, "item_dimensions,height,unit", "centimeters" },
                    { 13, "item_dimensions,width,unit", "centimeters" },
                    { 14, "supplier_declared_dg_hz_regulation,value", "not_applicable" },
                    { 15, "supplier_declared_material_regulation,value", "not_applicable" },
                    { 16, "item_name,value", "TitleMarketplace" },
                    { 17, "item_type_name,value", "TitleMarketplace" },
                    { 18, "item_package_weight,value", "SingleItemPackageWeight" },
                    { 19, "item_weight,value", "Weight" },
                    { 20, "item_package_dimensions,length,value", "SingleItemPackageLength" },
                    { 21, "item_package_dimensions,height,value", "SingleItemPackageHeight" },
                    { 22, "item_package_dimensions,width,value", "SingleItemPackageWidth" },
                    { 23, "item_dimensions,length,value", "Length" },
                    { 24, "item_dimensions,height,value", "Height" },
                    { 25, "item_dimensions,width,value", "Width" },
                    { 26, "product_description,value", "DescriptionLongMarketplaces" },
                    { 27, "bullet_point,value1", "MarketplaceBulletPoint1" },
                    { 28, "bullet_point,value2", "MarketplaceBulletPoint2" },
                    { 29, "bullet_point,value3", "MarketplaceBulletPoint3" },
                    { 30, "bullet_point,value4", "MarketplaceBulletPoint4" },
                    { 31, "bullet_point,value5", "MarketplaceBulletPoint5" },
                    { 32, "is_fragile,value", "Fragile" },
                    { 33, "mob", "SingleItemPreferredPackaging" },
                    { 34, "power_plug_type,value", "PowerPlug" },
                    { 35, "color,value", "Colors" },
                    { 36, "list_price,currency", "EUR" },
                    { 37, "generic_keyword,value", "SearchTermsMarketplace" },
                    { 38, "item_depth_width_height,depth,unit", "centimeters" },
                    { 39, "item_depth_width_height,height,unit", "centimeters" },
                    { 40, "item_depth_width_height,width,unit", "centimeters" },
                    { 41, "item_depth_width_height,depth,value", "Length" },
                    { 42, "item_depth_width_height,height,value", "Height" },
                    { 43, "item_depth_width_height,width,value", "Width" },
                    { 44, "number_of_boxes,value", "1" },
                    { 45, "recommended_browse_nodes,value", null },
                    { 46, "purchase_api_materials", "customerMaterials" },
                    { 47, "material_percentages", "materialPercentage" },
                    { 48, "images", "ImageUrl" },
                    { 49, "unit_count,value", "1" },
                    { 50, "unit_count,type,value", null },
                    { 51, "material,value", null },
                    { 52, "batteries_required,value", "EnergyStorageModelBattery" },
                    { 53, "battery_type_accumulator", "EnergyStorageModelAccumulator" },
                    { 54, "batteries_included,value", "EnergyStorageAmount" },
                    { 55, "parent_item_name,value", "parent_TitleMarketplace" },
                    { 56, "parent_bullet_point,value1", "parent_MarketplaceBulletPoint1" },
                    { 57, "parent_bullet_point,value2", "parent_MarketplaceBulletPoint2" },
                    { 58, "parent_bullet_point,value3", "parent_MarketplaceBulletPoint3" },
                    { 59, "parent_bullet_point,value4", "parent_MarketplaceBulletPoint4" },
                    { 60, "parent_bullet_point,value5", "parent_MarketplaceBulletPoint5" },
                    { 61, "parent_product_description,value", "parent_DescriptionLongMarketplaces" },
                    { 62, "color,standardized_values", null },
                    { 63, "is_expiration_dated_product,value", "false" },
                    { 64, "deprecated_offering_start_date,value", null },
                    { 65, "model_number,value", null },
                    { 66, "model_name,value", "TitleShop" },
                    { 67, "included_components,value", "DeliveryContentsText" },
                    { 68, "department,value", "CompanyName" },
                    { 69, "item_width_height,width,value", "Width" },
                    { 70, "item_width_height,height,value", "Height" },
                    { 71, "item_length_width,width,value", "Width" },
                    { 72, "item_length_width,length,value", "Length" },
                    { 73, "item_length_width_height,height,value", "Height" },
                    { 74, "item_length_width_height,length,value", "Length" },
                    { 75, "item_length_width_height,width,value", "Width" },
                    { 76, "item_length_width_thickness,thickness,value", "Height" },
                    { 77, "item_length_width_thickness,length,value", "Length" },
                    { 78, "item_length_width_thickness,width,value", "Width" },
                    { 79, "item_width_height,width,unit", "centimeters" },
                    { 80, "item_width_height,height,unit", "centimeters" },
                    { 81, "item_length_width,width,unit", "centimeters" },
                    { 82, "item_length_width,length,unit", "centimeters" },
                    { 83, "item_length_width_height,height,unit", "centimeters" },
                    { 84, "item_length_width_height,length,unit", "centimeters" },
                    { 85, "item_length_width_height,width,unit", "centimeters" },
                    { 86, "item_length_width_thickness,thickness,unit", "centimeters" },
                    { 87, "item_length_width_thickness,length,unit", "centimeters" },
                    { 88, "item_length_width_thickness,width,unit", "centimeters" },
                    { 89, "contains_food_or_beverage,value", "false" },
                    { 90, "is_oem_authorized,value", "false" },
                    { 91, "product_expiration_type,value", "Does Not Expire" },
                    { 92, "warranty_description,value", "-" },
                    { 93, "list_price,value_with_tax", "0" }
                });

            migrationBuilder.InsertData(
                table: "root_categories",
                columns: new[] { "id", "locale_code" },
                values: new object[,]
                {
                    { 1, "de_DE" },
                    { 2, "fr_FR" },
                    { 3, "nl_NL" },
                    { 4, "es_ES" },
                    { 5, "en_GB" },
                    { 6, "it_IT" },
                    { 7, "pl_PL" },
                    { 8, "sv_SE" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_article_category_categories_id",
                table: "article_category",
                column: "categories_id");

            migrationBuilder.CreateIndex(
                name: "ix_articles_article_number_characteristic_id",
                table: "articles",
                columns: new[] { "article_number", "characteristic_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_attribute_boolean_values_article_id",
                table: "attribute_boolean_values",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_boolean_values_attribute_id",
                table: "attribute_boolean_values",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_decimal_values_article_id",
                table: "attribute_decimal_values",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_decimal_values_attribute_id",
                table: "attribute_decimal_values",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_int_values_article_id",
                table: "attribute_int_values",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_int_values_attribute_id",
                table: "attribute_int_values",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_mappings_attribute_reference",
                table: "attribute_mappings",
                column: "attribute_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_attribute_string_values_article_id",
                table: "attribute_string_values",
                column: "article_id");

            migrationBuilder.CreateIndex(
                name: "ix_attribute_string_values_attribute_id",
                table: "attribute_string_values",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_attributes_marketplace_attribute_ids",
                table: "attributes",
                column: "marketplace_attribute_ids");

            migrationBuilder.CreateIndex(
                name: "ix_attributes_parent_attribute_id",
                table: "attributes",
                column: "parent_attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_attributes_product_type",
                table: "attributes",
                column: "product_type");

            migrationBuilder.CreateIndex(
                name: "ix_attributes_root_category_id",
                table: "attributes",
                column: "root_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_attributes_value_type",
                table: "attributes",
                column: "value_type");

            migrationBuilder.CreateIndex(
                name: "ix_attributes_categories_categories_id",
                table: "attributes_categories",
                column: "categories_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_category_number_root_category_id",
                table: "categories",
                columns: new[] { "category_number", "root_category_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_categories_is_leaf",
                table: "categories",
                column: "is_leaf");

            migrationBuilder.CreateIndex(
                name: "ix_categories_parent_id",
                table: "categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_categories_root_category_id",
                table: "categories",
                column: "root_category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_category");

            migrationBuilder.DropTable(
                name: "attribute_boolean_values");

            migrationBuilder.DropTable(
                name: "attribute_decimal_values");

            migrationBuilder.DropTable(
                name: "attribute_int_values");

            migrationBuilder.DropTable(
                name: "attribute_mappings");

            migrationBuilder.DropTable(
                name: "attribute_string_values");

            migrationBuilder.DropTable(
                name: "attributes_categories");

            migrationBuilder.DropTable(
                name: "articles");

            migrationBuilder.DropTable(
                name: "attributes");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "root_categories");
        }
    }
}
