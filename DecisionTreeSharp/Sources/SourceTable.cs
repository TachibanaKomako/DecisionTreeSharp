using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeSharp.Sources
{
    public class SourceTable
    {
        private readonly List<string> columns;
        private readonly List<object?[]> values;
        private readonly List<Type> types;
        public object?[] this[int column] => values[column];
        public object? this[int column, int row] => values[column][row];
        public object?[] this[string columnName] => this[columns.IndexOf(columnName)];
        public object? this[string columnName, int row] => this[columnName][row];
        public IEnumerable<string> GetColumns() => columns;
        public Type GetType(string columnName)
        {
            return types[columns.IndexOf(columnName)];
        }
        public SourceTable(params (string column, object?[] values, Type type)[] items)
        {
            columns = items.Select(r => r.column).ToList();
            values = items.Select(r => r.values).ToList();
            types = items.Select(r => r.type).ToList();
        }
        public SourceTable DropColumn(params string[] dropColumnNames)
        {
            HashSet<string> _dropColumnNames = new(dropColumnNames);
            List<(string, object?[], Type)> newItems = new();
            for (int i = 0; i < columns.Count; i++)
            {
                if (_dropColumnNames.Contains(columns[i]) is false)
                {
                    newItems.Add((columns[i], values[i], types[i]));
                }
            }
            return new SourceTable(newItems.ToArray());
        }
        /// <summary>
        /// NULLになっている項目を削除する
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public SourceTable DropNA(string columnName)
        {
            //NULLになっているINDEXを取得する
            HashSet<int> nullIndexes = this[columnName].Select((value, index) => (value, index)).Where((r) => r.value is null).Select(r => r.index).ToHashSet();
            //データを整理する
            List<(string column, object?[], Type)> items = new();
            for (int i = 0; i < columns.Count; i++)
            {
                List<object?> newValues = new();
                for (int j = 0; j < values[i].Length; j++)
                {
                    if (nullIndexes.Contains(j) is false)
                    {
                        newValues.Add(values[i][j]);
                    }
                }
                items.Add((columns[i], newValues.ToArray(), types[i]));
            }
            return new SourceTable(items.ToArray());
        }
    }
}
