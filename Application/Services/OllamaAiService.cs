using System.Text;
using System.Text.Json;
using AiApiOrchestrator.Application.Interfaces;
using AiApiOrchestrator.Domain.Dtos;
using Microsoft.Extensions.Options;

namespace AiApiOrchestrator.Application.Services;

/// <summary>
/// Ollama AI 서비스 구현
/// Ollama 로컬 서버와 HTTP 통신하여 AI 기능을 제공합니다.
/// </summary>
public class OllamaAiService : IOllamaAiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApiOrchestrator _apiOrchestrator;
    private readonly OllamaSettings _settings;

    public OllamaAiService(
        IHttpClientFactory httpClientFactory,
        IApiOrchestrator apiOrchestrator,
        IOptions<OllamaSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _apiOrchestrator = apiOrchestrator;
        _settings = settings.Value;
    }

    /// <summary>
    /// 사용자 프롬프트를 처리하고 AI 응답을 반환합니다.
    /// ApiOrchestrator로 Sample API를 호출하고, Ollama API에 요청하여 결과를 결합합니다.
    /// </summary>
    /// <param name="prompt">처리할 사용자 프롬프트</param>
    /// <returns>AI 처리 결과를 포함하는 AiResponse</returns>
    public async Task<AiResponse> ProcessPromptAsync(string prompt)
    {
        try
        {
            // 1. ApiOrchestrator로 Sample API 호출
            var apiCallResult = await _apiOrchestrator.DetermineAndCallApiAsync(prompt);

            // 2. API 결과를 포함한 향상된 프롬프트 생성
            var enhancedPrompt = BuildEnhancedPrompt(prompt, apiCallResult);
            
            // 디버깅: 향상된 프롬프트 출력
            Console.WriteLine("=== Enhanced Prompt ===");
            Console.WriteLine(enhancedPrompt);
            Console.WriteLine("======================");

            // 3. Ollama API에 HTTP POST 요청
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);

            var ollamaRequest = new
            {
                model = _settings.Model,
                prompt = enhancedPrompt,
                stream = false
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(ollamaRequest),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(
                $"{_settings.BaseUrl}/api/generate",
                requestContent);

            // 디버깅을 위한 상세 정보 출력
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Ollama API 호출 실패. Status: {response.StatusCode}, Content: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            
            // 디버깅: Ollama 응답 출력
            Console.WriteLine("=== Ollama Response ===");
            Console.WriteLine(responseContent);
            Console.WriteLine("======================");
            
            var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseContent);

            // 4. AI 응답 반환 (API 결과가 이미 AI 응답에 통합됨)
            return new AiResponse
            {
                Result = ollamaResponse?.Response ?? "응답 없음",
                ApiCalled = apiCallResult.ApiName,
                Success = true,
                ErrorMessage = null
            };
        }
        catch (HttpRequestException ex)
        {
            // HTTP 요청 실패 시 에러 처리
            return new AiResponse
            {
                Result = string.Empty,
                ApiCalled = string.Empty,
                Success = false,
                ErrorMessage = $"Ollama 서버에 연결할 수 없습니다. {_settings.BaseUrl}가 실행 중인지 확인하세요. 오류: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            // 기타 예외 처리
            return new AiResponse
            {
                Result = string.Empty,
                ApiCalled = string.Empty,
                Success = false,
                ErrorMessage = $"AI 처리 중 오류가 발생했습니다: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// API 결과를 포함한 향상된 프롬프트를 생성합니다.
    /// </summary>
    /// <param name="originalPrompt">원본 사용자 프롬프트</param>
    /// <param name="apiCallResult">API 호출 결과</param>
    /// <returns>향상된 프롬프트</returns>
    private string BuildEnhancedPrompt(string originalPrompt, ApiCallResult apiCallResult)
    {
        var promptBuilder = new StringBuilder();
        
        // API 결과가 있는 경우 구체적인 지시사항 제공
        if (apiCallResult.Success && apiCallResult.Data != null && apiCallResult.ApiName != "None")
        {
            if (apiCallResult.Data is SampleApiResponse sampleResponse && sampleResponse.Data != null)
            {
                // API 타입별로 구체적인 프롬프트 생성
                switch (apiCallResult.ApiName)
                {
                    case "GetWeather":
                        return BuildWeatherPrompt(originalPrompt, sampleResponse);
                    case "GetStockInfo":
                        return BuildStockPrompt(originalPrompt, sampleResponse);
                    case "GetNews":
                        return BuildNewsPrompt(originalPrompt, sampleResponse);
                }
            }
        }
        
        // 기본 프롬프트
        promptBuilder.AppendLine("사용자의 질문에 자연스럽고 친근한 한국어로 답변해주세요.");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine($"사용자 질문: {originalPrompt}");
        
        return promptBuilder.ToString();
    }

    /// <summary>
    /// 날씨 정보를 위한 구체적인 프롬프트 생성
    /// </summary>
    private string BuildWeatherPrompt(string originalPrompt, SampleApiResponse response)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response.Data));
            
            var location = data.GetProperty("location").GetString() ?? "알 수 없는 지역";
            var temperature = data.GetProperty("temperature").GetString() ?? "알 수 없음";
            var condition = data.GetProperty("condition").GetString() ?? "알 수 없음";
            var humidity = data.GetProperty("humidity").GetString() ?? "알 수 없음";
            var windSpeed = data.GetProperty("windSpeed").GetString() ?? "알 수 없음";

            return $@"사용자가 '{originalPrompt}'라고 질문했습니다.

다음 날씨 정보를 바탕으로 친근하고 자연스러운 한국어로 답변해주세요:

위치: {location}
기온: {temperature}
날씨 상태: {condition}
습도: {humidity}
바람: {windSpeed}

이 정보를 활용해서 '{location}의 현재 날씨는 {condition}이고, 기온은 {temperature}입니다' 같은 형식으로 자연스럽게 설명해주세요.";
        }
        catch
        {
            return $"사용자 질문: {originalPrompt}\n\n날씨 정보를 가져왔지만 처리 중 문제가 발생했습니다.";
        }
    }

    /// <summary>
    /// 주식 정보를 위한 구체적인 프롬프트 생성
    /// </summary>
    private string BuildStockPrompt(string originalPrompt, SampleApiResponse response)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response.Data));
            
            var symbol = data.GetProperty("symbol").GetString() ?? "알 수 없음";
            var price = data.GetProperty("price").GetString() ?? "알 수 없음";
            var change = data.GetProperty("change").GetString() ?? "알 수 없음";
            var volume = data.GetProperty("volume").GetString() ?? "알 수 없음";
            var marketCap = data.GetProperty("marketCap").GetString() ?? "알 수 없음";

            return $@"사용자가 '{originalPrompt}'라고 질문했습니다.

다음 주식 정보를 바탕으로 친근하고 자연스러운 한국어로 답변해주세요:

종목: {symbol}
현재가: {price}
변동률: {change}
거래량: {volume}
시가총액: {marketCap}

이 정보를 활용해서 '{symbol} 주식의 현재가는 {price}이고, 변동률은 {change}입니다' 같은 형식으로 자연스럽게 설명해주세요.";
        }
        catch
        {
            return $"사용자 질문: {originalPrompt}\n\n주식 정보를 가져왔지만 처리 중 문제가 발생했습니다.";
        }
    }

    /// <summary>
    /// 뉴스 정보를 위한 구체적인 프롬프트 생성
    /// </summary>
    private string BuildNewsPrompt(string originalPrompt, SampleApiResponse response)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response.Data));
            
            var topic = data.GetProperty("topic").GetString() ?? "알 수 없음";
            var headline = data.GetProperty("headline").GetString() ?? "알 수 없음";
            var summary = data.GetProperty("summary").GetString() ?? "알 수 없음";
            var publishedAt = data.GetProperty("publishedAt").GetString() ?? "알 수 없음";
            var source = data.GetProperty("source").GetString() ?? "알 수 없음";

            return $@"사용자가 '{originalPrompt}'라고 질문했습니다.

다음 뉴스 정보를 바탕으로 친근하고 자연스러운 한국어로 답변해주세요:

주제: {topic}
제목: {headline}
요약: {summary}
발행일: {publishedAt}
출처: {source}

이 정보를 활용해서 '{topic} 관련 최신 뉴스를 알려드리겠습니다. {headline}...' 같은 형식으로 자연스럽게 설명해주세요.";
        }
        catch
        {
            return $"사용자 질문: {originalPrompt}\n\n뉴스 정보를 가져왔지만 처리 중 문제가 발생했습니다.";
        }
    }

    /// <summary>
    /// Ollama API 응답 모델
    /// </summary>
    private class OllamaResponse
    {
        public string Model { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public bool Done { get; set; }
        public string? Created_at { get; set; }
        public string? Done_reason { get; set; }
        public int[]? Context { get; set; }
        public long? Total_duration { get; set; }
        public long? Load_duration { get; set; }
        public int? Prompt_eval_count { get; set; }
        public long? Prompt_eval_duration { get; set; }
        public int? Eval_count { get; set; }
        public long? Eval_duration { get; set; }
    }
}
