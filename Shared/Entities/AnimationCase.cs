namespace Shared.Entities;

public class AnimationCase(string playerName, Item[] skins) {
   public string PlayerName { get; set; } = playerName;
   public Item[] Skins { get; set; } = skins;
}