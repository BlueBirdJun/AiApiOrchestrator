# AI API Orchestrator

.NET 9 기반의 ASP.NET Web 애플리케이션으로, 사용자가 자연어 프롬프트를 입력하면 AI가 적절한 Sample API를 판단하여 호출하고 결과를 반환하는 시스템입니다.

## 목차

- [개요](#개요)
- [주요 기능](#주요-기능)
- [프로젝트 구조](#프로젝트-구조)
- [기술 스택](#기술-스택)
- [아키텍처](#아키텍처)
- [시작하기](#시작하기)
  - [사전 요구사항](#사전-요구사항)
  - [프로젝트 실행](#프로젝트-실행)
- [Ollama 설치 및 실행](#ollama-설치-및-실행)
- [사용 방법](#사용-방법)
- [API 엔드포인트](#api-엔드포인트)
- [확장 가능성](#확장-가능성)

## 개요

AI API Orchestrator는 자연어 프롬프트를 분석하여 적절한 API를 자동으로 선택하고 호출하는 지능형 오케스트레이션 시스템입니다. 두 가지 AI 모드(Ollama, Agent)를 지원하며, 계층형 아키텍처를 통해 확장 가능하고 유지보수가 쉬운 구조로 설계되었습니다.

## 주요 기능

- **다중 AI 모드 지원**: Ollama API와 Agent 방식 중 선택 가능
- **자동 API 판단**: 프롬프트 키워드 분석을 통한 자동 API 선택
- **Sample API 제공**: 날씨, 주식, 뉴스 정보 조회 API
- **계층형 아키텍처**: Domain, Application, Web 계층 분리
- **의존성 주입**: 느슨한 결합과 테스트 용이성
- **외부 의존성 최소화**: DB나 인증 없이 독립 실행 가능

## 프로젝트 구조

```
AiApiOrchestrator/
├── Domain/                          # 도메인 계층 - DTO 및 도메인 모델
│   ├── Dtos/
│   │   ├── AiRequest.cs            # AI 요청 DTO
│   │   ├── AiResponse.cs           # AI 응답 DTO
│   │   ├── ApiCallResult.cs        # API 호출 결과 DTO
│   │   └── SampleApiResponse.cs    # Sample API 응답 DTO
│   └── Enums/
│       └── AiMode.cs                # AI 모드 열거형 (Ollama, Agent)
│
├── Application/                     # 애플리케이션 계층 - 비즈니스 로직
│   ├── Interfaces/
│   │   ├── IAiService.cs           # AI 서비스 기본 인터페이스
│   │   ├── IOllamaAiService.cs     # Ollama AI 서비스 인터페이스
│   │   ├── IAgentAiService.cs      # Agent AI 서비스 인터페이스
│   │   ├── IApiOrchestrator.cs     # API 오케스트레이터 인터페이스
│   │   └── ISampleApiClient.cs     # Sample API 클라이언트 인터페이스
│   ├── Services/
│   │   ├── OllamaAiService.cs      # Ollama API 연동 서비스
│   │   ├── AgentAiService.cs       # Agent 방식 AI 서비스
│   │   └── ApiOrchestrator.cs      # API 판단 및 호출 오케스트레이터
│   └── SampleApis/
│       ├── ISampleApiClient.cs     # Sample API 클라이언트 인터페이스
│       └── SampleApiClient.cs      # Sample API 클라이언트 구현
│
├── Web/                             # 웹 계층 - UI 및 API 엔드포인트
│   ├── Controllers/
│   │   ├── AiController.cs         # AI 요청 처리 컨트롤러
│   │   └── SampleApiController.cs  # Sample API 엔드포인트
│   ├── Pages/
│   │   ├── Index.cshtml            # 메인 UI 페이지
│   │   └── Index.cshtml.cs         # 페이지 모델
│   ├── wwwroot/
│   │   ├── css/
│   │   │   └── site.css            # 스타일시트
│   │   └── js/
│   │       └── site.js             # JavaScript 로직
│   ├── Program.cs                   # 애플리케이션 진입점 및 DI 설정
│   └── appsettings.json            # 구성 파일
│
└── AiApiOrchestrator.csproj        # 프로젝트 파일
```

## 기술 스택

- **.NET 9**: 최신 .NET 플랫폼
- **ASP.NET Core Web**: 웹 애플리케이션 프레임워크
- **Razor Pages**: 서버 사이드 렌더링 UI
- **의존성 주입**: 내장 DI 컨테이너
- **HttpClient**: HTTP 통신 (IHttpClientFactory 사용)
- **추가 패키지 없음**: 기본 SDK만 사용

## 아키텍처

계층형 아키텍처를 따르며, 각 계층은 명확한 책임을 가집니다:

### Domain 계층
- 비즈니스 엔티티 및 DTO 정의
- 도메인 로직과 무관한 순수 데이터 구조
- 다른 계층에 의존하지 않음

### Application 계층
- 비즈니스 로직 및 서비스 구현
- AI 서비스 인터페이스 및 구현체
- API 오케스트레이션 로직
- Domain 계층에만 의존

### Web 계층
- 사용자 인터페이스 (Razor Pages)
- API 엔드포인트 (Controllers)
- 의존성 주입 설정
- Application 및 Domain 계층에 의존

### 데이터 흐름

```
사용자 입력 (UI)
    ↓
AiController
    ↓
[AI 모드 선택]
    ↓
OllamaAiService / AgentAiService
    ↓
ApiOrchestrator (키워드 분석)
    ↓
SampleApiClient
    ↓
SampleApiController
    ↓
응답 반환
```

## 시작하기

### 사전 요구사항

1. **.NET 9 SDK** 설치
   - [다운로드 링크](https://dotnet.microsoft.com/download/dotnet/9.0)
   - 설치 확인: `dotnet --version`

2. **Ollama** (선택사항 - Ollama 모드 사용 시)
   - [Ollama 설치 가이드](#ollama-설치-및-실행) 참조

### 프로젝트 실행

1. **프로젝트 클론 또는 다운로드**
   ```bash
   cd AiApiOrchestrator
   ```

2. **프로젝트 빌드**
   ```bash
   dotnet build
   ```

3. **애플리케이션 실행**
   ```bash
   dotnet run --project Web
   ```
   
   또는 프로젝트 루트에서:
   ```bash
   dotnet run
   ```

4. **브라우저에서 접속**
   - URL: `http://localhost:5000` 또는 `https://localhost:5001`
   - 콘솔에 표시된 URL 확인

5. **애플리케이션 종료**
   - 터미널에서 `Ctrl + C`

## Ollama 설치 및 실행

Ollama 모드를 사용하려면 로컬에서 Ollama 서버를 실행해야 합니다.

### Windows에서 Ollama 설치

1. **Ollama 다운로드**
   - [Ollama 공식 웹사이트](https://ollama.ai/) 방문
   - Windows용 설치 파일 다운로드

2. **설치 실행**
   - 다운로드한 설치 파일 실행
   - 설치 마법사 따라 진행

3. **Ollama 서버 실행**
   ```bash
   ollama serve
   ```
   - 기본적으로 `http://localhost:11434`에서 실행됨

4. **llama3 모델 다운로드**
   ```bash
   ollama pull llama3
   ```
   - 첫 실행 시 모델 다운로드 필요 (약 4.7GB)

5. **Ollama 실행 확인**
   ```bash
   ollama list
   ```
   - 설치된 모델 목록 확인

### macOS/Linux에서 Ollama 설치

```bash
# macOS/Linux
curl -fsSL https://ollama.ai/install.sh | sh

# 서버 실행
ollama serve

# 모델 다운로드
ollama pull llama3
```

### Ollama 설정 변경

`appsettings.json` 파일에서 Ollama 설정을 변경할 수 있습니다:

```json
{
  "OllamaSettings": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3",
    "TimeoutSeconds": 30
  }
}
```

## 사용 방법

### 1. 웹 UI 사용

1. 브라우저에서 `http://localhost:5000` 접속
2. 프롬프트 입력창에 자연어 질문 입력
3. AI 모드 선택 (Ollama 또는 Agent)
4. "전송" 버튼 클릭
5. 결과 확인

### 2. 프롬프트 예시

#### 날씨 정보 조회
```
서울 날씨 알려줘
오늘 날씨 어때?
weather in Seoul
```

#### 주식 정보 조회
```
애플 주식 정보 알려줘
AAPL stock price
삼성전자 주가는?
```

#### 뉴스 정보 조회
```
최신 기술 뉴스 알려줘
technology news
오늘의 뉴스는?
```

#### 일반 질문 (API 호출 없음)
```
안녕하세요
AI에 대해 설명해줘
How are you?
```

### 3. AI 모드 설명

#### Ollama 모드
- Ollama 로컬 서버와 연동
- llama3 모델 사용
- 실제 AI 응답 생성
- **요구사항**: Ollama 서버 실행 필요

#### Agent 모드
- 더미 Agent 로직 사용
- Ollama 없이 테스트 가능
- 빠른 응답 시간
- **장점**: 외부 의존성 없음

### 4. 응답 형식

```json
{
  "result": "AI가 생성한 응답 텍스트",
  "apiCalled": "GetWeather",
  "success": true,
  "errorMessage": null
}
```

## API 엔드포인트

### AI 처리 API

**POST** `/api/ai/process`

요청 본문:
```json
{
  "prompt": "서울 날씨 알려줘",
  "mode": 0
}
```

- `mode`: 0 = Ollama, 1 = Agent

응답:
```json
{
  "result": "서울의 현재 날씨는 맑음이며, 기온은 15°C입니다.",
  "apiCalled": "GetWeather",
  "success": true,
  "errorMessage": null
}
```

### Sample API 엔드포인트

#### 날씨 정보
**GET** `/api/sample/weather?location={location}`

응답:
```json
{
  "message": "Weather information retrieved",
  "data": {
    "location": "Seoul",
    "temperature": "15°C",
    "condition": "Sunny"
  }
}
```

#### 주식 정보
**GET** `/api/sample/stock?symbol={symbol}`

응답:
```json
{
  "message": "Stock information retrieved",
  "data": {
    "symbol": "AAPL",
    "price": "$150.25",
    "change": "+2.5%"
  }
}
```

#### 뉴스 정보
**GET** `/api/sample/news?topic={topic}`

응답:
```json
{
  "message": "News retrieved",
  "data": {
    "topic": "Technology",
    "headline": "Latest tech news",
    "summary": "Summary of the news"
  }
}
```

## 확장 가능성

### 새로운 AI 서비스 추가

1. `IAiService`를 구현하는 새 인터페이스 생성
2. 서비스 클래스 구현
3. `Program.cs`에 DI 등록
4. `AiController`에서 새 모드 처리

### 새로운 Sample API 추가

1. `SampleApiController`에 새 엔드포인트 추가
2. `ISampleApiClient`에 새 메서드 정의
3. `SampleApiClient`에 구현 추가
4. `ApiOrchestrator`의 키워드 매핑에 추가

### API 판단 로직 고도화

현재는 단순 키워드 매칭이지만, 다음과 같이 확장 가능:
- 정규표현식 기반 매칭
- ML 모델을 사용한 의도 분류
- Function Calling 패턴 적용

## 문제 해결

### Ollama 연결 실패
```
오류: Ollama 서버에 연결할 수 없습니다.
```

**해결 방법**:
1. Ollama 서버가 실행 중인지 확인: `ollama serve`
2. `appsettings.json`의 BaseUrl 확인
3. 방화벽 설정 확인

### 포트 충돌
```
오류: Address already in use
```

**해결 방법**:
1. 다른 포트 사용: `dotnet run --urls "http://localhost:5001"`
2. 또는 `launchSettings.json`에서 포트 변경

### 빌드 오류
```
오류: SDK를 찾을 수 없습니다.
```

**해결 방법**:
1. .NET 9 SDK 설치 확인: `dotnet --version`
2. 환경 변수 PATH 확인

## 라이선스

이 프로젝트는 교육 및 레퍼런스 목적으로 제공됩니다.

## 기여

이슈 및 개선 제안은 환영합니다!

## 참고 자료

- [.NET 9 문서](https://docs.microsoft.com/dotnet/)
- [ASP.NET Core 문서](https://docs.microsoft.com/aspnet/core/)
- [Ollama 문서](https://ollama.ai/)
