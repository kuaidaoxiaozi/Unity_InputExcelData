using NPOI.POIFS.FileSystem;
using NPOI;
using System;
using NPOI.HSSF.UserModel;
using NPOI.Util;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.OpenXml4Net.OPC;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using System.Runtime.Intrinsics.X86;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using System.Buffers;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;

namespace excel {
    class Program {

        public static string CsPathDir = @"E:\wj\excel1\";
        public static string DataPathDir = @"E:\wj\excel1\";
        public static string XlsPathDir = @"E:\wj\excel\";
        public static string e_TablePath = @"E:\wj\excel1\e_Table.cs";

        public static string tabs = "\t";
        public static string newline = "\r\n";

        public static FileStream DataFileStream;
        public static BinaryWriter DataBinaryWriter;

        public static string Enum_TableData = "public enum e_TableType {" + newline;

        /// <summary>
        /// 删除存在的文件
        /// </summary>
        public static void DeleteFile(string _dataPath) {
            if (DataUtil.IsExits(_dataPath)) {
                File.Delete(_dataPath);
            }
        }

        /// <summary>
        /// 创建数据包文件流
        /// </summary>
        /// <param name="_dataPath"></param>
        public static void CreatDataFileStream(string _dataPath) {
            if (DataUtil.IsExits(_dataPath)) {
                File.Delete(_dataPath);
            }
            DataFileStream = new FileStream(_dataPath, FileMode.Append);
            DataBinaryWriter = new BinaryWriter(DataFileStream, System.Text.Encoding.UTF8);
        }



        /// <summary>
        /// 关闭数据包文件流
        /// </summary>
        public static void CloseDataFileStream() {
            DataBinaryWriter.Close();
            DataFileStream.Close();
        }


        static void Main(string[] args) {
            //Console.WriteLine("Hello World!");
            //string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            getDirectory(XlsPathDir);
            Enum_TableData += "}";
            WriteScriptText(e_TablePath, Enum_TableData);

            //OneToOne ono = new OneToOne();
        }





        /// <summary>
        /// 打包指定路径下所有 excel 文件
        /// </summary>
        /// <param name="path"></param>
        public static void getDirectory(string path) {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles()) {
                if (f.Extension == ".xlsx" || f.Extension == ".xls") {
                    ImportOneExcelFile(f.FullName);
                }
            }
            foreach (DirectoryInfo d in root.GetDirectories()) {
                getDirectory(d.FullName);
            }
        }


        /// <summary>
        /// 导入一张 excel 文件
        /// </summary>
        /// <param name="excelPath">excel路径</param>
        /// <returns>返回datatable</returns>
        static void ImportOneExcelFile(string excelPath) {
            bool isExits = File.Exists(excelPath);
            if (!isExits) {
                Console.WriteLine("未能找到" + excelPath + "文件");
                return;
            }
            Console.WriteLine("开打打包文件：" + excelPath);
            //ExcelToDataTable(excelPath);

            FileStream fs = null;
            IWorkbook workbook = null;
            try {
                fs = new FileStream(excelPath, FileMode.Open);
                // 2007版本
                if (excelPath.IndexOf(".xlsx") > 0)
                    workbook = new XSSFWorkbook(fs);
                // 2003版本
                else if (excelPath.IndexOf(".xls") > 0)
                    workbook = new HSSFWorkbook(fs);

                if (workbook != null) {
                    for (int i = 0; i < workbook.NumberOfSheets; i++) {
                        ISheet sheet = workbook.GetSheetAt(i);
                        if (sheet != null) {
                            ImportOneSheetData(sheet);
                        }
                    }

                }
            } catch (Exception) {
                if (fs != null) {
                    fs.Close();
                }
                Console.WriteLine("文件错误，文件地址：" + excelPath);
            }
        }

        /// <summary>
        /// 导入 excel 文件的一个 sheet
        /// </summary>
        /// <param name="sheet"></param>
        static void ImportOneSheetData(ISheet sheet) {
            int rowCount = sheet.LastRowNum;//总行数
            if (rowCount > 0) {

                IRow firstRow = sheet.GetRow(0);//第一行
                IRow twoRow = sheet.GetRow(1);//第二行
                int cellCount = firstRow.LastCellNum;//列数
                string sheetName = firstRow.GetCell(0).StringCellValue; //表名

                Enum_TableData += tabs + sheetName + "," + newline;

                string sheetCsPath = CsPathDir + sheetName + ".cs";
                DeleteFile(sheetCsPath);
                DeclarationDescriptionInterface(firstRow, twoRow, sheetCsPath);
                CreatDataFileStream(DataPathDir + sheetName + ".data");
                DataBinaryWriter.Write(sheetName);

                //填充行
                for (int i = 2; i <= rowCount; ++i) {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                        continue;
                    ImportOneLineData(cellCount, twoRow, row);


                    string strr = String.Empty;
                    for (int j = row.FirstCellNum; j < cellCount; ++j) {
                        ICell cell = row.GetCell(j);
                        if (cell == null) {
                        } else {
                            switch (cell.CellType) {
                                case CellType.Unknown:
                                    Console.WriteLine("未知");
                                    break;
                                case CellType.Numeric:
                                    strr += cell.NumericCellValue + "_";
                                    break;
                                case CellType.String:
                                    strr += cell.StringCellValue + "_";
                                    break;
                                case CellType.Formula:
                                    Console.WriteLine("公示");
                                    break;
                                case CellType.Blank:
                                    Console.WriteLine("空单元格");
                                    break;
                                case CellType.Boolean:
                                    Console.WriteLine("布尔");
                                    break;
                                case CellType.Error:
                                    Console.WriteLine("Error");
                                    continue;
                            }
                        }
                    }
                    Console.WriteLine(strr);
                }

                int Table_End = DataUtil.DataKeywordToTypeNum(DataKeyword.Table_End);
                DataBinaryWriter.Write(Table_End);
                CloseDataFileStream();
            }
        }

        /// <summary>
        /// 导入 sheet 一行数据
        /// </summary>
        /// <param name="column">一行几个数据</param>
        /// <param name="twoRow">提供数据类型的行</param>
        /// <param name="row">数据行</param>
        public static void ImportOneLineData(int column, IRow twoRow, IRow row) {
            string rowStr = string.Empty;

            int Line_Start = DataUtil.DataKeywordToTypeNum(DataKeyword.Line_Start);
            DataBinaryWriter.Write(Line_Start);

            for (int i = 1; i < column; i++) {
                ICell data = row.GetCell(i);
                string dataType = twoRow.GetCell(i).StringCellValue;
                switch (dataType) {
                    case DataKeyword.Int:
                        if (data.CellType == CellType.Numeric) {
                            int dataInt = (int)data.NumericCellValue;
                            DataBinaryWriter.Write(dataInt);
                        } else {
                            Console.WriteLine("Int 数据错误");
                        }
                        break;
                    case DataKeyword.Float:
                        if (data.CellType == CellType.Numeric) {
                            float dataFlo = (float)data.NumericCellValue;
                            DataBinaryWriter.Write(dataFlo);
                        } else {
                            Console.WriteLine("Float 数据错误");
                        }
                        break;
                    case DataKeyword.Double:
                        if (data.CellType == CellType.Numeric) {
                            double dataDou = data.NumericCellValue;
                            DataBinaryWriter.Write(dataDou);
                        } else {
                            Console.WriteLine("Double 数据错误");
                        }
                        break;
                    case DataKeyword.String:
                        if (data.CellType == CellType.String) {
                            string dataStr = data.StringCellValue;
                            DataBinaryWriter.Write(dataStr);
                        } else if (data.CellType == CellType.Numeric) {
                            string dataStr = data.NumericCellValue.ToString();
                            DataBinaryWriter.Write(dataStr);
                        } else {
                            Console.WriteLine("String 数据错误");
                        }
                        break;
                    case DataKeyword.Bool:
                        if (data.CellType == CellType.Boolean) {
                            bool dataBool = data.BooleanCellValue;
                            DataBinaryWriter.Write(dataBool);
                        } else {
                            Console.WriteLine("Double 数据错误");
                        }
                        break;
                }
            }
        }


        public static void WriteScriptText(string csPath, string str) {
            try {
                File.WriteAllText(csPath, str, System.Text.Encoding.UTF8);
            } catch {
                Console.WriteLine("声明描述文件错误");
            }
        }

        /// <summary>
        /// IO 声明描述文件
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="twoRow"></param>
        /// <param name="csPath">脚本地址</param>
        public static void DeclarationDescriptionInterface(IRow firstRow, IRow twoRow, string csPath) {
            try {
                int cellCount = firstRow.LastCellNum;//列数

                string className = firstRow.GetCell(0).StringCellValue;
                string classTab = "using System.IO;" + newline;
                classTab += "public class " + className + " : TDBase {" + newline;
                string classEnd = "}" + newline;


                string classStr = string.Empty;
                string classStr1 = tabs + "public override void updateData(BinaryReader br) {" + newline;
                classStr1 += tabs + tabs + "if (canInit == false)" + newline;
                classStr1 += tabs + tabs + tabs + "return;" + newline;
                classStr1 += tabs + tabs + "canInit = false;" + newline;

                for (int i = 1; i < cellCount; i++) {
                    string dataType = twoRow.GetCell(i).StringCellValue;
                    string dataName = firstRow.GetCell(i).StringCellValue;
                    if (dataName != "id") {
                        classStr += tabs + "public " + dataType + " " + dataName + " {" + newline;
                        classStr += tabs + tabs + "get;" + newline;
                        classStr += tabs + tabs + "private set;" + newline;
                        classStr += tabs + classEnd;
                    }
                    classStr1 += tabs + tabs + dataName + " = ";
                    switch (dataType) {
                        case DataKeyword.Int:
                            classStr1 += "br.ReadInt32();";
                            break;
                        case DataKeyword.Float:
                            classStr1 += "br.ReadSingle();";
                            break;
                        case DataKeyword.Double:
                            classStr1 += "br.ReadDouble();";
                            break;
                        case DataKeyword.String:
                            classStr1 += "br.ReadString();";
                            break;
                        case DataKeyword.Bool:
                            classStr1 += "br.ReadBoolean();";
                            break;
                    }
                    classStr1 += newline;
                }

                string ssss = classTab + classStr + classStr1 + tabs + classEnd + classEnd;

                //string sa = "public class " + className + " : TableDataBase<I" + className + "> {" + newline;
                //sa += tabs + "public " + className + "() {" + newline;
                //sa += tabs + tabs + "string path = DataUtil.LoadingPath + " + '"' + className + ".data" + '"' + ";" + newline;
                //sa += tabs + tabs + "FileStream fs = new FileStream(path, FileMode.Open);" + newline;
                //sa += tabs + tabs + "BinaryReader br = new BinaryReader(fs);" + newline;
                //sa += tabs + tabs + "while (DataUtil.IsLineStart(br.ReadInt32())) {" + newline;
                //sa += tabs + tabs + tabs + "I" + className + " _" + className + "= new I" + className + "(br);" + newline;
                //sa += tabs + tabs + tabs + "AddData(_" + className + ".id, _" + className + ");" + newline;
                //sa += tabs + tabs + classEnd;
                //sa += tabs + classEnd;
                //sa += classEnd;
                //WriteScriptText(csPath, ssss + sa);

                WriteScriptText(csPath, ssss);
            } catch {
                Console.WriteLine("声明描述文件错误：" + firstRow.GetCell(0).StringCellValue);
            }
        }
    }


    class class1Data {
        public readonly int id;
        public class1Data(BinaryReader br) {
            id = br.ReadInt32();

        }
    }

    class class1 : TableDataBase<class1Data> {
        public class1() {
            string path = DataUtil.LoadingPath + "class1.data";
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            while (DataUtil.IsLineStart(br.ReadInt32())) {
                class1Data cd = new class1Data(br);
                AddData(cd.id, cd);
            }
        }
    }




    class TableDataBase<T> where T : class {
        private List<T> DataList;
        private Dictionary<int, T> DataMap;
        protected TableDataBase() {
            DataList = new List<T>();
            DataMap = new Dictionary<int, T>();
        }
        protected void AddData(int id, T t) {
            DataList.Add(t);
            DataMap.Add(id, t);
        }
    }

}
