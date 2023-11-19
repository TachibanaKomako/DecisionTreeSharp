using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeSharp.Sources;

namespace DecisionTreeSharp.Models
{
    public class Utils
    {
        //object型から数値型へ変換する
        private static double? ToNumber(object? value)
        {
            if (value is null)
            {
                return null;
            }
            else if (value is double _double)
            {
                return _double;
            }
            else if (value is int _int)
            {
                return _int;
            }
            else if (value is long _long)
            {
                return _long;
            }
            else if (value is float _float)
            {
                return _float;
            }
            else if (value is decimal _decimal)
            {
                return (double)_decimal;
            }
            else
            {
                throw new NotImplementedException(nameof(value));
            }
        }
        public static double?[] GetNumbers(SourceTable dataFrame, string columnName)
        {
            return dataFrame[columnName]
                .Select(ToNumber)
                .ToArray();
        }
        public static bool?[] GetBooleans(SourceTable dataFrame, string columnName, Func<object?, bool?> toBoolean)
        {
            return dataFrame[columnName].Select(toBoolean).ToArray();
        }
    }
}
