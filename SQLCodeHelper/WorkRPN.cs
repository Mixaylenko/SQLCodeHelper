using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace SQLCodeHelper
{
    internal class WorkRPN
    {
        private string SymAlf = "+-*:";
        private Queue<string> EquationArray = new Queue<string>();
        private int t;
        // Список токенов в порядке убывания длины
        private List<string> tokens = new List<string>
        {
            // Пятисимвольные
            "//*//",
            // Трехсимвольные
            "/+/", "/-/", "/*/", "/:/", "//*", "*//",
            // Двухсимвольные
            "/+", "+/", "/-", "-/", "/*", "*/", "/:", ":/",  "++", "--", "**",
            // Однозначные
            "+", "-", "*", ":", "(", ")"
        };

        public Queue<string> Do(string eq)
        {
            EquationArray.Clear();
            RPN(Processing(eq), 0, "");
            return EquationArray;
        }
        private string[] Processing(string eq)
        {
            // Экранируем спецсимволы для regex
            string pattern = string.Join("|", tokens.Select(Regex.Escape));
            // Заменяем каждое вхождение на " {token} "
            return (Regex.Replace(Regex.Replace(eq, pattern, m => " " + m.Value + " "), @"\s+", " ").Trim()).Split(new char[] { ' ' });
        }
        private void RPN(string[] mass, int i, string e)
        {
            Queue<string> AactionMult = new Queue<string>();
            Queue<string> AactionAd = new Queue<string>();
            t = i;
            while (t < mass.Length)
            {
                if (!mass[t].Contains("/") && (mass[t] != ")") && (mass[t] != "(") && !mass[t].Any(c => SymAlf.Contains(c)))
                {
                    EquationArray.Enqueue(mass[t]);
                    if (AactionMult.Count != 0)
                        EquationArray.Enqueue(AactionMult.Dequeue());
                }
                else if (mass[t].Any(c => SymAlf.Contains(c)))
                {
                    if (AactionMult.Count != 0)
                        EquationArray.Enqueue(AactionMult.Dequeue());
                    if (mass[t].Contains("+") || mass[t].Contains("-"))
                    {
                        if (AactionAd.Count != 0)
                            EquationArray.Enqueue(AactionAd.Dequeue());
                        AactionAd.Enqueue(mass[t] + " " + e);
                    }
                    else
                        AactionMult.Enqueue(mass[t] + " " + e);
                }
                else
                {
                    if (mass[t] == "(")
                    {
                        if (!SymAlf.Contains(mass[t - 1])) 
                            RPN(mass, t + 1, mass[t - 1]);
                        else
                            RPN(mass, t + 1, "");
                    }
                    else if (mass[t] == ")")
                        break;
                }
                t++;
            }
            if (AactionMult.Count != 0)
                EquationArray.Enqueue(AactionMult.Dequeue());
            if (AactionAd.Count != 0)
                EquationArray.Enqueue(AactionAd.Dequeue());
        }
    }

}