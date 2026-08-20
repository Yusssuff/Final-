using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Task.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditCriteria",
                columns: table => new
                {
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    TableFields = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "BS_Periods",
                columns: table => new
                {
                    YearId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PeriodId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    L1Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    L2Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    displayorder = table.Column<int>(type: "int", nullable: true),
                    PeriodKey = table.Column<int>(type: "int", nullable: false),
                    BUID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearPeriods", x => new { x.YearId, x.PeriodId });
                });

            migrationBuilder.CreateTable(
                name: "BS_Years",
                columns: table => new
                {
                    YearId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    YearStartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    YearEndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    L1Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    L2Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    BUID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Years", x => x.YearId);
                });

            migrationBuilder.CreateTable(
                name: "HH_AR_SalesmenCats",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    buid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordSource = table.Column<byte>(type: "tinyint", nullable: true),
                    CommissionSchemeID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_AR_SalesmenCats", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "HH_AR_SalesmenPcts",
                columns: table => new
                {
                    SalesmanId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    SubordinateId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Serial = table.Column<int>(type: "int", nullable: false),
                    BUID = table.Column<string>(type: "nchar(15)", fixedLength: true, maxLength: 15, nullable: true, defaultValueSql: "('1')"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    RecordSource = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_AR_SalesmenPcts", x => new { x.SalesmanId, x.SubordinateId });
                });

            migrationBuilder.CreateTable(
                name: "HH_Customer",
                columns: table => new
                {
                    CustomerNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesSectorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesmanNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerNameE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerNameA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AreaNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistrictNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EInvoiceCustomerType = table.Column<byte>(type: "tinyint", nullable: true),
                    StopOrder = table.Column<byte>(type: "tinyint", nullable: true),
                    BadPayer = table.Column<byte>(type: "tinyint", nullable: true),
                    StopDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ForceCreditLimit = table.Column<byte>(type: "tinyint", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AllowCheck = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowDeferred = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowCash = table.Column<byte>(type: "tinyint", nullable: true),
                    ContactType = table.Column<int>(type: "int", nullable: false),
                    PriceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopOutLimit = table.Column<byte>(type: "tinyint", nullable: false),
                    UnpaidInvoiceCustomerWarning = table.Column<byte>(type: "tinyint", nullable: true),
                    CustomerServiceManNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerStatus = table.Column<int>(type: "int", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Balance = table.Column<double>(type: "float", nullable: true),
                    MerchType = table.Column<int>(type: "int", nullable: true),
                    Sat = table.Column<bool>(type: "bit", nullable: true),
                    Sun = table.Column<bool>(type: "bit", nullable: true),
                    Mon = table.Column<bool>(type: "bit", nullable: true),
                    Tue = table.Column<bool>(type: "bit", nullable: true),
                    Wed = table.Column<bool>(type: "bit", nullable: true),
                    Thu = table.Column<bool>(type: "bit", nullable: true),
                    Fri = table.Column<bool>(type: "bit", nullable: true),
                    TargetNPS = table.Column<int>(type: "int", nullable: true),
                    ActualNPS = table.Column<int>(type: "int", nullable: true),
                    ToVisit = table.Column<bool>(type: "bit", nullable: true),
                    BUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    today = table.Column<int>(type: "int", nullable: true),
                    TermsBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitOrder = table.Column<int>(type: "int", nullable: true),
                    ActualAvg = table.Column<int>(type: "int", nullable: true),
                    TargetAvg = table.Column<int>(type: "int", nullable: true),
                    CashDiscountID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitFrequency = table.Column<int>(type: "int", nullable: true),
                    Latitude = table.Column<decimal>(type: "numeric(24,15)", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric(24,15)", nullable: true),
                    Altitude = table.Column<decimal>(type: "numeric(24,15)", nullable: true),
                    ErrorRadius = table.Column<int>(type: "int", nullable: true),
                    InvoiceAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inactive = table.Column<byte>(type: "tinyint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<byte>(type: "tinyint", nullable: true),
                    RecordSource = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))"),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiresManualInvNo = table.Column<byte>(type: "tinyint", nullable: true),
                    OrderCeiling = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AddressA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddAddressE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddAddressA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ElectricityNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GISCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowCreditCard = table.Column<byte>(type: "tinyint", nullable: true),
                    ConfirmedOrders = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProductRangeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsignmentProductRange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryInvoiceProductRange = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadOfficeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHeadOffice = table.Column<byte>(type: "tinyint", nullable: false),
                    ReturnWithoutInvoice = table.Column<byte>(type: "tinyint", nullable: true),
                    HasCreditControlArea = table.Column<byte>(type: "tinyint", nullable: true),
                    PotCustomerRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MandatoryPhoto = table.Column<byte>(type: "tinyint", nullable: true),
                    PointsBalance = table.Column<int>(type: "int", nullable: true),
                    NoOfAllowedRedeemPacks = table.Column<int>(type: "int", nullable: true),
                    ServicePatternSet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPhysicalLocation = table.Column<byte>(type: "tinyint", nullable: false),
                    CutomerPhysicalLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProofOfVisit = table.Column<byte>(type: "tinyint", nullable: true),
                    LegacyCustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegacyCustomerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPotential = table.Column<byte>(type: "tinyint", nullable: true),
                    RecommendedPriceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnType = table.Column<byte>(type: "tinyint", nullable: true),
                    WindowTypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CloseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenTimeTW1 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CloseTimeTW1 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpenTimeTW2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CloseTimeTW2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceTimeTypeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowTempCredit = table.Column<byte>(type: "tinyint", nullable: true),
                    DefaultTerritory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopReasonID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTradeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllowEPayment = table.Column<byte>(type: "tinyint", nullable: true),
                    PreventSalesmanfromCollectingOutStandingInvoices = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowInstallment = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowBankTransfer = table.Column<byte>(type: "tinyint", nullable: true),
                    TempCreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TempCreditItemCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowDebitCard = table.Column<byte>(type: "tinyint", nullable: true),
                    RequireCheckPhoto = table.Column<byte>(type: "tinyint", nullable: true),
                    ValidateRecommendedQty = table.Column<byte>(type: "tinyint", nullable: true),
                    TaxRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommercialRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommercialRegistrationDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MunicipalLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsibleID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsibleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailTemplateType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SalesDivisionID = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    SharedCredit = table.Column<byte>(type: "tinyint", nullable: true),
                    CreditItemCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkFLowStatus = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "((0))"),
                    CommerceChamberRegNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MunicipalLicenseDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChannelID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DueDateToleranceDays = table.Column<int>(type: "int", nullable: true),
                    SkipDueDateValidation = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowCoupon = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_Customer", x => x.CustomerNo);
                });

            migrationBuilder.CreateTable(
                name: "HH_CustomerLocations",
                columns: table => new
                {
                    CustomerNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    LineSerial = table.Column<int>(type: "int", nullable: false),
                    CustomerLocationsGUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    LocationNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    Createdby = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    PlanoGramImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Inactive = table.Column<bool>(type: "bit", nullable: true),
                    Year = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContractID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContractDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    AssetNO = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    AssetID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    InactiveDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TypeInfo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    BUID = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ManufSerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordSource = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_CustomerLocations", x => new { x.CustomerNo, x.LineSerial, x.CustomerLocationsGUID });
                });

            migrationBuilder.CreateTable(
                name: "HH_EntityBUControl",
                columns: table => new
                {
                    TableName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BUControl = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_EntityBUControl", x => x.TableName);
                });

            migrationBuilder.CreateTable(
                name: "HH_IC_UOMDetail",
                columns: table => new
                {
                    UOMID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    LinkedUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Factor = table.Column<decimal>(type: "numeric(28,8)", nullable: true),
                    MultiplyDivide = table.Column<byte>(type: "tinyint", nullable: true),
                    buid = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, defaultValueSql: "('1')"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    RecordSource = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_IC_UOMDetail", x => new { x.UOMID, x.LinkedUOM });
                });

            migrationBuilder.CreateTable(
                name: "HH_Item",
                columns: table => new
                {
                    ItemNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ItemNameE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ItemNameA = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DefaultUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    BrandNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TaxCodeID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PLT_CRT = table.Column<int>(type: "int", nullable: true),
                    Packet_PLT = table.Column<int>(type: "int", nullable: true),
                    ItemType = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    GroupID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CategoryID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    buid = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, defaultValueSql: "('1')"),
                    ShortNameA = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ShortNameE = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    barcode1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    barcode2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValueSql: "(1)"),
                    MasterBrandID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ItemConstraintID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    AutomaticCutQty = table.Column<bool>(type: "bit", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    MaxQty = table.Column<decimal>(type: "numeric(28,8)", nullable: true),
                    MaxQtyUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ReturnQuantity = table.Column<int>(type: "int", nullable: true),
                    ReturnUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    BatchSupport = table.Column<byte>(type: "tinyint", nullable: false),
                    SerialNumberSupport = table.Column<byte>(type: "tinyint", nullable: false),
                    AttributeVal = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SalesUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PurchaseUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Attribute1Val = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SmallUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    LargeUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ZeroCostPrice = table.Column<bool>(type: "bit", nullable: false),
                    ZeroSalePrice = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PalletQuantity = table.Column<decimal>(type: "numeric(28,8)", nullable: true),
                    RecordSource = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))"),
                    EnableReturn = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((1))"),
                    Points = table.Column<int>(type: "int", nullable: false),
                    SecUOMQty = table.Column<decimal>(type: "numeric(28,8)", nullable: false),
                    GLDimensionSetID = table.Column<int>(type: "int", nullable: true),
                    TaxExempted = table.Column<byte>(type: "tinyint", nullable: true),
                    DefaultVendor = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    MinimumStockPCT = table.Column<decimal>(type: "numeric(28,8)", nullable: true),
                    MinQty = table.Column<decimal>(type: "numeric(28,8)", nullable: true),
                    AssignBatchOnReturn = table.Column<byte>(type: "tinyint", nullable: true),
                    CWItem = table.Column<byte>(type: "tinyint", nullable: true),
                    InventoryTransactionUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    AssetLocationType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ReturnDamaged = table.Column<bool>(type: "bit", nullable: true),
                    DamagedReturnItemId = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ShelfLife = table.Column<int>(type: "int", nullable: true),
                    ReturnPercentageBeforeShelfLife = table.Column<int>(type: "int", nullable: true),
                    ReturnPercentageAfterShelfLife = table.Column<int>(type: "int", nullable: true),
                    ReturnPeriod = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_Item", x => x.ItemNo);
                });

            migrationBuilder.CreateTable(
                name: "HH_ItemUoms",
                columns: table => new
                {
                    ItemNo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SmallUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    LargeUOM = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Factor = table.Column<decimal>(type: "numeric(28,8)", nullable: true),
                    MultiplyDivide = table.Column<byte>(type: "tinyint", nullable: true),
                    buid = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, defaultValueSql: "('1')"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    RecordSource = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_hh_ItemUoms", x => new { x.ItemNo, x.SmallUOM, x.LargeUOM });
                });

            migrationBuilder.CreateTable(
                name: "HH_Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SalesManNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    BranchNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    SalesManType = table.Column<int>(type: "int", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    SMS = table.Column<byte>(type: "tinyint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    BUID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true, defaultValueSql: "('1')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_Messages", x => x.MessageID);
                });

            migrationBuilder.CreateTable(
                name: "HH_PARAMS",
                columns: table => new
                {
                    PARAM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PARAM_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PARAM_VALUE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SystemParam = table.Column<byte>(type: "tinyint", nullable: true),
                    PerBU = table.Column<byte>(type: "tinyint", nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParamControlType = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_PARAMS", x => x.PARAM_ID);
                });

            migrationBuilder.CreateTable(
                name: "HH_PARAMS_BU",
                columns: table => new
                {
                    PARAM_ID = table.Column<int>(type: "int", nullable: false),
                    BUID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PARAM_NAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PARAM_VALUE = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_PARAMS_BU", x => new { x.PARAM_ID, x.BUID });
                });

            migrationBuilder.CreateTable(
                name: "HH_SA_BU",
                columns: table => new
                {
                    BUID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentBU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<byte>(type: "tinyint", nullable: false),
                    HasChildren = table.Column<byte>(type: "tinyint", nullable: true),
                    ShortCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationType = table.Column<byte>(type: "tinyint", nullable: true),
                    ERPOrganizationID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemoLineId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ERPDistChannelID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ERPSalesDivisonID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeOfPayment = table.Column<byte>(type: "tinyint", nullable: true),
                    CompanyGlAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPGovernorate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPplant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesTypeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPSalesOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPSalesdistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPCompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPCreditControlArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPCreditSegment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SAPSalesOrg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemporaryCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_SA_BU", x => x.BUID);
                });

            migrationBuilder.CreateTable(
                name: "HH_SA_RolePermissions",
                columns: table => new
                {
                    RoleID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    KeyID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CanRead = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))"),
                    CanInsert = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))"),
                    CanUpdate = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))"),
                    CanDelete = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))"),
                    CanExecute = table.Column<byte>(type: "tinyint", nullable: true, defaultValueSql: "((0))"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CanViewAttachments = table.Column<byte>(type: "tinyint", nullable: true),
                    CanAddAttachments = table.Column<byte>(type: "tinyint", nullable: true),
                    CanDeleteAttachments = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HH_SA_RolePermis__341D6548", x => new { x.RoleID, x.KeyID });
                });

            migrationBuilder.CreateTable(
                name: "HH_SA_Roles",
                columns: table => new
                {
                    RoleID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeedExplicitUpdatePermission = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_SA_Roles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "HH_SA_SecurityKeys",
                columns: table => new
                {
                    KeyID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DescriptionA = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ParentKeyID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RedirectURI = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<byte>(type: "tinyint", nullable: true),
                    ModuleID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ModuleDesc = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsErpAware = table.Column<byte>(type: "tinyint", nullable: false),
                    EnableEditOnERPIntegration = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HH_SA_SecurityKe__35118981", x => x.KeyID);
                });

            migrationBuilder.CreateTable(
                name: "HH_SA_UserBUPermissions",
                columns: table => new
                {
                    UserID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    BUID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false, defaultValueSql: "('1')"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HH_SA_UserBUPerm__1FF7A424", x => new { x.UserID, x.BUID });
                });

            migrationBuilder.CreateTable(
                name: "HH_Salesman",
                columns: table => new
                {
                    SalesmanNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SalesmanNameE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesmanNameA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesManType = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashPass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUser = table.Column<bool>(type: "bit", nullable: false),
                    outOfRouteLimit = table.Column<int>(type: "int", nullable: true),
                    PreFix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextOrderNo = table.Column<int>(type: "int", nullable: false),
                    NextVisitNo = table.Column<int>(type: "int", nullable: false),
                    NextPaymentNo = table.Column<int>(type: "int", nullable: false),
                    Book1Start = table.Column<int>(type: "int", nullable: false),
                    Book1End = table.Column<int>(type: "int", nullable: false),
                    Book2Start = table.Column<int>(type: "int", nullable: false),
                    Book2End = table.Column<int>(type: "int", nullable: false),
                    PayBook1Start = table.Column<int>(type: "int", nullable: false),
                    PayBook1End = table.Column<int>(type: "int", nullable: false),
                    PayBook2Start = table.Column<int>(type: "int", nullable: false),
                    PayBook2End = table.Column<int>(type: "int", nullable: false),
                    BookSizeMult = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentOutOfRoute = table.Column<int>(type: "int", nullable: true),
                    WareHouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedDriverID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OutofOrderLimit = table.Column<int>(type: "int", nullable: true),
                    SectorID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UseNewPrintFW = table.Column<byte>(type: "tinyint", nullable: true),
                    GPSMinTrackingDist = table.Column<int>(type: "int", nullable: true),
                    GPSMinTrackingTime = table.Column<int>(type: "int", nullable: true),
                    ProofOfVisit = table.Column<byte>(type: "tinyint", nullable: true),
                    MaxVisitsWithoutProof = table.Column<int>(type: "int", nullable: true),
                    Permission = table.Column<byte>(type: "tinyint", nullable: true),
                    NextCustLocUpdateNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextRequestNo = table.Column<int>(type: "int", nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Inactive = table.Column<byte>(type: "tinyint", nullable: true),
                    PreferredLanguage = table.Column<byte>(type: "tinyint", nullable: true),
                    HWsupport = table.Column<byte>(type: "tinyint", nullable: true),
                    RebateDocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextReturnNo = table.Column<int>(type: "int", nullable: true),
                    EncPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SyncBeforeVisit = table.Column<byte>(type: "tinyint", nullable: true),
                    UploadAfterVisit = table.Column<byte>(type: "tinyint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JPCustCount = table.Column<int>(type: "int", nullable: true),
                    RecordSource = table.Column<byte>(type: "tinyint", nullable: true),
                    Driver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Helper1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Helper2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Helper3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultCustomerNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ERP_Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableOrderWithoutPOV = table.Column<byte>(type: "tinyint", nullable: true),
                    DefaultMSL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisableDayofvisitedit = table.Column<byte>(type: "tinyint", nullable: true),
                    SMDistanceRange = table.Column<int>(type: "int", nullable: true),
                    RouteID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowCreditOverride = table.Column<byte>(type: "tinyint", nullable: true),
                    CalendarID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverDueInvoicesAmountLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OverDueInvoicesCountLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OpenInvoicesAmountLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OpenInvoicesCountLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TemporaryCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TemporaryCreditBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SMSTemplateTypeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    preventcollect = table.Column<byte>(type: "tinyint", nullable: true),
                    CommissionSchemeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPasswordRenew = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnType = table.Column<byte>(type: "tinyint", nullable: true),
                    TransferPriceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnablePrePayment = table.Column<byte>(type: "tinyint", nullable: true),
                    CreditCardAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashJournalNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChequeJournalNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguageID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderTakerSuggestedValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DamagedWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableManualDiscountOnOrderLine = table.Column<int>(type: "int", nullable: true),
                    ParentEmployee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverduePendingPaymentsDays = table.Column<int>(type: "int", nullable: true),
                    ParentSalesmanID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsParent = table.Column<byte>(type: "tinyint", nullable: false),
                    LoadingLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UploadTruckNoToERP = table.Column<byte>(type: "tinyint", nullable: true),
                    ConsumptionFactor = table.Column<double>(type: "float", nullable: false),
                    RecQtyCalcType = table.Column<byte>(type: "tinyint", nullable: false),
                    AllowOnlineReturn = table.Column<int>(type: "int", nullable: true),
                    ReturnWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VanIndicator = table.Column<byte>(type: "tinyint", nullable: true),
                    mandatoryStockCheckPhoto = table.Column<byte>(type: "tinyint", nullable: true),
                    IgnoreCheckingPendingTransactions = table.Column<byte>(type: "tinyint", nullable: true),
                    ReplenishmentTemplateID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnableProforma = table.Column<byte>(type: "tinyint", nullable: true),
                    CreditCardReceiptMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChequeReceiptMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashReceiptMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultTerritory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesbuzzMobileClient = table.Column<byte>(type: "tinyint", nullable: true),
                    DisableLoadWithBalance = table.Column<byte>(type: "tinyint", nullable: true),
                    StopReasonID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllowOnlineConfirm = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowOnlineEdit = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowOnlineClose = table.Column<byte>(type: "tinyint", nullable: true),
                    MaxOfflineInvoices = table.Column<byte>(type: "tinyint", nullable: true),
                    CommissionType = table.Column<byte>(type: "tinyint", nullable: true),
                    CommissionValue = table.Column<double>(type: "float", nullable: true),
                    RouteOrigin = table.Column<byte>(type: "tinyint", nullable: true),
                    EnableAutoArrive = table.Column<byte>(type: "tinyint", nullable: true),
                    Mode = table.Column<byte>(type: "tinyint", nullable: true),
                    AllowPriceEdit = table.Column<byte>(type: "tinyint", nullable: true),
                    TransferReceiptMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowTwoActiveTrips = table.Column<byte>(type: "tinyint", nullable: true),
                    SuperVisorID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuperVisorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainWarehouseSellableBINLocation = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HH_Salesman", x => x.SalesmanNo);
                });

            migrationBuilder.CreateTable(
                name: "hh_ST_NumberSequance",
                columns: table => new
                {
                    NumberSequanceID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DescriptionAR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SerialStart = table.Column<byte>(type: "tinyint", nullable: true),
                    SerialLength = table.Column<byte>(type: "tinyint", nullable: true),
                    IncrementBy = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((1))"),
                    ForceFormat = table.Column<byte>(type: "tinyint", nullable: false),
                    MinValue = table.Column<long>(type: "bigint", nullable: true, defaultValueSql: "((1))"),
                    MaxValue = table.Column<long>(type: "bigint", nullable: true),
                    NextValue = table.Column<long>(type: "bigint", nullable: true, defaultValueSql: "((1))"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ContinuousSequence = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__hh_ST_NumberSequ__49EDDDF0", x => x.NumberSequanceID);
                });

            migrationBuilder.CreateTable(
                name: "hh_ST_NumberSequanceCanceledSerials",
                columns: table => new
                {
                    NumberSequanceID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CanceledSerial = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__hh_ST_NumberSequ__4AE20229", x => new { x.NumberSequanceID, x.CanceledSerial });
                });

            migrationBuilder.CreateTable(
                name: "hh_ST_NumberSequanceCanceledSerialsBU",
                columns: table => new
                {
                    ShortCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CanceledSerial = table.Column<long>(type: "bigint", nullable: false),
                    NumberSequanceID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hh_ST_NumberSequanceCanceledSerialsBU", x => new { x.ShortCode, x.CanceledSerial, x.NumberSequanceID });
                });

            migrationBuilder.CreateTable(
                name: "hh_ST_NumberSequenceBU",
                columns: table => new
                {
                    NumberSequanceID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NextValue = table.Column<long>(type: "bigint", nullable: true, defaultValueSql: "((1))"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hh_ST_NumberSequenceBU_1", x => new { x.NumberSequanceID, x.ShortCode });
                });

            migrationBuilder.CreateTable(
                name: "hh_Target",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemBased = table.Column<bool>(type: "bit", nullable: true),
                    isCustomer = table.Column<byte>(type: "tinyint", nullable: true),
                    isDistributor = table.Column<bool>(type: "bit", nullable: true),
                    HasDetail = table.Column<bool>(type: "bit", nullable: false),
                    ISCalculated = table.Column<bool>(type: "bit", nullable: true),
                    DisplayOrder = table.Column<short>(type: "smallint", nullable: true),
                    IsVisible = table.Column<byte>(type: "tinyint", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurveyRefId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestRefID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ISPercentage = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hh_Target", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "loginusers",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    role = table.Column<int>(type: "int", nullable: true),
                    password = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    userCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BUID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    branchNo = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    RoleID = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EncPassword = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPasswordRenew = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WindowsLogin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForceLogin = table.Column<byte>(type: "tinyint", nullable: true),
                    InActive = table.Column<byte>(type: "tinyint", nullable: false),
                    ADAuthentication = table.Column<byte>(type: "tinyint", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdvancedWFUser = table.Column<byte>(type: "tinyint", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupervisorModuleUser = table.Column<byte>(type: "tinyint", nullable: true),
                    WillNotExpired = table.Column<bool>(type: "bit", nullable: false),
                    EnableMFA = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loginusers", x => x.userName);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordLevelSecurity",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserOrRoleID = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    RecType = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordLevelSecurity", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SA_AuditLogs",
                columns: table => new
                {
                    Serial = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Operationtype = table.Column<byte>(type: "tinyint", nullable: true),
                    Olddata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    TableKeys = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SA_AuditLogs", x => x.Serial);
                });

            migrationBuilder.CreateTable(
                name: "SA_License",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EncryptedXmlData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SA_License", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SA_Sessions",
                columns: table => new
                {
                    JTI = table.Column<Guid>(type: "uniqueidentifier", maxLength: 255, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    S = table.Column<byte>(type: "tinyint", nullable: true),
                    MachineIP = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastAccess = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SA_Sessi__C4D18800571AE969", x => x.JTI);
                });

            migrationBuilder.CreateTable(
                name: "ST_EventDefinition",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false),
                    Eventlangid = table.Column<short>(type: "smallint", nullable: false),
                    EventText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EventType = table.Column<byte>(type: "tinyint", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ST_EventDefiniti__2CDE666E", x => new { x.EventID, x.Eventlangid });
                });

            migrationBuilder.CreateTable(
                name: "ST_EventLogMaster",
                columns: table => new
                {
                    ProcessID = table.Column<decimal>(type: "decimal(15,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SQLUserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    NTUserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    WorkStationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProcessStart = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ProcessEnd = table.Column<DateTime>(type: "datetime", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: true),
                    ArabicDescription = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    BUID = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsBatchJob = table.Column<byte>(type: "tinyint", nullable: true),
                    OverAllStatus = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ST_EventLogMaste__2EC6AEE0", x => x.ProcessID);
                });

            migrationBuilder.CreateTable(
                name: "ST_EventLogDetail",
                columns: table => new
                {
                    ProcessID = table.Column<decimal>(type: "decimal(15,0)", nullable: false),
                    EventSerialNo = table.Column<decimal>(type: "decimal(15,0)", nullable: false),
                    EventID = table.Column<int>(type: "int", nullable: false),
                    EventDateTime = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Var1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var3 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var4 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var5 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var6 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var7 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var8 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var9 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Var10 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    Createdby = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ST_EventLogDetai__31A31B8B", x => new { x.ProcessID, x.EventSerialNo });
                    table.ForeignKey(
                        name: "FK_ST_EventLogDetail__ST_EventLogMaster__ProcessID",
                        column: x => x.ProcessID,
                        principalTable: "ST_EventLogMaster",
                        principalColumn: "ProcessID");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__BS_Periods__565DD247",
                table: "BS_Periods",
                column: "PeriodKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_HH_CustomerLocations_LocationNo",
                table: "HH_CustomerLocations",
                column: "LocationNo");

            migrationBuilder.CreateIndex(
                name: "idx_hh_item_masterBrandID",
                table: "HH_Item",
                column: "MasterBrandID");

            migrationBuilder.CreateIndex(
                name: "idx_hh_itemUoms_ItemNo",
                table: "HH_ItemUoms",
                column: "ItemNo");

            migrationBuilder.CreateIndex(
                name: "IX_RecordLevelSecurity",
                table: "RecordLevelSecurity",
                columns: new[] { "UserOrRoleID", "RecType", "EntityName", "FieldName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditCriteria");

            migrationBuilder.DropTable(
                name: "BS_Periods");

            migrationBuilder.DropTable(
                name: "BS_Years");

            migrationBuilder.DropTable(
                name: "HH_AR_SalesmenCats");

            migrationBuilder.DropTable(
                name: "HH_AR_SalesmenPcts");

            migrationBuilder.DropTable(
                name: "HH_Customer");

            migrationBuilder.DropTable(
                name: "HH_CustomerLocations");

            migrationBuilder.DropTable(
                name: "HH_EntityBUControl");

            migrationBuilder.DropTable(
                name: "HH_IC_UOMDetail");

            migrationBuilder.DropTable(
                name: "HH_Item");

            migrationBuilder.DropTable(
                name: "HH_ItemUoms");

            migrationBuilder.DropTable(
                name: "HH_Messages");

            migrationBuilder.DropTable(
                name: "HH_PARAMS");

            migrationBuilder.DropTable(
                name: "HH_PARAMS_BU");

            migrationBuilder.DropTable(
                name: "HH_SA_BU");

            migrationBuilder.DropTable(
                name: "HH_SA_RolePermissions");

            migrationBuilder.DropTable(
                name: "HH_SA_Roles");

            migrationBuilder.DropTable(
                name: "HH_SA_SecurityKeys");

            migrationBuilder.DropTable(
                name: "HH_SA_UserBUPermissions");

            migrationBuilder.DropTable(
                name: "HH_Salesman");

            migrationBuilder.DropTable(
                name: "hh_ST_NumberSequance");

            migrationBuilder.DropTable(
                name: "hh_ST_NumberSequanceCanceledSerials");

            migrationBuilder.DropTable(
                name: "hh_ST_NumberSequanceCanceledSerialsBU");

            migrationBuilder.DropTable(
                name: "hh_ST_NumberSequenceBU");

            migrationBuilder.DropTable(
                name: "hh_Target");

            migrationBuilder.DropTable(
                name: "loginusers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "RecordLevelSecurity");

            migrationBuilder.DropTable(
                name: "SA_AuditLogs");

            migrationBuilder.DropTable(
                name: "SA_License");

            migrationBuilder.DropTable(
                name: "SA_Sessions");

            migrationBuilder.DropTable(
                name: "ST_EventDefinition");

            migrationBuilder.DropTable(
                name: "ST_EventLogDetail");

            migrationBuilder.DropTable(
                name: "ST_EventLogMaster");
        }
    }
}
