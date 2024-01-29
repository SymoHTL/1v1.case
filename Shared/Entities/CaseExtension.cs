namespace Shared.Entities;

public static class CaseExtension {
    public static string SimulateOpening(this Case c) {
        var random = Random.Shared.NextSingle() * 100;

        var rarityName = random switch {
            <= Rarity.MilSpecChance => Rarity.MilSpec,
            <= Rarity.MilSpecChance + Rarity.RestrictedChance => Rarity.Restricted,
            <= Rarity.MilSpecChance + Rarity.RestrictedChance + Rarity.ClassifiedChance => Rarity.Classified,
            <= Rarity.MilSpecChance + Rarity.RestrictedChance + Rarity.ClassifiedChance + Rarity.CovertChance => Rarity.Covert,
            <= Rarity.MilSpecChance + Rarity.RestrictedChance + Rarity.ClassifiedChance + Rarity.CovertChance + Rarity.SpecialChance => Rarity.Special,
            _ => throw new Exception($"Rarity not found for {random}")
        };

        if (rarityName == Rarity.Special){
            var rareSkins = c.RareSkins.Where(s => s.Rarity.Id == Rarity.SpecialName).ToList();
            var rareSkin = rareSkins[new Random().Next(rareSkins.Count)];
            return rareSkin.Id;
        }

        var skins = c.Skins.Where(s => s.Rarity.Id == rarityName).ToList();
        var skin = skins[new Random().Next(skins.Count)];
        return skin.Id;
    }
}