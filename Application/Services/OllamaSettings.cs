namespace AiApiOrchestrator.Application.Services;

/// <summary>
/// Ollama API 연동을 위한 설정 클래스
/// appsettings.json의 OllamaSettings 섹션과 매핑됩니다.
/// </summary>
public class OllamaSettings
{
    /// <summary>
    /// Ollama 서버의 기본 URL
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// 사용할 Ollama 모델 이름
    /// </summary>
    public string Model { get; set; } = "phi3:mini";

    /// <summary>
    /// HTTP 요청 타임아웃 시간 (초)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
