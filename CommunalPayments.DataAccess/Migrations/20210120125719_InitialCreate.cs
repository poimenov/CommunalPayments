using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CommunalPayments.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayModes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayModes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SurName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ErcId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ErcId = table.Column<long>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    ModeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bills_PayModes_ModeId",
                        column: x => x.ModeId,
                        principalTable: "PayModes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bills_PayStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "PayStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<long>(type: "INTEGER", nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Street = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Building = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Apartment = table.Column<string>(type: "TEXT", maxLength: 25, nullable: true),
                    PersonId = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    InternalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VolumeFrom = table.Column<decimal>(type: "TEXT", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", nullable: false),
                    Measure = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rates_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErcId = table.Column<long>(type: "INTEGER", nullable: false),
                    BillId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PeriodFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodTo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastIndication = table.Column<decimal>(type: "TEXT", nullable: true),
                    CurrentIndication = table.Column<decimal>(type: "TEXT", nullable: true),
                    Value = table.Column<decimal>(type: "TEXT", nullable: true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentItems_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentItems_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            // Services
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 1, 1, "ЭЛЕКТРИЧЕСТВО", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 2, 2, "КВАРТПЛАТА", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 3, 3, "ОТОПЛЕНИЕ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 4, 4, "ГОРЯЧАЯ ВОДА", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 5, 5, "ХОЛОДНАЯ ВОДА", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 6, 6, "КАНАЛИЗАЦИЯ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 7, 7, "ГАЗ ПРИРОДНЫЙ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 8, 10, "АНТЕННА", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 9, 11, "ЖИВОТНЫЕ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 10, 12, "ГАРАЖ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 11, 13, "ПОГРЕБ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 12, 14, "САРАЙ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 13, 15, "КЛАДОВКА", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 14, 16, "ТЕЛЕФОН", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 15, 17, "РАСПРЕДЕЛЕНИЕ ПРИРОДНОГО ГАЗА", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 16, 20, "ЛИФТ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 17, 21, "ХОЗРАСХОДЫ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 18, 22, "НАЛОГ НА ЗЕМЛЮ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 19, 24, "ОПЛАТА ПО АКТАМ", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 20, 30, "ЭЛ.ЭН. (рассрочка)", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 21, 33, "ВЫВОЗ ТБО", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 22, 35, "ДОМОФОН", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 23, 39, "КАНАЛИЗАЦИЯ(решение суда)", false });
            migrationBuilder.InsertData(table: "Services", columns: new[] { "Id", "ErcId", "Name", "Enabled" }, values: new object[] { 24, 49, "ВОЛЯ.ТЕЛЕКОММУНИКАЦ.УСЛУГИ", false });
            //Rates
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 11, new DateTime(2018, 11, 1), 8, 54892, "гр/м3", "Цена газа для бытовых потребителей", false, 7 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 1, new DateTime(2017, 3, 1), 0, 9M, "гр/квт", "При потреблении в мес. до 100квт/час", true, 1 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 2, new DateTime(2017, 3, 1), 1, 68M, "гр/квт", "При потреблении в мес. свыше 100квт/час", true, 1 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 3, new DateTime(2019, 1, 1), 39, 38M, "гр/м2", "Отопление по норме в отопительный период", false, 3 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 4, new DateTime(2019, 1, 1), 1539, 5M, "гр/ГКал", "Отопление по счетчику", false, 3 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 5, new DateTime(2019, 1, 1), 93, 22M, "гр/м3", "Горячая вода с полотенцесушителем", true, 4 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 6, new DateTime(2019, 1, 1), 86, 32M, "гр/м3", "Горячая вода без полотенцесушителя", false, 4 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 7, new DateTime(2019, 4, 21), 13, 14M, "гр/м3", "Услуги водоснабжения для домов с использованием внутридомовых сетей", true, 5 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 8, new DateTime(2019, 4, 21), 12, 25M, "гр/м3", "Услуги водоснабжения для домов без использованием внутридомовых сетей", false, 5 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 9, new DateTime(2019, 4, 21), 6, 696M, "гр/м3", "Услуги водоотведения для домов с использованием внутридомовых сетей", true, 6 });
            migrationBuilder.InsertData(table: "Rates", columns: new[] { "Id", "DateFrom", "VolumeFrom", "Value", "Measure", "Description", "Enabled", "ServiceId" }, values: new object[] { 10, new DateTime(2019, 4, 21), 6, 264M, "гр/м3", "Услуги водоотведения для домов без использованием внутридомовых сетей", false, 6 });
            //PayModes
            migrationBuilder.InsertData(table: "PayModes", columns: new[] { "Id", "Name" }, values: new object[] { 1, "Банковский перевод" });
            migrationBuilder.InsertData(table: "PayModes", columns: new[] { "Id", "Name" }, values: new object[] { 15, "Банковская карта" });
            //PayStatuses             
            migrationBuilder.InsertData(table: "PayStatuses", columns: new[] { "Id", "Name" }, values: new object[] {1, "Наверное, создан"});
            migrationBuilder.InsertData(table: "PayStatuses", columns: new[] { "Id", "Name" }, values: new object[] {2, "Статус №2"});
            migrationBuilder.InsertData(table: "PayStatuses", columns: new[] { "Id", "Name" }, values: new object[] {3, "Статус №3"});
            migrationBuilder.InsertData(table: "PayStatuses", columns: new[] { "Id", "Name" }, values: new object[] {4, "Статус №4"});
            migrationBuilder.InsertData(table: "PayStatuses", columns: new[] { "Id", "Name" }, values: new object[] {5, "Проведен успешно" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PersonId",
                table: "Accounts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_ModeId",
                table: "Bills",
                column: "ModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_StatusId",
                table: "Bills",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentItems_PaymentId",
                table: "PaymentItems",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentItems_ServiceId",
                table: "PaymentItems",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AccountId",
                table: "Payments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BillId",
                table: "Payments",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_ServiceId",
                table: "Rates",
                column: "ServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentItems");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "PayModes");

            migrationBuilder.DropTable(
                name: "PayStatuses");
        }

    }
}
