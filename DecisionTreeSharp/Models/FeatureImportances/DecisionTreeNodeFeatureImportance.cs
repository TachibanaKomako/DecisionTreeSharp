using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeSharp.Models.Nodes;

namespace DecisionTreeSharp.Models.FeatureImportances
{
    public class DecisionTreeNodeFeatureImportance
    {
        public DecisionTreeNodeFeatureImportance(DecisionTreeNode[] nodes, double allNodeWaitedSums)
        {
            Name = nodes[0].Name;
            Nodes = nodes;
            FeatureImportance = nodes.Sum(r => r.G) / allNodeWaitedSums;
        }
        public string Name { get; private init; }
        public double FeatureImportance { get; private init; }
        public DecisionTreeNode[] Nodes { get; private init; }
    }
}
