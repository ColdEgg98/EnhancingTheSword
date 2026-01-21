using UnityEngine;
using System.Reflection;

public class XlsxReadTest : MonoBehaviour
{
    string dataPath;

    // 시트 읽게하기
    // 시트 읽으려면 시스템IO의 컴바인 함수 이용
    void ReadTest()
    {
        dataPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Test.xlsx");

        if (dataPath == null)
            return;
            
        var test = XlsxDataReader<MyClass>.MapFromExcel(dataPath);
        Debug.Log($"읽은 정보의 갯수 : {test.Count}");

        foreach (var item in test)
        {
            var props = typeof(MyClass).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var value = prop.GetValue(item);
                Debug.Log($"{prop.Name} = {value}");
            }
        }
    }

    void Start()
    {
        Debug.Log("Run XlsxReadTest");
        ReadTest();
    }

}

public class MyClass
{
    public int 번호 { get; set; }
    public string 날짜 { get; set; }
    public string 상영시간 { get; set; }
    public string 영화 { get; set; }
    public string 극장명 { get; set; }
    public int 금액 { get; set; }
    public int 매수 { get; set; }
    public string 비고 { get; set; }
}