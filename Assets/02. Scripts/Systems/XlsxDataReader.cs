using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;

public static class XlsxDataReader<T> where T : new()
{
    public static List<T> MapFromExcel(Stream stream, LoadOptions options = null, int sheetIndex = 1)
    {
        var results = new List<T>();
        using (var workbook = options != null ? new XLWorkbook(stream, options) : new XLWorkbook(stream))
        {
            var worksheet = workbook.Worksheet(sheetIndex);
            var headerRow = worksheet.Row(1);

            // 헤더 행에서 실제로 헤더 텍스트가 존재하는 마지막 컬럼 번호
            int lastHeaderColumn = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

            var seenHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var headerMap = new List<(string Header, int ColumnIndex)>();

            // col 자체가 실제 워크시트 컬럼 번호 (리스트 인덱스가 아님)
            for (int col = 1; col <= lastHeaderColumn; col++)
            {
                string header = worksheet.Cell(1, col).GetString();
                if (string.IsNullOrWhiteSpace(header)) continue; // 빈 헤더 컬럼은 매칭 대상에서 제외
                if (seenHeaders.Add(header))
                {
                    headerMap.Add((header, col));
                }
            }

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                T obj = new();
                foreach (var (header, colIndex) in headerMap)
                {
                    PropertyInfo prop = props.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                    if (prop == null) continue;
                    if (colIndex > row.CellCount()) continue;

                    string cellValue = row.Cell(colIndex).GetString();
                    try
                    {
                        object convertedValue = ConvertValue(cellValue, prop.PropertyType);
                        prop.SetValue(obj, convertedValue);
                    }
                    catch (Exception)
                    {
                        // 파싱 실패 시 조용히 넘어감
                    }
                }
                results.Add(obj);
            }
        }
        return results;
    }

    private static object ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrEmpty(value)) return null;

        if (targetType == typeof(string)) return value;
        
        // 리스트 타입 처리 (예: "1,2,3")
        if (targetType == typeof(List<int>)) 
            return value.Split(',')
                .Select(v => int.TryParse(v.Trim(), out var i) ? i : 0)
                .Where(i => i != 0)
                .ToList();
                
        if (targetType == typeof(int)) return int.TryParse(value, out var i) ? i : 0;
        if (targetType == typeof(long)) return long.TryParse(value, out var l) ? l : 0L;
        if (targetType == typeof(float)) return float.TryParse(value, out var f) ? f : 0f;
        if (targetType == typeof(double)) return double.TryParse(value, out var d) ? d : 0d;
        if (targetType == typeof(bool)) return bool.TryParse(value, out var b) ? b : false;
        
        // Enum 처리
        if (targetType.IsEnum) 
        {
            try 
            {
                return Enum.Parse(targetType, value, true);
            }
            catch 
            {
                // 파싱 실패 시 첫 번째 값 반환 등의 예외 처리 가능
                var values = Enum.GetValues(targetType);
                return values.Length > 0 ? values.GetValue(0) : null;
            }
        }

        // 지원하지 않는 타입
        throw new Exception($"❌ 지원하지 않는 타입 변환입니다: {targetType.Name}");
    }
}