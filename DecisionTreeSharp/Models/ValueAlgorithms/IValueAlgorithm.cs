using DecisionTreeSharp.Models.FeatureValueAlgorithms;
using DecisionTreeSharp.Sources;

namespace DecisionTreeSharp.Models.ValueAlgorithms
{
    public interface IValueAlgorithm
    {
        IFeatureValueAlgorithm FeatureValueAlgorithm { get; set; }
        (double value, double featureValue,bool isNA) Calculate(ModelTable table, string columnName);
    }
}
