﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QLHS_DR.Core
{
    internal class DocScan
    {
        public static IEnumerable<string> GetFileList(string fileSearchPattern, string rootFolderPath)
        {
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                try
                {
                    tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }


        public static string GetDrawingNameFromFileName(string str) //remove ms
        {
            if (str != null)
            {
                int vitridaucahdautien = str.IndexOf(' '); // Tìm vị trí dấu cách đầu tiên
                if (vitridaucahdautien > 0)
                {
                    str = str.Substring(vitridaucahdautien);
                    str = str.Trim(' ');
                    str = str.Replace("  ", " ");
                }
                return str;
            }
            else return null;
        }
        public static string GetRatedPower(string str)
        {
            var regex1 = new Regex(@"([\d]+(\.?)+[\d]+MVA)|([\d]+(\.?)+[\d]+MVa)|([\d]+(\.?)+[\d]+MvA)|([\d]+(\.?)+[\d]+mVA)|([\d]+(\.?)+[\d]+Mva)|([\d]+(\.?)+[\d]+mVa)|([\d]+(\.?)+[\d]+mvA)|([\d]+(\.?)+[\d]+mva)|([\d]+(\.?)+[\d]+KVA)|([\d]+(\.?)+[\d]+kVA)|([\d]+(\.?)+[\d]+KvA)|([\d]+(\.?)+[\d]+KVa)|([\d]+(\.?)+[\d]+Kva)|([\d]+(\.?)+[\d]+kVa)|([\d]+(\.?)+[\d]+kvA)|([\d]+(\.?)+[\d]+kva)|([\d]+(\.?)+[\d]+VA)");
            MatchCollection matchCollection = regex1.Matches(str);
            return matchCollection[0].Value;
        }
        public static string GetRatedVoltage(string str)
        {
            var regex1 = new Regex(@"([\d]+(\.?)+[\d]+KV)|([\d]+(\.?)+[\d]+kV)|([\d]+(\.?)+[\d]+Kv)|([\d]+(\.?)+[\d]+kv)|([\d]+(\.?)+[\d]+v)|([\d]+(\.?)+[\d]+V)");
            MatchCollection matchCollection = regex1.Matches(str);
            return matchCollection[0].Value;
        }
        public static bool IsHSNT(string str)
        {
            str = str.ToLower();
            if (str.Contains("hsnt"))
                return true;
            else return false;
        }
        public static MatchCollection GetTransformerCode_Method1(string str)
        {
            str = str.Replace('_', '-');
            str = str.ToLower();
            var regex1 = new Regex(@"(([0-2]{1}\d{5})|([0-2]{1}[0-9]{1}[0][468]{1}))((\-)|(k-)|(kbh-)|(bh-))(\d{1,3})((\+(\d{1,3}))|(\-(\d{1,3}))|([bh])|([kbh])+)*(?=[^0-9\.]|$|[\.])");

            MatchCollection matchCollection = regex1.Matches(str);
            if (matchCollection != null)
            {
                if (matchCollection.Count > 0)
                    return matchCollection;
                else return null;
            }
            else return null;
        }
        protected internal bool IsLSX(string str)
        {
            str = str.ToLower();
            if (str.Contains("lsx"))
                return true;
            else return false;
        }
        protected internal bool IsHoDong(string str)
        {
            str = str.ToLower();
            if (str.Contains("hopdong") || str.Contains("hop dong") || str.Contains("hợp đồng"))
                return true;
            else return false;
        }

        public static string GetMS(string str) //lấy mã số từ tên file
        {
            if (str != null)
            {
                return str.Split(' ')[0];
            }
            else return null;
        }

        public static string GetNameFromFileNameNoExt(string fileName)
        {
            string name;
            if (fileName != null)
            {
                char[] arr = fileName.ToCharArray();
                arr = Array.FindAll<char>(arr, (c => (char.IsLetter(c) || char.IsWhiteSpace(c))));
                //arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')));
                name = new string(arr);
                return name;
            }
            else return null;
        }
        public static string MD5(string pathFile)
        {
            if (pathFile != null)
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = File.OpenRead(pathFile))
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                    }
                }
            }
            else return null;
        }
        public static bool IsTransformerCode(string str)
        {
            if (str == null) return false;
            str = str.Replace(" ", string.Empty).ToLower();
            var regex1 = new Regex("^[bhk0-9-+]{6,35}$"); //Chuỗi bao gồm bhk và các số từ 0 đến 9, dấu +, dấu -, độ dài >5 kí tự và <=35 kí tự
                                                          //TransformerDTO code contain charracter: 0 1 2 3 4 5 6 7 8 9 - : + k bh Regex: ^[bhk0-9-]$
            var regex2 = new Regex("-");
            var regex3 = new Regex(@"^\d{4}"); //Bắt đầu bằng tối thiểu 4 kí tự số 0 đến 9
            var regex4 = new Regex(@"^.+[\d{1}]$|bh{1}$"); //kết thúc là số hoặc bh
            if (!regex1.IsMatch(str)) return false;
            else if (!regex2.IsMatch(str)) return false;
            else if (!regex3.IsMatch(str)) return false;
            else if (!regex4.IsMatch(str)) return false;
            else return true;
        }

        public static bool IsInstrumentTransformerCode(string str)
        {
            str = str.Replace(" ", string.Empty).ToLower();
            var regex1 = new Regex("^[bhk0-9-+]{6,35}$"); //Chuỗi bao gồm bhk và các số từ 0 đến 9, dấu +, dấu -, độ dài >5 kí tự và <=35 kí tự
                                                          //TransformerDTO code contain charracter: 0 1 2 3 4 5 6 7 8 9 - : + k bh Regex: ^[bhk0-9-]$
            var regex2 = new Regex("-");
            var regex3 = new Regex(@"^\d{4}"); //Bắt đầu bằng tối thiểu 4 kí tự số 0 đến 9
            var regex4 = new Regex(@"^.+[\d{1}]$|bh{1}$"); //kết thúc là số hoặc bh
            if (!regex1.IsMatch(str)) return false;
            else if (!regex2.IsMatch(str)) return false;
            else if (!regex3.IsMatch(str)) return false;
            else if (!regex4.IsMatch(str)) return false;
            else return true;
        }


        public static string GetCOdeNoiDung(string str)
        {
            //Xóa chuỗi có mã số khỏi nội dung tên file

            if (string.IsNullOrEmpty(str)) return null;
            else
            {
                string[] listString = str.Split();
                List<string> listKetqua;
                if (listString != null)
                {
                    listKetqua = listString.ToList();
                    if (listString.Count() > 1)
                    {
                        string ketqua1 = string.Empty;
                        foreach (var item in listKetqua)
                        {
                            if (!DocScan.IsTransformerCode(item))
                                ketqua1 = ketqua1 + " " + item;
                        }
                        ketqua1 = ketqua1.Trim();
                        return ketqua1;
                    }
                }
                else return str;
            }
            return str;
        }
        public static string GetTransformerCode(string str)
        {
            str = str.ToUpper();
            if (string.IsNullOrEmpty(str)) return null;
            else
            {
                string[] listString = str.Split();
                for (int i = 0; i < listString.Length; i++)
                {
                    if (IsTransformerCode(listString[i]))
                    {
                        return listString[i];
                    }
                }
                return null;
            }
        }
        public static List<string> GetTransformerCodeSingle(string str)
        {
            var regex1 = new Regex(@"[-+]\d+(BH||)+");
            var regex2 = new Regex(@"[^0-9]");
            var regex3 = new Regex(@"[-+]");
            List<string> ketqua = new List<string>();
            string chuoimaso = GetTransformerCode(str);
            if (string.IsNullOrEmpty(chuoimaso)) return null;
            string[] arrMaSo = chuoimaso.Split('-', '+');

            List<IndexOfMS> listIndex = new List<IndexOfMS>();
            string typeCode = arrMaSo[0];


            MatchCollection matchCollection = regex1.Matches(chuoimaso);
            foreach (Match item in matchCollection)
            {
                IndexOfMS addItem = new IndexOfMS() { StringIndex = item.Value, StringIndexOnlyNumber = regex2.Replace(item.Value, string.Empty), ValueIndex = Convert.ToInt32(regex2.Replace(item.Value, string.Empty)) };
                listIndex.Add(addItem);
            }
            List<IndexOfMS> listIndexSorted = listIndex.OrderBy(o => o.ValueIndex).ToList(); //Sap xep lai
            if (listIndexSorted.Count == 0) return null;
            else if (listIndexSorted.Count == 1) ketqua.Add(typeCode + listIndexSorted[0].StringIndex);
            else for (int i = 0; i < listIndexSorted.Count - 1; i++)
                {
                    //TH1:
                    if (listIndexSorted[i + 1].StringIndex.StartsWith("-"))
                    {
                        ketqua.AddRange(GeneralFullTransformerCode(listIndexSorted[i], listIndexSorted[i + 1], typeCode));
                    }
                    //TH2:
                    else if (listIndexSorted[i + 1].StringIndex.StartsWith("+"))
                    {
                        ketqua.Add(typeCode + "-" + regex3.Replace(listIndexSorted[i].StringIndex, string.Empty));
                        ketqua.Add(typeCode + "-" + regex3.Replace(listIndexSorted[i + 1].StringIndex, string.Empty));
                    }
                    else return null;
                }
            List<string> result = ketqua.Distinct().ToList();

            return result;
        }
        public class IndexOfMS
        {
            private string _StringIndex;
            public string StringIndex { get => _StringIndex; set => _StringIndex = value; }
            private int _ValueIndex;
            public int ValueIndex { get => _ValueIndex; set => _ValueIndex = value; }
            public string StringIndexOnlyNumber { get; set; }
        }
        public static List<string> GeneralFullTransformerCode(IndexOfMS start, IndexOfMS stop, string typeCode)
        {
            List<string> ketqua = new List<string>();
            List<IndexOfMS> x = new List<IndexOfMS>();
            var regex3 = new Regex(@"[-+]");
            if (start.ValueIndex >= stop.ValueIndex) return null;
            else
            {
                x.Add(start);
                //Giá trị khởi tạo
                string oldStringIndex = start.StringIndex;
                string oldStringIndexOnlyNumber = start.StringIndexOnlyNumber;
                int oldVlaueIndex = start.ValueIndex;
                int newValueIndex;
                string newStringIndexOnlyNumber;
                string newStringIndex;
                int numberOfZero;
                for (int i = start.ValueIndex; i < stop.ValueIndex; i++)
                {
                    newValueIndex = oldVlaueIndex + 1;
                    numberOfZero = oldStringIndexOnlyNumber.Length - newValueIndex.ToString().Length;
                    newStringIndexOnlyNumber = newValueIndex.ToString();
                    for (int j = 0; j < numberOfZero; j++) { newStringIndexOnlyNumber = "0" + newStringIndexOnlyNumber; }

                    newStringIndex = oldStringIndex.Replace(oldStringIndexOnlyNumber, newStringIndexOnlyNumber);
                    IndexOfMS temp = new IndexOfMS() { ValueIndex = newValueIndex, StringIndex = newStringIndex, StringIndexOnlyNumber = newStringIndexOnlyNumber };
                    x.Add(temp);

                    oldStringIndex = newStringIndex;
                    oldStringIndexOnlyNumber = newStringIndexOnlyNumber;
                    oldVlaueIndex = newValueIndex;
                }
            }
            foreach (var item in x)
            {
                string maso = typeCode + "-" + regex3.Replace(item.StringIndex, string.Empty);
                ketqua.Add(maso);
            }
            return ketqua;
        }

    }
}
