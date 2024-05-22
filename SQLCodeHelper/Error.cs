using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCodeHelper
{
    internal class Error
    {
        private string AllAlf = "=.,()+*λμ0123456789абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string yourmistake;
        // проверка на знание правил 1 класса (примеры со кобками), правил SQL и использования λ-свёрнутых уравнений
        // в разработке
        public bool Error_log(string NameBase, string eq, string TableAndKeys)
        {
            bool e = true;
            int error = Record_check(NameBase, eq, TableAndKeys);
            e = Error_list(error, e);
            if (error == 0)
                e = false;
            return e;
        }
        private int Record_check(string NameBase, string eq, string TableAndKeys)
        {
            int p = 0;
            //Здесь будет анализ вводимых данны, но пока пусто :)

            return p;
        }
        public bool Error_list(int err, bool e)
        {
            if (err == 1)
                yourmistake = "Не правильное расположение скобок\n" +
                    "Incorrect placement of brackets";
            else if (err == 2)
                yourmistake = "Использован недопустимый символ\n" +
                    "Invalid symbol used";
            else if (err == 3)
                yourmistake = "Символ +,* не может находиться в начале или конце уравнения\n" +
                    "The symbol +,* cannot be at the beginning or end of the equation";
            else if (err == 4)
                yourmistake = "Символ +,* должен находиться между операндами (переменными), перед '(' или после ')'\n" +
                    "The symbol +,* must be between the operands (variables), before '(' or after ')'";
            else if (err == 5)
                yourmistake = "Введите уравнение для выполнения обработки(Пример: l,Mat(Ar.Prod*C.Prod)+Q.Prod)\n" +
                    "Enter the equation to perform the processing(Example: l,Mat(Ar.Prod*C.Prod)+Q.Prod)";
            else if (err == 6)
                yourmistake = "В скобках должно быть осуществленно хотя бы 1 действие\n" +
                    "";
            return e;
        }
    }
}
