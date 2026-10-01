using System;

namespace CoffeeNChill.Functions.Models
{
    /// Represents metadata for a single file stored in the staff-docs File Share.
    public class StaffDocumentInfo
    {
        public string FileName { get; set; } = default!;
        public long SizeInBytes { get; set; }
        public DateTimeOffset? LastModified { get; set; }
    }
}