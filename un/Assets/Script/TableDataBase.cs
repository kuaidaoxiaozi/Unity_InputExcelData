using System.Collections.Generic;
public class TableDataBase<T> : ITableDataBase where T : class {
    public TableDataBase() {
        tableList = new List<T>();
        tableMap = new Dictionary<int, T>();
    }
    protected void AddData(int id, T t) {
        tableList.Add(t);
        tableMap.Add(id, t);
    }

    public List<T> tableList {
        get;
    }
    public Dictionary<int, T> tableMap {
        get;
    }
}

public class ITableDataBase {
}
