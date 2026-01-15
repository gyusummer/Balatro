# Balatro (Unity Clone Project)

Unity로 구현한 **Balatro** 모작 프로젝트입니다.
덱 빌딩 로그라이크 장르의 핵심 메커니즘(점수 계산, 조커 시너지, 덱 관리)과 게임의 몰입감을 높이는 시각적 연출(Juicy Effect)을 충실히 재현하는 데 중점을 두었습니다.

---

## 🚀 프로젝트 개요 (Project Overview)

*   **목표**: 상용 게임 *Balatro*의 복잡한 덱 빌딩 시스템과 생동감있는 UI/UX를 Unity 엔진으로 분석 및 구현
*   **핵심 구현**: 포커 족보 판정, 조커/타로/행성 카드 시스템, 앤티(Ante) 기반의 게임 루프, 효과에 따른 애니메이션

## ✨ 주요 기능 및 기술적 특징 (Key Features)

### 1. 시스템 아키텍처 (System Architecture)
*   **제네릭 옵저버 패턴 (Generic Observer Pattern)**: `EventJoker<T>`와 `IngameEventManager`를 통해 다양한 게임 이벤트(득점, 리롤, 카드 폐기 등)를 유연하게 발행하고 구독하는 구조를 구축하여 결합도를 낮췄습니다.
*   **반응형 데이터 컬렉션 (Reactive Data Collection)**: 데이터(`List<T>`)의 변경(추가/삭제)이 자동으로 UI(`View`)의 생성 및 파괴로 이어지도록 `CustomList<T>`를 구현하여 데이터-뷰 바인딩을 처리했습니다.

### 2. 게임플레이 로직 (Gameplay Logic)
*   **연출 동기화 파이프라인 (Animation Pipeline)**: `DOTween.Sequence`를 활용하여 점수 계산 로직과 시각적 연출(카드 득점, 조커 발동 등)이 정확한 순서대로 실행되도록 동기화했습니다.
*   **카드 상호작용 (Card Interaction)**: 핸드 내 카드의 드래그 앤 드롭, 수동 정렬 알고리즘, 부채꼴 펼침(Fan Layout) 등 디테일한 UX를 구현했습니다.

### 3. 그래픽스 및 연출 (Visuals & Graphics)
*   **다이내믹 텍스트 연출 (Text Juice)**: 점수 갱신 시 글자 단위의 스케일/위치 변화(Punch/Shake)를 주어 타격감을 극대화했습니다.
*   **셰이더 효과 (Shader Effects)**: 카드의 등급(Holographic, Foil, Polychrome 등)에 따라 다르게 적용되는 셰이더와 아틀라스 UV 보정 로직을 포함합니다.
*   **동적 툴팁**: UI 요소의 화면 위치를 계산하여 적절한 위치에 툴팁을 표시하는 시스템을 구현했습니다.

## 🛠 기술 스택 (Tech Stack)

*   **Engine**: Unity 2022.3+
*   **Language**: C#
*   **Libraries**:
    *   **DOTween**: 애니메이션 시퀀싱 및 트윈 제어
*   **Tools**: Git (버전 관리)