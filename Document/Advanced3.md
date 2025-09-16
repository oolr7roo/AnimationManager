[README](../README.md)

# ⏱️ UniRx / UniTask 결합
> **목표**: `Observable.Timer`로 일정 시간 뒤 `Animator.Play()`/`CrossFade`를 호출하고,  
> 애니메이션이 **끝난 뒤**(또는 특정 지점) 다음 로직을 **이벤트로 연결**하기.

---

## 1) UniRx 패턴

### 1-1) Timer → Animator.Play (지연 실행)
```csharp
using UniRx;
using UnityEngine;
using System;

public class RxDelayPlay : MonoBehaviour
{
    [SerializeField] Animator animator;
    static readonly int AttackHash = Animator.StringToHash("Base Layer.Attack");

    void Start()
    {
        // 0.35초 후에 Attack 상태로 전환 (0.1s 블렌딩)
        Observable.Timer(TimeSpan.FromSeconds(0.35))
            .ObserveOnMainThread()
            .Subscribe(_ => animator.CrossFadeInFixedTime(AttackHash, 0.10f))
            .AddTo(this);
    }
}
```

```csharp
public static class AnimatorRx
{
    // 비루프 단발 상태 종료 대기 (layer=0 기본)
    public static IObservable<Unit> OnStateFinished(this Animator anim, int stateHash, int layer = 0)
    {
        return Observable.EveryUpdate()
            .Where(_ =>
            {
                var s = anim.GetCurrentAnimatorStateInfo(layer);
                bool inState = s.fullPathHash == stateHash;
                bool done = inState && s.normalizedTime >= 1f && !anim.IsInTransition(layer);
                return done;
            })
            .First() // 한 번만 방출
            .AsUnitObservable();
    }
}
public class RxPlayThenAfter : MonoBehaviour
{
    [SerializeField] Animator animator;
    static readonly int AttackHash = Animator.StringToHash("Base Layer.Attack");

    void Start()
    {
        // 0.2초 뒤 공격 시작 → 끝나면 후처리
        Observable.Timer(TimeSpan.FromSeconds(0.2))
            .Do(_ => animator.CrossFadeInFixedTime(AttackHash, 0.08f))
            .SelectMany(_ => animator.OnStateFinished(AttackHash)) // 끝날 때까지 기다림
            .ObserveOnMainThread()
            .Subscribe(_ =>
            {
                Debug.Log("Attack finished → 다음 로직 실행");
                // ex) 콤보 가능 플래그 켜기, 카메라 전환, 데미지 누적 처리 등
            })
            .AddTo(this);
    }
}
```
```csharp
using UniRx;
using UnityEngine;

public class AnimEventBridge : MonoBehaviour
{
    public readonly Subject<Unit> OnAttackHit = new Subject<Unit>(); // 임팩트 타이밍

    // 클립에 Animation Event로 이 함수를 찍어두기
    public void Event_AttackHit()
    {
        OnAttackHit.OnNext(Unit.Default);
    }
}

// 사용처
// bridge.OnAttackHit.Subscribe(_ => DoDamage()).AddTo(this);

```
```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class TaskDelayPlay : MonoBehaviour
{
    [SerializeField] Animator animator;
    static readonly int RollHash = Animator.StringToHash("Base Layer.Roll");

    async void Start()
    {
        var token = this.GetCancellationTokenOnDestroy();

        await UniTask.Delay(TimeSpan.FromSeconds(0.25), cancellationToken: token);

        animator.CrossFadeInFixedTime(RollHash, 0.10f);

        // 상태 진입 대기 (안전)
        await UniTask.WaitUntil(() =>
        {
            var s = animator.GetCurrentAnimatorStateInfo(0);
            return s.fullPathHash == RollHash && !animator.IsInTransition(0);
        }, cancellationToken: token);

        // 종료 대기 (비루프 가정)
        await UniTask.WaitUntil(() =>
        {
            var s = animator.GetCurrentAnimatorStateInfo(0);
            return s.fullPathHash == RollHash && s.normalizedTime >= 1f && !animator.IsInTransition(0);
        }, cancellationToken: token);

        Debug.Log("Roll finished → 다음 단계 진행");
    }
}

```
```csharp
public static class AnimatorTask
{
    public static async UniTask WaitStateEnd(this Animator anim, int stateHash, int layer = 0, CancellationToken token = default)
    {
        await UniTask.WaitUntil(() =>
        {
            var s = anim.GetCurrentAnimatorStateInfo(layer);
            return s.fullPathHash == stateHash && !anim.IsInTransition(layer);
        }, cancellationToken: token);

        await UniTask.WaitUntil(() =>
        {
            var s = anim.GetCurrentAnimatorStateInfo(layer);
            return s.fullPathHash == stateHash && s.normalizedTime >= 1f && !anim.IsInTransition(layer);
        }, cancellationToken: token);
    }
}

// 사용 예
// animator.CrossFadeInFixedTime(AttackHash, 0.08f);
// await animator.WaitStateEnd(AttackHash, 0, token);
// // 후처리...

```
```csharp
using UnityEngine;

public class NotifyExitSMB : StateMachineBehaviour
{
    public string message = "OnExit";

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SendMessage(message, SendMessageOptions.DontRequireReceiver);
    }
}

```