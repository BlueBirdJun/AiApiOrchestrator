using AiApiOrchestrator.Application.Interfaces;
using AiApiOrchestrator.Domain.Dtos;

namespace AiApiOrchestrator.Application.Services;

/// <summary>
/// API 오케스트레이터 구현
/// 프롬프트를 분석하여 적절한 Sample API를 판단하고 호출합니다.
/// </summary>
public class ApiOrchestrator : IApiOrchestrator
{
    private readonly ISampleApiClient _sampleApiClient;
    private readonly Dictionary<string, Func<string, Task<SampleApiResponse>>> _keywordMapping;

    /// <summary>
    /// ApiOrchestrator 생성자
    /// </summary>
    /// <param name="sampleApiClient">Sample API 클라이언트</param>
    public ApiOrchestrator(ISampleApiClient sampleApiClient)
    {
        _sampleApiClient = sampleApiClient;

        // 키워드 매핑 Dictionary 생성
        _keywordMapping = new Dictionary<string, Func<string, Task<SampleApiResponse>>>(StringComparer.OrdinalIgnoreCase)
        {
            { "날씨", param => _sampleApiClient.GetWeatherAsync(param) },
            { "weather", param => _sampleApiClient.GetWeatherAsync(param) },
            { "주식", param => _sampleApiClient.GetStockInfoAsync(param) },
            { "stock", param => _sampleApiClient.GetStockInfoAsync(param) },
            { "뉴스", param => _sampleApiClient.GetNewsAsync(param) },
            { "news", param => _sampleApiClient.GetNewsAsync(param) }
        };
    }

    /// <summary>
    /// 프롬프트를 분석하여 적절한 Sample API를 판단하고 호출합니다.
    /// </summary>
    /// <param name="prompt">분석할 사용자 프롬프트</param>
    /// <returns>API 호출 결과를 포함하는 ApiCallResult</returns>
    public async Task<ApiCallResult> DetermineAndCallApiAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return new ApiCallResult
            {
                ApiName = "None",
                Data = null,
                Success = false
            };
        }

        // 프롬프트를 소문자로 변환하여 키워드 매칭
        var lowerPrompt = prompt.ToLower();

        // 키워드 매칭 및 API 호출
        foreach (var keyword in _keywordMapping.Keys)
        {
            if (lowerPrompt.Contains(keyword.ToLower()))
            {
                try
                {
                    // 기본 파라미터 추출 (간단한 구현)
                    var parameter = ExtractParameter(prompt, keyword);
                    var response = await _keywordMapping[keyword](parameter);

                    return new ApiCallResult
                    {
                        ApiName = GetApiName(keyword),
                        Data = response,
                        Success = true
                    };
                }
                catch (Exception)
                {
                    return new ApiCallResult
                    {
                        ApiName = GetApiName(keyword),
                        Data = null,
                        Success = false
                    };
                }
            }
        }

        // 매칭되는 키워드가 없을 경우 기본 응답 반환
        return new ApiCallResult
        {
            ApiName = "None",
            Data = new SampleApiResponse
            {
                Message = "No matching API found for the given prompt",
                Data = null
            },
            Success = true
        };
    }

    /// <summary>
    /// 키워드로부터 API 이름을 반환합니다.
    /// </summary>
    /// <param name="keyword">키워드</param>
    /// <returns>API 이름</returns>
    private string GetApiName(string keyword)
    {
        return keyword.ToLower() switch
        {
            "날씨" or "weather" => "GetWeather",
            "주식" or "stock" => "GetStockInfo",
            "뉴스" or "news" => "GetNews",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// 프롬프트에서 파라미터를 추출합니다.
    /// </summary>
    /// <param name="prompt">프롬프트</param>
    /// <param name="keyword">키워드</param>
    /// <returns>추출된 파라미터</returns>
    private string ExtractParameter(string prompt, string keyword)
    {
        var lowerPrompt = prompt.ToLower();
        
        // 키워드별 파라미터 추출 로직
        return keyword.ToLower() switch
        {
            "날씨" or "weather" => ExtractLocation(prompt),
            "주식" or "stock" => ExtractStockSymbol(prompt),
            "뉴스" or "news" => ExtractNewsTopic(prompt),
            _ => "default"
        };
    }

    /// <summary>
    /// 프롬프트에서 위치 정보를 추출합니다.
    /// </summary>
    private string ExtractLocation(string prompt)
    {
        var lowerPrompt = prompt.ToLower();
        
        // 일반적인 도시명 패턴 매칭
        var cities = new[] { "서울", "seoul", "부산", "busan", "대구", "daegu", "인천", "incheon", 
                           "광주", "gwangju", "대전", "daejeon", "울산", "ulsan", "제주", "jeju",
                           "tokyo", "도쿄", "osaka", "오사카", "new york", "뉴욕", "london", "런던",
                           "paris", "파리", "beijing", "베이징", "shanghai", "상하이" };
        
        foreach (var city in cities)
        {
            if (lowerPrompt.Contains(city))
            {
                return city;
            }
        }
        
        return "Seoul"; // 기본값
    }

    /// <summary>
    /// 프롬프트에서 주식 심볼을 추출합니다.
    /// </summary>
    private string ExtractStockSymbol(string prompt)
    {
        var lowerPrompt = prompt.ToLower();
        
        // 일반적인 주식 심볼이나 회사명 패턴 매칭
        var stocks = new Dictionary<string, string>
        {
            { "애플", "AAPL" }, { "apple", "AAPL" }, { "aapl", "AAPL" },
            { "삼성", "005930.KS" }, { "samsung", "005930.KS" },
            { "구글", "GOOGL" }, { "google", "GOOGL" }, { "googl", "GOOGL" },
            { "마이크로소프트", "MSFT" }, { "microsoft", "MSFT" }, { "msft", "MSFT" },
            { "테슬라", "TSLA" }, { "tesla", "TSLA" }, { "tsla", "TSLA" }
        };
        
        foreach (var stock in stocks)
        {
            if (lowerPrompt.Contains(stock.Key))
            {
                return stock.Value;
            }
        }
        
        return "AAPL"; // 기본값
    }

    /// <summary>
    /// 프롬프트에서 뉴스 주제를 추출합니다.
    /// </summary>
    private string ExtractNewsTopic(string prompt)
    {
        var lowerPrompt = prompt.ToLower();
        
        // 뉴스 주제 패턴 매칭
        var topics = new[] { "기술", "technology", "tech", "경제", "economy", "정치", "politics",
                           "스포츠", "sports", "연예", "entertainment", "과학", "science",
                           "건강", "health", "환경", "environment" };
        
        foreach (var topic in topics)
        {
            if (lowerPrompt.Contains(topic))
            {
                return topic;
            }
        }
        
        return "Technology"; // 기본값
    }
}
