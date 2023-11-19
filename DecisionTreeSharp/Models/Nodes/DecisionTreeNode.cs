using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeSharp.Models.Nodes
{
    public class DecisionTreeNode
    {
        public DecisionTreeNode(string name, double featureValue, double value, int nodeSamples, int allSamples)
        {
            Name = name;
            FeatureValue = featureValue;
            Value = value;
            AllSamples = allSamples;
            NodeSamples = nodeSamples;
        }
        public bool IsEmpty => string.IsNullOrEmpty(Name);
        public string Name { get; private init; }
        public double FeatureValue { get; private init; }
        public double Value { get; private init; }
        public int AllSamples { get; private init; }
        public int NodeSamples { get; private init; }
        public double NI => FeatureValue * NodeSamples;
        public double G => NI - Children.Sum(r => r.NI);
        public double GetWeightedSum()
        {
            double ip = NodeSamples * FeatureValue;
            double ilp = Children.Sum(r => r.NodeSamples * r.FeatureValue);
            
            return (ip - ilp) / AllSamples;
        }
        public List<DecisionTreeNode> Children { get; } = new();
    }
}
