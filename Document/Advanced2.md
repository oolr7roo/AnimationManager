[README](../README.md)

# 🎛️ DOTween + Animator 혼합
> **목표**: `Animator`로 캐릭터 모션을, **DOTween Sequence**로 UI/카메라/소품(Transform, CanvasGroup, Material 등)을 **동시에** 연출.

---

## 1) 언제/왜 섞나?
- **캐릭터 동작(모션/표정/전투)**: `Animator`(상태머신, 파라미터, CrossFade, Root Motion)
- **UI/카메라/효과**: `DOTween`(패널 슬라이드, 페이드, 숫자 카운트, FOV/포스트이펙트 값 변화)
- 컷씬/스킬 연출처럼 **정밀 타이밍**이 필요할 때 DOTween의 **Sequence**가 “마스터 타임라인” 역할을 잘함.

---

## 2) 기본 설계 패턴

### A) **병렬(Parallel) 패턴**
- 캐릭터는 `Animator`로 전환 → 동시에 `DOTween`으로 UI/카메라 연출을 **같은 길이로** 실행.
- 간단하고 유지보수 쉬움.

### B) **마스터 타임라인 패턴**
- **DOTween Sequence**가 전체 타이밍을 주도.  
- 중간 지점에 `InsertCallback()`으로 `Animator` 트리거나 `CrossFade`를 **정확한 시점**에 호출.

### C) **상·하체 분리 + 시각 효과**
- 캐릭터 상·하체는 `Animator Layer/AvatarMask`로,  
- 배경/카메라는 DOTween으로. 서로 **관할 분리**해 충돌 방지.

---

## 3) 주의할 점 (베스트 프랙티스)
- **동일 Transform 충돌 금지**: Animator가 제어하는 본(예: 팔/손)에 DOTween으로 포지션/회전을 직접 Tween하지 말 것(싸움 남).  
  → **UI/카메라/소품** 위주로 Tween, 캐릭터 본은 Animator에 맡김.
- **시간 스케일**: 일시정지 중에도 연출을 돌리고 싶다면 `SetUpdate(true)`(unscaled) 사용.
- **수명 관리**: `SetLink(gameObject)`로 오브젝트 파괴 시 자동 Kill. 또는 씬 종료 시 `DOTween.Kill(target)`.
- **동기 포인트**: Animator 클립의 임팩트 타이밍(정규화 시간)을 DOTween의 `Insert` 시점과 맞추면 이질감 ↓
- **네트워크/물리**: 이동 권위는 물리/서버에 두고, DOTween은 **시각적 보조**(UI/카메라/후가공)로 사용.

---

## 4) 미니 사용 예 (간단하게)

### 4-1) 병렬 실행: 캐릭터 감정 연출 + UI 배너
```csharp
using DG.Tweening;
using UnityEngine;

public class EmoteWithUI : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] CanvasGroup banner;   // UI 배너
    [SerializeField] RectTransform panel;  // 슬라이드 패널

    public void Play()
    {
        // 1) 캐릭터 모션 전환 (Animator)
        animator.ResetTrigger("Emote");
        animator.SetTrigger("Emote"); // 또는 CrossFadeInFixedTime("Base Layer.Emote", 0.1f);

        // 2) UI 연출 (DOTween) - 병렬
        var seq = DOTween.Sequence()
            .SetUpdate(false)                     // timeScale 영향 받음(기본)
            .SetLink(gameObject)                  // 오브젝트 파괴 시 자동 Kill
            .Append(banner.DOFade(1f, 0.25f))     // 배너 페이드인
            .Join(panel.DOAnchorPosY(0, 0.25f))   // 패널 슬라이드 인
            .AppendInterval(1.0f)                 // 잠시 머무름
            .Append(banner.DOFade(0f, 0.25f))     // 페이드아웃
            .Join(panel.DOAnchorPosY(-200, 0.25f));
    }
}
