namespace AiApiOrchestrator.Domain.Dtos;

/// <summary>
/// Sample API의 표준 응답 형식
/// </summary>
public class SampleApiResponse
{
    /// <summary>
    /// 응답 메시지
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 응답 데이터
    /// </summary>
    public object? Data { get; set; }
}
