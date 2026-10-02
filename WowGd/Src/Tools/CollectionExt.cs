using System.Collections.Generic;

public static class CollectionExt
{
    public static bool SwapRemove<T>(this IList<T> collection, T item)
    {
        int index = collection.IndexOf(item);

        if (index < 0)
            return false;
        
        collection[index] = collection[^1];
        collection.RemoveAt(collection.Count - 1);
        return true;
    }
}