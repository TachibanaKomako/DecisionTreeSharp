using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeSharp.Models.Nodes;

namespace DecisionTreeSharp.Models.FeatureImportances
{
    internal class DecisionTreeNodeFeatureImportanceFactory
    {
        public static IEnumerable<DecisionTreeNodeFeatureImportance> Create(DecisionTreeNode[] allNodes)
        {
            var nodes = allNodes.Where(r => r.IsEmpty is false).ToArray();
            double allNodeWeightedSum = nodes.Sum(r => r.G);
            foreach (var group in nodes.GroupBy(r => r.Name))
            {
                yield return new DecisionTreeNodeFeatureImportance(group.ToArray(), allNodeWeightedSum);
            }
        }
    }
}
