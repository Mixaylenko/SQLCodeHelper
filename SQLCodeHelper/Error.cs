using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLCodeHelper
{
    public class Error
    {
        public void Error_log(string baseName, string equation, DataGridView dgv, out string e, out bool b)
        {
            b = false; e = ""; // начальные значения: Ошибок нет


            //if (string.IsNullOrEmpty(equation))
            //    return ShowError("Введите уравнение");

            // 2. Проверка скобок
            //if (!CheckBrackets(equation))
            //    return ShowError("Неверная расстановка скобок");

            // 3. Проверка существования таблиц
            //var tables = ExtractTables(equation);
            //foreach (var table in tables)
            //{
            //    if (!TableExistsInDGV(table, dgv))
            //        return ShowError($"Таблица '{table}' не найдена в метаданных");
            //}


        }

        private bool CheckBrackets(string equation)
        {
            int balance = 0;
            foreach (char c in equation)
            {
                if (c == '(') balance++;
                if (c == ')') balance--;
                if (balance < 0) return false;
            }
            return balance == 0;
        }

        private List<string> ExtractTables(string equation)
        {
            // Убираем операторы и скобки, оставляем только идентификаторы таблиц
            var operators = new[] { '+', '-', '*', ':', '(', ')', '/', ' ' };
            var parts = equation.Split(operators, StringSplitOptions.RemoveEmptyEntries);
            return parts.Where(p => !int.TryParse(p, out _)).Distinct().ToList();
        }

        private bool TableExistsInDGV(string tableName, DataGridView dgv)
        {
            // Проверяем, есть ли таблица в заголовках столбцов
            for (int col = 1; col < dgv.ColumnCount; col++)
            {
                if (dgv.Rows[0].Cells[col].Value?.ToString() == tableName)
                    return true;
            }
            return false;
        }

        private bool ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK);
            return true;
        }
    }
}