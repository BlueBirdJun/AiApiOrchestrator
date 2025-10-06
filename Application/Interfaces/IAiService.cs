using AiApiOrchestrator.Domain.Dtos;

namespace AiApiOrchestrator.Application.Interfaces;

/// <summary>
/// 모든 AI 서비스의 기본 인터페이스
/// AI 프롬프트를 처리하고 응답을 반환하는 공통 계약을 정의합니다.
/// </summary>
public interface IAiService
{
    /// <summary>
    /// 사용자 프롬프트를 처리하고 AI 응답을 반환합니다.
    /// </summary>
    /// <param name="prompt">처리할 사용자 프롬프트</param>
    /// <returns>AI 처리 결과를 포함하는 AiResponse</returns>
    Task<AiResponse> ProcessPromptAsync(string prompt);
}
