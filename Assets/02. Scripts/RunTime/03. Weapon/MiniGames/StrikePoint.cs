using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class StrikePoint : MonoBehaviour, IMiniGame
{
    [SerializeField] private StrikePointView strikePointUI;
    [SerializeField] private InputActions inputActions;
    private UniTaskCompletionSource<bool> tcs; // UniTask 전용으로 변경

    private void Awake()
    {
        strikePointUI.isTimeOver
            .Subscribe(b =>
            {
                if (b)
                {
                    tcs?.TrySetResult(false);
                }
            })
            .AddTo(this)
            .AddTo(strikePointUI);
    }

    public async UniTask<MiniGameResult> Play()
    {
        // 플레이어 액션 맵 변경 및 액션 가져오기
        inputActions.SwitchToMiniGameMap();

        // 🚨 이벤트 구독은 액션을 가져온 뒤에 해야 합니다.
        inputActions.Actions.MiniGame.StrikePoint.performed += OnClickPoint;

        // UI 오픈
        strikePointUI.gameObject.SetActive(true);

        // 플레이어 입력 대기
        tcs = new UniTaskCompletionSource<bool>();
        bool isClicked = await tcs.Task;

        // TODO : 플레이어 입력시 파티클 및 효과음
        await UniTask.DelayFrame(3);

        // 입력시 값 가져옴
        float hitPoint = strikePointUI.hitPoint;

        // UI 닫기
        strikePointUI.gameObject.SetActive(false);

        // 🚨 메모리 누수 및 중복 실행 방지를 위해 반드시 이벤트 구독을 해제합니다.
        inputActions.Actions.MiniGame.StrikePoint.performed -= OnClickPoint;
        inputActions.SwitchToForgeMap();

        // 시간 초과시 미스
        if (!isClicked) { return new MiniGameResult() { grade = MiniGameGrade.Miss }; }

        // 클릭시 이 분기로 연결
        return EvaluateResult(hitPoint);
    }

    private MiniGameResult EvaluateResult(float hitPoint)
    {
        MiniGameResult result = new();

        (float pointX, float halfWidth) = strikePointUI.GetPointValues();
        float leftPoint = pointX - halfWidth;
        float rightPoint = pointX + halfWidth;

        result.grade = (hitPoint > leftPoint && hitPoint < rightPoint) ? MiniGameGrade.Perfect : MiniGameGrade.Miss;

        return result;
    }

    private void OnClickPoint(InputAction.CallbackContext context)
    {
        // 이미 완료된 Task에 중복 셋팅되는 것을 방지
        tcs?.TrySetResult(true);
    }
}