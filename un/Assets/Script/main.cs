using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class main : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {


        TableDataManager tbm = TableDataManager.Instance;
        Dictionary<int, aaaa> d = tbm.GetDT<aaaa>(e_TableType.aaaa);
        List<aaaa> l = tbm.GetLT<aaaa>(e_TableType.aaaa);
    }

    // Update is called once per frame
    void Update() {

        cs2<cs1> sdad = new cs2<cs1>();

    }

    public void sdadK<T>() where T : class {

    }
}



public class cs1 {
    public int a;
    public int b;
}
public class cs2<T> {
    public int a;
    public int b;

    public List<T> t = new List<T>();
}