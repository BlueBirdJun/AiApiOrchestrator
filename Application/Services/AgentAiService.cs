using System.Text;
using System.Text.Json;
using AiApiOrchestrator.Application.Interfaces;
using AiApiOrchestrator.Domain.Dtos;

namespace AiApiOrchestrator.Application.Services;

/// <summary>
/// Agent AI 서비스 구현
/// 더미 Agent 로직으로 AI 기능을 제공합니다.
/// </summary>
public class AgentAiService : IAgentAiService
{
    private readonly IApiOrchestrator _apiOrchestrator;

    public AgentAiService(IApiOrchestrator apiOrchestrator)
    {
        _apiOrchestrator = apiOrchestrator;
    }

    /// <summary>
    /// 사용자 프롬프트를 처리하고 AI 응답을 반환합니다.
    /// ApiOrchestrator로 Sample API를 호출하고, 더미 Agent 응답을 생성하여 결과를 결합합니다.
    /// </summary>
    /// <param name="prompt">처리할 사용자 프롬프트</param>
    /// <returns>AI 처리 결과를 포함하는 AiResponse</returns>
    public async Task<AiResponse> ProcessPromptAsync(string prompt)
    {
        try
        {
            // 1. ApiOrchestrator로 Sample API 호출
            var apiCallResult = await _apiOrchestrator.DetermineAndCallApiAsync(prompt);

            // 2. API 결과를 활용한 Agent 응답 생성
            var agentResponse = GenerateAgentResponse(prompt, apiCallResult);

            return new AiResponse
            {
                Result = agentResponse,
                ApiCalled = apiCallResult.ApiName,
                Success = true,
                ErrorMessage = null
            };
        }
        catch (Exception ex)
        {
            // 예외 처리
            return new AiResponse
            {
                Result = string.Empty,
                ApiCalled = string.Empty,
                Success = false,
                ErrorMessage = $"Agent AI 처리 중 오류가 발생했습니다: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// API 결과를 활용하여 Agent 응답을 생성합니다.
    /// </summary>
    /// <param name="prompt">사용자 프롬프트</param>
    /// <param name="apiCallResult">API 호출 결과</param>
    /// <returns>생성된 Agent 응답</returns>
    private string GenerateAgentResponse(string prompt, ApiCallResult apiCallResult)
    {
        Console.WriteLine($"=== Agent Response Generation ===");
        Console.WriteLine($"API Success: {apiCallResult.Success}");
        Console.WriteLine($"API Name: {apiCallResult.ApiName}");
        Console.WriteLine($"API Data Type: {apiCallResult.Data?.GetType().Name}");
        Console.WriteLine($"API Data: {JsonSerializer.Serialize(apiCallResult.Data)}");
        Console.WriteLine($"================================");

        if (!apiCallResult.Success || apiCallResult.Data == null || apiCallResult.ApiName == "None")
        {
            return $"안녕하세요! '{prompt}'에 대한 질문을 받았습니다. 하지만 관련된 특별한 데이터를 찾지 못했습니다. 일반적인 도움을 드릴 수 있습니다.";
        }

        if (apiCallResult.Data is SampleApiResponse sampleResponse && sampleResponse.Data != null)
        {
            Console.WriteLine($"SampleApiResponse detected - Message: {sampleResponse.Message}");
            Console.WriteLine($"SampleApiResponse Data: {JsonSerializer.Serialize(sampleResponse.Data)}");
            
            return apiCallResult.ApiName switch
            {
                "GetWeather" => GenerateWeatherResponse(prompt, sampleResponse),
                "GetStockInfo" => GenerateStockResponse(prompt, sampleResponse),
                "GetNews" => GenerateNewsResponse(prompt, sampleResponse),
                _ => $"'{prompt}'에 대한 정보를 찾았습니다: {JsonSerializer.Serialize(sampleResponse.Data)}"
            };
        }

        Console.WriteLine("No SampleApiResponse match - returning generic response");
        return $"'{prompt}'에 대한 정보를 처리했습니다.";
    }

    /// <summary>
    /// 날씨 정보를 활용한 응답을 생성합니다.
    /// </summary>
    private string GenerateWeatherResponse(string prompt, SampleApiResponse response)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response.Data));
            
            // 실제 JSON 속성명에 맞게 수정 (소문자 시작)
            var location = data.GetProperty("location").GetString() ?? "알 수 없는 지역";
            var temperature = data.GetProperty("temperature").GetString() ?? "알 수 없음";
            var condition = data.GetProperty("condition").GetString() ?? "알 수 없음";
            var humidity = data.GetProperty("humidity").GetString() ?? "알 수 없음";
            var windSpeed = data.GetProperty("windSpeed").GetString() ?? "알 수 없음";

            return $"{location}의 현재 날씨를 알려드리겠습니다.\n\n" +
                   $"🌡️ 기온: {temperature}\n" +
                   $"☀️ 날씨: {condition}\n" +
                   $"💧 습도: {humidity}\n" +
                   $"💨 바람: {windSpeed}\n\n" +
                   $"좋은 하루 되세요!";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Weather response generation error: {ex.Message}");
            Console.WriteLine($"Data: {JsonSerializer.Serialize(response.Data)}");
            return $"날씨 정보를 가져왔지만 데이터 처리 중 문제가 발생했습니다.";
        }
    }

    /// <summary>
    /// 주식 정보를 활용한 응답을 생성합니다.
    /// </summary>
    private string GenerateStockResponse(string prompt, SampleApiResponse response)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response.Data));
            
            // 실제 JSON 속성명에 맞게 수정 (소문자 시작)
            var symbol = data.GetProperty("symbol").GetString() ?? "알 수 없음";
            var price = data.GetProperty("price").GetString() ?? "알 수 없음";
            var change = data.GetProperty("change").GetString() ?? "알 수 없음";
            var volume = data.GetProperty("volume").GetString() ?? "알 수 없음";
            var marketCap = data.GetProperty("marketCap").GetString() ?? "알 수 없음";

            return $"{symbol} 주식 정보를 알려드리겠습니다.\n\n" +
                   $"📈 현재가: {price}\n" +
                   $"📊 변동: {change}\n" +
                   $"📦 거래량: {volume}\n" +
                   $"💰 시가총액: {marketCap}\n\n" +
                   $"투자에 참고하시기 바랍니다.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Stock response generation error: {ex.Message}");
            Console.WriteLine($"Data: {JsonSerializer.Serialize(response.Data)}");
            return $"주식 정보를 가져왔지만 데이터 처리 중 문제가 발생했습니다.";
        }
    }

    /// <summary>
    /// 뉴스 정보를 활용한 응답을 생성합니다.
    /// </summary>
    private string GenerateNewsResponse(string prompt, SampleApiResponse response)
    {
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(response.Data));
            
            // SampleApiController 기준 속성명 (소문자)
            string topic, headline, summary, publishedDate, source;
            
            try
            {
                // Controller에서 사용하는 속성명
                topic = data.GetProperty("topic").GetString() ?? "알 수 없음";
                headline = data.GetProperty("headline").GetString() ?? "알 수 없음";
                summary = data.GetProperty("summary").GetString() ?? "알 수 없음";
                publishedDate = data.GetProperty("publishedAt").GetString() ?? "알 수 없음"; // publishedAt 주의!
                source = data.GetProperty("source").GetString() ?? "알 수 없음";
            }
            catch
            {
                // Fallback에서 사용하는 속성명 (대문자)
                topic = data.GetProperty("Topic").GetString() ?? "알 수 없음";
                headline = data.GetProperty("Headline").GetString() ?? "알 수 없음";
                summary = data.GetProperty("Summary").GetString() ?? "알 수 없음";
                publishedDate = data.GetProperty("PublishedDate").GetString() ?? "알 수 없음";
                source = data.GetProperty("Source").GetString() ?? "알 수 없음";
            }

            return $"{topic} 관련 최신 뉴스를 알려드리겠습니다.\n\n" +
                   $"📰 제목: {headline}\n" +
                   $"📝 요약: {summary}\n" +
                   $"📅 발행일: {publishedDate}\n" +
                   $"📺 출처: {source}\n\n" +
                   $"더 자세한 내용은 해당 뉴스 사이트를 확인해보세요.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"News response generation error: {ex.Message}");
            Console.WriteLine($"Data: {JsonSerializer.Serialize(response.Data)}");
            return $"뉴스 정보를 가져왔지만 데이터 처리 중 문제가 발생했습니다.";
        }
    }
}
