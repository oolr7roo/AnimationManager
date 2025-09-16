[README](../README.md)


# 🧱 Animator Layer 완전 가이드

**Animator Layer**는 “여러 개의 애니메이션 포즈를 층(layer)처럼 쌓아” 최종 포즈를 만드는 시스템입니다.  
기본(하체) 위에 **상체/표정/무기/반동** 같은 동작을 “부분적으로” 얹거나, **가벼운 흔들림/호흡**을 **가산(Additive)** 으로 더할 수 있습니다.

---

## 1) 동작 원리 (한눈에)
- **Base Layer (Layer 0)**: 가장 아래층. 보통 **이동/런/점프** 등 핵심 모션과 **루트모션**을 담당.
- **추가 레이어 (1, 2, …)**: 위에서 **가중치(Weight)** 와 **방식(Override/Additive)** 로 Base 결과에 덧입힘.
- **Avatar Mask**: 각 레이어가 **어떤 본(뼈)** 에만 영향을 줄지 선택(예: 상체만).
- **Layer Weight**: 0~1. 값이 클수록 해당 레이어 영향이 커짐.
- **Blending**:
  - **Override**: 마스크로 지정한 본을 **대체**(덮어쓰기).
  - **Additive**: 아래 레이어와의 **차(델타)** 를 **가산**(섞어 더함).

> 최종 포즈 = Base 결과 → (Layer1 적용) → (Layer2 적용) … 순차 합성

---

## 2) 레이어 속성 정리
| 항목 | 의미 | 권장/주의 |
|---|---|---|
| **Blending** | Override / Additive | 상체 전용 동작 = Override, 미세 흔들림·호흡 = Additive |
| **Weight** | 0~1 레이어 영향도 | 런타임에서 서서히 가감(크로스페이드 느낌) |
| **Avatar Mask** | 영향 줄 본(Transforms) 선택 | 상체 레이어는 **Hips 제외**(루트/하체 제외) 권장 |
| **IK Pass** | 이 레이어에서 IK 계산 허용 | 손/발 IK 조정이 필요할 때 ON |
| **Synced Layer** | 다른 레이어와 상태머신 **동기화** | 표정/무기 레이어를 Base와 같은 상태명으로 맞춰 동기화할 때 유용 |

---

## 3) Override vs Additive
| 구분 | 포즈가 아래 레이어와 어떻게 합성되나 | 대표 용도 | 팁 |
|---|---|---|---|
| **Override** | 마스크로 선택된 본을 **덮어씀** | 상체(조준/사격/리로드), 손 제스처, 표정 교체 | 마스크 범위 **겹침 최소화** |
| **Additive** | 아래 레이어 포즈에 **차이(델타)를 더함** | 호흡, 카메라 반동, 미세 흔들림, 감정 미묘 변화 | **Additive용 클립** 필요(레퍼런스 포즈 설정) |

> Additive 클립: FBX/Clip Import의 **Animation 탭 → Additive Reference Pose**(Pose Frame/Custom) 설정.

---

## 4) Avatar Mask 핵심
- **상체 레이어**를 만들 때는 `Hips/Root`와 하체를 **체크 해제** → 하체는 Base가 유지, 상체만 변경.
- 표정/손가락만 제어할 레이어는 해당 본만 체크.
- 하나의 본을 **여러 Override 레이어**가 중복으로 건드리면 튐/깜빡임 위험.

---

## 5) 루트모션·파라미터·전환
- **루트모션**은 보통 **Base Layer**에서만 사용(상층은 Root/hips 제외 마스크 권장).
- **파라미터**(Bool/Float/Trigger)는 **레이어 간 공유**. 각 레이어의 전환은 **서로 독립**.
- 같은 순간 서로 다른 레이어에서 전환이 일어나도 됨. 최종 포즈는 레이어 스택 결과.

---

## 6) 실전 워크플로우

### 6-1) “달리면서 상체만 리로드”
1. **Base Layer**: Locomotion(Idle/Walk/Run) Blend Tree.
2. **UpperBody Layer (Layer 1)**  
   - Blending = **Override**  
   - Avatar Mask = **상체 본만 체크**  
   - Weight = 0(기본), 리로드 때 1로 올림  
   - 상태: `Reload`(Loop Off, Exit Time으로 끝나면 0으로 복귀)
3. **루트모션**은 Base에서만.

**코드 예시**
```csharp
// 레이어 인덱스 얻기
int upper = animator.GetLayerIndex("UpperBody");

// 리로드 시작: 상체 레이어 가중치 올리면서 상태 진입
void StartReload()
{
    animator.CrossFadeInFixedTime("UpperBody.Reload", 0.08f, upper);
    StartCoroutine(FadeLayerWeight(upper, 1f, 0.12f));
}

// 종료 시 가중치 제거
void EndReload()
{
    StartCoroutine(FadeLayerWeight(upper, 0f, 0.15f));
}

System.Collections.IEnumerator FadeLayerWeight(int layer, float target, float dur)
{
    float t = 0f;
    float from = animator.GetLayerWeight(layer);
    while (t < dur)
    {
        t += Time.deltaTime;
        float w = Mathf.Lerp(from, target, t / dur);
        animator.SetLayerWeight(layer, w);
        yield return null;
    }
    animator.SetLayerWeight(layer, target);
}
```