using Microsoft.AspNetCore.Mvc;
using AiApiOrchestrator.Domain.Dtos;
using Microsoft.Extensions.Logging;

namespace AiApiOrchestrator.Web.Controllers;

/// <summary>
/// Sample API 엔드포인트를 제공하는 컨트롤러
/// 더미 데이터를 반환하여 AI Orchestrator 테스트를 지원합니다.
/// </summary>
[ApiController]
[Route("api/sample")]
public class SampleApiController : ControllerBase
{
    private readonly ILogger<SampleApiController> _logger;

    public SampleApiController(ILogger<SampleApiController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 날씨 정보를 반환하는 엔드포인트
    /// </summary>
    /// <param name="location">위치 (예: Seoul, Tokyo)</param>
    /// <returns>날씨 정보</returns>
    [HttpGet("weather")]
    public ActionResult<SampleApiResponse> GetWeather([FromQuery] string location = "Seoul")
    {
        _logger.LogInformation("날씨 정보 조회 요청 - 위치: {Location}", location);

        try
        {
            var response = new SampleApiResponse
            {
                Message = "Weather information retrieved",
                Data = new
                {
                    location = location,
                    temperature = "65°C",
                    condition = "Sunny",
                    humidity = "60%",
                    windSpeed = "10 km/h"
                }
            };

            _logger.LogInformation("날씨 정보 조회 성공 - 위치: {Location}, 온도: {Temperature}", 
                location, "15°C");

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "날씨 정보 조회 중 오류 발생 - 위치: {Location}", location);
            return StatusCode(500, new SampleApiResponse
            {
                Message = "Error retrieving weather information",
                Data = null
            });
        }
    }

    /// <summary>
    /// 주식 정보를 반환하는 엔드포인트
    /// </summary>
    /// <param name="symbol">주식 심볼 (예: AAPL, GOOGL)</param>
    /// <returns>주식 정보</returns>
    [HttpGet("stock")]
    public ActionResult<SampleApiResponse> GetStock([FromQuery] string symbol = "AAPL")
    {
        _logger.LogInformation("주식 정보 조회 요청 - 심볼: {Symbol}", symbol);

        try
        {
            var response = new SampleApiResponse
            {
                Message = "Stock information retrieved",
                Data = new
                {
                    symbol = symbol,
                    price = "$150.25",
                    change = "+2.5%",
                    volume = "1,234,567",
                    marketCap = "$2.5T"
                }
            };

            _logger.LogInformation("주식 정보 조회 성공 - 심볼: {Symbol}, 가격: {Price}, 변동률: {Change}", 
                symbol, "$150.25", "+2.5%");

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "주식 정보 조회 중 오류 발생 - 심볼: {Symbol}", symbol);
            return StatusCode(500, new SampleApiResponse
            {
                Message = "Error retrieving stock information",
                Data = null
            });
        }
    }

    /// <summary>
    /// 뉴스 정보를 반환하는 엔드포인트
    /// </summary>
    /// <param name="topic">뉴스 주제 (예: Technology, Sports)</param>
    /// <returns>뉴스 정보</returns>
    [HttpGet("news")]
    public ActionResult<SampleApiResponse> GetNews([FromQuery] string topic = "Technology")
    {
        _logger.LogInformation("뉴스 정보 조회 요청 - 주제: {Topic}", topic);

        try
        {
            var publishedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            
            var response = new SampleApiResponse
            {
                Message = "News retrieved",
                Data = new
                {
                    topic = topic,
                    headline = $"Latest {topic} news",
                    summary = $"This is a summary of the latest news about {topic}.",
                    publishedAt = publishedAt,
                    source = "Sample News API"
                }
            };

            _logger.LogInformation("뉴스 정보 조회 성공 - 주제: {Topic}, 발행시간: {PublishedAt}", 
                topic, publishedAt);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "뉴스 정보 조회 중 오류 발생 - 주제: {Topic}", topic);
            return StatusCode(500, new SampleApiResponse
            {
                Message = "Error retrieving news information",
                Data = null
            });
        }
    }
}
