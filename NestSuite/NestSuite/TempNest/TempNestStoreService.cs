using System.IO;
using System.Text.Json;
using NestSuite.Services;

namespace NestSuite.TempNest;

public static class TempNestStoreService
{
    private static readonly string DataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                     "NoteNest", "tempnest.json");

    private static readonly JsonSerializerOptions JsonOpts =
        new() { WriteIndented = false };

    public static TempNestSlot[] Load()
    {
        try
        {
            if (!File.Exists(DataPath)) return CreateEmptySlots();
            var data = JsonSerializer.Deserialize<TempNestStoreData>(
                           File.ReadAllText(DataPath), JsonOpts);
            if (data == null) return CreateEmptySlots();
            var slots = new TempNestSlot[4];
            for (int i = 0; i < 4; i++)
                slots[i] = i < data.Slots.Count ? data.Slots[i] : new TempNestSlot();
            return slots;
        }
        catch (Exception ex)
        {
            ErrorLogService.Log("TempNestLoad", ex, "TempNest", DataPath);
            return CreateEmptySlots();
        }
    }

    public static void Save(TempNestSlot[] slots)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DataPath)!);
            var data = new TempNestStoreData
            {
                Version = 1,
                Slots   = slots.ToList(),
            };
            File.WriteAllText(DataPath, JsonSerializer.Serialize(data, JsonOpts));
        }
        catch (Exception ex)
        {
            ErrorLogService.Log("TempNestSave", ex, "TempNest", DataPath);
        }
    }

    private static TempNestSlot[] CreateEmptySlots()
        => Enumerable.Range(0, 4).Select(_ => new TempNestSlot()).ToArray();
}
