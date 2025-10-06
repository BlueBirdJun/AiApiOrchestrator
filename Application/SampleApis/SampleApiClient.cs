using AiApiOrchestrator.Application.Interfaces;
using AiApiOrchestrator.Domain.Dtos;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AiApiOrchestrator.Application.SampleApis;

/// <summary>
/// Sample API 클라이언트 구현체
/// 실제 Sample API Controller를 HTTP 호출합니다.
/// </summary>
public class SampleApiClient : ISampleApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SampleApiClient> _logger;
    private readonly string _baseUrl;

    public SampleApiClient(IHttpClientFactory httpClientFactory, ILogger<SampleApiClient> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _baseUrl = "http://localhost:5152"; // 현재 애플리케이션의 기본 URL
    }
    /// <summary>
    /// 날씨 정보를 조회합니다.
    /// </summary>
    /// <param name="location">조회할 위치</param>
    /// <returns>날씨 정보를 포함하는 SampleApiResponse</returns>
    public async Task<SampleApiResponse> GetWeatherAsync(string location)
    {
        try
        {
            _logger.LogInformation("SampleApiClient: 날씨 API 호출 시작 - 위치: {Location}", location);
            
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_baseUrl}/api/sample/weather?location={Uri.EscapeDataString(location ?? "Seoul")}";
            
            _logger.LogInformation("SampleApiClient: HTTP 요청 URL: {Url}", url);
            
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<SampleApiResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            _logger.LogInformation("SampleApiClient: 날씨 API 호출 성공 - 응답: {Response}", jsonContent);
            
            return apiResponse ?? new SampleApiResponse { Message = "No data", Data = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SampleApiClient: 날씨 API 호출 실패 - 위치: {Location}", location);
            
            // 실패 시 기본 더미 데이터 반환
            return new SampleApiResponse
            {
                Message = "Weather information retrieved (fallback)",
                Data = new
                {
                    Location = location ?? "Seoul",
                    Temperature = "15°C",
                    Condition = "Sunny",
                    Humidity = "60%",
                    WindSpeed = "10 km/h"
                }
            };
        }
    }

    /// <summary>
    /// 주식 정보를 조회합니다.
    /// </summary>
    /// <param name="symbol">조회할 주식 심볼</param>
    /// <returns>주식 정보를 포함하는 SampleApiResponse</returns>
    public async Task<SampleApiResponse> GetStockInfoAsync(string symbol)
    {
        try
        {
            _logger.LogInformation("SampleApiClient: 주식 API 호출 시작 - 심볼: {Symbol}", symbol);
            
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_baseUrl}/api/sample/stock?symbol={Uri.EscapeDataString(symbol ?? "AAPL")}";
            
            _logger.LogInformation("SampleApiClient: HTTP 요청 URL: {Url}", url);
            
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<SampleApiResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            _logger.LogInformation("SampleApiClient: 주식 API 호출 성공 - 응답: {Response}", jsonContent);
            
            return apiResponse ?? new SampleApiResponse { Message = "No data", Data = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SampleApiClient: 주식 API 호출 실패 - 심볼: {Symbol}", symbol);
            
            // 실패 시 기본 더미 데이터 반환
            return new SampleApiResponse
            {
                Message = "Stock information retrieved (fallback)",
                Data = new
                {
                    Symbol = symbol ?? "AAPL",
                    Price = "$150.25",
                    Change = "+2.5%",
                    Volume = "1,234,567",
                    MarketCap = "$2.5T"
                }
            };
        }
    }

    /// <summary>
    /// 뉴스 정보를 조회합니다.
    /// </summary>
    /// <param name="topic">조회할 뉴스 주제</param>
    /// <returns>뉴스 정보를 포함하는 SampleApiResponse</returns>
    public async Task<SampleApiResponse> GetNewsAsync(string topic)
    {
        try
        {
            _logger.LogInformation("SampleApiClient: 뉴스 API 호출 시작 - 주제: {Topic}", topic);
            
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"{_baseUrl}/api/sample/news?topic={Uri.EscapeDataString(topic ?? "Technology")}";
            
            _logger.LogInformation("SampleApiClient: HTTP 요청 URL: {Url}", url);
            
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<SampleApiResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            _logger.LogInformation("SampleApiClient: 뉴스 API 호출 성공 - 응답: {Response}", jsonContent);
            
            return apiResponse ?? new SampleApiResponse { Message = "No data", Data = null };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SampleApiClient: 뉴스 API 호출 실패 - 주제: {Topic}", topic);
            
            // 실패 시 기본 더미 데이터 반환
            return new SampleApiResponse
            {
                Message = "News retrieved (fallback)",
                Data = new
                {
                    Topic = topic ?? "Technology",
                    Headline = "Latest tech news headline",
                    Summary = "This is a summary of the latest technology news article.",
                    PublishedDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Source = "Tech News Daily"
                }
            };
        }
    }
}
