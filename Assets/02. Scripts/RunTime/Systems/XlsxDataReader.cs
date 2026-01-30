using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
public static class XlsxDataReader<T> where T : new()
{
    // 변경점: string filePath 대신 Stream stream을 받습니다.
    public static List<T> MapFromExcel(Stream stream, int sheetIndex = 1)
    {
        var results = new List<T>();
        
        // 폰트가 없는 환경(WebGL/Linux Server 등)을 위한 예외 처리 옵션
        // 만약 폰트 관련 에러가 발생한다면 이 옵션을 활성화해야 합니다.
        // var options = new LoadOptions {
        //     GraphicEngine = DefaultGraphicEngine.CreateWithFontsAndSystemFonts(null) 
        // };

        // Stream을 사용하여 워크북 생성
        using (var workbook = new XLWorkbook(stream)) 
        {
            var worksheet = workbook.Worksheet(sheetIndex);

            // 첫 번째 행을 헤더로 사용
            var headerOrigins = worksheet.Row(1).Cells().Select(c => c.GetString()).ToList();

            List<string> headers = headerOrigins
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var row in worksheet.RowsUsed().Skip(1)) 
            {
                T obj = new();
                PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                for (int i = 0; i < headers.Count; i++)
                {
                    string header = headers[i];
                    PropertyInfo prop = props.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        // 인덱스 안전 장치 추가
                        if (i + 1 > row.CellCount()) continue; 
                        
                        string cellValue = row.Cell(i + 1).GetString();
                        object convertedValue = ConvertValue(cellValue, prop.PropertyType);
                        prop.SetValue(obj, convertedValue);
                    }
                }
                results.Add(obj);
            }
        } // using 블록이 끝나면 workbook이 올바르게 Dispose 됩니다.

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
        if (targetType == typeof(long)) return long.TryParse(value, out var l) ? l : 0L;
        if (targetType == typeof(float)) return float.TryParse(value, out var f) ? f : 0f;
        if (targetType == typeof(double)) return double.TryParse(value, out var d) ? d : 0d;
        if (targetType == typeof(bool)) return bool.TryParse(value, out var b) ? b : false;
        if (targetType.IsEnum) return Enum.TryParse(targetType, value, true, out var e) ? e : Enum.Parse(targetType, "End", true);

        // 타입 변환 에러
        throw new Exception("타입 변환 에러");
    }
}