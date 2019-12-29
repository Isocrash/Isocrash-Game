using System;
/// <summary>
/// 2D double precision vector.
/// </summary>
[Serializable]
public struct Vector2D// : IComparable, IFormattable, IComparable<Vector2D>, IEquatable<Vector2D>
{
    #region Definition
    /// <summary>
    /// X component of the vector.
    /// </summary>
    public double X
    {
        get { return x; }
        set { x = value; }
    }
    internal double x;
    /// <summary>
    /// Y component of the vector.
    /// </summary>
    public double Y
    {
        get { return y; }
        set { y = value; }
    }
    internal double y;

    /// <summary>
    /// 2D double precision vector.
    /// </summary>
    /// <param name="x">X component. (right)</param>
    /// <param name="y">Y component. (up)</param>
    public Vector2D(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    /// <summary>
    /// 2D double precision vector.
    /// </summary>
    /// <param name="x">X component. (right)</param>
    public Vector2D(double x)
    {
        this.x = x;
        this.y = 0;
    }

    /// <summary>
    /// [+X] The right direction. (1, 0)
    /// </summary>
    public static Vector2D Right
    {
        get
        {
            return new Vector2D(1D, 0D);
        }
    }
    /// <summary>
    /// [-X] The left direction. (-1, 0)
    /// </summary>
    public static Vector2D Left
    {
        get
        {
            return new Vector2D(-1D, 0D);
        }
    }
    /// <summary>
    /// [+Y] The up direction. (0, 1)
    /// </summary>
    public static Vector2D Up
    {
        get
        {
            return new Vector2D(0D, 1D);
        }
    }
    /// <summary>
    /// [-Y] The down direction. (0, -1)
    /// </summary>
    public static Vector2D Down
    {
        get
        {
            return new Vector2D(0D, -1D);
        }
    }

    /// <summary>
    /// Null vector. (0, 0, 0)
    /// </summary>
    public static Vector2D Null
    {
        get
        {
            return new Vector2D();
        }
    }
    /// <summary>
    /// [+X, +Y] All positive directions vector. (1, 1);
    /// </summary>
    public static Vector2D Positive
    {
        get
        {
            return new Vector2D(1D, 1D);
        }
    }
    /// <summary>
    /// [-X, -Y] All negative directions vector. (-1, -1);
    /// </summary>
    public static Vector2D Negative
    {
        get
        {
            return new Vector2D(-1D, -1D);
        }
    }

    /// <summary>
    /// Minimal vector value possible.
    /// </summary>
    public static Vector2D MinValue
    {
        get
        {
            return Positive * -1.7976931348623157E+308;
        }
    }
    /// <summary>
    /// Maximal vector value possible.
    /// </summary>
    public static Vector2D MaxValue
    {
        get
        {
            return Positive * 1.7976931348623157E+308;
        }
    }
    /// <summary>
    /// Smallest positive vector value possible.
    /// </summary>
    public static Vector2D Epsilon
    {
        get
        {
            return Positive * 4.94065645841247E-324;
        }
    }
    /// <summary>
    /// -Infinity vector.
    /// </summary>
    public static Vector2D NegativeInfinity
    {
        get
        {
            return Negative / 0D;
        }
    }
    /// <summary>
    /// +Infinity vector.
    /// </summary>
    public static Vector2D PositiveInfinity
    {
        get
        {
            return Positive / 0D;
        }
    }
    /// <summary>
    /// Not a Number vector.
    /// </summary>
    public static Vector2D NaN
    {
        get
        {
            return Vector2D.Null / 0D;
        }
    }

    /// <summary>
    /// Checks if at least one of the values of the vector is infinite.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsInfinity(Vector2D v)
    {
        return double.IsInfinity(v.x) || double.IsInfinity(v.y);
    }
    /// <summary>
    /// Checks if at least one of the values of the vector is Not a Number.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsNaN(Vector2D v)
    {
        return double.IsNaN(v.x) || double.IsNaN(v.y);
    }
    /// <summary>
    /// Checks if at least one of the values of the vector is a -Infinity.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsNegativeInfinity(Vector2D v)
    {
        return double.IsNegativeInfinity(v.x) || double.IsNegativeInfinity(v.y);
    }
    /// <summary>
    /// Checks if at least one of the values of the vector is a +Infinity.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsPositiveInfinity(Vector2D v)
    {
        return double.IsPositiveInfinity(v.x) || double.IsPositiveInfinity(v.y);
    }

    /// <summary>
    /// Get the angle in degree between two vectors.
    /// </summary>
    public static double Angle(Vector2D v1, Vector2D v2)
    {
        return Math.Acos(Scalar(v1, v2) / (v1.Length * v2.Length)) * 180D / Math.PI;
    }

    /// <summary>
    /// Get the scalar (dot product) between two vectors.
    /// </summary>
    public static double Scalar(Vector2D u, Vector2D v)
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
            double l = Length;
            return new Vector2D(x / l, y / l);
        }
    }
    /// <summary>
    /// Transforms the vector to its direction.
    /// </summary>
    /// <returns>Returns itself.</returns>
    public Vector2D Normalize()
    {
        if (this == Vector2D.Null)
        {
            return Vector2D.Null;
        }

        if (this != Vector2D.NaN)
            this = Direction;

        return this;
    }
    #endregion

    #region Interface Implementations
    public int CompareTo(object value)
    {
        Vector2D? vvalue = (Vector2D)value;

        if (vvalue != null)
        {
            return CompareTo(vvalue.Value);
        }

        return 1;
    }
    /// <summary>
    /// Compare to another 3D vector by length
    /// </summary>
    public int CompareTo(Vector2D value)
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
    public bool Equals(Vector2D o)
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
        Vector2D? o = (Vector2D)obj;

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
    public static bool operator ==(Vector2D v1, Vector2D v2)
    {
        return v1.x == v2.x && v1.y == v2.y;
    }
    public static bool operator !=(Vector2D v1, Vector2D v2)
    {
        return v1.x != v2.x || v1.y != v2.y;
    }
    public static Vector2D operator +(Vector2D v, double n)
    {
        return new Vector2D(v.x + n, v.y + n);
    }
    public static Vector2D operator +(Vector2D v1, Vector2D v2)
    {
        return new Vector2D(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2D operator -(Vector2D v, double n)
    {
        return new Vector2D(v.x - n, v.y - n);
    }
    public static Vector2D operator -(Vector2D v)
    {
        return v *= -1;
    }
    public static Vector2D operator -(Vector2D v1, Vector2D v2)
    {
        return new Vector2D(v1.x - v2.x, v1.y - v2.y);
    }

    public static Vector2D operator *(Vector2D v, double n)
    {
        return new Vector2D(v.x * n, v.y * n);
    }
    public static Vector2D operator *(Vector2D v1, Vector2D v2)
    {
        return new Vector2D(v1.x * v2.x, v1.y * v2.y);
    }

    public static Vector2D operator /(Vector2D v, double n)
    {
        return new Vector2D(v.x / n, v.y / n);
    }
    public static Vector2D operator /(Vector2D v1, Vector2D v2)
    {
        return new Vector2D(v1.x / v2.x, v1.y / v2.y);
    }
    #endregion
}