using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Extensions
{
    public static T _Clamp<T>(T num, T min, T max) where T : IComparable<T>
    {
        if (num.CompareTo(min) <= 0) return min;
        if (num.CompareTo(max) >= 0) return max;
        return num;
    }

    public static T Clamp<T>(this T input, T min, T max) where T : IComparable<T>
    {
        return _Clamp(input, min, max);
    }

    public static bool InRange<T>(this T input, T min, T max) where T : IComparable<T>
    {
        return _InRange(input, min, max);
    }

    public static bool _InRange<T>(T input, T min, T max) where T : IComparable<T>
    {
        if (input.CompareTo(min) <= 0)
            return false;
        if (input.CompareTo(max) >= 0)
            return false;
        return true;
    }
}

