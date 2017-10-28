using UnityEngine;

public struct IntVector2
{
    public int x;
    public int z;

    public IntVector2(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public void set(Vector3 values)
    {
        x = Mathf.FloorToInt(values.x);
        z = Mathf.FloorToInt(values.z);
    }

    public bool equals(object obj)
    {
        if (!(obj is IntVector2))
            return false;

        IntVector2 v = (IntVector2)obj;
        return (x == v.x) && (z == v.z);
    }

    public static IntVector2 operator +(IntVector2 v, int a)
    {
        return new IntVector2(v.x + a, v.z + a);
    }
}
