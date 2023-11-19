using DecisionTreeSharp.Models.FeatureImportances;
using DecisionTreeSharp.Models.FeatureValueAlgorithms;
using DecisionTreeSharp.Models.FeatureValueAlgorithms.Concrete;
using DecisionTreeSharp.Models.Nodes;
using DecisionTreeSharp.Models.ValueAlgorithms;
using DecisionTreeSharp.Models.ValueAlgorithms.Concrete;
using DecisionTreeSharp.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeSharp.Models
{
    public class DecisionTreeModel
    {
        public SourceTable? Source { get; set; }
        public string? TargetColumnName { get; set; }
        public IValueAlgorithm ValueAlgorithm { get; set; } = new ValueAlgorithm(new CARTFeatureValueAlgorithm());
        public IFeatureValueAlgorithm FeatureValueAlgorithm { get => ValueAlgorithm.FeatureValueAlgorithm;set=> ValueAlgorithm.FeatureValueAlgorithm = value; }
        public DecisionTreeNode Node => nodes?.First() ?? throw new ArgumentNullException(nameof(nodes));
        public DecisionTreeNodeFeatureImportance[] FeatureImportances => importances ?? throw new ArgumentNullException(nameof(importances));

        private DecisionTreeNode[]? nodes;
        private DecisionTreeNodeFeatureImportance[]? importances;

        public void Make()
        {
            if(Source is null)
            {
                throw new NullReferenceException(nameof(Source));
            }
            if(TargetColumnName is null)
            {
                throw new NullReferenceException(nameof(TargetColumnName));
            }

            ModelTable modelTable = new(Source,TargetColumnName);
            /*
            DecisionTreeNodeFactory factory = new(ValueAlgorithm, 100);
            nodes = factory.Create(modelTable).ToArray();
            */
            nodes = new DecisionTreeNodeCollection(modelTable, ValueAlgorithm, 100).ToArray();
            importances = DecisionTreeNodeFeatureImportanceFactory.Create(nodes ?? throw new NullReferenceException(nameof(nodes))).ToArray();
        }
    }
}
