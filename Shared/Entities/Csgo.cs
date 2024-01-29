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
    public const string MilSpec = "rarity_rare_weapon";
    public const float MilSpecChance = 79.92f;
    public const string Restricted = "rarity_mythical_weapon";
    public const float RestrictedChance = 15.98f;
    public const string Classified = "rarity_legendary_weapon";
    public const float ClassifiedChance = 3.2f;
    public const string Covert = "rarity_ancient_weapon";
    public const float CovertChance = 0.64f;
    public const string Special = "Special";
    public const string SpecialName = "rarity_ancient_weapon";
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