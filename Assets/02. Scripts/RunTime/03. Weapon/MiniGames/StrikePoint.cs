using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 게이지 위에 커서가 왔다갔다하고, 중앙에 맞히면 성공
/// </summary>
public class StrikePoint : MonoBehaviour, IMiniGame
{
    [SerializeField] private StrikePointData StrikePointUI;
    private PlayerInput playerInput;
    private InputAction _skrikeAction;
    private StrikePointData strikeUI;
    private TaskCompletionSource<bool> task;

    void Awake()
    {
        _skrikeAction = playerInput.actions["MiniGame/StrikePoint"];
        _skrikeAction.performed += OnClickPoint;
    }

    public async UniTask<MiniGameResult> Play()
    {
        // 플레이어 액션 맵 변경
        // UI 오픈 (strikeUI = SetActive)
        task = new TaskCompletionSource<bool>();

        // 플레이어 입력 대기 (TaskCompletionSource)
        await task.Task;

        // UI 닫기
        // 플레이어 액션 맵 원상복구
        return EvaluateResult();
    }

    private MiniGameResult EvaluateResult()
    {
        throw new NotImplementedException();
    }


    private void OnClickPoint(InputAction.CallbackContext context)
    {
        task.TrySetResult(true);
    }
}