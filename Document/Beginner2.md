[README](../README.md)

# 🎥 Unity Timeline 핵심 요소 정리

| 개념 | 무엇인지 | 어디에 붙는지 / 어디서 쓰는지 | 대표 용도 |
|---|---|---|---|
| **Playable Director** | 타임라인을 재생·정지·스크럽하는 “재생기” | GameObject 컴포넌트(예: `CutsceneController`) | 특정 Timeline Asset을 재생/중지, 코드에서 제어 |
| **Track** | 타임라인의 “트랙(레이어)” | Timeline Asset 내부(창에서 추가) | 카메라, 애니메이션, 오브젝트 활성화, 오디오 등 대상별 제어 |
| **Clip** | 트랙 위에 올리는 “조각(구간)” | 특정 Track 위 | 해당 구간 동안 카메라 샷 전환, 애니메이션 재생, 활성/비활성 등 수행 |
| **Signal** | 타임라인 도중에 쏘는 “이벤트 큐(마커)” | Signal Track의 마커(Emitter) + 대상의 `Signal Receiver` | 정확한 시점에 함수 호출/이펙트/사운드 등 트리거 |

---

## 🧭 컷씬 구성 예시(카메라 이동 + 오브젝트 등장)

1. **세팅**
   - 메인 카메라에 **CinemachineBrain** 추가.
   - 가상 카메라들(VCam1, VCam2…)을 씬에 배치.
   - 빈 오브젝트 `CutsceneController`에 **Playable Director** 추가 → `Timeline Asset` 새로 생성/연결.

2. **트랙 구성**
   - **Cinemachine Track** 추가 → 바인딩을 `VCam1`, `VCam2` 등으로 맞추고 **Clip**(샷)을 배치해 카메라 이동/전환.
   - **Activation Track** 추가 → ‘등장시킬 오브젝트’(예: `Boss`)를 바인딩하고, 해당 시점에 **활성화 Clip**을 배치(등장 타이밍 정확).
   - (선택) **Animation Track**으로 오브젝트의 등장 애니메이션(스케일 업, 등장 포즈 등)도 함께 재생.
   - (선택) **Signal Track** 추가 → 원하는 프레임에 **Signal Emitter** 마커를 놓고, 대상에 **Signal Receiver**를 붙여 **UnityEvent**로 사운드/이펙트/함수 호출.

3. **재생**
   - Playable Director의 **Play On Awake**를 켜거나, 아래처럼 코드에서 `Play()` 호출.

---

## 🧩 언제 무엇을 쓰나?

- **카메라 샷 전환/이동** → *Cinemachine Track + Clips*  
  (클립 구간마다 VCam을 바꿔 자연스러운 블렌딩)
- **오브젝트 등장/퇴장** → *Activation Track + Clip*  
  (해당 구간에서 `SetActive(true/false)` 토글)
- **정확한 타이밍의 함수 호출** → *Signal*  
  (Emitter 시점에 Receiver가 미리 등록한 UnityEvent를 실행: 파티클 재생, SFX, 스크립트 함수 등)
- **정교한 모션** → *Animation Track + Clip*  
  (등장 시 스케일/위치/머터리얼 변화 등도 키프레임으로 제어)

---

## 🧪 최소 예제 코드

### 1) 타임라인 재생/종료 감지
```csharp
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneStarter : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private void Start()
    {
        if (director == null) director = GetComponent<PlayableDirector>();
        director.stopped += OnCutsceneEnd;
        director.Play(); // Start에서 자동 재생
    }

    private void OnCutsceneEnd(PlayableDirector d)
    {
        // 컷씬 끝난 뒤 게임플레이 전환 등
        Debug.Log("Cutscene finished!");
    }
}
