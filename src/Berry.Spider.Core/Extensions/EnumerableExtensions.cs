namespace Berry.Spider.Core;

public static class EnumerableExtensions
{
    public static bool In<T>(this T value, params T[] values)
    {
        return values.Contains(value);
    }

    public static bool Between<T>(this T i, T start, T end) where T : IComparable<T>
    {
        return i.CompareTo(start) >= 0 && i.CompareTo(end) <= 0;
    }
}