using System;
using Raymarcher;
using System.Numerics;

/// <summary>
/// 3D double precision vector.
/// </summary>
[Serializable]
public struct Vector3D// : IComparable, IFormattable, IComparable<Vector3D>, IEquatable<Vector3D>
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
    /// Z component of the vector.
    /// </summary>
    public double Z
    {
        get { return z; }
        set { z = value; }
    }
    internal double z;

    /// <summary>
    /// 3D double precision vector.
    /// </summary>
    /// <param name="x">X component. (right)</param>
    /// <param name="y">Y component. (up)</param>
    /// <param name="z">Z component. (forward)</param>
    public Vector3D(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    /// <summary>
    /// 3D double precision vector.
    /// </summary>
    /// <param name="x">X component. (right)</param>
    /// <param name="y">Y component. (up)</param>
    public Vector3D(double x, double y)
    {
        this.x = x;
        this.y = y;
        this.z = 0;
    }
    /// <summary>
    /// 3D double precision vector.
    /// </summary>
    /// <param name="x">X component. (right)</param>
    public Vector3D(double x)
    {
        this.x = x;
        this.y = this.z = 0;
    }

    /// <summary>
    /// [+X] The right direction. (1, 0, 0)
    /// </summary>
    public static Vector3D Right
    {
        get
        {
            return new Vector3D(1D, 0D, 0D);
        }
    }
    /// <summary>
    /// [-X] The left direction. (-1, 0, 0)
    /// </summary>
    public static Vector3D Left
    {
        get
        {
            return new Vector3D(-1D, 0D, 0D);
        }
    }
    /// <summary>
    /// [+Y] The up direction. (0, 1, 0)
    /// </summary>
    public static Vector3D Up
    {
        get
        {
            return new Vector3D(0D, 1D, 0D);
        }
    }
    /// <summary>
    /// [-Y] The down direction. (0, -1, 0)
    /// </summary>
    public static Vector3D Down
    {
        get
        {
            return new Vector3D(0D, -1D, 0D);
        }
    }
    /// <summary>
    /// [+Z] The forward direction. (0, 0, 1)
    /// </summary>
    public static Vector3D Forward
    {
        get
        {
            return new Vector3D(0D, 0D, 1D);
        }
    }
    /// <summary>
    /// [-Z] The backward direction. (0, 0, -1)
    /// </summary>
    public static Vector3D Backward
    {
        get
        {
            return new Vector3D(0D, 0D, -1D);
        }
    }

    /// <summary>
    /// Null vector. (0, 0, 0)
    /// </summary>
    public static Vector3D Null
    {
        get
        {
            return new Vector3D();
        }
    }
    /// <summary>
    /// [+X, +Y, +Z] All positive directions vector. (1, 1, 1);
    /// </summary>
    public static Vector3D Positive
    {
        get
        {
            return new Vector3D(1D, 1D, 1D);
        }
    }
    /// <summary>
    /// [-X, -Y, -Z] All negative directions vector. (-1, -1, -1);
    /// </summary>
    public static Vector3D Negative
    {
        get
        {
            return new Vector3D(-1D, -1D, -1D);
        }
    }

    /// <summary>
    /// Minimal vector value possible.
    /// </summary>
    public static Vector3D MinValue
    {
        get
        {
            return Positive * -1.7976931348623157E+308;
        }
    }
    /// <summary>
    /// Maximal vector value possible.
    /// </summary>
    public static Vector3D MaxValue
    {
        get
        {
            return Positive * 1.7976931348623157E+308;
        }
    }
    /// <summary>
    /// Smallest positive vector value possible.
    /// </summary>
    public static Vector3D Epsilon
    {
        get
        {
            return Positive * 4.94065645841247E-324;
        }
    }
    /// <summary>
    /// -Infinity vector.
    /// </summary>
    public static Vector3D NegativeInfinity
    {
        get
        {
            return Negative / 0D;
        }
    }
    /// <summary>
    /// +Infinity vector.
    /// </summary>
    public static Vector3D PositiveInfinity
    {
        get
        {
            return Positive / 0D;
        }
    }
    /// <summary>
    /// Not a Number vector.
    /// </summary>
    public static Vector3D NaN
    {
        get
        {
            return Vector3D.Null / 0D;
        }
    }

    /// <summary>
    /// Checks if at least one of the values of the vector is infinite.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsInfinity(Vector3D v)
    {
        return double.IsInfinity(v.x) || double.IsInfinity(v.y) || double.IsInfinity(v.z);
    }
    /// <summary>
    /// Checks if at least one of the values of the vector is Not a Number.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsNaN(Vector3D v)
    {
        return double.IsNaN(v.x) || double.IsNaN(v.y) || double.IsNaN(v.z);
    }
    /// <summary>
    /// Checks if at least one of the values of the vector is a -Infinity.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsNegativeInfinity(Vector3D v)
    {
        return double.IsNegativeInfinity(v.x) || double.IsNegativeInfinity(v.y) || double.IsNegativeInfinity(v.z);
    }
    /// <summary>
    /// Checks if at least one of the values of the vector is a +Infinity.
    /// </summary>
    /// <param name="v">The checked vector.</param>
    public static bool IsPositiveInfinity(Vector3D v)
    {
        return double.IsPositiveInfinity(v.x) || double.IsPositiveInfinity(v.y) || double.IsPositiveInfinity(v.z);
    }

    /// <summary>
    /// Get the squared distance between two points
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static double DistanceSquared(Vector3D v1, Vector3D v2)
    {
        return (v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y) + (v2.z - v1.z) * (v2.z - v1.z);
    }

    /// <summary>
    /// Get the distance between two points
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static double Distance(Vector3D v1, Vector3D v2)
    {
        return Math.Sqrt(DistanceSquared(v1, v2));

    }

    /// <summary>
    /// Get the angle in degree between two vectors.
    /// </summary>
    [Hybridizer.Runtime.CUDAImports.Kernel]
    public static double Angle(Vector3D v1, Vector3D v2)
    {
        return Math.Acos(Scalar(v1, v2) / (v1.Length * v2.Length)) * 180D / Math.PI;
    }

    /// <summary>
    /// Get the scalar (dot product) between two vectors.
    /// </summary>
    public static double Scalar(Vector3D u, Vector3D v)
    {
        return u.x * v.x + u.y * v.y + u.z * v.z;
    }

    /// <summary>
    /// The length (magnitude) of the vector.
    /// </summary>
    public double Length
    {
        get
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
    }
    /// <summary>
    /// The direction of the vector. (normalized vector)
    /// </summary>
    public Vector3D Direction
    {
        get
        {
            double l = Length;
            return new Vector3D(x / l, y / l, z / l);
        }
    }

    /// <summary>
    /// Transforms the vector to its direction.
    /// </summary>
    /// <returns>Returns itself.</returns>
    [Hybridizer.Runtime.CUDAImports.Kernel]
    public Vector3D Normalize()
    {
        if (this == Vector3D.Null)
        {
            return Vector3D.Null;
        }

        if (this != Vector3D.NaN)
            this = Direction;

        return this;
    }

    public static Vector3D RotateAroundZ(Vector3D dir, double angle)
    {
        double a = angle * 0.0174533D;

        double rx = dir.x * Math.Cos(a) - dir.y * Math.Sin(a);
        double ry = dir.x * Math.Sin(a) + dir.y * Math.Cos(a);
        double rz = dir.z;

        return new Vector3D(rx, ry, rz);
    }

    public static Vector3D RotateAroundY(Vector3D dir, double angle)
    {
        double a = angle * 0.0174533D;

        double rx = dir.x * Math.Cos(a) + dir.z * Math.Sin(a);
        double ry = dir.y;
        double rz = -dir.x * Math.Sin(a) + dir.z * Math.Cos(a);

        return new Vector3D(rx, ry, rz);
    }

    public static Vector3D RotateAroundX(Vector3D dir, double angle)
    {
        double a = angle * 0.0174533D;

        double rx = dir.x;
        double ry = dir.y * Math.Cos(a) - dir.z * Math.Sin(a);
        double rz = dir.y * Math.Sin(a) + dir.z * Math.Cos(a);

        return new Vector3D(rx, ry, rz);
    }

    /// <summary>
    /// Get the biggest vector
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    [Hybridizer.Runtime.CUDAImports.Kernel]
    public static Vector3D Max(Vector3D a, Vector3D b)
    {
        return a.Length > b.Length ? a : b;
    }
    /// <summary>
    /// Get the smallest vector
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    [Hybridizer.Runtime.CUDAImports.Kernel]
    public static Vector3D Min(Vector3D a, Vector3D b)
    {
        return a.Length < b.Length ? a : b;
    }
    /// <summary>
    /// Get the absolute value of a vector
    /// </summary>
    /// <param name="v">The vector</param>
    [Hybridizer.Runtime.CUDAImports.Kernel]
    public static Vector3D Abs(Vector3D v)
    {
        return new Vector3D(Hybridizer.Runtime.CUDAImports.HybMath.Abs((float)v.x), Hybridizer.Runtime.CUDAImports.HybMath.Abs((float)v.y), Hybridizer.Runtime.CUDAImports.HybMath.Abs((float)v.z));
    }

    public static Vector3D Round(Vector3D v, int decimals)
    {
        return new Vector3D(Math.Round(v.x, decimals), Math.Round(v.y, decimals), Math.Round(v.z, decimals));
    }

    [Hybridizer.Runtime.CUDAImports.Kernel]
    public static Vector3D RotatePointAroundPivot(Vector3D point, Vector3D pivot, Vector3D angles)
    {
        Vector3D dir = point - pivot; // get point direction relative to pivot
        dir = EQuaternion.FromEuler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }


    #endregion

    #region Interface Implementations
    public int CompareTo(object value)
    {
        Vector3D? vvalue = (Vector3D)value;

        if (vvalue != null)
        {
            return CompareTo(vvalue.Value);
        }

        return 1;
    }
    /// <summary>
    /// Compare to another 3D vector by length
    /// </summary>
    public int CompareTo(Vector3D value)
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
    public bool Equals(Vector3D o)
    {
        if (x == o.x && y == o.y && z == o.z)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Inherited Methods
    public override bool Equals(object obj)
    {
        Vector3D? o = (Vector3D)obj;

        if (o != null)
        {
            return Equals(o.Value);
        }

        return false;
    }
    public override string ToString()
    {
        return $"({x.ToString()}, {y.ToString()}, {z.ToString()})";
    }
    public override int GetHashCode()
    {
        return string.Format("{0}--{1}--{2}", x, y, z).GetHashCode();
    }

    public string ToString(IFormatProvider provider)
    {
        return $"({x.ToString(provider)}, {y.ToString(provider)}, {z.ToString(provider)})";
    }
    public string ToString(string format)
    {
        return $"({x.ToString(format)}, {y.ToString(format)}, {z.ToString(format)})";
    }
    public string ToString(string format, IFormatProvider provider)
    {
        return $"({x.ToString(format, provider)}, {y.ToString(format, provider)}, {z.ToString(format, provider)})";
    }
    #endregion

    #region Operators
    public static bool operator ==(Vector3D v1, Vector3D v2)
    {
        return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
    }
    public static bool operator !=(Vector3D v1, Vector3D v2)
    {
        return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
    }
    public static Vector3D operator +(Vector3D v, double n)
    {
        return new Vector3D(v.x + n, v.y + n, v.z + n);
    }
    public static Vector3D operator +(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vector3D operator -(Vector3D v, double n)
    {
        return new Vector3D(v.x - n, v.y - n, v.z - n);
    }
    public static Vector3D operator -(Vector3D v)
    {
        return v *= -1;
    }
    public static Vector3D operator -(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static Vector3D operator *(Vector3D v, double n)
    {
        return new Vector3D(v.x * n, v.y * n, v.z * n);
    }
    public static Vector3D operator *(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static Vector3D operator /(Vector3D v, double n)
    {
        return new Vector3D(v.x / n, v.y / n, v.z / n);
    }
    public static Vector3D operator /(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }

    public static Vector3D operator *(Vector3D point, Quaternion rotation)
    {
        return rotation * point;
    }


    // Shamefully copy pasted from 
    // https://github.com/Unity-Technologies/UnityCsReference/blob/5bc2902a12bd9f919e03a60f1f1ffffe5c31204c/Runtime/Export/Math/Quaternion.cs#L89
    public static Vector3D operator *(Quaternion rotation, Vector3D point)
    {
        double x = rotation.X * 2D;
        double y = rotation.Y * 2D;
        double z = rotation.Z * 2D;
        double xx = rotation.X * x;
        double yy = rotation.Y * y;
        double zz = rotation.Z * z;
        double xy = rotation.X * y;
        double xz = rotation.X * z;
        double yz = rotation.Y * z;
        double wx = rotation.W * x;
        double wy = rotation.W * y;
        double wz = rotation.W * z;

        Vector3D res;
        res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
        res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
        res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
        return res;
    }

    #endregion
}
