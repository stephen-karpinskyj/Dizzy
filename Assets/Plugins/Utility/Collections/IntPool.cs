using System.Collections.Generic;

public class IntPool : GenericPool<int>
{
    public IntPool(int count)
        : base(GenerateList(count))
    {
    }
    
    private static List<int> GenerateList(int count)
    {
        var list = new List<int>(count);

        for (var i = 0; i < count; i++)
        {
            list.Add(i);
        }

        return list;
    }
}
