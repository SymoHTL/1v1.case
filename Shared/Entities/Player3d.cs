
namespace Shared.Entities;

public class Player3d {
    public string Name { get; set; } = null!;
    public string ConnId { get; set; } = null!;

    public Vector3 Pos { get; set; }  = Vector3.Zero;

}