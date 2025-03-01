using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Donors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Donations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DonorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeOfOperation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Donations_Donors_DonorId",
                        column: x => x.DonorId,
                        principalTable: "Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Donations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fundings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeOfOperation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fundings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fundings_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fundings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdministrativeSpending = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaterialsSpending = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LabourSpending = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Donors",
                columns: new[] { "Id", "Email", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { "1ea12edd-2a72-4a41-a4c8-e90e7532e198", "donor2@mail.com", "Donor 2", "380502000000" },
                    { "23500d82-c23a-4d24-9ac7-8d1e698498f5", "donor5@mail.com", "Donor 5", "380505000000" },
                    { "33d7d29c-eae3-46da-98ae-3ae5217da99e", "donor6@mail.com", "Donor 6", "380506000000" },
                    { "6adad1ca-bbd5-4206-a5de-01322ad64df9", "donor4@mail.com", "Donor 4", "380504000000" },
                    { "7ec212ed-a6cd-43da-9062-51eb3565703a", "donor1@mail.com", "Donor 1", "380501000000" },
                    { "b2fdd407-4270-4bcb-84e9-5129df7f2f66", "donor3@mail.com", "Donor 3", "380503000000" }
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Address", "Description", "Email", "Name", "PhoneNumber" },
                values: new object[,]
                {
                    { "143e7fda-51b1-49d6-a915-6f931ef38a1b", "Street 3, City", "Description for Org 3", "org3@charity.com", "Organization 3", "380443000000" },
                    { "44724604-1138-4ef4-a6a6-8be13e5fbc05", "Street 4, City", "Description for Org 4", "org4@charity.com", "Organization 4", "380444000000" },
                    { "a7474e3f-c946-4a03-8464-d3d582bb88bc", "Street 5, City", "Description for Org 5", "org5@charity.com", "Organization 5", "380445000000" },
                    { "ceecc567-2fc6-48d7-a82c-c912993f5ec1", "Street 1, City", "Description for Org 1", "org1@charity.com", "Organization 1", "380441000000" },
                    { "e4463913-a731-4ef9-a745-6720bc719fec", "Street 2, City", "Description for Org 2", "org2@charity.com", "Organization 2", "380442000000" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Description", "Location", "Name" },
                values: new object[,]
                {
                    { "16c6ceb0-7082-4108-9e97-a8319f4a28e6", "Project 1 description", "Region 1", "Project 1 for Org 2" },
                    { "19c358cc-8115-4366-b51e-895a6d1806e9", "Project 2 description", "Region 2", "Project 2 for Org 2" },
                    { "30c88134-87b8-4ff0-a119-cc0cd62b44ad", "Project 3 description", "Region 3", "Project 3 for Org 2" },
                    { "4a81d64d-bb22-4293-a372-2c29d1e1976f", "Project 1 description", "Region 1", "Project 1 for Org 3" },
                    { "64500732-9a8e-4fbe-addc-b34f0c3a49e3", "Project 1 description", "Region 1", "Project 1 for Org 1" },
                    { "a1322056-4a6f-4397-af7c-0299bc93dd91", "Project 2 description", "Region 2", "Project 2 for Org 4" },
                    { "aa19feb2-f8ff-4f8c-9934-13e2fd551f4d", "Project 2 description", "Region 2", "Project 2 for Org 1" },
                    { "ad50d34f-1c0f-4744-95d6-1934238b49ef", "Project 3 description", "Region 3", "Project 3 for Org 4" },
                    { "b1f3b002-94bd-4e5b-97a5-1708d1ae669d", "Project 1 description", "Region 1", "Project 1 for Org 4" },
                    { "b621ab84-05d3-45d7-aad1-2226f9251de2", "Project 3 description", "Region 3", "Project 3 for Org 3" },
                    { "bc16fa28-fbd1-4572-8da3-447e72272ee7", "Project 3 description", "Region 3", "Project 3 for Org 5" },
                    { "cc64c897-7ac9-4972-a018-9abba89aa4f9", "Project 3 description", "Region 3", "Project 3 for Org 1" },
                    { "d05753dd-57b5-40be-a349-787cf2ff7004", "Project 2 description", "Region 2", "Project 2 for Org 5" },
                    { "eb02df2a-3280-4933-a101-2fa51c6087ff", "Project 1 description", "Region 1", "Project 1 for Org 5" },
                    { "fc214fe7-8e6c-40e6-ac91-817d0ac59b1b", "Project 2 description", "Region 2", "Project 2 for Org 3" }
                });

            migrationBuilder.InsertData(
                table: "Donations",
                columns: new[] { "Id", "Amount", "DonorId", "OrganizationId", "TimeOfOperation" },
                values: new object[,]
                {
                    { "12e90770-8b3c-4caf-adaa-78517e63ba16", 8964m, "23500d82-c23a-4d24-9ac7-8d1e698498f5", "143e7fda-51b1-49d6-a915-6f931ef38a1b", new DateTime(2025, 1, 19, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(618) },
                    { "193f8ec5-fcf7-4b4c-96e8-6f90a14459e6", 8437m, "7ec212ed-a6cd-43da-9062-51eb3565703a", "44724604-1138-4ef4-a6a6-8be13e5fbc05", new DateTime(2024, 5, 10, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(9988) },
                    { "27e719d5-0711-4f89-b34a-262f1892de07", 7592m, "6adad1ca-bbd5-4206-a5de-01322ad64df9", "44724604-1138-4ef4-a6a6-8be13e5fbc05", new DateTime(2024, 10, 31, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(587) },
                    { "395c2870-8ff6-4ed7-8cc2-31d01e41b88f", 7322m, "7ec212ed-a6cd-43da-9062-51eb3565703a", "143e7fda-51b1-49d6-a915-6f931ef38a1b", new DateTime(2024, 12, 2, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(260) },
                    { "4967a611-02e0-4138-b5f4-f7a2ee10a1dc", 3130m, "23500d82-c23a-4d24-9ac7-8d1e698498f5", "e4463913-a731-4ef9-a745-6720bc719fec", new DateTime(2024, 4, 9, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(623) },
                    { "4e50059d-e524-434d-a7d9-d6d7d8ab906e", 7437m, "b2fdd407-4270-4bcb-84e9-5129df7f2f66", "ceecc567-2fc6-48d7-a82c-c912993f5ec1", new DateTime(2024, 8, 9, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(534) },
                    { "4ed7555c-94b9-4c0f-9890-55df9f649f0b", 2210m, "33d7d29c-eae3-46da-98ae-3ae5217da99e", "143e7fda-51b1-49d6-a915-6f931ef38a1b", new DateTime(2024, 5, 25, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(660) },
                    { "5f9fe36d-6371-49c8-b2e9-7ef219760ade", 7786m, "23500d82-c23a-4d24-9ac7-8d1e698498f5", "ceecc567-2fc6-48d7-a82c-c912993f5ec1", new DateTime(2024, 10, 20, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(610) },
                    { "64bc3276-8a04-4745-b0a6-a8da0308cbee", 9895m, "1ea12edd-2a72-4a41-a4c8-e90e7532e198", "143e7fda-51b1-49d6-a915-6f931ef38a1b", new DateTime(2024, 10, 25, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(461) },
                    { "68d9d9b7-8f7f-4a05-8c0a-8888c1554258", 5508m, "7ec212ed-a6cd-43da-9062-51eb3565703a", "a7474e3f-c946-4a03-8464-d3d582bb88bc", new DateTime(2024, 3, 6, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(265) },
                    { "6c83f69e-24a5-41b4-b59f-969b4836f5a2", 7159m, "23500d82-c23a-4d24-9ac7-8d1e698498f5", "44724604-1138-4ef4-a6a6-8be13e5fbc05", new DateTime(2024, 8, 28, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(627) },
                    { "6e5e4269-0d81-4bae-8607-ffc62103354f", 4250m, "33d7d29c-eae3-46da-98ae-3ae5217da99e", "ceecc567-2fc6-48d7-a82c-c912993f5ec1", new DateTime(2025, 1, 12, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(644) },
                    { "6e8b78be-17a3-404e-920d-c7be9a9bb99c", 5202m, "33d7d29c-eae3-46da-98ae-3ae5217da99e", "e4463913-a731-4ef9-a745-6720bc719fec", new DateTime(2025, 1, 30, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(655) },
                    { "82a7c4b6-ad23-4d85-ab19-a4bb520c1a0a", 4460m, "1ea12edd-2a72-4a41-a4c8-e90e7532e198", "ceecc567-2fc6-48d7-a82c-c912993f5ec1", new DateTime(2024, 9, 11, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(469) },
                    { "847e6cfd-c2ab-4cf0-8f29-be2b47e4bf61", 5111m, "33d7d29c-eae3-46da-98ae-3ae5217da99e", "44724604-1138-4ef4-a6a6-8be13e5fbc05", new DateTime(2024, 9, 9, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(649) },
                    { "8a2401a0-34ef-40bd-bcc7-b6c0be608ac3", 3234m, "7ec212ed-a6cd-43da-9062-51eb3565703a", "e4463913-a731-4ef9-a745-6720bc719fec", new DateTime(2024, 11, 3, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(249) },
                    { "9a6b50b2-4a30-4316-ac68-7b9389b8b562", 1609m, "b2fdd407-4270-4bcb-84e9-5129df7f2f66", "143e7fda-51b1-49d6-a915-6f931ef38a1b", new DateTime(2024, 9, 10, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(528) },
                    { "9bc2cd1e-2fc7-40f3-af45-ceb4c729a10b", 1706m, "1ea12edd-2a72-4a41-a4c8-e90e7532e198", "a7474e3f-c946-4a03-8464-d3d582bb88bc", new DateTime(2024, 10, 24, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(474) },
                    { "c4bcddbe-0545-4d3a-b22d-69dfb4f1edde", 5946m, "6adad1ca-bbd5-4206-a5de-01322ad64df9", "ceecc567-2fc6-48d7-a82c-c912993f5ec1", new DateTime(2024, 4, 10, 12, 51, 21, 863, DateTimeKind.Local).AddTicks(593) }
                });

            migrationBuilder.InsertData(
                table: "Fundings",
                columns: new[] { "Id", "Amount", "OrganizationId", "ProjectId", "TimeOfOperation" },
                values: new object[,]
                {
                    { "1afa9941-3456-4cb0-a6fa-276b38373536", 7495m, "ceecc567-2fc6-48d7-a82c-c912993f5ec1", "cc64c897-7ac9-4972-a018-9abba89aa4f9", new DateTime(2025, 2, 4, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2799) },
                    { "1bcb389d-46f1-43dd-b20e-fbc3c06830c1", 6675m, "143e7fda-51b1-49d6-a915-6f931ef38a1b", "b621ab84-05d3-45d7-aad1-2226f9251de2", new DateTime(2025, 1, 16, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2952) },
                    { "1e49b078-ecbe-4570-b51d-6a3ad1a076de", 6614m, "a7474e3f-c946-4a03-8464-d3d582bb88bc", "eb02df2a-3280-4933-a101-2fa51c6087ff", new DateTime(2024, 12, 28, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(3048) },
                    { "4addfd92-1a50-4886-a6d9-fa63ef07260f", 10650m, "44724604-1138-4ef4-a6a6-8be13e5fbc05", "a1322056-4a6f-4397-af7c-0299bc93dd91", new DateTime(2024, 11, 30, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2989) },
                    { "5609c29c-fa00-4efd-946d-a2daa47d9eee", 2845m, "a7474e3f-c946-4a03-8464-d3d582bb88bc", "d05753dd-57b5-40be-a349-787cf2ff7004", new DateTime(2025, 2, 18, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(3065) },
                    { "a6ba75ca-d352-405c-8233-e9d2c321d867", 13069m, "ceecc567-2fc6-48d7-a82c-c912993f5ec1", "64500732-9a8e-4fbe-addc-b34f0c3a49e3", new DateTime(2024, 11, 30, 12, 51, 21, 860, DateTimeKind.Local).AddTicks(4802) },
                    { "aa991c82-6703-4f38-8658-06c8ac40cbc8", 2844m, "143e7fda-51b1-49d6-a915-6f931ef38a1b", "4a81d64d-bb22-4293-a372-2c29d1e1976f", new DateTime(2024, 12, 4, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2919) },
                    { "ae88c44d-a757-4618-9699-b5d57b884427", 5730m, "e4463913-a731-4ef9-a745-6720bc719fec", "19c358cc-8115-4366-b51e-895a6d1806e9", new DateTime(2025, 2, 11, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2849) },
                    { "ba677f09-1189-4cf6-8932-a3c989c78ed4", 2217m, "ceecc567-2fc6-48d7-a82c-c912993f5ec1", "aa19feb2-f8ff-4f8c-9934-13e2fd551f4d", new DateTime(2024, 12, 4, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2769) },
                    { "c092fda8-64ac-4595-8c41-8adf04bcdbbd", 11028m, "a7474e3f-c946-4a03-8464-d3d582bb88bc", "bc16fa28-fbd1-4572-8da3-447e72272ee7", new DateTime(2025, 2, 18, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(3079) },
                    { "c48a871e-797f-4191-9f5a-30223732051a", 14508m, "143e7fda-51b1-49d6-a915-6f931ef38a1b", "fc214fe7-8e6c-40e6-ac91-817d0ac59b1b", new DateTime(2024, 12, 16, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2934) },
                    { "cb034b04-d2f1-4e85-baf4-64e0a0b2e66c", 5616m, "44724604-1138-4ef4-a6a6-8be13e5fbc05", "ad50d34f-1c0f-4744-95d6-1934238b49ef", new DateTime(2024, 12, 31, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(3003) },
                    { "d28a81d0-b036-4759-ab13-bd74cc4a10f8", 17357m, "e4463913-a731-4ef9-a745-6720bc719fec", "16c6ceb0-7082-4108-9e97-a8319f4a28e6", new DateTime(2024, 12, 30, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2830) },
                    { "d6307229-d3a9-4d0b-8b01-581586c15588", 18300m, "44724604-1138-4ef4-a6a6-8be13e5fbc05", "b1f3b002-94bd-4e5b-97a5-1708d1ae669d", new DateTime(2025, 2, 16, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2974) },
                    { "ddfe2604-de40-47aa-bb24-0057c77ab9ff", 5925m, "e4463913-a731-4ef9-a745-6720bc719fec", "30c88134-87b8-4ff0-a119-cc0cd62b44ad", new DateTime(2025, 1, 26, 12, 51, 21, 862, DateTimeKind.Local).AddTicks(2865) }
                });

            migrationBuilder.InsertData(
                table: "Reports",
                columns: new[] { "Id", "AdministrativeSpending", "LabourSpending", "MaterialsSpending", "ProjectId" },
                values: new object[,]
                {
                    { "1aa22666-8871-4bde-866b-8b5e956d2612", 1522m, 3336m, 2623m, "eb02df2a-3280-4933-a101-2fa51c6087ff" },
                    { "1c93aae3-eee9-449c-a41e-ccb6804ba205", 1194m, 6100m, 5650m, "cc64c897-7ac9-4972-a018-9abba89aa4f9" },
                    { "1db57be7-7497-44c6-8bcc-9212220813ea", 1278m, 4236m, 4299m, "30c88134-87b8-4ff0-a119-cc0cd62b44ad" },
                    { "31e0c7af-6b73-417a-bfdb-f89bdb7e23e7", 1796m, 4447m, 4188m, "aa19feb2-f8ff-4f8c-9934-13e2fd551f4d" },
                    { "603cb07b-2679-4f03-ac32-764730f2aac2", 1968m, 7213m, 3362m, "64500732-9a8e-4fbe-addc-b34f0c3a49e3" },
                    { "7c48b796-86c8-4192-a37d-688d08f429af", 1507m, 6515m, 6871m, "19c358cc-8115-4366-b51e-895a6d1806e9" },
                    { "8a33a0a3-0245-4b42-8502-ec726053f36d", 4351m, 6228m, 3944m, "a1322056-4a6f-4397-af7c-0299bc93dd91" },
                    { "9c5b84f5-2ba1-4463-b07a-2cb2fa58eabd", 1541m, 6871m, 5933m, "4a81d64d-bb22-4293-a372-2c29d1e1976f" },
                    { "c617d7d8-20cb-45e6-a6d5-a761372bec74", 1860m, 5314m, 6732m, "fc214fe7-8e6c-40e6-ac91-817d0ac59b1b" },
                    { "cd73c60f-d53c-483e-b22f-a7d8dbb97a78", 2984m, 5336m, 4539m, "b621ab84-05d3-45d7-aad1-2226f9251de2" },
                    { "d91ec204-7227-483e-be58-6e554a5e5b9d", 2221m, 3824m, 6630m, "bc16fa28-fbd1-4572-8da3-447e72272ee7" },
                    { "deda7dc2-6a46-41eb-a19b-971de781c02c", 3647m, 5086m, 3318m, "d05753dd-57b5-40be-a349-787cf2ff7004" },
                    { "ea0d96a0-19ed-4f85-a5d6-e18658301d3f", 2111m, 6265m, 3058m, "16c6ceb0-7082-4108-9e97-a8319f4a28e6" },
                    { "f37448b0-be9b-4d75-918a-23e51228ca58", 4014m, 5853m, 5619m, "ad50d34f-1c0f-4744-95d6-1934238b49ef" },
                    { "f3abeaf4-7c08-4e34-87aa-ce53bb3ce088", 4846m, 3673m, 2347m, "b1f3b002-94bd-4e5b-97a5-1708d1ae669d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Donations_DonorId",
                table: "Donations",
                column: "DonorId");

            migrationBuilder.CreateIndex(
                name: "IX_Donations_OrganizationId",
                table: "Donations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Fundings_OrganizationId",
                table: "Fundings",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Fundings_ProjectId",
                table: "Fundings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ProjectId",
                table: "Reports",
                column: "ProjectId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Donations");

            migrationBuilder.DropTable(
                name: "Fundings");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Donors");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
