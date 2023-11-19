using DecisionTreeSharp.Sources;
using Gini.DecisionTrees.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeSharp.Models
{
    public class ModelTable
    {
        private readonly Dictionary<string, double?[]> values;
        private readonly bool[] actives;
        public double?[] this[string columnName] => values[columnName];
        public bool[] GetActives()
        {
            return actives;
        }
        public int Samples => actives.Length;
        public IEnumerable<string> GetColumnNames() => values.Keys;
        public ModelTable(SourceTable dataFrame, string targetColumnName)
        {
            bool?[] targetValues = Utils.GetBooleans(dataFrame, targetColumnName, r => r?.Equals(1.0)).ToArray();
            if (targetValues.Any(r => r is null))
            {
                throw new NullReferenceException(targetColumnName);
            }
            actives = targetValues.Select(x => x ?? false).ToArray();

            //OneHotEncoderで整える
            OneHotEncoder encoder = new(new[] { targetColumnName });
            //
            SourceTable encoded_df = encoder.Encoding(dataFrame);
            string[] columnNames = encoded_df.GetColumns().ToArray();

            values = new();
            for (int i = 0; i < columnNames.Length; i++)
            {
                values.Add(columnNames[i], Utils.GetNumbers(encoded_df, columnNames[i]));
            }
        }
        private ModelTable(Dictionary<string, double?[]> values, bool[] actives)
        {
            this.values = values;
            this.actives = actives;
        }
        public ModelTable[] Split(string columnName, double value)
        {
            //**********
            //初期設定
            //**********
            List<(Dictionary<string, List<double?>> values, List<bool> actives)> items = new()
            {
                (new Dictionary<string, List<double?>>(), new List<bool>()),
                (new Dictionary<string, List<double?>>(), new List<bool>())
            };
            //カラム名の設定
            foreach (string c in values.Keys)
            {
                if (c != columnName)
                {
                    foreach (var item in items)
                    {
                        item.values.Add(c, new List<double?>());
                    }
                }
            }

            //**********
            //メイン処理
            //**********
            //行で追加するメソッド
            void AddRow(int row, int itemIndex)
            {
                foreach (string c in values.Keys)
                {
                    if (c != columnName)
                    {
                        items[itemIndex].values[c].Add(values[c][row]);
                    }
                }
                items[itemIndex].actives.Add(actives[row]);
            }
            //
            for (int r = 0; r < actives.Length; r++)
            {
                double? v = values[columnName][r];
                if (v == null)
                {
                    AddRow(r, 0);
                    AddRow(r, 1);
                }
                else if (v <= value)
                {
                    AddRow(r, 0);
                }
                else
                {
                    AddRow(r, 1);
                }
            }
            //**********
            //終了処理
            //**********
            return items
                .Select(r => new ModelTable(
                    r.values.ToDictionary(r1 => r1.Key, r1 => r1.Value.ToArray()),
                    r.actives.ToArray()))
                .ToArray();
        }
        public ModelTable Drop(string dropColumnName)
        {
            Dictionary<string, double?[]> newValues = new();
            foreach (string columnName in values.Keys)
            {
                if (columnName != dropColumnName)
                {
                    newValues.Add(columnName, values[columnName]);
                }
            }
            return new(newValues, actives);
        }
    }
}
