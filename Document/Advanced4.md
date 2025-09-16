[README](../README.md)

# 🗡️ 애니메이션 이벤트로 “공격 타이밍 = 데미지 판정” 맞추기
> 두 가지 축을 함께 쓰면 정확하고 관리가 쉬워요  
> 1) **AnimationEvent**: 클립의 특정 프레임에서 함수 호출  
> 2) **StateMachineBehaviour(OnStateEnter/Update/Exit)**: 상태 수명 주기 제어

---

## 1) AnimationEvent
- **무엇**: 애니메이션 **클립 타임라인** 위에 마커를 찍어, 그 시점에 **MonoBehaviour 메서드**를 호출.
- **어디**: `Animator`가 붙은 **같은 GameObject**의 스크립트 메서드가 호출됨.
- **언제 쓰나**: “**정확히 이 프레임에** 히트/이펙트/사운드”.

### 메서드 시그니처
- 매개변수 0개 또는 **float / int / string / Object / AnimationEvent** 중 1개

---

## 2) StateMachineBehaviour (SMB) – OnStateEnter/Exit/Update
- **무엇**: 특정 **상태(State)**에 붙여 상태 **진입/반복/종료** 시점을 콜백으로 받음.
- **언제 쓰나**: “히트박스 **열림~닫힘 윈도우** 관리”, “상태 끝났을 때 후처리”.

---

## 3) 언제 무엇을?
| 목적 | 권장 |
|---|---|
| 임팩트 **정확 프레임** | **AnimationEvent**로 한 번 “찌르기” |
| 임팩트 **구간(윈도우)** | **SMB OnStateUpdate**로 `normalizedTime` 범위 관리 |
| 상태 시작/끝 처리 | **SMB OnStateEnter/Exit** |

---

## 4) 최소 구현 레시피

### A) 히트박스 컴포넌트 (Trigger 콜라이더)
```csharp
using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    [SerializeField] LayerMask targetMask;
    [SerializeField] int baseDamage = 10;
    bool active;

    HashSet<Collider> hitThisSwing = new HashSet<Collider>();

    public void Open(int dmg = -1) { active = true; hitThisSwing.Clear(); if (dmg >= 0) baseDamage = dmg; }
    public void Close()           { active = false; }

    void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        if ((targetMask.value & (1 << other.gameObject.layer)) == 0) return;
        if (!hitThisSwing.Add(other)) return; // 한 스윙 중 중복 타격 방지

        // TODO: Health 컴포넌트에 데미지 전달
        other.SendMessage("ApplyDamage", baseDamage, SendMessageOptions.DontRequireReceiver);
    }
}
