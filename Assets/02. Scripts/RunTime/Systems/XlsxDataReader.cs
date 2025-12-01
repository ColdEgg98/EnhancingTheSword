using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;

public static class XlsxDataReader<T> where T : new()
{
    public static List<T> MapFromExcel(string filePath, int sheetIndex = 1)
    {
        var results = new List<T>();
        var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheetIndex);

        // 첫 번째 행을 헤더로 사용
        var headerOrigins = worksheet.Row(1).Cells().Select(c => c.GetString()).ToList();

        // 헤더에 중복이 있을경우 걸러줌 (대소문자 무시)
        List<string> headers = headerOrigins
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList<string>();

        foreach (var row in worksheet.RowsUsed().Skip(1)) // 헤더 제외
        {
            T obj = new();
            // Reflection 활용. 들어온 클래스의 퍼블릭 인스턴스 변수를 PropertyInfo[] 구조로 그룹화
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                PropertyInfo prop = props.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
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
        if (targetType == typeof(List<int>)) return value
                .Split(',')
                .Select(v => int.TryParse(v.Trim(), out var i) ? i : 0)
                .Where(i => i != 0)
                .ToList();
        if (targetType == typeof(int)) return int.TryParse(value, out var i) ? i : 0;
        if (targetType == typeof(float)) return float.TryParse(value, out var f) ? f : 0f;
        if (targetType == typeof(double)) return double.TryParse(value, out var d) ? d : 0d;
        if (targetType == typeof(bool)) return bool.TryParse(value, out var b) ? b : false;

        // 필요하다면 DateTime, Enum 등 추가 가능
        return null;
    }
}