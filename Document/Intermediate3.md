[README](../README.md)

# 🧍‍♂️ Rig & Avatar: Humanoid vs Generic, Retargeting (Mixamo → 커스텀 캐릭터)

Unity에서 애니메이션을 **스켈레톤이 다른 모델**에 재사용하려면 Rig/Avatar/Retargeting을 이해해야 합니다.

---

## 1) 핵심 개념 요약

| 개념 | 의미 | 포인트 |
|---|---|---|
| **Rig** | FBX의 본(Bone) 구조와 스킨 정보 | Import Settings > **Rig** 탭에서 Humanoid/Generic 선택 |
| **Avatar** | Unity가 이해하는 **표준 인체(머슬) 좌표계**에 본을 매핑한 결과물 | Humanoid일 때 생성됨. Retargeting의 핵심 |
| **Retargeting** | 서로 다른 본 구조의 캐릭터 사이에서 **동작(애니메이션)** 을 재사용 | Humanoid만의 강점(머슬 공간) |

---

## 2) Humanoid vs Generic

| 항목 | **Humanoid** | **Generic** |
|---|---|---|
| 대상 | 두 팔·두 다리의 **인간형** | 동물·메카·특수 리그 등 **비인간형** |
| Avatar | **필수** (머리/팔/다리 등 인체 매핑) | 사용 안 함(또는 제한적) |
| Retargeting | **강력** (다른 스켈레톤에도 재생 가능) | **제한적** (뼈 구조가 같아야 유리) |
| 제어 | **Muscle & Settings**로 포즈/가동범위 제어 | 본 로컬 회전값 그대로 |
| 장점 | 애니 공용화, Mixamo/Asset Store 애니 쉽게 재사용 | 커스텀 본 그대로의 자유도 |
| 단점 | 매핑 실패/경고 시 튜닝 필요 | 다른 스켈과의 호환성 떨어짐 |

> **요약**: “인간형 캐릭터 + 애니 재사용”이면 **Humanoid**가 정답.  
> 비인간형·특수 리그면 **Generic** 유지.

---

## 3) Avatar & Retargeting 동작 원리 (한 줄 이해)
- Humanoid는 FBX 본을 **Unity 표준 인체(머슬 공간)** 에 매핑(Avatar 생성) → 애니메이션은 **머슬 공간**에서 재생 → **다른 캐릭터에도** 자연스럽게 전달.

---

## 4) Mixamo 애니메이션 → 커스텀 캐릭터 적용 절차

### A) 파일 준비 (Mixamo)
1. **캐릭터 FBX** 1개: (스킨 포함)  
2. **애니메이션 FBX** 여러 개: 보통 **With Skin/Without Skin** 둘 다 가능하지만, **Without Skin** 추천(가볍고 캐릭터 메시 중복 방지)

### B) 캐릭터 FBX 임포트 (Unity)
1. Project에서 캐릭터 FBX 선택 → **Rig 탭**
   - Animation Type: **Humanoid**
   - **Configure…** 클릭 → 본 자동 매핑 확인(초록 체크). 빨간 경고 시 수동 매핑.
   - Pose: T-Pose 권장(필요 시 **Enforce T-Pose**)
2. **Apply** → 이 FBX에서 **Avatar**가 생성됨 (ex. `CharacterAvatar`)

### C) 애니메이션 FBX 임포트
1. 각 애니 FBX → **Rig 탭**  
   - Animation Type: **Humanoid**  
   - Avatar Definition: **Copy From Other Avatar**  
   - Source: **캐릭터 Avatar** 선택(`CharacterAvatar`)
2. **Animation 탭**  
   - Loop 모션이면 **Loop Time/Loop Pose** 체크  
   - Root Transform(ROT/POS Y/XZ) 옵션 점검 (필요 시 Bake Into Pose/Original)
3. **Apply**

> 이렇게 하면 **스켈레톤이 달라도**(Mixamo vs 내 캐릭터) Humanoid Avatar를 통해 **동작이 재생**됩니다.

### D) Animator 세팅
- 캐릭터 오브젝트에 **Animator** 컴포넌트  
- `Runtime Animator Controller`에 위 애니 클립들(Idle/Walk/Run/Attack…) 상태로 배치  
- 필요 파라미터(Speed/Jump/Attack 등)로 전환 제어

---

## 5) 흔한 문제 & 해결 팁

- **손/발 방향 이상**  
  - Avatar **Configure**에서 Bone Orientation 확인, 손목/발 뒤집힘 시 보정  
  - **Muscle & Settings**에서 리미트/디폴트 포즈 미세 조정  
  - **Foot IK**(State 옵션) 활성화로 접지 개선

- **스케일/위치 튐**  
  - FBX **Model 탭 > Scale Factor**, **Convert Units** 확인  
  - Root Transform에서 **Based Upon**(Original/Center of Mass 등)와 **Bake Into Pose** 조합 조정

- **루트모션이 안 먹힘**  
  - Animator의 **Apply Root Motion** 확인  
  - 애니 FBX의 Root Transform 옵션에서 루트 이동이 **Bake Into Pose**로 제거되지 않았는지 확인

- **어깨/목 뻣뻣함**  
  - Muscle 범위 재조정  
  - 애니 Import의 **Rig > Configure > Upper Chest/Spine** 매핑 상태 점검  
  - 필요 시 **Avatar Mask**로 상·하체 레이어 분리

- **클립 속도가 실재 이동과 불일치**  
  - 루프 러닝 모션이면 클립 길이/루트 거리 조정(또는 Animator.speed 미세 조정)

---

## 6) 빠른 적용 체크리스트
- [ ] 캐릭터 FBX: **Humanoid + Avatar** 생성 완료(초록 체크)  
- [ ] 애니 FBX: **Humanoid + Copy From Other Avatar → 캐릭터 Avatar**  
- [ ] Animator Controller에 상태/전환 구성  
- [ ] Root Transform/Loop/IK 옵션 점검  
- [ ] (필요 시) UpperBody 레이어 + Avatar Mask로 상·하체 분리

---

## 7) 초간단 사용 예제 (런타임 전환)

```csharp
using UnityEngine;

public class MixamoRetargetDemo : MonoBehaviour
{
    [SerializeField] Animator animator;
    static readonly int Speed = Animator.StringToHash("Speed");
    static readonly int Jump  = Animator.StringToHash("Jump");

    void Update()
    {
        // 0~6 사이 스피드로 Blend Tree(Idle/Walk/Run) 구동
        float speed = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude * 6f;
        animator.SetFloat(Speed, speed);

        // 점프 트리거
        if (Input.GetKeyDown(KeyCode.Space))
            animator.SetTrigger(Jump);
    }
}
