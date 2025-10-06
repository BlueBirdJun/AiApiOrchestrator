namespace AiApiOrchestrator.Domain.Dtos;

/// <summary>
/// Sample API 호출 결과
/// </summary>
public class ApiCallResult
{
    /// <summary>
    /// API 이름
    /// </summary>
    public string ApiName { get; set; } = string.Empty;

    /// <summary>
    /// API 응답 데이터
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// 성공 여부
    /// </summary>
    public bool Success { get; set; }
}
