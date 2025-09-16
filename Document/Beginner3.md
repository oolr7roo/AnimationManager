[README](../README.md)

# 🧱 Unity Animation: Position / Rotation / Scale / Property Curve

**Transform 3요소와 임의 속성(Property)을 애니메이션으로 다루는 방법**을 정리했습니다.

---

## 📑 한눈에 표

| 개념 | 무엇인지 | 어디서/어떻게 쓰나 | 자주 쓰는 예 | 주의/팁 |
|---|---|---|---|---|
| **Position** | 오브젝트의 **위치(좌표)** | Animation Clip에서 `Transform → Position (x,y,z)`에 키프레임 | 카메라 이동, 등장 연출, 슬라이드 인/아웃 | **Local 기준**(부모 영향), 급격한 커브면 튐 → 키 보강 |
| **Rotation** | 오브젝트의 **회전** | `Transform → Rotation (Euler)`에 키 | 캐릭터 회전, 카메라 팬/틸트 | 내부는 **쿼터니언** 보간 → 180° 넘나들 때 **플립** 방지용으로 중간 키 추가 |
| **Scale** | 오브젝트의 **크기** | `Transform → Scale (x,y,z)`에 키 | 팝업 확대, 등장/퇴장 효과 | **비균등/음수 스케일**은 노멀/그림자/물리 문제 유의 |
| **Property Curve** | 임의 컴포넌트/머티리얼의 **속성 곡선** | Animation Clip에서 **Add Property**로 선택(숫자/벡터/컬러 등) | `Light.intensity`, `Camera.fieldOfView`, `CanvasGroup.alpha`, **BlendShape** | 머티리얼은 인스턴스/공유 구분, 값 **클램프** 권장(오버슈트 방지) |

> ✅ **Timeline**에서도 동일: Animation Track의 Clip을 **Record**하거나 **Add Property**로 같은 속성들을 키잉합니다.  
> ✅ **Activation Track**은 등장/퇴장(**SetActive**)만 담당, 세밀한 값 변화는 **Property Curve**로!

## 🎞 Position (위치)
- **정의**: 키프레임 사이를 보간하며 로컬 좌표(x,y,z)를 움직임.
- **활용**: 카메라/오브젝트의 이동, 슬라이드 인/아웃.
- **팁**
  - 부모가 움직이면 **자식 Position도 상대적으로** 따라감(의도된 연출인지 확인).
  - 급커브에서 가속/감속이 과하면 **키 간격/탄젠트**(Auto/Linear/Constant)로 부드럽게.

## 🧭 Rotation (회전)
- **정의**: 에디터는 Euler(각도)로 보이지만 내부는 **Quaternion**으로 보간.
- **문제 패턴**: 180° 경계에서 **불필요한 반대 회전(플립)**이 생길 수 있음.
- **해결 팁**
  - **중간 키**를 더 넣어 회전 방향을 명시.
  - 큰 각도(>180°)를 여러 구간으로 나눠 부드럽게.
  - 카메라엔 **Cinemachine** 블렌딩을 적극 고려.

## 📐 Scale (스케일)
- **정의**: 로컬 스케일(x,y,z) 보간.
- **활용**: 등장 시 **팝업 확대**, 퇴장 시 축소.
- **주의**
  - **비균등 스케일**(1,2,1)은 조명/쉐이더/물리에서 아티팩트 가능.
  - **음수 스케일**은 면 뒤집힘/그림자 이상 → 가급적 지양.

## 🎚 Property Curve (속성 곡선)
- **정의**: Transform 외 임의 속성(숫자/벡터/컬러/BlendShape/오디오/포스트 등)을 곡선으로 애니메이션.
- **추가 방법**: Animation Window → **Add Property** → 대상 컴포넌트의 속성 선택.
- **예시**
  - `Light.intensity`로 씬 밝기 페이드
  - `Camera.fieldOfView`로 줌 인/아웃
  - `CanvasGroup.alpha`로 UI 페이드
  - `SkinnedMeshRenderer → BlendShapes`로 표정/형상 변화
  - `Renderer → Material → _Color.a`로 머티리얼 투명도(※ 공유/인스턴스 머티리얼 구분!)
