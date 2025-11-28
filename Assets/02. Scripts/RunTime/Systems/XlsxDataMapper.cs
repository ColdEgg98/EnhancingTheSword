using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;

public static class XlsxDataMapper<T> where T : new()
{
    public static List<T> MapFromExcel(string filePath, int sheetIndex = 1)
    {
        var results = new List<T>();
        var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetIndex);

        // 첫 번째 행을 헤더로 사용
        var headers = worksheet.Row(1).Cells().Select(c => c.GetString()).ToList();

        foreach (var row in worksheet.RowsUsed().Skip(1)) // 헤더 제외
        {
            var obj = new T();
            // Reflection : 대상 클래스의 멤버 변수 배열을 저장 (public, Instance가능한 변수)
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 변수와 헤더 매핑 + 대입
            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                var prop = props.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    string cellValue = row.Cell(i + 1).GetString();
                    object convertedValue = ConvertValue(cellValue, prop.PropertyType);
                    prop.SetValue(obj, convertedValue);
                }
            }

            results.Add(obj);
        }

        return results;
    }

    private static object ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrEmpty(value)) return null;

        if (targetType == typeof(string)) return value;
        if (targetType == typeof(int)) return int.TryParse(value, out var i) ? i : 0;
        if (targetType == typeof(float)) return float.TryParse(value, out var f) ? f : 0f;
        if (targetType == typeof(double)) return double.TryParse(value, out var d) ? d : 0d;
        if (targetType == typeof(bool)) return bool.TryParse(value, out var b) ? b : false;

        // 필요하다면 DateTime, Enum 등 추가 가능
        return null;
    }
}