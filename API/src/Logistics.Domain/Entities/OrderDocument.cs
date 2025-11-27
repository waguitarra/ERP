using Logistics.Domain.Enums;

namespace Logistics.Domain.Entities;

public class OrderDocument
{
    private OrderDocument() { } // EF Core

    public OrderDocument(Guid orderId, string fileName, DocumentType type, Guid uploadedBy)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId inválido");
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("FileName inválido");

        Id = Guid.NewGuid();
        OrderId = orderId;
        FileName = fileName;
        Type = type;
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public DocumentType Type { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public string MimeType { get; private set; } = "image/webp";
    public Guid UploadedBy { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    // Navigation
    public Order Order { get; private set; } = null!;

    public void SetFilePath(string filePath, string fileUrl, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("FilePath não pode ser vazio");

        FilePath = filePath;
        FileUrl = fileUrl;
        FileSizeBytes = sizeBytes;
    }

    public void SoftDelete(Guid deletedBy)
    {
        if (deletedBy == Guid.Empty)
            throw new ArgumentException("DeletedBy inválido");

        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
