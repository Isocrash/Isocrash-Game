using System;
/// <summary>
/// 2D integer vector.
/// </summary>
[Serializable]
public struct Vector2I// : IComparable, IFormattable, IComparable<Vector2I>, IEquatable<Vector2I>
{
    #region Definition
    /// <summary>
    /// X component of the vector.
    /// </summary>
    public int X
    {
        get { return x; }
        set { x = value; }
    }
    internal int x;
    /// <summary>
    /// Y component of the vector.
    /// </summary>
    public int Y
    {
        get { return y; }
        set { y = value; }
    }
    internal int y;

    /// <summary>
    /// 2D integer vector.
    /// </summary>
    /// <param name="x">X component. (right)</param>
    /// <param name="y">Y component. (up)</param>
    public Vector2I(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    /// <summary>
    /// 2D integer vector.
    /// </summary>
    /// <param name="x">X component. (right)</param>
    public Vector2I(int x)
    {
        this.x = x;
        this.y = 0;
    }

    /// <summary>
    /// [+X] The right direction. (1, 0)
    /// </summary>
    public static Vector2I Right
    {
        get
        {
            return new Vector2I(1, 0);
        }
    }
    /// <summary>
    /// [-X] The left direction. (-1, 0)
    /// </summary>
    public static Vector2I Left
    {
        get
        {
            return new Vector2I(-1, 0);
        }
    }
    /// <summary>
    /// [+Y] The up direction. (0, 1)
    /// </summary>
    public static Vector2I Up
    {
        get
        {
            return new Vector2I(0, 1);
        }
    }
    /// <summary>
    /// [-Y] The down direction. (0, -1)
    /// </summary>
    public static Vector2I Down
    {
        get
        {
            return new Vector2I(0, -1);
        }
    }

    /// <summary>
    /// Null vector. (0, 0)
    /// </summary>
    public static Vector2I Null
    {
        get
        {
            return new Vector2I();
        }
    }
    /// <summary>
    /// [+X, +Y] All positive directions vector. (1, 1);
    /// </summary>
    public static Vector2I Positive
    {
        get
        {
            return new Vector2I(1, 1);
        }
    }
    /// <summary>
    /// [-X, -Y] All negative directions vector. (-1, -1);
    /// </summary>
    public static Vector2I Negative
    {
        get
        {
            return new Vector2I(-1, -1);
        }
    }

    /// <summary>
    /// Minimal vector value possible.
    /// </summary>
    public static Vector2I MinValue
    {
        get
        {
            return new Vector2I(int.MinValue, int.MinValue);
        }
    }
    /// <summary>
    /// Maximal vector value possible.
    /// </summary>
    public static Vector2I MaxValue
    {
        get
        {
            return new Vector2I(int.MaxValue, int.MaxValue);
        }
    }

    /// <summary>
    /// Get the angle in degree between two vectors.
    /// </summary>
    public static double Angle(Vector2I v1, Vector2I v2)
    {
        return Vector2D.Angle(v1, v2);
    }

    /// <summary>
    /// Get the scalar (dot product) between two vectors.
    /// </summary>
    public static double Scalar(Vector2I u, Vector2I v)
    {
        return u.x * v.x + u.y * v.y;
    }

    /// <summary>
    /// The length (magnitude) of the vector.
    /// </summary>
    public double Length
    {
        get
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
    /// <summary>
    /// The direction of the vector. (normalized vector)
    /// </summary>
    public Vector2D Direction
    {
        get
        {
            return new Vector2D(x, y).Direction;
        }
    }
    #endregion

    #region Interface Implementations
    public int CompareTo(object value)
    {
        Vector2I? vvalue = (Vector2I)value;

        if (vvalue != null)
        {
            return CompareTo(vvalue.Value);
        }

        return 1;
    }
    /// <summary>
    /// Compare to another 3D vector by length
    /// </summary>
    public int CompareTo(Vector2I value)
    {
        double l1 = this.Length, l2 = value.Length;

        if (l1 > l2)
        {
            return 1;
        }
        if (l1 < l2)
        {
            return -1;
        }

        return 0;
    }
    public bool Equals(Vector2I o)
    {
        if (x == o.x && y == o.y)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Inherited Methods
    public override bool Equals(object obj)
    {
        Vector2I? o = (Vector2I)obj;

        if (o != null)
        {
            return Equals(o.Value);
        }

        return false;
    }
    public override string ToString()
    {
        return $"({x.ToString()}, {y.ToString()})";
    }
    public override int GetHashCode()
    {
        return string.Format("{0}--{1}", x, y).GetHashCode();
    }

    public string ToString(IFormatProvider provider)
    {
        return $"({x.ToString(provider)}, {y.ToString(provider)})";
    }
    public string ToString(string format)
    {
        return $"({x.ToString(format)}, {y.ToString(format)})";
    }
    public string ToString(string format, IFormatProvider provider)
    {
        return $"({x.ToString(format, provider)}, {y.ToString(format, provider)})";
    }
    #endregion

    #region Operators
    public static bool operator ==(Vector2I v1, Vector2I v2)
    {
        return v1.x == v2.x && v1.y == v2.y;
    }
    public static bool operator !=(Vector2I v1, Vector2I v2)
    {
        return v1.x != v2.x || v1.y != v2.y;
    }
    public static Vector2I operator +(Vector2I v, int n)
    {
        return new Vector2I(v.x + n, v.y + n);
    }
    public static Vector2I operator +(Vector2I v1, Vector2I v2)
    {
        return new Vector2I(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2I operator -(Vector2I v, int n)
    {
        return new Vector2I(v.x - n, v.y - n);
    }
    public static Vector2I operator -(Vector2I v)
    {
        return v *= -1;
    }
    public static Vector2I operator -(Vector2I v1, Vector2I v2)
    {
        return new Vector2I(v1.x - v2.x, v1.y - v2.y);
    }

    public static Vector2I operator *(Vector2I v, int n)
    {
        return new Vector2I(v.x * n, v.y * n);
    }
    public static Vector2I operator *(Vector2I v1, Vector2I v2)
    {
        return new Vector2I(v1.x * v2.x, v1.y * v2.y);
    }

    public static Vector2I operator /(Vector2I v, int n)
    {
        return new Vector2I(v.x / n, v.y / n);
    }
    public static Vector2I operator /(Vector2I v1, Vector2I v2)
    {
        return new Vector2I(v1.x / v2.x, v1.y / v2.y);
    }

    public static implicit operator Vector2I(Vector2D v)
    {
        return new Vector2I((int)v.x, (int)v.y);
    }

    public static implicit operator Vector2D(Vector2I v)
    {
        return new Vector2D(v.x, v.y);
    }
    #endregion
}
