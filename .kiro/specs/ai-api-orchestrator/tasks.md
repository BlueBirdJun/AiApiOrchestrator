# Implementation Plan

- [x] 1. 프로젝트 초기 설정 및 구조 생성





  - .NET 9 ASP.NET Web 프로젝트 생성
  - Domain, Application, Web 폴더 구조 생성
  - 필요한 NuGet 패키지 참조 추가 (없음, 기본 SDK만 사용)
  - _Requirements: 1.1, 1.2_
- [x] 2. Domain 계층 구현



- [ ] 2. Domain 계층 구현

  - [x] 2.1 Enum 정의


    - AiMode enum 생성 (Ollama, Agent)
    - _Requirements: 2.1, 2.3_
  - [x] 2.2 DTO 클래스 생성


    - AiRequest.cs 작성 (Prompt, Mode 속성)
    - AiResponse.cs 작성 (Result, ApiCalled, Success, ErrorMessage 속성)
    - ApiCallResult.cs 작성 (ApiName, Data, Success 속성)
    - SampleApiResponse.cs 작성 (Message, Data 속성)
    - _Requirements: 3.1, 3.2, 3.3, 4.3_


- [x] 3. Application 계층 - 인터페이스 정의



  - [x] 3.1 AI 서비스 인터페이스 작성


    - IAiService.cs 작성 (ProcessPromptAsync 메서드)
    - IOllamaAiService.cs 작성 (IAiService 상속)
    - IAgentAiService.cs 작성 (IAiService 상속)
    - _Requirements: 2.4, 5.2_
  - [x] 3.2 Orchestrator 및 Sample API 인터페이스 작성


    - IApiOrchestrator.cs 작성 (DetermineAndCallApiAsync 메서드)
    - ISampleApiClient.cs 작성 (GetWeatherAsync, GetStockInfoAsync, GetNewsAsync 메서드)
    - _Requirements: 3.1, 3.2, 3.4_


- [x] 4. Application 계층 - Sample API Client 구현




  - SampleApiClient.cs 구현
  - GetWeatherAsync 메서드: 더미 날씨 데이터 반환
  - GetStockInfoAsync 메서드: 더미 주식 데이터 반환
  - GetNewsAsync 메서드: 더미 뉴스 데이터 반환
  - 각 메서드에 적절한 주석 추가
  - _Requirements: 3.2, 3.4_

- [x] 5. Application 계층 - API Orchestrator 구현





  - ApiOrchestrator.cs 구현
  - 키워드 매핑 Dictionary 생성 (날씨/weather → GetWeather, 주식/stock → GetStockInfo, 뉴스/news → GetNews)
  - DetermineAndCallApiAsync 메서드: 프롬프트에서 키워드 추출 및 매칭
  - ISampleApiClient를 생성자 주입으로 받아 적절한 API 호출
  - 매칭되는 키워드가 없을 경우 기본 응답 반환
  - _Requirements: 3.1, 3.5, 3.6_


- [x] 6. Application 계층 - Ollama AI Service 구현



  - [x] 6.1 OllamaSettings 구성 클래스 작성


    - BaseUrl, Model, TimeoutSeconds 속성 정의
    - _Requirements: 2.1_
  - [x] 6.2 OllamaAiService.cs 구현


    - IHttpClientFactory와 IApiOrchestrator를 생성자 주입
    - IOptions<OllamaSettings>로 설정 주입
    - ProcessPromptAsync 메서드 구현:
      - ApiOrchestrator로 Sample API 호출
      - Ollama API에 HTTP POST 요청 (/api/generate 엔드포인트)
      - 요청 본문: { "model": "llama3", "prompt": "...", "stream": false }
      - Ollama 응답과 API 호출 결과를 결합하여 AiResponse 반환
    - 예외 처리: HttpRequestException 발생 시 Success=false, ErrorMessage 설정
    - _Requirements: 2.1, 2.2, 3.2, 3.3, 5.1, 5.4_

- [x] 7. Application 계층 - Agent AI Service 구현





  - AgentAiService.cs 구현
  - IApiOrchestrator를 생성자 주입
  - ProcessPromptAsync 메서드 구현:
    - ApiOrchestrator로 Sample API 호출
    - 더미 Agent 응답 생성 (예: "Agent 모드로 처리: {prompt}")
    - API 호출 결과와 결합하여 AiResponse 반환
  - _Requirements: 2.3, 3.2, 3.3, 5.1_

- [x] 8. Web 계층 - Sample API Controller 구현





  - SampleApiController.cs 작성
  - GET /api/sample/weather 엔드포인트: location 쿼리 파라미터 받아 더미 날씨 데이터 반환
  - GET /api/sample/stock 엔드포인트: symbol 쿼리 파라미터 받아 더미 주식 데이터 반환
  - GET /api/sample/news 엔드포인트: topic 쿼리 파라미터 받아 더미 뉴스 데이터 반환
  - 각 엔드포인트는 SampleApiResponse 형식으로 JSON 반환
  - _Requirements: 3.4_

- [x] 9. Web 계층 - AI Controller 구현





  - AiController.cs 작성
  - IOllamaAiService와 IAgentAiService를 생성자 주입
  - POST /api/ai/process 엔드포인트 구현:
    - AiRequest를 요청 본문으로 받음
    - Mode에 따라 적절한 AI 서비스 선택 (switch 또는 Dictionary 사용)
    - 선택된 서비스의 ProcessPromptAsync 호출
    - AiResponse 반환
  - try-catch로 예외 처리: 500 Internal Server Error 반환
  - _Requirements: 4.2, 4.3, 5.2, 5.3_
-

- [x] 10. Web 계층 - Program.cs 구성




  - HttpClient 등록: builder.Services.AddHttpClient()
  - AI 서비스 등록: AddScoped<IOllamaAiService, OllamaAiService>(), AddScoped<IAgentAiService, AgentAiService>()
  - Orchestrator 등록: AddScoped<IApiOrchestrator, ApiOrchestrator>()
  - Sample API Client 등록: AddScoped<ISampleApiClient, SampleApiClient>()
  - OllamaSettings 구성: builder.Services.Configure<OllamaSettings>(builder.Configuration.GetSection("OllamaSettings"))
  - Controllers 등록: builder.Services.AddControllers()
  - Razor Pages 등록: builder.Services.AddRazorPages()
  - 미들웨어 파이프라인 구성: UseStaticFiles, UseRouting, MapControllers, MapRazorPages
  - _Requirements: 5.1, 5.2, 5.3, 5.4_

- [x] 11. Web 계층 - appsettings.json 구성





  - OllamaSettings 섹션 추가 (BaseUrl: "http://localhost:11434", Model: "llama3", TimeoutSeconds: 30)
  - Logging 설정
  - _Requirements: 2.2_

- [x] 12. Web 계층 - Razor Page UI 구현




  - [x] 12.1 Index.cshtml.cs 작성






    - PageModel 클래스 생성
    - 필요한 경우 OnGet, OnPost 메서드 정의 (선택적, JavaScript로 API 호출 시 불필요)
    - _Requirements: 4.1_
  - [x] 12.2 Index.cshtml 작성







    - HTML 구조: 프롬프트 입력 textarea, AI 모드 선택 select (Ollama/Agent), 전송 button, 결과 표시 div
    - 기본 CSS 스타일 적용 (간단한 레이아웃)
    - _Requirements: 4.1, 4.2, 4.5_
-

  - [x] 12.3 JavaScript 작성 (site.js 또는 인라인)





    - 전송 버튼 클릭 이벤트 핸들러
    - fetch API로 /api/ai/process에 POST 요청
    - 요청 본문: { "prompt": "...", "mode": 0 또는 1 }
    - 응답 받아 결과 영역에 표시
    - 로딩 상태 표시 (선택적)
    - 에러 처리
    - _Requirements: 4.3, 4.4_


- [x] 13. 기본 스타일링 추가




  - wwwroot/css/site.css 작성
  - 간단한 레이아웃 스타일 (중앙 정렬, 패딩, 마진)
  - 입력 요소 스타일
  - 결과 영역 스타일
  - _Requirements: 4.5_

- [x] 14. 프로젝트 파일 검증 및 빌드 테스트





  - dotnet build 실행하여 컴파일 오류 확인
  - 모든 의존성이 올바르게 주입되는지 확인
  - _Requirements: 1.1, 1.4_


- [x] 15. 통합 테스트 및 검증




  - dotnet run 실행
  - 브라우저에서 http://localhost:5000 접속
  - Agent 모드로 다양한 프롬프트 테스트 (날씨, 주식, 뉴스 키워드 포함)
  - Ollama 모드 테스트 (Ollama 서버 실행 필요)
  - Ollama 서버 미실행 시 에러 메시지 확인
  - _Requirements: 7.1, 7.2, 7.3, 7.4_


- [x] 16. 문서화 및 실행 가이드 작성



  - README.md 작성
  - 프로젝트 구조 설명
  - 실행 방법 (dotnet run)
  - Ollama 설치 및 실행 방법
  - 사용 예시 (프롬프트 샘플)
  - _Requirements: 7.1, 1.3_
