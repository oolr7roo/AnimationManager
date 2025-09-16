[README](../README.md)

# 🧰 Unity Animator 핵심 개념: Parameters / CrossFade / Blend Tree

## 1) Parameters (Bool, Float, Trigger)
Animator Controller 안에서 **상태 전환 조건**이나 **블렌드 제어**에 쓰는 값들.

- **Bool**: 켜짐/꺼짐으로 상태를 토글. 예) `IsGrounded = true`면 점프 상태로 못 감.
- **Float**: 연속 값. **Blend Tree**의 축으로 가장 많이 사용. 예) `Speed`(0~6) 값에 따라 Idle↔Walk↔Run 블렌드.
- **Trigger**: “1회성 이벤트” 스위치. 전환에 **소비되면 자동으로 꺼짐**. 예) `Jump` 눌렀을 때만 점프 진입. :contentReference[oaicite:0]{index=0}

> 스크립트 제어: `SetFloat`, `SetBool`, `SetTrigger`, 필요 시 `ResetTrigger`. :contentReference[oaicite:1]{index=1}

---

## 2) CrossFade
현재 상태에서 **다른 상태로 부드럽게 전환**하는 API.

- `Animator.CrossFade(stateNameOrHash, transitionDuration, layer, normalizedTransitionTime)`
  - `transitionDuration`: 블렌딩 시간(초가 아닌 **정규화 시간** 기준 버전도 있음)
  - `normalizedTransitionTime`: 새 상태의 **시작 위치(정규화 0~1)**  
- 고급: **초 단위**로 전환하려면 `CrossFadeInFixedTime` 사용.  
- 성능/오타 방지: `Animator.StringToHash`로 **해시**를 미리 만들어 쓰면 좋음.  
- **레이어명 포함 경로** 사용 권장: `"Base Layer.Run"` 형태. :contentReference[oaicite:2]{index=2}

---

## 3) Blend Tree
여러 **비슷한 모션을 1~2개의 파라미터로 블렌딩**해 자연스런 전환을 만드는 노드.

- **1D**: `Speed` 같은 단일 축(예: Idle↔Walk↔Run)  
- **2D**: `X/Y` 입력에 따라 방향 전환(예: 상하좌우 이동 모션)  
- **품질 팁**: 클립의 발 착지 등 **동기 포인트**가 정규화 시간상 비슷해야 블렌딩이 깨끗함.  
- 생성: Animator 창 우클릭 → `Create State > From New Blend Tree` → 더블클릭으로 그래프 편집. :contentReference[oaicite:3]{index=3}

---
