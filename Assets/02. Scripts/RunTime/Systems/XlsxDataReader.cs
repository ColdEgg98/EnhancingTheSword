using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using UnityEngine.Scripting; // 유니티 빌드 시 코드 삭제 방지용

[Preserve] // Linker가 이 클래스를 삭제하지 않도록 보호
public static class XlsxDataReader<T> where T : new()
{
    /// <summary>
    /// 엑셀 데이터를 읽어 리스트로 반환합니다.
    /// WebGL 호환을 위해 파일 경로(string) 대신 Stream을 받습니다.
    /// </summary>
    /// <param name="stream">파일 스트림 (MemoryStream 등)</param>
    /// <param name="options">폰트 설정이 포함된 로드 옵션 (WebGL 필수)</param>
    /// <param name="sheetIndex">시트 번호 (기본 1)</param>
    public static List<T> MapFromExcel(Stream stream, LoadOptions options = null, int sheetIndex = 1)
    {
        var results = new List<T>();

        // 옵션이 있으면 적용해서 열고, 없으면 그냥 엽니다.
        // using 문을 사용하여 작업이 끝나면 워크북 메모리를 해제합니다.
        using (var workbook = options != null ? new XLWorkbook(stream, options) : new XLWorkbook(stream))
        {
            var worksheet = workbook.Worksheet(sheetIndex);

            // 첫 번째 행을 헤더로 사용
            var headerOrigins = worksheet.Row(1).Cells().Select(c => c.GetString()).ToList();

            // 헤더에 중복이 있을경우 걸러줌 (대소문자 무시)
            List<string> headers = headerOrigins
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // 데이터 행 순회 (헤더 제외)
            foreach (var row in worksheet.RowsUsed().Skip(1)) 
            {
                T obj = new();
                
                // Reflection 활용: T 클래스의 퍼블릭 프로퍼티 가져오기
                PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                for (int i = 0; i < headers.Count; i++)
                {
                    string header = headers[i];
                    
                    // 엑셀 헤더와 이름이 같은 프로퍼티 찾기
                    PropertyInfo prop = props.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                    
                    if (prop != null)
                    {
                        // 셀 데이터 범위 체크 (안전장치)
                        if (i + 1 > row.CellCount()) continue;

                        string cellValue = row.Cell(i + 1).GetString();
                        
                        try 
                        {
                            object convertedValue = ConvertValue(cellValue, prop.PropertyType);
                            prop.SetValue(obj, convertedValue);
                        }
                        catch (Exception)
                        {
                            // 파싱 실패 시 기본값 유지하거나 로그 출력 (여기선 조용히 넘어감)
                        }
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