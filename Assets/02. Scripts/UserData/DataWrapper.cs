using System.Collections.Generic;

[System.Serializable]
public class DataWrapper<T>
{
    public List<T> items;

    public DataWrapper(List<T> items)
    {
        this.items = items;
    }
}
