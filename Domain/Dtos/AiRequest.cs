using AiApiOrchestrator.Domain.Enums;

namespace AiApiOrchestrator.Domain.Dtos;

/// <summary>
/// 클라이언트에서 AI 서비스로 전송되는 요청
/// </summary>
public class AiRequest
{
    /// <summary>
    /// 사용자 입력 프롬프트
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// 선택된 AI 모드
    /// </summary>
    public AiMode Mode { get; set; }
}
