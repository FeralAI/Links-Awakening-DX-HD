using System;

namespace ProjectZ.InGame.GameObjects;

public class GameObjectItem(string index, object[] parameter) : IComparable
{
    public string Index = index;
    public object[] Parameter = parameter;

    public int CompareTo(object compareObject)
    {
        if (!(compareObject is GameObjectItem item)) return 0;

        if (item.Parameter.Length >= 3 && Parameter.Length >= 3)
            return Index.CompareTo(item.Index) * 4 +
                   ((int)Parameter[1]).CompareTo((int)item.Parameter[1]) * 2 +
                   ((int)Parameter[2]).CompareTo((int)item.Parameter[2]);

        return Index.CompareTo(item.Index);
    }
}