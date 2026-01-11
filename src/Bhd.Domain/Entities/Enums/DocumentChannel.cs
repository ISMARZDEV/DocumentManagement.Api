namespace Bhd.Domain.Enums
{
    public enum DocumentChannel
    {
        Unknown = 0, // Desconocido / Not Specified
        Digital = 1, // App, Web Banking
        Backoffice = 2, // Sucursal, Sistema Interno
        ApiIntegration = 3 // Third-Party API
    }
}