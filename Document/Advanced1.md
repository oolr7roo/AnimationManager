[README](../README.md)

# 🎛️ Unity Playables API 코드 기반 재생·블렌딩

`Playables API`는 **Animator Controller 없이**도 코드를 통해 애니메이션을 재생/블렌딩/레이어링할 수 있는 저수준(=미세제어) 시스템입니다.  
컷씬, 절차적 전환, 런타임에 클립을 동적으로 갈아끼우는 상황에 특히 유용합니다.

---

## 1) 핵심 개념

| 요소 | 역할 | 한 줄 요약 |
|---|---|---|
| **PlayableGraph** | 모든 플레이어블을 담는 그래프(파이프라인) | “그래프 컨테이너” |
| **AnimationClipPlayable** | 단일 애니메이션 클립을 재생하는 노드 | “클립 재생기” |
| **AnimationMixerPlayable** | 여러 입력을 가중치로 섞는 노드 | “블렌더(가중치 합성)” |
| **AnimationLayerMixerPlayable** | 레이어 기반 합성(+ **AvatarMask**) | “상·하체 등 부분 레이어링” |
| **AnimationPlayableOutput** | 그래프 출력 → **Animator**에 전달 | “출구(Animator로 보냄)” |

> ⚙️ **필수**: 씬에 **Animator 컴포넌트**가 있어야 하며(Avatar 포함),  
> Controller는 **비워도** 됩니다(Playables가 직접 구동). 둘을 **혼용**하면 간섭될 수 있어요.

---

## 2) 언제 Playables를 쓰나?
- Animator Controller로 표현하기 어려운 **즉석 블렌딩/크로스페이드**  
- 런타임에 **애니메이션 세트가 바뀌는** 시스템(랜덤/모듈식)  
- **상·하체 분리**, 특정 본만 교체 등 **부분 레이어링**(AvatarMask)  
- 타임라인/시스템과 연동한 **정밀 타이밍 제어**

---

## 3) 기본 패턴(생명주기)

```csharp
// OnEnable에서 Create → 연결 → Play
graph = PlayableGraph.Create("MyGraph");
var output = AnimationPlayableOutput.Create(graph, "AnimOut", animator);
var clipPlayable = AnimationClipPlayable.Create(graph, someClip);
output.SetSourcePlayable(clipPlayable);
graph.Play();

// OnDisable/OnDestroy에서 Destroy
graph.Destroy();
