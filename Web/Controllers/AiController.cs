using AiApiOrchestrator.Application.Interfaces;
using AiApiOrchestrator.Domain.Dtos;
using AiApiOrchestrator.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AiApiOrchestrator.Web.Controllers;

/// <summary>
/// AI 요청을 처리하는 메인 컨트롤러
/// 사용자의 프롬프트를 받아 선택된 AI 모드에 따라 적절한 서비스를 호출합니다.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IOllamaAiService _ollamaAiService;
    private readonly IAgentAiService _agentAiService;
    private readonly ILogger<AiController> _logger;

    /// <summary>
    /// AiController 생성자
    /// </summary>
    /// <param name="ollamaAiService">Ollama AI 서비스</param>
    /// <param name="agentAiService">Agent AI 서비스</param>
    /// <param name="logger">로거</param>
    public AiController(
        IOllamaAiService ollamaAiService,
        IAgentAiService agentAiService,
        ILogger<AiController> logger)
    {
        _ollamaAiService = ollamaAiService;
        _agentAiService = agentAiService;
        _logger = logger;
    }

    /// <summary>
    /// AI 요청을 처리하는 엔드포인트
    /// </summary>
    /// <param name="request">AI 요청 (프롬프트 및 모드)</param>
    /// <returns>AI 처리 결과</returns>
    [HttpPost("process")]
    public async Task<ActionResult<AiResponse>> Process([FromBody] AiRequest request)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8]; // 짧은 요청 ID 생성
        
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["Mode"] = request.Mode.ToString(),
            ["PromptLength"] = request.Prompt?.Length ?? 0
        }))
        {
            _logger.LogInformation("AI 요청 처리 시작 - RequestId: {RequestId}, Mode: {Mode}, Prompt: {Prompt}", 
                requestId, request.Mode, request.Prompt);

            try
            {
                // 요청 유효성 검사
                if (string.IsNullOrWhiteSpace(request.Prompt))
                {
                    _logger.LogWarning("빈 프롬프트 요청 - RequestId: {RequestId}", requestId);
                    return BadRequest(new AiResponse
                    {
                        Success = false,
                        ErrorMessage = "프롬프트가 비어있습니다."
                    });
                }

                // Mode에 따라 적절한 AI 서비스 선택
                IAiService selectedService = request.Mode switch
                {
                    AiMode.Ollama => _ollamaAiService,
                    AiMode.Agent => _agentAiService,
                    _ => throw new ArgumentException($"지원하지 않는 AI 모드입니다: {request.Mode}")
                };

                _logger.LogInformation("AI 서비스 선택 완료 - RequestId: {RequestId}, Service: {ServiceType}", 
                    requestId, selectedService.GetType().Name);

                // 처리 시작 시간 기록
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // 선택된 서비스의 ProcessPromptAsync 호출
                var response = await selectedService.ProcessPromptAsync(request.Prompt);

                stopwatch.Stop();

                _logger.LogInformation("AI 요청 처리 완료 - RequestId: {RequestId}, Success: {Success}, ApiCalled: {ApiCalled}, Duration: {Duration}ms", 
                    requestId, response.Success, response.ApiCalled, stopwatch.ElapsedMilliseconds);

                // 응답 결과에 따른 추가 로깅
                if (response.Success)
                {
                    _logger.LogDebug("AI 응답 상세 - RequestId: {RequestId}, ResultLength: {ResultLength}", 
                        requestId, response.Result?.Length ?? 0);
                }
                else
                {
                    _logger.LogWarning("AI 처리 실패 - RequestId: {RequestId}, Error: {ErrorMessage}", 
                        requestId, response.ErrorMessage);
                }

                return Ok(response);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "잘못된 요청 파라미터 - RequestId: {RequestId}", requestId);
                return BadRequest(new AiResponse
                {
                    Success = false,
                    ErrorMessage = argEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI 요청 처리 중 예상치 못한 오류 발생 - RequestId: {RequestId}", requestId);

                return StatusCode(500, new AiResponse
                {
                    Success = false,
                    ErrorMessage = $"서버 내부 오류가 발생했습니다: {ex.Message}"
                });
            }
        }
    }
}
