using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeSharp.Sources;

namespace Gini.DecisionTrees.Encoders
{
    public class OneHotEncoder
    {
        private readonly HashSet<string> drops;
        public OneHotEncoder(string[]? drops = null)
        {
            this.drops = new(drops ?? Array.Empty<string>());
        }
        public SourceTable Encoding(SourceTable dataFrame)
        {
            //結果
            List<(string, object?[],Type)> newItems = new();
            //dropし、
            string[] target_columns = dataFrame.GetColumns().Where(r => drops.Contains(r) is false).ToArray();

            foreach (string target_column in target_columns)
            {
                newItems.AddRange(EncodingColumn(dataFrame, target_column));
            }

            return new SourceTable(newItems.ToArray());
        }

        //
        private IEnumerable<(string, object?[],Type)> EncodingColumn(SourceTable dataFrame, string columnName)
        {
            //数値データの時は無視する
            if(dataFrame.GetType(columnName) == typeof(double?))
            {
                yield return (columnName, dataFrame[columnName], dataFrame.GetType(columnName));
                yield break;
            }
            //スペースのみも除外する
            string ToValueString(object? value)
            {
                string valueString = value?.ToString() ?? string.Empty;
                return string.IsNullOrWhiteSpace(valueString) ? string.Empty : valueString;
            }
            //値の一覧を取得する
            string[] valueStrings = dataFrame[columnName].Select(ToValueString).ToArray();
            //OneHot
            IEnumerable<object?> ToValues(string valueGroup)
            {
                foreach (string value in valueStrings)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        yield return null;
                    }
                    else
                    {
                        yield return value == valueGroup ? 1.0 : 0.0;
                    }
                }
            }
            //グルーピングした
            string[] valueGroups = valueStrings
                                    .GroupBy(r => r)
                                    //欠損値は除外
                                    .Where(r => string.IsNullOrEmpty(r.Key) is false)
                                    .Select(r => r.Key).ToArray();
            //結果を返す
            foreach (string valueGroup in valueGroups)
            {
                yield return ($"{columnName}_{valueGroup}", ToValues(valueGroup).ToArray(),typeof(double?));
            }
        }
    }
}
