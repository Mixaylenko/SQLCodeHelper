using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SQLCodeHelper
{
    internal class WorkRPN
    {
        private string LAlf = "λlLμ", LetterAlf = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private string SymbolAlf = "+*";
        private Queue<string> ArrayEquation = new Queue<string>();
        public Queue<string> Do(string eq)
        {
            ArrayEquation.Clear();      //сброс массива
            RPN(eq, 0, "*");            //вход в цикл создания обратной польской записи
            return ArrayEquation;
        }
        // обратная польская запись
        private int RPN(string eq, int i, string doing)
        {
            Queue<string> ArraySUM = new Queue<string>();
            int t = 0, m = i;
            i = СycleTest(eq, i);
            if(i < eq.Length)
            {
                while (SymbolAlf.Contains(eq[m]) == false)
                    m++;
                i = SymbRead(eq, i, m, ArraySUM, doing, 1) + 1;
                for (t = i; t < eq.Length; t++)
                {
                    if (eq[t] == ')')
                    {
                        t++;
                        break;  // выход из цикла на следующем после ')' элемента
                    }
                    else
                        t = SymbRead(eq, t + 1, t, ArraySUM, doing, 0);
                }
                if (ArraySUM.Count > 0)
                    ArrayEquation.Enqueue(ArraySUM.Dequeue());
            }
            return t;
        }
        // чтение и запись элемента (параметр m на выходе оторажает положение последней буквы элемента)
        private int ElemRead(string eq, int t)
        {
            int m;
            for (m = t; m < eq.Length; m++)
                if ((LetterAlf.Contains(eq[m]) || eq[m]=='.') == false) { break; }
            ArrayEquation.Enqueue(eq.Substring(t, m - t));
            return --m;
        }
        // учёт знака действия
        private int SymbRead(string eq, int t, int m, Queue<string> ArraySUM, string doing, int num)
        {
            if (eq[m] == '*')
            {
                if ((t == eq.Length - 1) || (LetterAlf.Contains(eq[t])) && (eq[t + 1] != ','))
                    t = ElemRead(eq, t); // получаем положение последней буквы элемента
                else
                    t = СycleTest(eq, t) - 1; // получаем положение окончания цикла ( на символе ')')
                if(num == 0)
                    ArrayEquation.Enqueue(doing);
            }
            else if (eq[m] == '+')
            {
                if (num == 0)
                {
                    if (ArraySUM.Count == 0)
                        ArraySUM.Enqueue("+");
                    else
                        ArrayEquation.Enqueue("+");
                }
                if ((t == eq.Length - 1) || (LetterAlf.Contains(eq[t])) && (eq[t + 1] != ','))
                    t = ElemRead(eq, t); // получаем положение последней буквы элемента
                else
                    t = СycleTest(eq, t) - 1; // получаем положение окончания цикла ( на символе ')')
            }
            return t;
        }
        // рекурсия
        private int СycleTest(string eq, int t)
        {
            if ((LAlf.Contains(eq[t]) && (eq[t + 1] == ','))) 
            {
                int m;
                for (m = t; m < eq.Length; m++)
                    if (eq[m] == '(') { break; }
                t = RPN(eq, ++m, eq.Substring(t, m - t - 1));
            }
            else if (eq[t] == '(') 
            {
                t++;
                t = RPN(eq, t, "*");
            }
            return t;
        }
    }
}
