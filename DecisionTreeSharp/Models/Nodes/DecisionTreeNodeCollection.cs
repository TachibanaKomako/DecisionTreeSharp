using DecisionTreeSharp.Models.FeatureValueAlgorithms;
using DecisionTreeSharp.Models.ValueAlgorithms;
using DecisionTreeSharp.Models.ValueAlgorithms.Concrete;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeSharp.Models.Nodes
{
    internal class DecisionTreeNodeCollection : IEnumerable<DecisionTreeNode>
    {
        private readonly List<DecisionTreeNode> items = new();
        private readonly IValueAlgorithm algorithm;
        private readonly int maxDepth;
        public DecisionTreeNodeCollection(ModelTable table,IValueAlgorithm valueAlgorithm, int maxDepth) 
        {
            algorithm = valueAlgorithm;
            this.maxDepth = maxDepth;
            _ = CreateAndConstruct(table, table.Samples, 0);
        }
        public IEnumerator<DecisionTreeNode> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        private DecisionTreeNode? CreateAndConstruct(ModelTable table,int allSamples,int depth)
        {

            //初期化処理
            string[] columnNames = table.GetColumnNames().ToArray();
            double featureValue = algorithm.FeatureValueAlgorithm.Calculate(table);
            if (columnNames.Length == 0 || featureValue == double.NegativeZero)
            {
                var _current = new DecisionTreeNode("", featureValue, 0, table.Samples, allSamples);
                items.Add(_current);
                return _current;
            }
            //深さが一定以上
            if (depth == maxDepth)
            {
                return null;
            }

            //特徴量とその分割量を表すデータを返す
            (string columnName, double featureValue, double value, bool isNA) GetValue(string columnName)
            {
                (double value, double featureValue, bool isNA) = algorithm.Calculate(table, columnName);
                return (columnName, featureValue, value, isNA);
            }
            var values = columnNames
                                .Select(GetValue)
                                //エラーは省く
                                .Where(r => r.isNA is false)
                                .OrderBy(r => r.featureValue)
                                .ToArray();
            //分割出来るものがない時
            if (values.Length == 0)
            {
                var _current = new DecisionTreeNode("", featureValue, 0, table.Samples, allSamples);
                items.Add(_current);
                return _current;
            }
            (string columnName, double value) = values
                                                //分割後の特徴量が最小のものを選択する
                                                .OrderBy(r => r.featureValue)
                                                .Select(r => (r.columnName, r.value))
                                                .First();
            //対象のデータを返す
            var current = new DecisionTreeNode(columnName, featureValue, value, table.Samples, allSamples);
            items.Add(current);
            //分割
            var splitTables = table.Split(columnName, value);
            //下層を作成する
            foreach (var splitTable in splitTables)
            {
                var child = CreateAndConstruct(splitTable, allSamples, depth + 1);
                if(child is not null)
                {
                    current.Children.Add(child);
                }
            }
            return current;
        }
    }
}
