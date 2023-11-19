using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeSharp.Models.FeatureValueAlgorithms;
using DecisionTreeSharp.Models.ValueAlgorithms;
using DecisionTreeSharp.Sources;

namespace DecisionTreeSharp.Models.ValueAlgorithms.Concrete
{
    internal class ValueAlgorithm : IValueAlgorithm
    {
        public ValueAlgorithm(IFeatureValueAlgorithm featureValueAlgorithm)
        {
            FeatureValueAlgorithm = featureValueAlgorithm;
        }
        public IFeatureValueAlgorithm FeatureValueAlgorithm { get; set; }
        public (double value, double featureValue,bool isNA) Calculate(ModelTable table, string columnName)
        {
            IEnumerable<(double value, double featureValue)> GetFeatureValues()
            {
                //昇順に変更
                double[] numbers = table[columnName]
                        .Where(r => r is not null)
                        .Select(r => r ?? throw new Exception())
                        .GroupBy(r=>r)
                        .OrderBy(r => r.Key)
                        .Select(r=>r.Key)
                        .ToArray();
                for (int i = 0; i < numbers.Length; i++)
                {
                    if (i != 0)
                    {
                        double value = (numbers[i] + numbers[i - 1]) / 2;
                        yield return (value, FeatureValueAlgorithm.Calculate(table, columnName, value));
                    }
                }
            }
            var datas = GetFeatureValues().ToArray();
            if(datas.Length == 0)
            {
                return (0.0, 0.0, true);
            }
            (double value,double featureValue) = GetFeatureValues().OrderBy(r => r.featureValue).First();
            return (value, featureValue, false);
        }
    }
}
