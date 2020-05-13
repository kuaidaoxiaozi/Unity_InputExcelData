using System;
using System.Diagnostics;
using System.IO;

namespace excel {

    /// <summary>
    /// 数据处理的工具类
    /// </summary>
    class DataUtil {


        public const string LoadingPath = "";


        /// <summary>
        /// int转化为byte[]:
        /// </summary>
        /// <returns></returns>
        public static byte[] IntToBitConverter(int num) {
            byte[] bytes = BitConverter.GetBytes(num);
            return bytes;
        }
        /// <summary>
        /// byte[] 转化为int:
        /// </summary>
        /// <returns></returns>
        public static int IntToBitConverter(byte[] bytes) {
            int temp = BitConverter.ToInt32(bytes, 0);
            return temp;
        }


        /// <summary>
        /// utf-8 转 base64
        /// </summary>
        /// <param name="utf8">utf8 字符串</param>
        /// <returns></returns>
        public static string utf8ToBase64(string utf8) {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(utf8);
            string b64 = Convert.ToBase64String(b);
            return b64;
        }

        /// <summary>
        /// base64 转 utf-8
        /// </summary>
        /// <param name="utf8">utf8 字符串</param>
        /// <returns></returns>
        public static string base64ToUtf8(string b64) {
            byte[] b = Convert.FromBase64String(b64);
            string utf8 = System.Text.Encoding.UTF8.GetString(b);
            return utf8;
        }

        /// <summary>
        /// Exists()判断文件是否存在; 存在 : true  2.不存在: false 
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns></returns>
        public static bool IsExits(string filePath) {
            if (File.Exists(filePath)) {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 是否是行数据开头
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsLineStart(int num) {
            if (num == (int)DataTypeNum.Line_Start) {
                return true;
            }
            Console.WriteLine("不是行开头，" + num);
            if (num == (int)DataTypeNum.Table_End)
                Console.WriteLine("表数据结束");
            return false;
        }


        /// <summary>
        /// DataKeyword 转化成一个 数字编号
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static int DataKeywordToTypeNum(string dataType) {
            DataTypeNum typeNum = DataTypeNum.Error;
            switch (dataType) {
                case DataKeyword.Line_End:
                    typeNum = DataTypeNum.Line_End;
                    break;
                case DataKeyword.Line_Start:
                    typeNum = DataTypeNum.Line_Start;
                    break;
                case DataKeyword.Table_End:
                    typeNum = DataTypeNum.Table_End;
                    break;
                case DataKeyword.Table_Start:
                    typeNum = DataTypeNum.Table_Start;
                    break;
                case DataKeyword.Int:
                    typeNum = DataTypeNum.Int;
                    break;
                case DataKeyword.String:
                    typeNum = DataTypeNum.String;
                    break;
                case DataKeyword.Double:
                    typeNum = DataTypeNum.Double;
                    break;
                default:
                    Console.WriteLine("未知数据Type：" + dataType);
                    break;
            }
            return (int)typeNum;
        }


    }

    public enum DataTypeNum {
        Error = -1,
        Line_End = 0,
        Line_Start = 1,
        Table_End = 2,
        Table_Start = 3,
        Int = 100,
        String = 101,
        Double = 101,
    }
}
