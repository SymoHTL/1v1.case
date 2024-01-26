namespace Shared.Entities;

public record Item(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("rarity")] Rarity Rarity,
    [property: JsonPropertyName("paint_index")]
    string PaintIndex,
    [property: JsonPropertyName("image")] string Image) {
    public static Item FromRareItem(RareItem rareItem) {
        return new Item(rareItem.Id, rareItem.Name, rareItem.Rarity, rareItem.PaintIndex, rareItem.Image);
    }
}

public record RareItem(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("rarity")] Rarity Rarity,
    [property: JsonPropertyName("paint_index")]
    string PaintIndex,
    [property: JsonPropertyName("phase")] object Phase,
    [property: JsonPropertyName("image")] string Image);

public record Rarity(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("color")] string Color) {
    public const string MilSpec = "Mil-Spec Grade";
    public const float MilSpecChance = 79.92f;
    public const string Restricted = "Restricted";
    public const float RestrictedChance = 15.98f;
    public const string Classified = "Classified";
    public const float ClassifiedChance = 3.2f;
    public const string Covert = "Covert";
    public const float CovertChance = 0.64f;
    public const string Special = "Extraordinary";
    public const float SpecialChance = 0.26f;
}

public record Case(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")]
    string Description,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("first_sale_date")]
    string FirstSaleDate,
    [property: JsonPropertyName("contains")]
    IReadOnlyList<Item> Skins,
    [property: JsonPropertyName("contains_rare")]
    IReadOnlyList<RareItem> RareSkins,
    [property: JsonPropertyName("image")] string Image);