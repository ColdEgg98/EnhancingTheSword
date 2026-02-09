using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ExcelBaker
{
    public static readonly string weaponFileName = "WeaponsData";
    public static readonly string achieveFileName = "AchivementsData";
    public static readonly string itemFileName = "ItemData";

    [MenuItem("Tools/🍪 Excel to JSON Bake")]
    public static void Bake()
    {
        Debug.Log("🍪 데이터 베이킹 시작");

        // 저장할 경로 설정
        string savePath = Path.Combine(Application.dataPath, "Resources/Data");
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        // 사용할 엑셀 폴더 경로 (예 : Asset/04. Bakery Factory)
        string excelFolderPath = Path.Combine(Application.dataPath, "04. Bakery Factory");
        string weaponXlsxPath = Path.Combine(excelFolderPath, $"{weaponFileName}.xlsx");
        string achieveXlsxPath = Path.Combine(excelFolderPath, $"{achieveFileName}.xlsx");
        string itemXlsxPath = Path.Combine(excelFolderPath, $"{itemFileName}.xlsx");

        if (!File.Exists(weaponXlsxPath) || !File.Exists(achieveXlsxPath))
        {
            Debug.LogError("❌ [ExcelBaker] : 파일을 찾을 수 없습니다.");
            return;
        }

        // 변환 실행부 <T>도 변경할것
        BakeFile<Weapon>(weaponXlsxPath, Path.Combine(savePath, $"{weaponFileName}.json"));
        BakeFile<Achievement>(achieveXlsxPath, Path.Combine(savePath, $"{achieveFileName}.json"));
        BakeFile<MaterialItem>(itemXlsxPath, Path.Combine(savePath, $"{itemFileName}.json"));

        // 파일 만들었으니 유니티 새로 고침
        AssetDatabase.Refresh();

        Debug.Log("🍪 데이터 베이킹 완료");
    }

    public static void BakeFile<T>(string excelFolderPath, string jsonPath) where T : new()
    {
        using (var stream = File.Open(excelFolderPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            List<T> dataList = XlsxDataReader<T>.MapFromExcel(stream, null);
            string json = JsonConvert.SerializeObject(new DataWrapper<T>(dataList), Formatting.Indented);
            File.WriteAllText(jsonPath, json);
        }
    }
}
