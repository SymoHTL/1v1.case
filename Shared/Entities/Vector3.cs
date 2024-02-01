namespace Shared.Entities;

public class Vector3 {
    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public Vector3() { }

    public Vector3(float z, float y, float x) {
        Z = z;
        Y = y;
        X = x;
    }

    [field: NonSerialized]
    public static Vector3 Zero { get; } = new(0, 0, 0);


    public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

    public Vector3 Normalized => this / Magnitude;

    public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.Z + b.Z, a.Y + b.Y, a.X + b.X);

    public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.Z - b.Z, a.Y - b.Y, a.X - b.X);

    public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.Z * b, a.Y * b, a.X * b);

    public static Vector3 operator /(Vector3 a, float b) => new Vector3(a.Z / b, a.Y / b, a.X / b);
}