[README](../README.md)
# 🎬 Unity Animation 핵심 개념 정리

Unity 애니메이션 시스템의 기본 구성 요소 4가지를 정리했습니다.

---

## 📑 개요 표

| 개념 | 설명 | 역할 | 예시 |
|------|------|------|------|
| **Animation Clip** | 오브젝트의 속성 변화를 시간에 따라 기록한 데이터 (`.anim`) | 실제 재생되는 애니메이션 | Cube 이동, 캐릭터 Walk/Run 모션 |
| **Animator** | GameObject에 붙는 컴포넌트 | 어떤 Animation Clip을 재생할지 관리, 블렌딩 및 전환 실행 | Animator 컴포넌트를 캐릭터에 붙여서 애니메이션 재생 |
| **Animator Controller** | Animator가 사용할 설계도 (`.controller`) | State Machine, Transition, Parameter 정의 | Idle → Walk (Speed > 0.1), Walk → Run (Speed > 3) |
| **State Machine** | 애니메이션 상태(Idle, Walk, Run...)와 전환 규칙 모음 | 현재 상태를 추적하고 조건 충족 시 전환 | Entry → Idle → Walk, Jump Trigger → Jump 상태 |

---

## 📝 추가 설명

### 🎞️ Animation Clip
- Unity에서 만든 `.anim` 파일
- Transform, Material, Property 값이 시간에 따라 변하는 데이터
- 여러 Clip을 만들어 상황별로 재생 가능

### 🎛️ Animator
- Animator Controller를 참조하여 애니메이션 실행
- Avatar(리깅 정보)와 연결되어 Humanoid 캐릭터에 맞춤
- **주요 필드**  
  - Runtime Animator Controller  
  - Avatar  

### 🗂️ Animator Controller
- 상태머신(State Machine) 기반으로 Clip을 전환
- Parameter (Bool, Int, Float, Trigger)로 조건 제어
- Transition(전환 조건)으로 자연스럽게 상태 변경

### 🔀 State Machine
- 상태(Idle, Walk, Run 등)와 전환 관계를 시각적으로 표현
- Entry → Exit 로직까지 한눈에 확인 가능
- 복잡한 애니메이션 로직도 직관적으로 관리

---

> 💡 **비유로 이해하기**
> - Animation Clip = 🎥 “동영상 파일”
> - Animator = ▶️ “재생기(Player)”
> - Animator Controller = 📋 “재생 리스트 & 조건표”
> - State Machine = 🔀 “재생 리스트 안의 상태 전환 로직”
