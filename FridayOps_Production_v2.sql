IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630091118_InitialOfficialDatabase'
)
BEGIN
    CREATE TABLE [DraftCustomers] (
        [Id] int NOT NULL IDENTITY,
        [Line] nvarchar(200) NOT NULL,
        [Market] nvarchar(200) NOT NULL,
        [Branch] nvarchar(50) NOT NULL,
        [SalesDistrict] nvarchar(50) NOT NULL,
        [CustomerType] nvarchar(50) NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [Status] nvarchar(30) NOT NULL DEFAULT N'Draft',
        CONSTRAINT [PK_DraftCustomers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630091118_InitialOfficialDatabase'
)
BEGIN
    CREATE TABLE [GenerationHistories] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [FileName] nvarchar(250) NOT NULL,
        [CustomersCount] int NOT NULL,
        [FirstBpCode] int NOT NULL,
        [LastBpCode] int NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        CONSTRAINT [PK_GenerationHistories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630091118_InitialOfficialDatabase'
)
BEGIN
    CREATE TABLE [SystemSettings] (
        [Id] int NOT NULL IDENTITY,
        [Key] nvarchar(100) NOT NULL,
        [Value] nvarchar(500) NOT NULL,
        CONSTRAINT [PK_SystemSettings] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630091118_InitialOfficialDatabase'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SystemSettings_Key] ON [SystemSettings] ([Key]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630091118_InitialOfficialDatabase'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260630091118_InitialOfficialDatabase', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630104213_AddAppUsers'
)
BEGIN
    CREATE TABLE [AppUsers] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(150) NOT NULL,
        [UserName] nvarchar(100) NOT NULL,
        [Password] nvarchar(200) NOT NULL,
        [Role] nvarchar(50) NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_AppUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630104213_AddAppUsers'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AppUsers_UserName] ON [AppUsers] ([UserName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260630104213_AddAppUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260630104213_AddAppUsers', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260701121158_AddRejectionReasonToDraftCustomers'
)
BEGIN
    ALTER TABLE [DraftCustomers] ADD [RejectionReason] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260701121158_AddRejectionReasonToDraftCustomers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260701121158_AddRejectionReasonToDraftCustomers', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702075708_AddProductsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702075708_AddProductsTable', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702075931_AddProductsTableChanges'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [ItemCode] nvarchar(50) NOT NULL,
        [ItemName] nvarchar(250) NOT NULL,
        [OutletSellingPrice] decimal(18,2) NULL,
        [RetailSellingPrice] decimal(18,2) NULL,
        [Currency] nvarchar(10) NULL,
        [MarketValidFrom] datetime2 NULL,
        [MarketValidTo] datetime2 NULL,
        [RetailValidFrom] datetime2 NULL,
        [RetailValidTo] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702075931_AddProductsTableChanges'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_ItemCode] ON [Products] ([ItemCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702075931_AddProductsTableChanges'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702075931_AddProductsTableChanges', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702084421_AddProductRequestsTable'
)
BEGIN
    CREATE TABLE [ProductRequests] (
        [Id] int NOT NULL IDENTITY,
        [ItemCode] nvarchar(50) NOT NULL,
        [ItemName] nvarchar(250) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [PurchasePrice] decimal(18,2) NOT NULL,
        [PiecesCount] int NOT NULL,
        [PiecePrice] decimal(18,2) NOT NULL,
        [HasBonus] bit NOT NULL,
        [BonusQuantity] int NULL,
        [OutletSellingPrice] decimal(18,2) NULL,
        [RetailSellingPrice] decimal(18,2) NULL,
        [Currency] nvarchar(10) NOT NULL,
        [Status] int NOT NULL,
        [RejectionReason] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [SubmittedAt] datetime2 NULL,
        [AccountsManagerReviewedAt] datetime2 NULL,
        [ExecutiveManagerReviewedAt] datetime2 NULL,
        [CreatedAsProductAt] datetime2 NULL,
        CONSTRAINT [PK_ProductRequests] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702084421_AddProductRequestsTable'
)
BEGIN
    CREATE INDEX [IX_ProductRequests_ItemCode] ON [ProductRequests] ([ItemCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702084421_AddProductRequestsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702084421_AddProductRequestsTable', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702102926_RenameProductPricesToRetailAndOutlet'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702102926_RenameProductPricesToRetailAndOutlet', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702103552_RenameProductsPricesToRetailAndOutlet'
)
BEGIN

    IF COL_LENGTH('Products', 'MarketSellingPrice') IS NOT NULL
       AND COL_LENGTH('Products', 'OutletSellingPrice') IS NULL
    BEGIN
        EXEC sp_rename 'Products.MarketSellingPrice', 'OutletSellingPrice', 'COLUMN';
    END

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702103552_RenameProductsPricesToRetailAndOutlet'
)
BEGIN

    IF COL_LENGTH('ProductRequests', 'MarketSellingPrice') IS NOT NULL
       AND COL_LENGTH('ProductRequests', 'OutletSellingPrice') IS NULL
    BEGIN
        EXEC sp_rename 'ProductRequests.MarketSellingPrice', 'OutletSellingPrice', 'COLUMN';
    END

END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702103552_RenameProductsPricesToRetailAndOutlet'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702103552_RenameProductsPricesToRetailAndOutlet', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702121709_AddActivityLogsTable'
)
BEGIN
    CREATE TABLE [ActivityLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserName] nvarchar(100) NULL,
        [UserRole] nvarchar(100) NULL,
        [Action] nvarchar(50) NOT NULL,
        [Module] nvarchar(100) NOT NULL,
        [EntityName] nvarchar(100) NOT NULL,
        [EntityId] int NULL,
        [Description] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_ActivityLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260702121709_AddActivityLogsTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260702121709_AddActivityLogsTable', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714064642_AddCustomerModificationRequests'
)
BEGIN
    CREATE TABLE [CustomerModificationRequests] (
        [Id] int NOT NULL IDENTITY,
        [BranchId] int NOT NULL,
        [BranchName] nvarchar(150) NOT NULL,
        [MarketCode] nvarchar(50) NOT NULL,
        [MarketName] nvarchar(250) NOT NULL,
        [CurrentCustomerType] int NOT NULL,
        [ModificationType] int NOT NULL,
        [NewMarketName] nvarchar(250) NULL,
        [Notes] nvarchar(500) NULL,
        [Status] int NOT NULL,
        [RejectionReason] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [SubmittedAt] datetime2 NULL,
        [ReviewedAt] datetime2 NULL,
        [CreatedBy] nvarchar(150) NULL,
        CONSTRAINT [PK_CustomerModificationRequests] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714064642_AddCustomerModificationRequests'
)
BEGIN
    CREATE INDEX [IX_CustomerModificationRequests_MarketCode] ON [CustomerModificationRequests] ([MarketCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714064642_AddCustomerModificationRequests'
)
BEGIN
    CREATE INDEX [IX_CustomerModificationRequests_Status] ON [CustomerModificationRequests] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714064642_AddCustomerModificationRequests'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260714064642_AddCustomerModificationRequests', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714115012_AddCreatedByToDraftCustomers'
)
BEGIN
    ALTER TABLE [DraftCustomers] ADD [CreatedBy] nvarchar(150) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714115012_AddCreatedByToDraftCustomers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260714115012_AddCreatedByToDraftCustomers', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE TABLE [SalesAnalyticsCustomers] (
        [Id] int NOT NULL IDENTITY,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [CustomerAccountGroup] nvarchar(50) NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [SalesDistrictCode] nvarchar(50) NULL,
        [SalesDistrictName] nvarchar(150) NULL,
        [CustomerClassificationCode] nvarchar(50) NULL,
        [CustomerClassificationName] nvarchar(150) NULL,
        [IncotermsCode] nvarchar(50) NULL,
        [IncotermsName] nvarchar(150) NULL,
        [SearchTerm] nvarchar(150) NULL,
        [SearchTerm2] nvarchar(150) NULL,
        [SourceCreatedDate] datetime2 NULL,
        [SourceCreatedBy] nvarchar(150) NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NULL,
        CONSTRAINT [PK_SalesAnalyticsCustomers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE TABLE [SalesAnalyticsSalesReps] (
        [Id] int NOT NULL IDENTITY,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [RegionCode] nvarchar(50) NULL,
        [RegionName] nvarchar(150) NULL,
        [InternalCode] nvarchar(100) NULL,
        [SearchTerm2] nvarchar(150) NULL,
        [SourceCreatedDate] datetime2 NULL,
        [SourceCreatedBy] nvarchar(150) NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NULL,
        CONSTRAINT [PK_SalesAnalyticsSalesReps] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE TABLE [SalesAnalyticsUploadBatches] (
        [Id] int NOT NULL IDENTITY,
        [FileType] int NOT NULL,
        [Status] int NOT NULL,
        [OriginalFileName] nvarchar(250) NOT NULL,
        [UploadDate] datetime2 NOT NULL DEFAULT (GETDATE()),
        [ReportDate] datetime2 NULL,
        [TotalRows] int NOT NULL,
        [ImportedRows] int NOT NULL,
        [FailedRows] int NOT NULL,
        [UploadedBy] nvarchar(150) NULL,
        [ErrorMessage] nvarchar(1000) NULL,
        CONSTRAINT [PK_SalesAnalyticsUploadBatches] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE TABLE [SalesRepRouteAssignments] (
        [Id] int NOT NULL IDENTITY,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [SalesDistrictCode] nvarchar(50) NOT NULL,
        [SalesDistrictName] nvarchar(150) NOT NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [IsActive] bit NOT NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NULL,
        CONSTRAINT [PK_SalesRepRouteAssignments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsCustomers_BranchCode] ON [SalesAnalyticsCustomers] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SalesAnalyticsCustomers_CustomerCode] ON [SalesAnalyticsCustomers] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsCustomers_SalesDistrictCode] ON [SalesAnalyticsCustomers] ([SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsCustomers_UploadBatchId] ON [SalesAnalyticsCustomers] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsSalesReps_BranchCode] ON [SalesAnalyticsSalesReps] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsSalesReps_InternalCode] ON [SalesAnalyticsSalesReps] ([InternalCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SalesAnalyticsSalesReps_SalesRepCode] ON [SalesAnalyticsSalesReps] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsSalesReps_UploadBatchId] ON [SalesAnalyticsSalesReps] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsUploadBatches_FileType] ON [SalesAnalyticsUploadBatches] ([FileType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsUploadBatches_ReportDate] ON [SalesAnalyticsUploadBatches] ([ReportDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsUploadBatches_UploadDate] ON [SalesAnalyticsUploadBatches] ([UploadDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_BranchCode] ON [SalesRepRouteAssignments] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_IsActive] ON [SalesRepRouteAssignments] ([IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_SalesDistrictCode] ON [SalesRepRouteAssignments] ([SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_SalesRepCode] ON [SalesRepRouteAssignments] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_UploadBatchId] ON [SalesRepRouteAssignments] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727164958_AddSalesAnalyticsFoundation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727164958_AddSalesAnalyticsFoundation', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727173131_AddSalesAnalyticsDailySalesReports'
)
BEGIN
    CREATE TABLE [SalesAnalyticsDailySalesReports] (
        [Id] int NOT NULL IDENTITY,
        [ReportDate] datetime2 NOT NULL,
        [LineCode] nvarchar(50) NOT NULL,
        [LineName] nvarchar(150) NOT NULL,
        [ProductName] nvarchar(250) NOT NULL,
        [Quantity] decimal(18,3) NOT NULL,
        [SalesAmount] decimal(18,2) NOT NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsDailySalesReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727173131_AddSalesAnalyticsDailySalesReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_LineCode] ON [SalesAnalyticsDailySalesReports] ([LineCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727173131_AddSalesAnalyticsDailySalesReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_ProductName] ON [SalesAnalyticsDailySalesReports] ([ProductName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727173131_AddSalesAnalyticsDailySalesReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_ReportDate] ON [SalesAnalyticsDailySalesReports] ([ReportDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727173131_AddSalesAnalyticsDailySalesReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_UploadBatchId] ON [SalesAnalyticsDailySalesReports] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727173131_AddSalesAnalyticsDailySalesReports'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727173131_AddSalesAnalyticsDailySalesReports', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727180759_AddSalesAnalyticsDailyVisitReports'
)
BEGIN
    CREATE TABLE [SalesAnalyticsDailyVisitReports] (
        [Id] int NOT NULL IDENTITY,
        [ReportDate] datetime2 NOT NULL,
        [SupervisorName] nvarchar(150) NULL,
        [CityName] nvarchar(150) NULL,
        [VisitCode] nvarchar(100) NULL,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [VisitStatus] nvarchar(100) NULL,
        [NegativeReason] nvarchar(250) NULL,
        [SuccessfulVisitValue] decimal(18,2) NOT NULL,
        [VisitStartTime] datetime2 NULL,
        [VisitEndTime] datetime2 NULL,
        [VisitDurationText] nvarchar(100) NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsDailyVisitReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727180759_AddSalesAnalyticsDailyVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_CustomerCode] ON [SalesAnalyticsDailyVisitReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727180759_AddSalesAnalyticsDailyVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_ReportDate] ON [SalesAnalyticsDailyVisitReports] ([ReportDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727180759_AddSalesAnalyticsDailyVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_SalesRepCode] ON [SalesAnalyticsDailyVisitReports] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727180759_AddSalesAnalyticsDailyVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_UploadBatchId] ON [SalesAnalyticsDailyVisitReports] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727180759_AddSalesAnalyticsDailyVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_VisitStatus] ON [SalesAnalyticsDailyVisitReports] ([VisitStatus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260727180759_AddSalesAnalyticsDailyVisitReports'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260727180759_AddSalesAnalyticsDailyVisitReports', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728073530_AddSalesDistrictMonthlyTargets'
)
BEGIN
    CREATE TABLE [SalesDistrictMonthlyTargets] (
        [Id] int NOT NULL IDENTITY,
        [Year] int NOT NULL,
        [Month] int NOT NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [SalesDistrictCode] nvarchar(50) NOT NULL,
        [SalesDistrictName] nvarchar(150) NOT NULL,
        [MonthlySalesTarget] decimal(18,2) NOT NULL,
        [PlannedVisits] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [CreatedBy] nvarchar(150) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(150) NULL,
        CONSTRAINT [PK_SalesDistrictMonthlyTargets] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728073530_AddSalesDistrictMonthlyTargets'
)
BEGIN
    CREATE INDEX [IX_SalesDistrictMonthlyTargets_BranchCode] ON [SalesDistrictMonthlyTargets] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728073530_AddSalesDistrictMonthlyTargets'
)
BEGIN
    CREATE INDEX [IX_SalesDistrictMonthlyTargets_SalesDistrictCode] ON [SalesDistrictMonthlyTargets] ([SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728073530_AddSalesDistrictMonthlyTargets'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SalesDistrictMonthlyTargets_Year_Month_SalesDistrictCode] ON [SalesDistrictMonthlyTargets] ([Year], [Month], [SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728073530_AddSalesDistrictMonthlyTargets'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260728073530_AddSalesDistrictMonthlyTargets', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728133155_AddSalesAnalyticsMtdSalesReport'
)
BEGIN
    CREATE TABLE [SalesAnalyticsMtdSalesReports] (
        [Id] int NOT NULL IDENTITY,
        [Year] int NOT NULL,
        [Month] int NOT NULL,
        [ToDate] datetime2 NOT NULL,
        [LineCode] nvarchar(50) NOT NULL,
        [LineName] nvarchar(250) NOT NULL,
        [ProductName] nvarchar(250) NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [SalesAmount] decimal(18,2) NOT NULL,
        [ImportedAt] datetime2 NOT NULL,
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsMtdSalesReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728133155_AddSalesAnalyticsMtdSalesReport'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_LineCode] ON [SalesAnalyticsMtdSalesReports] ([LineCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728133155_AddSalesAnalyticsMtdSalesReport'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_Year_Month_ToDate] ON [SalesAnalyticsMtdSalesReports] ([Year], [Month], [ToDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260728133155_AddSalesAnalyticsMtdSalesReport'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260728133155_AddSalesAnalyticsMtdSalesReport', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    EXEC sp_rename N'[SalesAnalyticsMtdSalesReports].[LineName]', N'CustomerName', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    EXEC sp_rename N'[SalesAnalyticsMtdSalesReports].[LineCode]', N'ProductCode', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    EXEC sp_rename N'[SalesAnalyticsMtdSalesReports].[IX_SalesAnalyticsMtdSalesReports_LineCode]', N'IX_SalesAnalyticsMtdSalesReports_ProductCode', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [CityCode] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [CityName] nvarchar(250) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [CustomerCode] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [DiscountAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [FromDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [TaxAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [TaxPercentage] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [TotalAfterTax] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [TotalBeforeTax] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsMtdSalesReports] ADD [Unit] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_CityCode] ON [SalesAnalyticsMtdSalesReports] ([CityCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_CustomerCode] ON [SalesAnalyticsMtdSalesReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729055455_UpdateMtdSalesReportForCustomerCity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729055455_UpdateMtdSalesReportForCustomerCity', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729063031_AddSalesAnalyticsMtdVisitReports'
)
BEGIN
    CREATE TABLE [SalesAnalyticsMtdVisitReports] (
        [Id] int NOT NULL IDENTITY,
        [Year] int NOT NULL,
        [Month] int NOT NULL,
        [FromDate] datetime2 NOT NULL,
        [ToDate] datetime2 NOT NULL,
        [VisitDate] datetime2 NULL,
        [SupervisorName] nvarchar(250) NULL,
        [CityName] nvarchar(250) NULL,
        [VisitCode] nvarchar(100) NULL,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [VisitStatus] nvarchar(150) NULL,
        [NegativeReason] nvarchar(500) NULL,
        [SuccessfulVisitValue] decimal(18,2) NOT NULL,
        [VisitStartTime] datetime2 NULL,
        [VisitEndTime] datetime2 NULL,
        [VisitDurationText] nvarchar(100) NULL,
        [ImportedAt] datetime2 NOT NULL,
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsMtdVisitReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729063031_AddSalesAnalyticsMtdVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_CustomerCode] ON [SalesAnalyticsMtdVisitReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729063031_AddSalesAnalyticsMtdVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_SalesRepCode] ON [SalesAnalyticsMtdVisitReports] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729063031_AddSalesAnalyticsMtdVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_VisitCode] ON [SalesAnalyticsMtdVisitReports] ([VisitCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729063031_AddSalesAnalyticsMtdVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_VisitDate] ON [SalesAnalyticsMtdVisitReports] ([VisitDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729063031_AddSalesAnalyticsMtdVisitReports'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_Year_Month_ToDate] ON [SalesAnalyticsMtdVisitReports] ([Year], [Month], [ToDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729063031_AddSalesAnalyticsMtdVisitReports'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729063031_AddSalesAnalyticsMtdVisitReports', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    DROP INDEX [IX_SalesAnalyticsDailySalesReports_ProductName] ON [SalesAnalyticsDailySalesReports];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    DROP INDEX [IX_SalesAnalyticsDailySalesReports_UploadBatchId] ON [SalesAnalyticsDailySalesReports];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SalesAnalyticsDailySalesReports]') AND [c].[name] = N'Quantity');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [SalesAnalyticsDailySalesReports] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [SalesAnalyticsDailySalesReports] ALTER COLUMN [Quantity] decimal(18,2) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SalesAnalyticsDailySalesReports]') AND [c].[name] = N'LineName');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [SalesAnalyticsDailySalesReports] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [SalesAnalyticsDailySalesReports] ALTER COLUMN [LineName] nvarchar(250) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SalesAnalyticsDailySalesReports]') AND [c].[name] = N'ImportedAt');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [SalesAnalyticsDailySalesReports] DROP CONSTRAINT [' + @var2 + '];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [CityCode] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [CityName] nvarchar(250) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [CustomerCode] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [CustomerName] nvarchar(250) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [DiscountAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [ProductCode] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [TaxAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [TaxPercentage] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [TotalAfterTax] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [TotalBeforeTax] decimal(18,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    ALTER TABLE [SalesAnalyticsDailySalesReports] ADD [Unit] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_CityCode] ON [SalesAnalyticsDailySalesReports] ([CityCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_CustomerCode] ON [SalesAnalyticsDailySalesReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_ProductCode] ON [SalesAnalyticsDailySalesReports] ([ProductCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074653_UpdateDailySalesReportForCustomerCity'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729074653_UpdateDailySalesReportForCustomerCity', N'9.0.17');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [ActivityLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserName] nvarchar(100) NULL,
        [UserRole] nvarchar(100) NULL,
        [Action] nvarchar(50) NOT NULL,
        [Module] nvarchar(100) NOT NULL,
        [EntityName] nvarchar(100) NOT NULL,
        [EntityId] int NULL,
        [Description] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        CONSTRAINT [PK_ActivityLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [CustomerModificationRequests] (
        [Id] int NOT NULL IDENTITY,
        [BranchId] int NOT NULL,
        [BranchName] nvarchar(150) NOT NULL,
        [MarketCode] nvarchar(50) NOT NULL,
        [MarketName] nvarchar(250) NOT NULL,
        [CurrentCustomerType] int NOT NULL,
        [ModificationType] int NOT NULL,
        [NewMarketName] nvarchar(250) NULL,
        [Notes] nvarchar(500) NULL,
        [Status] int NOT NULL,
        [RejectionReason] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [SubmittedAt] datetime2 NULL,
        [ReviewedAt] datetime2 NULL,
        [CreatedBy] nvarchar(150) NULL,
        CONSTRAINT [PK_CustomerModificationRequests] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [ProductRequests] (
        [Id] int NOT NULL IDENTITY,
        [ItemCode] nvarchar(50) NOT NULL,
        [ItemName] nvarchar(250) NOT NULL,
        [Notes] nvarchar(500) NULL,
        [PurchasePrice] decimal(18,2) NOT NULL,
        [PiecesCount] int NOT NULL,
        [PiecePrice] decimal(18,2) NOT NULL,
        [HasBonus] bit NOT NULL,
        [BonusQuantity] int NULL,
        [OutletSellingPrice] decimal(18,2) NULL,
        [RetailSellingPrice] decimal(18,2) NULL,
        [Currency] nvarchar(10) NOT NULL,
        [Status] int NOT NULL,
        [RejectionReason] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [SubmittedAt] datetime2 NULL,
        [AccountsManagerReviewedAt] datetime2 NULL,
        [ExecutiveManagerReviewedAt] datetime2 NULL,
        [CreatedAsProductAt] datetime2 NULL,
        CONSTRAINT [PK_ProductRequests] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [ItemCode] nvarchar(50) NOT NULL,
        [ItemName] nvarchar(250) NOT NULL,
        [OutletSellingPrice] decimal(18,2) NULL,
        [RetailSellingPrice] decimal(18,2) NULL,
        [Currency] nvarchar(10) NULL,
        [MarketValidFrom] datetime2 NULL,
        [MarketValidTo] datetime2 NULL,
        [RetailValidFrom] datetime2 NULL,
        [RetailValidTo] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesAnalyticsCustomers] (
        [Id] int NOT NULL IDENTITY,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [CustomerAccountGroup] nvarchar(50) NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [SalesDistrictCode] nvarchar(50) NULL,
        [SalesDistrictName] nvarchar(150) NULL,
        [CustomerClassificationCode] nvarchar(50) NULL,
        [CustomerClassificationName] nvarchar(150) NULL,
        [IncotermsCode] nvarchar(50) NULL,
        [IncotermsName] nvarchar(150) NULL,
        [SearchTerm] nvarchar(150) NULL,
        [SearchTerm2] nvarchar(150) NULL,
        [SourceCreatedDate] datetime2 NULL,
        [SourceCreatedBy] nvarchar(150) NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NULL,
        CONSTRAINT [PK_SalesAnalyticsCustomers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesAnalyticsDailySalesReports] (
        [Id] int NOT NULL IDENTITY,
        [ReportDate] datetime2 NOT NULL,
        [LineCode] nvarchar(50) NOT NULL,
        [LineName] nvarchar(250) NOT NULL,
        [CityCode] nvarchar(50) NULL,
        [CityName] nvarchar(250) NULL,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [ProductCode] nvarchar(50) NOT NULL,
        [ProductName] nvarchar(250) NOT NULL,
        [Unit] nvarchar(50) NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [SalesAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxPercentage] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [TotalBeforeTax] decimal(18,2) NOT NULL,
        [TotalAfterTax] decimal(18,2) NOT NULL,
        [ImportedAt] datetime2 NOT NULL,
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsDailySalesReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesAnalyticsDailyVisitReports] (
        [Id] int NOT NULL IDENTITY,
        [ReportDate] datetime2 NOT NULL,
        [SupervisorName] nvarchar(150) NULL,
        [CityName] nvarchar(150) NULL,
        [VisitCode] nvarchar(100) NULL,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [VisitStatus] nvarchar(100) NULL,
        [NegativeReason] nvarchar(250) NULL,
        [SuccessfulVisitValue] decimal(18,2) NOT NULL,
        [VisitStartTime] datetime2 NULL,
        [VisitEndTime] datetime2 NULL,
        [VisitDurationText] nvarchar(100) NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsDailyVisitReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesAnalyticsMtdSalesReports] (
        [Id] int NOT NULL IDENTITY,
        [Year] int NOT NULL,
        [Month] int NOT NULL,
        [FromDate] datetime2 NOT NULL,
        [ToDate] datetime2 NOT NULL,
        [CityCode] nvarchar(50) NULL,
        [CityName] nvarchar(250) NULL,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [ProductCode] nvarchar(50) NOT NULL,
        [ProductName] nvarchar(250) NOT NULL,
        [Unit] nvarchar(50) NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [SalesAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [TaxPercentage] decimal(18,2) NOT NULL,
        [TaxAmount] decimal(18,2) NOT NULL,
        [TotalBeforeTax] decimal(18,2) NOT NULL,
        [TotalAfterTax] decimal(18,2) NOT NULL,
        [ImportedAt] datetime2 NOT NULL,
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsMtdSalesReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesAnalyticsMtdVisitReports] (
        [Id] int NOT NULL IDENTITY,
        [Year] int NOT NULL,
        [Month] int NOT NULL,
        [FromDate] datetime2 NOT NULL,
        [ToDate] datetime2 NOT NULL,
        [VisitDate] datetime2 NULL,
        [SupervisorName] nvarchar(250) NULL,
        [CityName] nvarchar(250) NULL,
        [VisitCode] nvarchar(100) NULL,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [CustomerCode] nvarchar(50) NOT NULL,
        [CustomerName] nvarchar(250) NOT NULL,
        [VisitStatus] nvarchar(150) NULL,
        [NegativeReason] nvarchar(500) NULL,
        [SuccessfulVisitValue] decimal(18,2) NOT NULL,
        [VisitStartTime] datetime2 NULL,
        [VisitEndTime] datetime2 NULL,
        [VisitDurationText] nvarchar(100) NULL,
        [ImportedAt] datetime2 NOT NULL,
        [UploadBatchId] int NOT NULL,
        CONSTRAINT [PK_SalesAnalyticsMtdVisitReports] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesAnalyticsSalesReps] (
        [Id] int NOT NULL IDENTITY,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [RegionCode] nvarchar(50) NULL,
        [RegionName] nvarchar(150) NULL,
        [InternalCode] nvarchar(100) NULL,
        [SearchTerm2] nvarchar(150) NULL,
        [SourceCreatedDate] datetime2 NULL,
        [SourceCreatedBy] nvarchar(150) NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NULL,
        CONSTRAINT [PK_SalesAnalyticsSalesReps] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesAnalyticsUploadBatches] (
        [Id] int NOT NULL IDENTITY,
        [FileType] int NOT NULL,
        [Status] int NOT NULL,
        [OriginalFileName] nvarchar(250) NOT NULL,
        [UploadDate] datetime2 NOT NULL DEFAULT (GETDATE()),
        [ReportDate] datetime2 NULL,
        [TotalRows] int NOT NULL,
        [ImportedRows] int NOT NULL,
        [FailedRows] int NOT NULL,
        [UploadedBy] nvarchar(150) NULL,
        [ErrorMessage] nvarchar(1000) NULL,
        CONSTRAINT [PK_SalesAnalyticsUploadBatches] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesDistrictMonthlyTargets] (
        [Id] int NOT NULL IDENTITY,
        [Year] int NOT NULL,
        [Month] int NOT NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [SalesDistrictCode] nvarchar(50) NOT NULL,
        [SalesDistrictName] nvarchar(150) NOT NULL,
        [MonthlySalesTarget] decimal(18,2) NOT NULL,
        [PlannedVisits] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [CreatedBy] nvarchar(150) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(150) NULL,
        CONSTRAINT [PK_SalesDistrictMonthlyTargets] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE TABLE [SalesRepRouteAssignments] (
        [Id] int NOT NULL IDENTITY,
        [SalesRepCode] nvarchar(50) NOT NULL,
        [SalesRepName] nvarchar(250) NOT NULL,
        [SalesDistrictCode] nvarchar(50) NOT NULL,
        [SalesDistrictName] nvarchar(150) NOT NULL,
        [BranchCode] nvarchar(50) NULL,
        [BranchName] nvarchar(150) NULL,
        [IsActive] bit NOT NULL,
        [EffectiveFrom] datetime2 NULL,
        [EffectiveTo] datetime2 NULL,
        [ImportedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
        [UploadBatchId] int NULL,
        CONSTRAINT [PK_SalesRepRouteAssignments] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_CustomerModificationRequests_MarketCode] ON [CustomerModificationRequests] ([MarketCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_CustomerModificationRequests_Status] ON [CustomerModificationRequests] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_ProductRequests_ItemCode] ON [ProductRequests] ([ItemCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_ItemCode] ON [Products] ([ItemCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsCustomers_BranchCode] ON [SalesAnalyticsCustomers] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SalesAnalyticsCustomers_CustomerCode] ON [SalesAnalyticsCustomers] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsCustomers_SalesDistrictCode] ON [SalesAnalyticsCustomers] ([SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsCustomers_UploadBatchId] ON [SalesAnalyticsCustomers] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_CityCode] ON [SalesAnalyticsDailySalesReports] ([CityCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_CustomerCode] ON [SalesAnalyticsDailySalesReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_LineCode] ON [SalesAnalyticsDailySalesReports] ([LineCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_ProductCode] ON [SalesAnalyticsDailySalesReports] ([ProductCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailySalesReports_ReportDate] ON [SalesAnalyticsDailySalesReports] ([ReportDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_CustomerCode] ON [SalesAnalyticsDailyVisitReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_ReportDate] ON [SalesAnalyticsDailyVisitReports] ([ReportDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_SalesRepCode] ON [SalesAnalyticsDailyVisitReports] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_UploadBatchId] ON [SalesAnalyticsDailyVisitReports] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsDailyVisitReports_VisitStatus] ON [SalesAnalyticsDailyVisitReports] ([VisitStatus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_CityCode] ON [SalesAnalyticsMtdSalesReports] ([CityCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_CustomerCode] ON [SalesAnalyticsMtdSalesReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_ProductCode] ON [SalesAnalyticsMtdSalesReports] ([ProductCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdSalesReports_Year_Month_ToDate] ON [SalesAnalyticsMtdSalesReports] ([Year], [Month], [ToDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_CustomerCode] ON [SalesAnalyticsMtdVisitReports] ([CustomerCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_SalesRepCode] ON [SalesAnalyticsMtdVisitReports] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_VisitCode] ON [SalesAnalyticsMtdVisitReports] ([VisitCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_VisitDate] ON [SalesAnalyticsMtdVisitReports] ([VisitDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsMtdVisitReports_Year_Month_ToDate] ON [SalesAnalyticsMtdVisitReports] ([Year], [Month], [ToDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsSalesReps_BranchCode] ON [SalesAnalyticsSalesReps] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsSalesReps_InternalCode] ON [SalesAnalyticsSalesReps] ([InternalCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SalesAnalyticsSalesReps_SalesRepCode] ON [SalesAnalyticsSalesReps] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsSalesReps_UploadBatchId] ON [SalesAnalyticsSalesReps] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsUploadBatches_FileType] ON [SalesAnalyticsUploadBatches] ([FileType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsUploadBatches_ReportDate] ON [SalesAnalyticsUploadBatches] ([ReportDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesAnalyticsUploadBatches_UploadDate] ON [SalesAnalyticsUploadBatches] ([UploadDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesDistrictMonthlyTargets_BranchCode] ON [SalesDistrictMonthlyTargets] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesDistrictMonthlyTargets_SalesDistrictCode] ON [SalesDistrictMonthlyTargets] ([SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SalesDistrictMonthlyTargets_Year_Month_SalesDistrictCode] ON [SalesDistrictMonthlyTargets] ([Year], [Month], [SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_BranchCode] ON [SalesRepRouteAssignments] ([BranchCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_IsActive] ON [SalesRepRouteAssignments] ([IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_SalesDistrictCode] ON [SalesRepRouteAssignments] ([SalesDistrictCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_SalesRepCode] ON [SalesRepRouteAssignments] ([SalesRepCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    CREATE INDEX [IX_SalesRepRouteAssignments_UploadBatchId] ON [SalesRepRouteAssignments] ([UploadBatchId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260824084202_SalesAnalyticsV2Production'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260824084202_SalesAnalyticsV2Production', N'9.0.17');
END;

COMMIT;
GO

