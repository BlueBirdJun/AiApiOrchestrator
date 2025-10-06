using AiApiOrchestrator.Domain.Dtos;

namespace AiApiOrchestrator.Application.Interfaces;

/// <summary>
/// Sample API 클라이언트 인터페이스
/// 다양한 Sample API를 호출하는 메서드를 정의합니다.
/// </summary>
public interface ISampleApiClient
{
    /// <summary>
    /// 날씨 정보를 조회합니다.
    /// </summary>
    /// <param name="location">조회할 위치</param>
    /// <returns>날씨 정보를 포함하는 SampleApiResponse</returns>
    Task<SampleApiResponse> GetWeatherAsync(string location);

    /// <summary>
    /// 주식 정보를 조회합니다.
    /// </summary>
    /// <param name="symbol">조회할 주식 심볼</param>
    /// <returns>주식 정보를 포함하는 SampleApiResponse</returns>
    Task<SampleApiResponse> GetStockInfoAsync(string symbol);

    /// <summary>
    /// 뉴스 정보를 조회합니다.
    /// </summary>
    /// <param name="topic">조회할 뉴스 주제</param>
    /// <returns>뉴스 정보를 포함하는 SampleApiResponse</returns>
    Task<SampleApiResponse> GetNewsAsync(string topic);
}
