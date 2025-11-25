namespace Logistics.Domain.Entities;

public class OrderStatusConfig
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string NamePT { get; private set; } = string.Empty;
    public string NameEN { get; private set; } = string.Empty;
    public string NameES { get; private set; } = string.Empty;
    public string? DescriptionPT { get; private set; }
    public string? DescriptionEN { get; private set; }
    public string? DescriptionES { get; private set; }
    public string ColorHex { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Construtor privado para EF Core
    private OrderStatusConfig() { }

    public OrderStatusConfig(
        int id, 
        string code, 
        string namePT, 
        string nameEN, 
        string nameES, 
        string colorHex, 
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code é obrigatório", nameof(code));
        if (string.IsNullOrWhiteSpace(namePT))
            throw new ArgumentException("NamePT é obrigatório", nameof(namePT));
        if (string.IsNullOrWhiteSpace(nameEN))
            throw new ArgumentException("NameEN é obrigatório", nameof(nameEN));
        if (string.IsNullOrWhiteSpace(nameES))
            throw new ArgumentException("NameES é obrigatório", nameof(nameES));
        if (string.IsNullOrWhiteSpace(colorHex))
            throw new ArgumentException("ColorHex é obrigatório", nameof(colorHex));

        Id = id;
        Code = code;
        NamePT = namePT;
        NameEN = nameEN;
        NameES = nameES;
        ColorHex = colorHex;
        SortOrder = sortOrder;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetDescriptions(string? descriptionPT, string? descriptionEN, string? descriptionES)
    {
        DescriptionPT = descriptionPT;
        DescriptionEN = descriptionEN;
        DescriptionES = descriptionES;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
