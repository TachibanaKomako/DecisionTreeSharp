using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeSharp.Sources;

namespace DecisionTreeSharp.Models.FeatureValueAlgorithms
{
    public interface IFeatureValueAlgorithm
    {
        double Calculate(ModelTable df, string columnName, double value);
        double Calculate(ModelTable df);
    }
}
