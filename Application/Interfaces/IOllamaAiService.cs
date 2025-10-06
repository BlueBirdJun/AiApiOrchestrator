namespace AiApiOrchestrator.Application.Interfaces;

/// <summary>
/// Ollama AI 서비스 인터페이스
/// Ollama 로컬 서버와 통신하여 AI 기능을 제공합니다.
/// </summary>
public interface IOllamaAiService : IAiService
{
    // IAiService의 ProcessPromptAsync 메서드를 상속받아 사용
}
