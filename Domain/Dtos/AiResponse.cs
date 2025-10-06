namespace AiApiOrchestrator.Domain.Dtos;

/// <summary>
/// AI 서비스에서 클라이언트로 반환되는 응답
/// </summary>
public class AiResponse
{
    /// <summary>
    /// AI 처리 결과
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// 호출된 Sample API 이름
    /// </summary>
    public string ApiCalled { get; set; } = string.Empty;

    /// <summary>
    /// 성공 여부
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 오류 메시지 (실패 시)
    /// </summary>
    public string? ErrorMessage { get; set; }
}
