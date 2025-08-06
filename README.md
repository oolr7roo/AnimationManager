

[Doc](../../../../Document/Component.md)
# CustomPaint
<details open>
<summary><font size ="4"> <b>설계 </b></font></summary>

TempleteUI와 ItemUI로 나뉜다.


## 주요 클래스 
[TempleteUI](./Document~/TempleteUI.md)
[MultiItemUI](./Document~/MultiItemUI.md)
[ItemUI](./Document~/ItemUI.md)

</details>

<details open>
<summary><font size ="4"> <b>동적 Templete 세팅 방법 </b></font></summary>

1. Scene Installer세팅
        a. public const string TitleBarAction; // 이런식으로 PrefabInstaller에 이름붙이기
        b.  Container.Bind( LayoutAction) 외부의 Action들 주입시키기
2. Prefab Installer세팅
        a. BaseInject()에서 LayoutAction 주입
        b. 커스텀할  Prefab이 있으면 CustomPrefabSetting을 통하여 주입
        c. ViewModel, View 바인딩
        d. ViewModel에서 정의된 이름을 가져와서 LayoutAction 바인드
3. ViewModel 세팅하기
        a. GetLayoutType으로 분류해서 잘 집어넣기
4. View에 알맞게 배치하기
</details>


<details open>
<summary><font size ="4"> <b>정적 Templete 세팅 방법 </b></font></summary>

1. Scene Installer세팅
        a. public const string TitleBarAction; // 이런식으로 PrefabInstaller에 이름붙이기
        b.  Container.Bind( LayoutAction) 외부의 Action들 주입시키기
2. Prefab Installer세팅
        a. BaseInject()에서 LayoutAction 주입
        b. 기존의 Model에 끼워넣기.
        c. ViewModel, View 바인딩
3. ViewModel 세팅하기
        a. GetLayoutType으로 분류해서 잘 집어넣기
4. View에 알맞게 배치하기
</details>




<details open>
<summary><font size ="4"> <b>PrefabInstaller간의 통신?방법 </b></font></summary>

1. Scene Installer세팅
        a. 공통된 모듈을 만들어둔다.
        b. 주고 받을 값을 ReactiveProperty에 넣어둔다. 
        c. Container.Bind( LayoutAction) 넣는 곳에 b값을 넣는다.

2. Prefab Installer세팅
        a. BaseInject()에서 LayoutAction 주입
        b. 기존의 Model에 끼워넣기.
        c. ViewModel, View 바인딩
        d. ViewModel에서 정의된 이름을 가져와서 WithID에 넣고 LayoutAction 바인드
3. ViewModel 세팅하기
        a. GetLayoutType으로 분류해서 잘 집어넣기
4. View에 알맞게 배치하기
</details>




<details open>
<summary><font size ="4"> <b>복합 컴포넌트 조립방법법 - ex)Popup </b></font></summary>

1. Scene Installer세팅
        a. popupModel 선언
        b. popupModel 에 이것 저것 이벤트 정의
2. Prefab Installer세팅
        a. BaseInject() 에서 model주입
        b. 복합 아이템의 Model에 ProjectInstaller모델 주입.
        c. _uxDictionary에 넣고 VM에 전달 후 만들기
3. ViewModel 세팅하기
        a. GetLayoutType으로 분류해서 잘 집어넣기
4. View에 알맞게 배치하기
</details>

