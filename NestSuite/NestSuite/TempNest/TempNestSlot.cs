using System.Text.Json.Serialization;

namespace NestSuite.TempNest;

public class TempNestSlot
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
}

public class TempNestStoreData
{
    public int Version { get; set; } = 1;
    public List<TempNestSlot> Slots { get; set; } = new();
}
