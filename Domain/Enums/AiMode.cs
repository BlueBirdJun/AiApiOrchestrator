namespace AiApiOrchestrator.Domain.Enums;

/// <summary>
/// AI 연동 모드를 정의하는 열거형
/// </summary>
public enum AiMode
{
    /// <summary>
    /// Ollama 로컬 서버를 사용하는 모드
    /// </summary>
    Ollama = 0,

    /// <summary>
    /// 일반 Agent 방식을 사용하는 모드
    /// </summary>
    Agent = 1
}
