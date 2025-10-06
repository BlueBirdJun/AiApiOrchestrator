namespace AiApiOrchestrator.Application.Interfaces;

/// <summary>
/// Agent AI 서비스 인터페이스
/// 일반 Agent 방식으로 AI 기능을 제공합니다.
/// </summary>
public interface IAgentAiService : IAiService
{
    // IAiService의 ProcessPromptAsync 메서드를 상속받아 사용
}
