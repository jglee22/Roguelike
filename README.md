# ⚔️ 그림자의 탑 (Tower of Shadows)
> **"데이터 기반 시스템과 확장성 있는 FSM을 활용한 로그라이크 액션 프레임워크"**

## 📝 프로젝트 요약
층별 무작위 업그레이드와 보스 전투 중심의 로그라이크 액션 게임입니다. **콘텐츠 확장이 용이한 구조적 설계**를 실무 수준으로 구현하여 유지보수성을 극대화하는 데 초점을 맞췼습니다.

## 🛠 핵심 설계 및 아키텍처 (6년 차 역량 포인트)
- **Data-Driven System:** `ScriptableObject`를 활용해 밸런스 데이터를 외부화하여, 비개발 직군도 데이터 에셋만으로 게임성을 조절할 수 있는 환경을 구축했습니다.
- **Scalable AI (FSM):** 적 AI 상태를 독립 클래스로 모듈화하여 신규 패턴 추가 시 기존 코드 수정을 최소화하는 개방-폐쇄 원칙(OCP)을 준수합니다.
- **Centralized Game Flow:** `GameManager`를 통한 중앙 집중식 상태 관리 및 `DontDestroyOnLoad`를 이용한 데이터 영속성을 확보했습니다.
- **Player Interaction:** 최신 Input System 연동 및 스태미나 기반의 정교한 콤보 로직을 구현했습니다.

## ⚙️ 개발 환경 및 실행 방법
- **Unity Version:** 2023.3 LTS 이상 (URP 17.x 호환)
- **Target Platform:** PC (Windows)
- **How to Start:**
  1. `git clone https://github.com/jglee22/Roguelike`
  2. Unity Hub에서 프로젝트 오픈 후 패키지 동기화 대기
  3. `Assets/Scenes/Main.unity` 실행

## 🤝 협업 및 개발 규칙 (Professional Workflow)
팀 프로젝트의 효율성과 코드 품질을 위해 아래 규칙을 준수합니다.
- **Git Flow:** `main`(배포), `develop`(통합), `feature/<name>`(기능 단위 개발) 브랜치 전략 사용
- **Commit Convention:** 한 번의 커밋은 하나의 목적만 수행하며, 제목+본문 구조의 명확한 메시지 작성
- **PR Workflow:** 변경 요약 및 테스트 방법이 포함된 PR 템플릿을 통한 코드 리뷰 지향

## 🧪 테스트 및 품질 관리
- **Test Framework:** `com.unity.test-framework`를 도입하여 핵심 로직 검증
- **Strategy:** Play Mode와 Edit Mode 테스트를 분리하여 로직의 무결성 확보 권장

## 🚀 프로젝트 고도화 및 유지보수 로드맵
현재 프로토타입 단계를 넘어 상용 수준의 안정성을 확보하기 위해 아래 항목을 순차적으로 고도화하고 있습니다.
- [ ] **Setup.md 작성:** 신규 팀원 온보딩을 위한 상세 에디터 설정 가이드 구축
- [ ] **Scene Workflow Guide:** 각 씬의 역할과 진입점 명시로 협업 효율 증대
- [ ] **CI 자동화:** Unity Test Runner 기반의 자동 테스트 및 빌드 파이프라인 구축