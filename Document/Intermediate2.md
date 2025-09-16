[README](../README.md)



| 루트모션(Root Motion) 이해 | Root Motion on/off, Apply Root Motion | 애니메이션 기반 캐릭터 이동 |
# 🧭 Unity 애니메이션: Root Motion 완전 정리
> **Root Motion** = 애니메이션 클립에 포함된 **루트(기준) 본의 이동/회전**을 읽어 **오브젝트의 실제 Transform**(위치/회전)을 움직이게 하는 방식.

## 1) 개념 한눈에
| 항목 | 의미 | 켜면(ON) | 끄면(OFF) |
|---|---|---|---|
| **Root Motion** | 클립의 루트 본 이동/회전 값을 Transform에 적용 | 애니메이션이 캐릭터를 “끌고” 감 | 포즈만 재생, 이동/회전은 **코드/물리**로 제어 |
| **Apply Root Motion** | Animator 컴포넌트의 토글 | 체크 시 루트 모션 적용 | 해제 시 `deltaPosition/Rotation` 무시 |

- **장점(ON)**: 발 미끄러짐 감소, **롤/대시/피격 밀림** 등 “연출 그대로” 구현 쉽다.  
- **장점(OFF)**: 네트워크/물리 일관성, 경사/충돌 대응, **조작감** 제어가 용이하다.

---

## 2) 어디서 켜고 끄나?
### (A) 에디터(Animator 컴포넌트)
- `Animator` → **Apply Root Motion** 체크/해제.

### (B) 코드로 토글
```csharp
animator.applyRootMotion = true;  // or false
