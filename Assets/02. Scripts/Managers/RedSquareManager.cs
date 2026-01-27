using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RedSquareManager
{
    public Action<RectTransform> RedSquareGenerater;
    public Action<RectTransform> RedSquareRemover;
    public Action RedSquareAllRemover;
    
    public Dictionary<RectTransform, RedSquare> activeRedSquares;
    private RedSquare instance;

    public RedSquareManager()
    {
        activeRedSquares = new Dictionary<RectTransform, RedSquare>();
        RedSquareGenerater += GenerateRedSquare;
        RedSquareRemover += RemoveRedSquare;
        RedSquareAllRemover += RemoveAllOfRedSquare;
    }

    private void GenerateRedSquare(RectTransform TargetRect)
    {
        if (activeRedSquares.ContainsKey(TargetRect)) return;

        if (!TargetRect.TryGetComponent(out instance))
            instance = TargetRect.AddComponent<RedSquare>();
        _ = instance.Generate(TargetRect);

        activeRedSquares.Add(TargetRect, instance);
        Debug.Log($"[RedDotManager] 해당 게임 오브젝트에 부착됨 : {instance.gameObject}");
    }
    
    private void RemoveRedSquare(RectTransform TargetRect)
    {
        if (!activeRedSquares.ContainsKey(TargetRect)) return;

        activeRedSquares[TargetRect].Remove();
        activeRedSquares.Remove(TargetRect);
    }

    private void RemoveAllOfRedSquare()
    {
        if (activeRedSquares.Count == 0) return;

        foreach (var r in activeRedSquares)
        {
            activeRedSquares[r.Key].Remove();
        }
        activeRedSquares.Clear();
    }
}
