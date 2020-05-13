using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TableDataManager {
    private Dictionary<e_TableType, ITableDataBase> Dic_TableData = new Dictionary<e_TableType, ITableDataBase>();

    private TableDataManager() {
        InitTDPath(DataUtil.LoadingPath);
    }

    private static TableDataManager _instance = null;
    public static TableDataManager Instance {
        get {
            if (_instance == null) {
                _instance = new TableDataManager();
            }
            return _instance;
        }
    }

    //public void sdadada(e_TableType et) {
    //    List<string> ss = dic_TDPath[et];
    //    for (int i = 0; i < ss.Count; i++) {
    //        string p = ss[i];
    //        FileStream fs = new FileStream(p, FileMode.Open);
    //        BinaryReader br = new BinaryReader(fs);
    //        string tabName = br.ReadString();
    //    }
    //}

    //public T GetTable<T>(e_TableType type) where T : ITableDataBase, new() {
    //    bool ContainsKey = Dic_TableData.ContainsKey(type);
    //    if (ContainsKey == false) {
    //        if (type.ToString() != typeof(T).ToString()) {
    //            Debug.LogError("TableType 与 TableClass 不对应");
    //            return null;
    //        }
    //        T t = new T();
    //        Dic_TableData.Add(type, t);
    //    }
    //    return Dic_TableData[type] as T;
    //}

    //private Dictionary<e_TableType, ITableDataBase> Dic_TableData1 = new Dictionary<e_TableType, ITableDataBase>();

    //public List<T> GetTableList<T, U>(e_TableType type) where T : bB where U : TableDataBase<T>, new() {

    //    bool ContainsKey = Dic_TableData1.ContainsKey(type);
    //    if (ContainsKey == false) {
    //        if (type.ToString() != typeof(U).ToString()) {
    //            Debug.LogError("TableType 与 TableClass 不对应");
    //            return null;
    //        }
    //        U u = new U();
    //        Dic_TableData1.Add(type, u);
    //    }

    //    Dictionary<e_TableType, ITableDataBase> Dic = Dic_TableData1;
    //    U baa = Dic[type] as U;
    //    List<T> sw = baa.tableList as List<T>;

    //    return sw as List<T>;
    //}


    //public class bB {
    //}



    // ---------------- 一对多

    /// <summary>
    /// 数据包路径
    /// </summary>
    public Dictionary<e_TableType, List<string>> dic_TDPath = new Dictionary<e_TableType, List<string>>();
    /// <summary>
    /// 数据解析类
    /// </summary>
    private Dictionary<e_TableType, TDPB> Dic_TDBP = new Dictionary<e_TableType, TDPB>();


    //private Dictionary<int ,dynamic>dada=new 

    /// <summary>
    /// 初始化数据包路径
    /// </summary>
    /// <param name="path"></param>
    private void InitTDPath(string path) {
        DirectoryInfo root = new DirectoryInfo(path);
        foreach (FileInfo f in root.GetFiles()) {
            if (f.Extension == ".data") {
                FileStream fs = new FileStream(f.FullName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);

                string tabName = br.ReadString();
                e_TableType et = (e_TableType)Enum.Parse(typeof(e_TableType), tabName);
                br.Close();
                fs.Close();

                bool ContainsKey = dic_TDPath.ContainsKey(et);
                if (ContainsKey == false) {
                    List<string> ls = new List<string>();
                    dic_TDPath.Add(et, ls);
                }
                List<string> ls1 = dic_TDPath[et];
                ls1.Add(f.FullName);
            }
        }
        foreach (DirectoryInfo d in root.GetDirectories()) {
            InitTDPath(d.FullName);
        }
    }

    /// <summary>
    /// 检查并创建对应表数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="et"></param>
    private void ContainsTD<T>(e_TableType et) where T : TDBase, new() {
        if (Dic_TDBP.ContainsKey(et) == false) {
            TDParsing<T> _parsing = new TDParsing<T>();
            List<string> lp = dic_TDPath[et];
            _parsing.ParsingPath(lp);
            Dic_TDBP.Add(et, _parsing);
        }
    }

    public Dictionary<int, T> GetDT<T>(e_TableType et) where T : TDBase, new() {
        ContainsTD<T>(et);
        TDParsing<T> ss = Dic_TDBP[et] as TDParsing<T>;
        return ss.dt;
    }

    public List<T> GetLT<T>(e_TableType et) where T : TDBase, new() {
        ContainsTD<T>(et);
        TDParsing<T> ss = Dic_TDBP[et] as TDParsing<T>;
        return ss.lt;
    }
}

public class TDParsing<T> : TDPB where T : TDBase, new() {

    public List<T> lt = new List<T>();
    public Dictionary<int, T> dt = new Dictionary<int, T>();

    public void ParsingPath(List<string> lp) {
        for (int i = 0; i < lp.Count; i++) {
            string p = lp[i];
            FileStream fs = new FileStream(p, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            string tabNam = br.ReadString();
            string TN = typeof(T).ToString();
            if (tabNam != TN) {
                Debug.LogError("加载数据包与解析类不相符，tabNam：" + tabNam + " T：" + TN);
                continue;
            }

            while (DataUtil.IsLineStart(br.ReadInt32())) {
                T t = new T();
                t.updateData(br);
                lt.Add(t);
                dt.Add(t.id, t);
            }
        }
    }
}

/// <summary>
/// 数据包解析基类
/// </summary>
public class TDPB {

}

public class TDBase {
    public int id {
        get;
        protected set;
    }
    public bool canInit {
        get;
        protected set;
    }

    public TDBase() {
        canInit = true;
    }

    public virtual void updateData(BinaryReader br) {
    }
}