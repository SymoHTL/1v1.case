namespace Shared.Entities;

public class AnimationCase(Player player, Item[] skins) {
   public Player Player { get; set; } = player;
   public Item[] Skins { get; set; } = skins;
}