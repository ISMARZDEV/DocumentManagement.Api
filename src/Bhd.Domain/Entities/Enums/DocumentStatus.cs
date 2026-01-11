namespace Bhd.Domain.Enums;

public enum DocumentStatus
{
    Received = 1, // Received, Queued
    Processing = 2, // Processing, In Progress
    Processed = 3, // Ready / Completed
    Failed = 4 // Error
}