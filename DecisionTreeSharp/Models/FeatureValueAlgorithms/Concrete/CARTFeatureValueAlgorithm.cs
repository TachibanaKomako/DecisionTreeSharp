using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeSharp.Sources;

namespace DecisionTreeSharp.Models.FeatureValueAlgorithms.Concrete
{
    internal class CARTFeatureValueAlgorithm : IFeatureValueAlgorithm
    {

        public double Calculate(ModelTable df)
        {
            return GetIg(df.GetActives());
        }
        public double Calculate(ModelTable df, string columnName, double value)
        {
            //カラムの値を取得する
            double?[] values = df[columnName];
            bool[] actives = df.GetActives();

            List<(int category, bool active)> items = new();
            List<bool> naActives = new();
            for (int i = 0; i < values.Length; i++)
            {
                bool active = actives[i];
                if (values[i] is not null)
                {
                    int category = values[i] <= value ? 1 : 0;
                    items.Add((category, active));
                }
                else
                {
                    naActives.Add(active);
                }
            }

            int n = values.Length;

            double totalIg = items
                    .GroupBy(r => r.category)
                    .Sum(r =>
                    {
                        double ig = GetIg(naActives.Concat(r.Select(r => r.active)).ToArray());
                        return (ig * r.Count()) / n;
                    });
            return totalIg;
        }
        //ジニ不純度を計算する
        private static double GetIg(bool[] actives)
        {
            //*
            //Ni
            int count = actives.Length;
            //ジニ不純度を計算する
            //1-Σ(1,c)(p(i|t)^2)
            double result = 1.0 - actives.GroupBy(r => r).Sum(r => {
                double ni = r.Count();
                double rate = ni / count;
                return rate * rate;
                }
            );
            return result;
        }
    }
}
