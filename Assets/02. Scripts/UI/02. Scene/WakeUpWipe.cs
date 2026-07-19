using UnityEngine;

public class WakeUpWipe : MonoBehaviour
{
    [SerializeField] private GameObject wipeObj;

    void Awake()
    {
        wipeObj.SetActive(true);
        // 씬 작업하는데 가리니깐
    }
}
