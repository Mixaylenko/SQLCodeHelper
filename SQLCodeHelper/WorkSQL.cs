using System.Collections.Generic;
using System.Linq;

namespace SQLCodeHelper
{
    internal class WorkSQL
    {
        private string LAlf = "λlLμ", Num ="01234567890", im, first, second, str, error;
        private int ij, ua, dub;
        private Stack<string> AdditionalMass = new Stack<string>();
        private Queue<string> SqlWas = new Queue<string>();
        bool neverwas;
        //создание SQL кода
        public string CodeCreate(Queue<string> RPN, Queue<string> SqlTable)
        {
            //сброс переменных и массивов
            SqlWas.Clear();
            AdditionalMass.Clear();
            ij = 1;ua = 1;dub = 1; error = "";
            //тело цикла создания кода
            while (RPN.Count > 0)
            {
                if (error != "")
                    return error;
                neverwas = true;
                im = RPN.Dequeue();
                if ((im == "+") || (im == "*")) 
                {
                    ItWas(im, SqlTable);
                }
                else if((LAlf.Contains(im[0])) && (im[1] == ','))
                    ItWas(im.Substring(2), SqlTable);
                else
                    AdditionalMass.Push(im);
            }
            if (error != "")
                return error;
            else
            {
                string[] text = SqlTable.ElementAt(SqlTable.Count - 1).Split('/');
                return text[2];
            }
        }
        // проверка на существование выполняемого действия в истории действий
        private void ItWas(string im, Queue<string> SqlTable)
        {
            second = AdditionalMass.Pop();
            first = AdditionalMass.Pop();
            string[] s = second.Split('.'),f = first.Split('.');
            str = first + "/" + second + "/" + im + "/";
            for (int i = 0; i < SqlWas.Count; i++)
            {
                if (SqlWas.ElementAt(i).Contains(str))
                {
                    neverwas = false;
                    string[] SqlD = SqlWas.ElementAt(i).Split('/');
                    AdditionalMass.Push(SqlD[3]);
                    break;
                }
            }
            if (neverwas)
            {
                if(im=="+")
                    SqlUnionAll(f, s, TableСheck(f, s, SqlTable, im), SqlTable);
                else
                    SqlInnerJoin(f, s, TableСheck(f, s, SqlTable, im), im, SqlTable);
            }
        }
        //Inner Join
        private void SqlInnerJoin(string[] f, string[] s, string[] selectandgroup, string doing, Queue<string> SqlTable)
        {
            string str3 = first+"/"+second+"/"+doing;
            string code = $"SELECT {selectandgroup[0]} SUM({first}*{second}) AS {f[1]}\n";
            // проверка элементов
            code += FirstElemWriter(f, SqlTable) + $" Inner Join " + SecondElemWriter(f, s, doing, SqlTable);
            // создания строки ON
            code += $"\nON {selectandgroup[4].Remove(selectandgroup[4].Length - 4, 4)}";
            //создания строки для группировки
            if (selectandgroup[0] != "")
                code += $"\nGROUP BY {selectandgroup[0]}{f[0]}.{f[1]}";
            //добавление нового элемента в очереди и стеки
            AdditionalMass.Push($"IJ{ij}.{f[1]}");
            SqlTable.Enqueue($"IJ{ij}/{selectandgroup[3].Remove(selectandgroup[3].Length - 1, 1)}/{code}");
            SqlWas.Enqueue($"{str3}/IJ{ij}.{f[1]}");
            ij++;
        }

        // UNION ALL
        private void SqlUnionAll(string[] f, string[] s, string[] selectandgroup, Queue<string> SqlTable)
        {
            if (error == "")
            {
                string str3 = first + "/" + second + "/+";
                //Первый элемент
                string code = $"SELECT {selectandgroup[0].Replace($"{f[0]}.", $"UA{ua}.")} SUM(UA{ua}.{f[1]}) AS {f[1]} FROM\n" +
                $"(SELECT {selectandgroup[0]}{f[0]}.{f[1]} ";
                code += FirstElemWriter(f, SqlTable);
                //Создание подтаблицы для 2 элемента, на основе общих ключей, для дальнейшего выполнения UNION ALL
                code += $" Union All SELECT UA0{ua}.* FROM\n";
                //Второй элемент
                code += $"(SELECT {selectandgroup[0].Replace($"{f[0]}.", $"{s[0]}.")}{second} FROM ";
                code += SecondElemWriter(f, s, "", SqlTable) + ")";
                code += $" AS UA0{ua})\n" +
                    $"AS UA{ua}\n";
                if (selectandgroup[0] != "")
                {
                    selectandgroup[0] = selectandgroup[0].Substring(0, selectandgroup[0].Length - 1);
                    code += $"GROUP BY {(selectandgroup[0].Replace($"{f[0]}.", $"UA{ua}."))}\n";
                }
                //добавление нового элемента в очереди и стеки
                if (selectandgroup[3][selectandgroup[3].Length-1] == ',')
                    selectandgroup[3] = selectandgroup[3].Substring(0, selectandgroup[3].Length - 1);
                AdditionalMass.Push($"UA{ua}.{f[1]}");
                SqlTable.Enqueue($"UA{ua}/{selectandgroup[3]}/{code}");
                SqlWas.Enqueue($"{str3}/UA{ua}.{f[1]}");
                ua++;
            }
        }
        private string FirstElemWriter(string[] f, Queue<string> SqlTable)
        {
            string code, str1 = f[0];
            string[] Sqlelem;
            if ((f[0].Length > 2) && ((f[0].Substring(0, 2) == "IJ") || (f[0].Substring(0, 2) == "UA")) &&
                (Num.Contains(f[0].Substring(2, 1))))
            {
                for (int i = SqlTable.Count - 1; i > 0; i--)
                {
                    Sqlelem = SqlTable.ElementAt(i).Split('/');
                    if (Sqlelem[0] == f[0])
                    {
                        str1 = $"({Sqlelem[2]}) AS {f[0]}";
                        break;
                    }
                }
            }
            code = $"FROM {str1}";
            return code;
        }
        private string SecondElemWriter(string[] f,string[] s, string doing, Queue<string> SqlTable)
        {
            string code;
            string str2 = s[0], l_word = "", f_word = "";
            string[] Sqlelem;
            if ((s[0].Length > 2) && ((s[0].Substring(0, 2) == "IJ") || (s[0].Substring(0, 2) == "UA")) &&
                (Num.Contains(s[0].Substring(2, 1))))
            {
                for (int i = SqlTable.Count - 1; i > 0; i--)
                {
                    Sqlelem = SqlTable.ElementAt(i).Split('/');
                    if (Sqlelem[0] == s[0])
                    {
                        l_word = "\n";
                        f_word = $" AS {str2}";
                        str2 = $"({Sqlelem[2]})";
                    }
                }
            }
            if (f[0] == s[0] && doing != "+")
            {
                code = $"{l_word}{str2} AS DUB{dub}";
                dub++;
            }
            else
                code = $"{l_word}{str2}{f_word}";
            return code;
        }
        //проверка списка таблиц для получения элементов нужной таблицы
        private string[] TableСheck(string[] f, string[] s, Queue<string> SqlTable, string doing)
        {
            string[] elem1 = null, elem2 = null, selectandgroup = new string[5];
            // selectandgroup 0 - строка таблица.ключ образуемой таблицы без элементов,
            // над которыми выполняются действия
            // selectandgroup 1 - ключи 1 таблицы
            // selectandgroup 2 - ключи 2 таблицы
            // selectandgroup 3 - ключи образуемой таблицы
            // selectandgroup 4 - строка таблица.ключ для ON
            int k = 2, t = 0;
            string str2 = s[0];
            if (f[0] == s[0])
            {
                str2 = $"DUB{dub}";
                t++;
            }
            for (int i = 0; i < selectandgroup.Length; i++)
                selectandgroup[i] = "";
            //поиск элементов среди таблиц
            for (int i = SqlTable.Count - 1; i > -1 && t < k; i--)
            {
                string[] sa = SqlTable.ElementAt(i).Split('/');
                if (f[0] == sa[0] || s[0] == sa[0])
                {
                    if (f[0] == sa[0])
                        elem1 = sa[1].Split(',');
                    if (s[0] == sa[0])
                        elem2 = sa[1].Split(',');
                    t++;
                }
            }
            //создание строки ON, если Inner Join
            if (doing == "*")
            {
                for(int i = 0; i < elem1.Length; i++)
                {
                    for (int j = 0; j < elem2.Length; j++)
                    {
                        if (elem1[i] == elem2[j])
                        {
                            string word = $"{f[0]}.{elem1[i]}={str2}.{elem1[i]}";
                            selectandgroup[4] += "(" + word + ") AND ";
                        }
                    }
                }
            }
            else if (doing != "+")
            {
                string[] dword = doing.Split(',');
                for (int i = 0; i < dword.Length; i++)
                {
                    if (dword[i].Contains('='))
                    {
                        string[] d = dword[i].Split('=');
                        dword[i] = $"{f[0]}.{d[0]}={str2}.{d[1]}";
                    }
                    else
                        dword[i] = $"{f[0]}.{dword[i]}={str2}.{dword[i]}";
                    selectandgroup[4] += "(" + dword[i] + ") AND ";
                }
            }
            //сборка selectandgroup
            selectandgroup = SAG(f, elem1, selectandgroup, 1);
            selectandgroup = SAG(s, elem2, selectandgroup, 2);
            //проверка на ошибку при Union ALL
            if (doing == "+" && selectandgroup[1] != selectandgroup[2])
                error += $"{f[0]}.{f[1]} и {s[0]}.{s[1]} имеют различия во второстепенных ключах, а именно:\n" +
                    $"среди ключей {f[0]} ({selectandgroup[1].Remove(selectandgroup[1].Length - 1, 1)})\n" +
                    $"есть различия с {s[0]} ({selectandgroup[2].Remove(selectandgroup[2].Length - 1, 1)})\n";
            return selectandgroup;
        }
        private string[] SAG(string[] e, string[] elem, string[] selectandgroup, int o)//сборка selectandgroup
        {
            for (int i = 0; i < elem.Length; i++)
            {
                if (elem[i] != e[1])
                {
                    selectandgroup[o] += $"{elem[i]},";
                    if (selectandgroup[0].Contains($".{elem[i]},") == false &&
                        selectandgroup[4].Contains($"{e[0]}.{elem[i]}") == false)
                    {
                        selectandgroup[0] += $"{e[0]}.{elem[i]},";
                        selectandgroup[3] += $"{elem[i]},";
                    }
                }
                else if (o == 1)
                    selectandgroup[3] = $"{elem[i]},{selectandgroup[3]}";
            }
            return selectandgroup;
        }
        public string GetSqlWas() //вывод собранных таблиц
        {
            string gsw="";
            for (int i = 0; i < SqlWas.Count; i++)
                gsw += $"{SqlWas.ElementAt(i)}\n";
            return gsw;
        }
    }
}
