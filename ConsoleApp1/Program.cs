using DecisionTreeSharp.Models;
using DecisionTreeSharp.Models.Nodes;
using DecisionTreeSharp.Sources;

var reader = new SourceTableReader();
var sourceTable = reader.Read("test3.csv");
var model = new DecisionTreeModel() { Source = sourceTable,TargetColumnName= "survived" };
model.Make();

//*
DecisionTreeNode node = model.Node;
void DispNode(DecisionTreeNode _node,int depth)
{
    double g = (_node.NodeSamples * _node.FeatureValue) - _node.Children.Sum(r => r.FeatureValue * r.NodeSamples);
    Console.WriteLine($"{depth},{_node.Name},{_node.FeatureValue:0.00000},{_node.NodeSamples},{g},{_node.G}");
    foreach(DecisionTreeNode child in _node.Children)
    {
        DispNode(child,depth+1);
    }
}

DispNode(node, 0);
//*/
foreach(var importance in model.FeatureImportances)
{
    Console.WriteLine($"{importance.Name},{importance.FeatureImportance:0.00}");
}
//*/


