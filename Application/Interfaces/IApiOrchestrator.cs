using AiApiOrchestrator.Domain.Dtos;

namespace AiApiOrchestrator.Application.Interfaces;

/// <summary>
/// API 오케스트레이터 인터페이스
/// 프롬프트를 분석하여 적절한 Sample API를 판단하고 호출합니다.
/// </summary>
public interface IApiOrchestrator
{
    /// <summary>
    /// 프롬프트를 분석하여 적절한 Sample API를 판단하고 호출합니다.
    /// </summary>
    /// <param name="prompt">분석할 사용자 프롬프트</param>
    /// <returns>API 호출 결과를 포함하는 ApiCallResult</returns>
    Task<ApiCallResult> DetermineAndCallApiAsync(string prompt);
}
