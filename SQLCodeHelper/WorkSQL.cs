using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SQLCodeHelper
{
    internal class WorkSQL
    {
        public class QueryInfo
        {
            public string Name { get; set; }
            public string NameWithKey { get; set; }
            public string SQL { get; set; }
            public string LeftOperand { get; set; }
            public string RightOperand { get; set; }
            public string Operation { get; set; }
            public string BaseName { get; set; }
            // Исходные массивы — не отображаются в DataGridView
            [Browsable(false)]
            public string[] ColumnsTable { get; set; }

            [Browsable(false)]
            public string[] KeyColumns { get; set; }

            [Browsable(false)]
            public string[] ColumnsTableLeft { get; set; }

            [Browsable(false)]
            public string[] ColumnsTableRight { get; set; }

            // Строковые представления для отображения
            public string ColumnsTableDisplay => ColumnsTable != null ? string.Join(", ", ColumnsTable) : "";
            public string KeyColumnsDisplay => KeyColumns != null ? string.Join(", ", KeyColumns) : "";
            public string ColumnsTableLeftDisplay => ColumnsTableLeft != null ? string.Join(", ", ColumnsTableLeft) : "";
            public string ColumnsTableRightDisplay => ColumnsTableRight != null ? string.Join(", ", ColumnsTableRight) : "";
        }
        public List<QueryInfo> GetQueriesFromDatabase()
        {
            return generatedQueries.ToList();
        }
        private int uaCount = 1;
        private int ijCount = 1;
        private int ljCount = 1;
        private int rjCount = 1;

        private Dictionary<string, string[]> TableList = new Dictionary<string, string[]>
        {
            ["firstlist"] = Array.Empty<string>(),
            ["firstSQL"] = Array.Empty<string>(),
            ["secondlist"] = Array.Empty<string>(),
            ["secondSQL"] = Array.Empty<string>()
        };

        private List<QueryInfo> generatedQueries = new List<QueryInfo>();
        private Stack<string> ElemStack = new Stack<string>();
        private string sqlbasename;
        public string CodeCreate(Queue<string> QueRPN, string[][] sqlbase, string SqlBaseName)
        {
            try
            {
                ElemStack.Clear();
                sqlbasename = SqlBaseName;
                AddorUpdateTableDefinitions(sqlbase);
                while (QueRPN.Count > 0)
                {
                    string elem = QueRPN.Dequeue();
                    if (elem.Contains("+") || elem.Contains("-") || elem.Contains("*") || elem.Contains(":"))
                    {
                        string right = ElemStack.Pop();
                        string left = ElemStack.Pop();

                        var existing = Check(right, left, elem);
                        if (existing != null)
                            ElemStack.Push(existing.NameWithKey);
                        else
                            SqlProcess(right, left, elem);
                    }
                    else
                    {
                        if (elem[0] == '.')
                            elem = ElemStack.Pop().Split('.')[0] + elem;
                        ElemStack.Push(elem);
                    }
                }
                string s = ElemStack.Pop();
                var result = generatedQueries.FirstOrDefault(q => q.NameWithKey == s && q.BaseName == sqlbasename);
                return result.SQL;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        // Проверка взаимодействующих таблиц
        public QueryInfo Check(string rightOperand, string leftOperand, string operation)
        {
            foreach (var key in TableList.Keys.ToArray())
                TableList[key] = Array.Empty<string>();

            ForTable("first",leftOperand);
            ForTable("second", rightOperand);

            return generatedQueries.FirstOrDefault(q =>
                q.LeftOperand == leftOperand &&
                q.RightOperand == rightOperand &&
                q.Operation == operation &&
                q.ColumnsTableLeft == TableList["firstlist"] &&
                q.ColumnsTableRight == TableList["secondlist"] &&
                q.BaseName == sqlbasename);
        }
        void ForTable(string num,string Operand)
        {
            string[] name = Operand.Split('.', ',');
            var existing = generatedQueries.FirstOrDefault(q => q.NameWithKey == Operand && q.BaseName == sqlbasename);
            if (existing != null)
            {
                TableList[$"{num}list"] = existing.ColumnsTable;
                TableList[$"{num}SQL"] = new[] { existing.SQL };
            }
            else
                foreach (var q in generatedQueries)
                    if (q.Name == name[0] && q.BaseName == sqlbasename)
                    {
                        TableList[$"{num}list"] = q.ColumnsTable;
                        TableList[$"{num}SQL"] = new[] { q.SQL };
                        break;
                    }
        }
        // Основной метод
        public void SqlProcess(string rightOperand, string leftOperand, string operation)
        {
            var parsedOp = ParseOperation(operation);
            var left = GetOperandColumns(leftOperand,parsedOp);
            var right = GetOperandColumns(rightOperand,parsedOp);
            
            string[] leftParts = leftOperand.Split('.', ',');
            string[] rightParts = rightOperand.Split('.', ',');

            string fb = GetFromBlock(leftParts[0], TableList["firstSQL"]);
            string sb = GetFromBlock(rightParts[0], TableList["secondSQL"]);

            string sql;
            string newTableName;
            string[] resultColumns;

            
            if (parsedOp.Type == OperationType.UnionAll)
            {
                sql = GenerateUnionAllSql(leftParts, rightParts,
                                      left.allKey, left.opKey,
                                      right.allKey, right.opKey,
                                      parsedOp, fb, sb,
                                      out newTableName, out resultColumns);
            }
            else
            {
                sql = GenerateJoinSql(leftParts, rightParts,
                                          left.allKey, left.opKey,
                                          right.allKey, right.opKey,
                                          parsedOp, fb, sb,
                                          out newTableName, out resultColumns);
            }

            var newQuery = new QueryInfo
            {
                LeftOperand = leftOperand,
                RightOperand = rightOperand,
                Operation = operation,
                ColumnsTableLeft = TableList["firstlist"],
                ColumnsTableRight = TableList["secondlist"],
                BaseName = sqlbasename,
                SQL = sql,
                Name = newTableName,
                NameWithKey = $"{newTableName}.{leftOperand.Split('.')[1]}",
                ColumnsTable = resultColumns,
                KeyColumns = left.opKey
            };
            generatedQueries.Add(newQuery);

            ElemStack.Push(newQuery.NameWithKey);
        }
        // ---------- Внутренние типы ----------
        private enum OperationType { UnionAll, Join }
        private enum JoinKind { Left, Right, Inner }

        
        // Класс для полной информации об операции
        private class ParsedOperation
        {
            public string Operator { get; set; }
            public OperationType Type { get; set; }
            public JoinKind? Join { get; set; }
            public List<(string Type, string Value)> Ordered { get; set; } = new List<(string, string)>();
        }

        //ввод исходных таблиц
        public void AddorUpdateTableDefinitions(string[][] tableDefinitions)
        {
            foreach (var def in tableDefinitions)
            {
                // Пропускаем пустые определения
                if (def == null || def.Length == 0) continue;

                string tableName = def[0];
                // Столбцы — все элементы, кроме первого
                string[] columns = def.Skip(1).ToArray();

                // Ищем существующую запись с таким же именем и sqlbasename
                var existing = generatedQueries.FirstOrDefault(q => q.Name == tableName && q.BaseName == sqlbasename);

                if (existing != null)
                {
                    // Проверяем, изменился ли набор столбцов
                    bool columnsChanged = existing.ColumnsTable == null ||
                                          !existing.ColumnsTable.SequenceEqual(columns);

                    if (columnsChanged)
                        // Обновляем столбцы
                        existing.ColumnsTable = columns;
                }
                else
                {
                    // Создаём новую запись
                    var newQuery = new QueryInfo
                    {
                        Name = tableName,
                        BaseName = sqlbasename,
                        ColumnsTable = columns
                    };
                    generatedQueries.Add(newQuery);
                }
            }
        }

        // Новая функция централизованной обработки условия с сохранением порядка
        private ParsedOperation ParseOperation(string conditionStr)
        {
            var result = new ParsedOperation();
            string[] tokens = conditionStr.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) return result;

            result.Operator = tokens[0];
            result.Type = (result.Operator == "*" || result.Operator == "+") ? OperationType.UnionAll : OperationType.Join;

            if (result.Type == OperationType.Join)
            {
                result.Join = result.Operator switch
                {
                    "+/" or "*/" => JoinKind.Left,
                    "/+" or "/*" => JoinKind.Right,
                    "/+/" or "/*/" => JoinKind.Inner,
                    _ => JoinKind.Inner
                };
            }

            int o = 1;
            var rules = result.Ordered;
            while (o < tokens.Length)
            {
                string current = tokens[o];
                if (string.IsNullOrEmpty(current)) { o++; continue; }

                char ruleType = current[0];
                int slashIndex = current.IndexOf('/');
                string firstArg = slashIndex >= 0 ? current.Substring(slashIndex + 1) : "";
                if (!string.IsNullOrEmpty(firstArg))
                {
                    rules.Add((ruleType.ToString(), firstArg));
                }

                o++;

                while (o < tokens.Length && !tokens[o].Contains('/'))
                {
                    string arg = tokens[o];
                    rules.Add((ruleType.ToString(), arg));
                    o++;
                }
            }

            return result;
        }

        private string GetNextName(string prefix)
        {
            int counter = prefix switch
            {
                "UA" => uaCount,
                "IJ" => ijCount,
                "LJ" => ljCount,
                "RJ" => rjCount,
                _ => 1
            };

            while (generatedQueries.Any(q => q.Name == $"{prefix}{counter}"))
                counter++;

            switch (prefix)
            {
                case "UA": uaCount = counter + 1; break;
                case "IJ": ijCount = counter + 1; break;
                case "LJ": ljCount = counter + 1; break;
                case "RJ": rjCount = counter + 1; break;
            }
            return $"{prefix}{counter}";
        }

        private string GetFromBlock(string tableInfo, string[] sqlArray)
        {
            if (sqlArray != null && sqlArray.Length > 0 && !string.IsNullOrEmpty(sqlArray[0]))
                return $"({sqlArray[0]}) AS {tableInfo}";
            return tableInfo;
        }
        // Применение правил m к спискам столбцов
        private (string[] allKey, string[] opKey) GetOperandColumns(string operand,ParsedOperation parsedOp)
        {
            string[] parts = operand.Split('.', ',');
            string name = parts[0];
            string[] opKey = parts.Skip(1).Where(s => !string.IsNullOrEmpty(s)).ToArray();

            var existing = generatedQueries.FirstOrDefault(q => q.Name == name && q.BaseName == sqlbasename);
            var mExclude = new HashSet<string>(parsedOp.Ordered
                .Where(x => x.Type == "m")
                .Select(x => x.Value)
                );
            var newKey = existing.ColumnsTable.Where(c => !mExclude.Contains(c)).ToArray();
            newKey = (newKey.Select(s => name + s).ToArray()).Where(c => !mExclude.Contains(c)).ToArray();
            newKey = newKey.Select(s => s.Remove(0, name.Length)).ToArray();
            newKey = newKey.Where(c => !opKey.Contains(c)).ToArray();
            return (newKey, opKey);
        }
        // Парсит строку вида "таблица.элемент" или "элемент"
        private static (string TableName, string ElementName) ParseWithTable(string input)
        {
            if (input.Contains('.'))
            {
                var parts = input.Split('.');
                return (parts[0], parts[1]);
            }
            return (null, input);
        }
        // ---------- UNION ALL ----------
        private string GenerateUnionAllSql(string[] leftParts, string[] rightParts,
                                       string[] leftKeyCols, string[] leftOpCols,
                                       string[] rightKeyCols, string[] rightOpCols,
                                       ParsedOperation op,
                                       string fb, string sb,
                                       out string newTableName,
                                       out string[] resultColumns)
        {
            int opNumber = uaCount;
            newTableName = GetNextName("UA");
            string localName = newTableName;
            var rules = op.Ordered
                .Where(x => x.Type == "p")
                .Select(x => x.Value
                );
            // Обработка правил p
            string[] resultTable = leftKeyCols.ToArray();
            if (rules != null)
            {
                var pRules = new Dictionary<string, string>();
                foreach (var value in rules)
                {
                    var parts = value.Split('=');
                    if (parts.Length != 2) continue;

                    string leftRaw = parts[0].Trim();
                    string rightRaw = parts[1].Trim();

                    // Разбор левой части (должна указывать на первую таблицу)
                    var left = ParseWithTable(leftRaw);
                    if (left.TableName != null && left.TableName != leftParts[0])
                        continue; // Указана не первая таблица → правило не применяется

                    string leftElement = left.ElementName;
                    // Проверка наличия элемента в первой таблице
                    if (!leftKeyCols.Contains(leftElement))
                        continue;

                    // Разбор правой части (должна указывать на вторую таблицу)
                    var right = ParseWithTable(rightRaw);
                    if (right.TableName != null && right.TableName != rightParts[0])
                        continue; // Указана не вторая таблица → правило не применяется

                    string rightElement = right.ElementName;
                    // Проверка наличия элемента во второй таблице
                    if (!rightKeyCols.Contains(rightElement))
                        continue;

                    // Всё корректно — добавляем замену
                    pRules[leftElement] = rightElement;

                }
                for (int i = 0; i < resultTable.Length; i++)
                {
                    if (pRules.TryGetValue(resultTable[i], out string newValue))
                        resultTable[i] = newValue;
                }
            }
            // Левая часть UNION ALL
            var leftSelectCols = new List<string>();
            foreach (var col in leftKeyCols)
                leftSelectCols.Add($"{leftParts[0]}.{col}");
            foreach (var col in leftOpCols)
                leftSelectCols.Add($"{leftParts[0]}.{col}");
            string leftSelect = string.Join(",", leftSelectCols);
            string leftPart = $@"(SELECT {leftSelect}
FROM {fb})";

            // Правая часть UNION ALL
            var rightSelectCols = new List<string>();
            foreach (var col in resultTable)
                rightSelectCols.Add($"{rightParts[0]}.{col}");
            foreach (var col in rightOpCols)
                rightSelectCols.Add($"{rightParts[0]}.{col}");
            string rightSelect = string.Join(",", rightSelectCols);
            string rightAlias = $"UA0{opNumber}";
            string rightPart = $@"SELECT {rightAlias}.* FROM
(SELECT {rightSelect}
FROM {sb}) AS {rightAlias}";

            // Итоговый SELECT
            var finalSelectCols = new List<string>();
            foreach (var col in leftKeyCols)
                finalSelectCols.Add($"{localName}.{col}");
            if (leftOpCols.Any())
            {
                if (op.Operator == "*")
                {
                    foreach (var col in leftOpCols)
                        finalSelectCols.Add($@"
    CASE WHEN COUNT(CASE WHEN {localName}.{col} = 0 THEN 1 END) > 0 THEN 0
         ELSE EXP(SUM(LOG(ABS(NULLIF({localName}.{col}, 0))))) *
              CASE WHEN SUM(CASE WHEN {localName}.{col} < 0 THEN 1 ELSE 0 END) % 2 = 1 THEN -1 ELSE 1 END
    END AS {col}");
                }
                else // "+"
                {
                    foreach (var col in leftOpCols)
                        finalSelectCols.Add($"SUM({localName}.{col}) AS {col}");
                }
            }
            string finalSelect = string.Join(",", finalSelectCols);
            string groupBy = string.Join(",", leftKeyCols.Select(c => $"{localName}.{c}"));

            string sql = $@"SELECT {finalSelect}
FROM ({leftPart} UNION ALL {rightPart}) AS {localName}
GROUP BY {groupBy}";

            resultColumns = leftKeyCols.Concat(leftOpCols).ToArray();
            return sql;
        }

        // ---------- JOIN ----------
        private string GenerateJoinSql(string[] leftParts, string[] rightParts,
                                       string[] leftKeyCols, string[] leftOpCols,
                                       string[] rightKeyCols, string[] rightOpCols,
                                       ParsedOperation op,
                                       string fb, string sb,
                                       out string newTableName,
                                       out string[] resultColumns)
        {
            newTableName = op.Join switch
            {
                JoinKind.Left => GetNextName("LJ"),
                JoinKind.Right => GetNextName("RJ"),
                JoinKind.Inner => GetNextName("IJ"),
                _ => GetNextName("J")
            };
            string localName = newTableName;
            var common = leftKeyCols.Intersect(rightKeyCols).ToList();

            var onClauses = new List<string>();
            var cLeft = new HashSet<string>();
            var cRight = new HashSet<string>();
            bool hasL = false, hasBigL = false;
            var rules = op.Ordered
                .Where(x => x.Type == "l" || x.Type == "L")
                .Select(x => (x.Type, x.Value))
                .ToList();
            //Обработка правил в порядке их появления
            foreach (var (type, value) in rules)
            {
                // Устанавливаем флаги
                if (type == "l") hasL = true;
                if (type == "L") hasBigL = true;

                // Обработка правила
                if (!value.Contains('='))
                {
                    // Простое имя столбца (без '=') — должно быть общим для обеих таблиц
                    if (leftKeyCols.Contains(value) && rightKeyCols.Contains(value))
                    {
                        onClauses.Add($"{leftParts[0]}.{value} = {rightParts[0]}.{value}");
                        if (type == "L")
                        {
                            cLeft.Add(value);
                            cRight.Add(value);
                        }
                    }
                }
                else
                {
                    var parts = value.Split('=');
                    if (parts.Length != 2) continue;

                    string leftRaw = parts[0].Trim();
                    string rightRaw = parts[1].Trim();

                    // Разбор левой части
                    var left = ParseWithTable(leftRaw);
                    if (left.TableName != null && left.TableName != leftParts[0])
                        continue; // Указана не первая таблица → правило не применяется

                    string leftElement = left.ElementName;
                    if (!leftKeyCols.Contains(leftElement))
                        continue;

                    // Разбор правой части
                    var right = ParseWithTable(rightRaw);
                    if (right.TableName != null && right.TableName != rightParts[0])
                        continue; // Указана не вторая таблица → правило не применяется

                    string rightElement = right.ElementName;
                    if (!rightKeyCols.Contains(rightElement))
                        continue;

                    // Всё корректно — добавляем условие JOIN
                    onClauses.Add($"{leftParts[0]}.{leftElement} = {rightParts[0]}.{rightElement}");
                    if (type == "L")
                    {
                        cLeft.Add(leftElement);
                        cRight.Add(rightElement);
                    }
                }
            }

            // Если нет ни l ни L, используем все общие столбцы как L
            if (!hasL && !hasBigL)
            {
                foreach (var col in common)
                {
                    onClauses.Add($"{leftParts[0]}.{col} = {rightParts[0]}.{col}");
                    cLeft.Add(col);
                    cRight.Add(col);
                }
            }
            // исключаем ключи 
            var leftK = leftKeyCols.Except(cLeft).ToList();
            var rightK = rightKeyCols.Except(cRight).ToList();

            common = leftK.Intersect(rightK).ToList();
            var leftOnly = leftK.Except(common).ToList();
            var rightOnly = rightK.Except(common).ToList();

            List<string> allCols;
            string selectCols = "";
            var joinType = op.Join.Value;

            if (joinType == JoinKind.Left)
            {
                allCols = leftOnly.Concat(common).ToList();
                selectCols = string.Join(",", allCols.Select(col => $"{leftParts[0]}.{col}"));
            }
            else if (joinType == JoinKind.Right)
            {
                allCols = common.Concat(rightOnly).ToList();
                if (common.Count > 0) 
                    selectCols = string.Join(",", common.Select(col => $"{leftParts[0]}.{col}"));
                selectCols = string.Join(",", rightOnly.Select(col => $"{rightParts[0]}.{col}"));
            }
            else // Inner
            {
                allCols = common.ToList();
                selectCols = string.Join(",", common.Select(col => $"{leftParts[0]}.{col}"));
            }

            var aggList = new List<string>();
            bool isMultiply = op.Operator.Contains('*');
            bool isAdd = op.Operator.Contains('+');
            for (int i = 0; i < leftOpCols.Length; i++)
            {
                string leftExpr = $"{leftParts[0]}.{leftOpCols[i]}";
                string rightExpr = $"{rightParts[0]}.{rightOpCols[i]}";

                // Определяем, нужно ли оборачивать операнды в COALESCE
                string leftOperand = joinType == JoinKind.Right
                    ? $"COALESCE({leftExpr}, {(isMultiply ? "1" : "0")})"
                    : leftExpr;
                string rightOperand = joinType == JoinKind.Left
                    ? $"COALESCE({rightExpr}, {(isMultiply ? "1" : "0")})"
                    : rightExpr;

                // Строим выражение для SUM
                string operand = isMultiply ? $"{leftOperand} * {rightOperand}"
                                : isAdd ? $"{leftOperand} + {rightOperand}"
                                : joinType == JoinKind.Right ? rightOperand : leftOperand;

                aggList.Add($"SUM({operand}) AS {leftOpCols[i]}");
            }
            string aggCols = "";
            if (aggList.Count != 0)
                aggCols = string.Join(", ", aggList);
            string joinOn = string.Join(" AND ", onClauses);
            if (string.IsNullOrEmpty(joinOn))
                joinOn = "1=1";
            string dot = "";
            if (!string.IsNullOrEmpty(selectCols) && aggCols != "")
                dot = ", ";
            string joinKeyword = joinType switch
            {
                JoinKind.Left => "LEFT JOIN",
                JoinKind.Right => "RIGHT JOIN",
                JoinKind.Inner => "INNER JOIN",
                _ => "INNER JOIN"
            };

            string sql = $@"SELECT {selectCols}{dot}{aggCols}
        FROM {fb} {joinKeyword} {sb}
        ON {joinOn}";
            if (!string.IsNullOrEmpty(selectCols))
            sql += $@"GROUP BY {selectCols}";

            resultColumns = allCols.Concat(leftOpCols).ToArray();
            return sql;
        }


    }
}