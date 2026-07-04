namespace MasterDataAutomation.Application.Modules.ProductMaster.Enums;

public enum ProductRequestStatus
{
    Draft = 1,

    SubmittedToAccountsManager = 2,
    RejectedByAccountsManager = 3,
    ApprovedByAccountsManager = 4,

    SubmittedToExecutiveManager = 5,
    RejectedByExecutiveManager = 6,

    ReadyForFinalCreation = 7,
    CreatedAsProduct = 8
}