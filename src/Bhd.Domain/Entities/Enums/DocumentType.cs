
namespace Bhd.Domain.Enums
{
    public enum DocumentType
    {
        Unknown = 0,

        // Identity
        NationalId = 1,
        Passport = 2,
        DrivingLicense = 3,
        TaxId = 4,

        // Business / Legal
        Contract = 10,
        Invoice = 11,

        // Generic
        SupportingDocument = 20,
        Other = 99
    }
}