# Requirements Document

## Introduction

이 프로젝트는 .NET 9 기반의 ASP.NET Web 애플리케이션으로, 사용자가 자연어 프롬프트를 입력하면 AI가 적절한 Sample API를 판단하여 호출하고 결과를 반환하는 AI API Orchestrator입니다. Ollama 로컬 서버와 일반 Agent 방식 두 가지 AI 연동 모드를 지원하며, 계층형 아키텍처(Domain, Application, Web)로 구조화되어 확장 가능하고 유지보수가 용이한 레퍼런스 코드를 제공합니다.

## Requirements

### Requirement 1: 프로젝트 기본 구조

**User Story:** 개발자로서, .NET 9 기반의 체계적인 ASP.NET 프로젝트 구조를 원하며, 이를 통해 코드의 가독성과 유지보수성을 확보하고 싶습니다.

#### Acceptance Criteria

1. WHEN 프로젝트가 생성되면 THEN 시스템은 .NET 9 SDK를 사용하여 ASP.NET Web 프로젝트를 생성해야 합니다.
2. WHEN 프로젝트 구조를 확인하면 THEN 시스템은 Domain, Application, Web 세 개의 계층으로 분리된 폴더 구조를 가져야 합니다.
3. WHEN 코드를 검토하면 THEN 각 파일은 역할을 설명하는 주석을 포함해야 합니다.
4. IF 프로젝트가 실행되면 THEN 시스템은 DB나 인증 없이 독립적으로 실행되어야 합니다.

### Requirement 2: AI 연동 방식 지원

**User Story:** 개발자로서, Ollama API와 일반 Agent 방식 두 가지 AI 연동 모드를 지원하여, 다양한 AI 서비스와 통합할 수 있는 유연한 구조를 원합니다.

#### Acceptance Criteria

1. WHEN Ollama 모드가 선택되면 THEN 시스템은 http://localhost:11434 Ollama 로컬 서버와 HTTP 통신으로 연결해야 합니다.
2. WHEN Ollama API를 호출하면 THEN 시스템은 "llama3" 모델을 사용하여 요청을 전송해야 합니다.
3. WHEN Agent 모드가 선택되면 THEN 시스템은 더미 로직으로 구현된 Agent 서비스를 호출해야 합니다.
4. WHEN AI 서비스가 구현되면 THEN 각 서비스는 IOllamaAiService, IAgentAiService 인터페이스를 구현해야 합니다.
5. IF 새로운 AI 서비스를 추가하려면 THEN 개발자는 인터페이스를 구현하고 DI에 등록하는 것만으로 확장할 수 있어야 합니다.

### Requirement 3: Sample API 판단 및 호출

**User Story:** 사용자로서, 자연어 프롬프트를 입력하면 AI가 적절한 Sample API를 자동으로 판단하여 호출하고 결과를 받고 싶습니다.

#### Acceptance Criteria

1. WHEN 사용자가 프롬프트를 입력하면 THEN AI는 프롬프트를 분석하여 호출할 Sample API를 판단해야 합니다.
2. WHEN AI가 API를 판단하면 THEN 시스템은 해당 Sample API를 내부적으로 호출해야 합니다.
3. WHEN Sample API가 호출되면 THEN 시스템은 API 결과를 사용자에게 반환해야 합니다.
4. WHEN 시스템이 구현되면 THEN 최소 2개 이상의 Sample API(예: GetWeather, GetStockInfo)를 제공해야 합니다.
5. IF API 판단 로직이 구현되면 THEN 단순한 키워드 매칭 또는 사전 매핑 방식으로 구성되어야 합니다.
6. IF 새로운 Sample API를 추가하려면 THEN 판단 로직을 쉽게 확장할 수 있는 구조여야 합니다.

### Requirement 4: 사용자 인터페이스

**User Story:** 사용자로서, 간단한 웹 UI를 통해 프롬프트를 입력하고 AI 모드를 선택하여 결과를 확인하고 싶습니다.

#### Acceptance Criteria

1. WHEN UI 페이지에 접속하면 THEN 프롬프트 입력창, AI 모드 선택 드롭다운, 결과 표시 영역이 표시되어야 합니다.
2. WHEN 사용자가 AI 모드를 선택하면 THEN "Ollama" 또는 "Agent" 옵션을 선택할 수 있어야 합니다.
3. WHEN 사용자가 프롬프트를 입력하고 전송하면 THEN 시스템은 선택된 모드에 따라 AI 서비스를 호출해야 합니다.
4. WHEN AI 처리가 완료되면 THEN 결과가 결과 표시 영역에 표시되어야 합니다.
5. IF UI가 구현되면 THEN Razor Pages 또는 최소한의 HTML로 구성되어야 합니다.

### Requirement 5: 의존성 주입 및 서비스 등록

**User Story:** 개발자로서, DI(Dependency Injection) 패턴을 사용하여 서비스를 관리하고, 테스트 가능하고 유지보수가 쉬운 코드를 작성하고 싶습니다.

#### Acceptance Criteria

1. WHEN Program.cs가 실행되면 THEN 모든 AI 서비스가 DI 컨테이너에 등록되어야 합니다.
2. WHEN Controller가 생성되면 THEN 생성자를 통해 필요한 서비스를 주입받아야 합니다.
3. WHEN 서비스가 호출되면 THEN DI 컨테이너가 인스턴스를 관리하고 제공해야 합니다.
4. IF HttpClient가 필요하면 THEN IHttpClientFactory를 통해 주입받아야 합니다.

### Requirement 6: 계층형 아키텍처

**User Story:** 개발자로서, Domain, Application, Web 계층으로 명확히 분리된 구조를 통해 관심사의 분리와 코드 재사용성을 확보하고 싶습니다.

#### Acceptance Criteria

1. WHEN Domain 계층이 구현되면 THEN DTO 및 도메인 모델만 포함해야 합니다.
2. WHEN Application 계층이 구현되면 THEN AI 서비스 인터페이스, 구현체, API 판단 로직을 포함해야 합니다.
3. WHEN Web 계층이 구현되면 THEN Controller, Razor Page, Program.cs를 포함해야 합니다.
4. IF 계층 간 의존성이 발생하면 THEN Web → Application → Domain 방향으로만 의존해야 합니다.
5. WHEN 코드를 검토하면 THEN 각 계층의 책임이 명확히 분리되어 있어야 합니다.

### Requirement 7: 실행 및 테스트

**User Story:** 개발자로서, 프로젝트를 쉽게 실행하고 테스트할 수 있는 명확한 가이드를 원합니다.

#### Acceptance Criteria

1. WHEN 실행 가이드를 확인하면 THEN 프로젝트 실행 방법이 명확히 설명되어야 합니다.
2. WHEN 프로젝트를 실행하면 THEN 외부 의존성(DB, 인증) 없이 독립적으로 실행되어야 합니다.
3. IF Ollama 모드를 사용하려면 THEN Ollama 로컬 서버가 실행 중이어야 합니다.
4. WHEN Agent 모드를 사용하면 THEN Ollama 없이도 더미 응답으로 테스트할 수 있어야 합니다.
