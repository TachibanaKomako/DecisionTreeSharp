using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeSharp.Sources
{
    public class SourceTableReader
    {
        public SourceTable Read(string filePath)
        {
            using StreamReader streamReader = new(filePath);
            List<string[]> lines = new();
            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                lines.Add(line.Split(','));
            }
            return new(ToItems(lines).ToArray());
        }
        IEnumerable<(string columnName, object?[] values, Type type)> ToItems(List<string[]> lines)
        {
            int columnCount = lines[0].Length;
            for(int i=0;i<columnCount; i++)
            {
                yield return ToItem(lines, i);
            }
        }
        (string columnName, object?[] values,Type type) ToItem(List<string[]> lines,int columnId)
        {
            //
            //数値チェック
            bool IsNumber()
            {
                foreach ((string[] line,int index) in lines.Select((line,index)=>(line,index)))
                {
                    //1行目はカラム
                    if( index != 0)
                    {
                        string value = line[columnId];
                        //Emptyか数値に変換できない時は数値でない
                        if ((string.IsNullOrEmpty(value) || double.TryParse(value,out _)) == false)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            IEnumerable<object?> GetValues(bool isNumber)
            {
                foreach ((string[] line, int index) in lines.Select((line, index) => (line, index)))
                {
                    //1行目はカラム
                    if (index != 0)
                    {
                        string value = line[columnId];
                        yield return isNumber 
                                        ? double.TryParse(value, out double _double) 
                                            ? _double : null 
                                        : value;
                    }
                }
            }
            string columnName = lines[0][columnId];
            bool isNumber = IsNumber();
            object?[] values = GetValues(isNumber).ToArray();
            return (columnName, values, isNumber ? typeof(double?) : typeof(string));
        }
    }
}
