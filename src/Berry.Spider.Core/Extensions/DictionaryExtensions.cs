namespace Berry.Spider.Core;

public static class DictionaryExtensions
{
    public static SortedDictionary<TKey, TVal> AsDictionary<TKey, TVal>(this IEnumerable<KeyValuePair<TKey, TVal>> enumerable) where TKey : notnull
    {
        var dictionary = new SortedDictionary<TKey, TVal>();
        foreach (var kvp in enumerable)
        {
            if (kvp.Value != null)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        return dictionary;
    }
}