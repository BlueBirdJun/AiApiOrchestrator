using AiApiOrchestrator.Application.Interfaces;
using AiApiOrchestrator.Application.Services;
using AiApiOrchestrator.Application.SampleApis;
using Serilog;
using Serilog.Events;

// Serilog 구성
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("애플리케이션 시작");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog을 기본 로거로 설정
    builder.Host.UseSerilog();

    // Add services to the container.

    // HttpClient 등록
    builder.Services.AddHttpClient();

    // AI 서비스 등록
    builder.Services.AddScoped<IOllamaAiService, OllamaAiService>();
    builder.Services.AddScoped<IAgentAiService, AgentAiService>();

    // Orchestrator 등록
    builder.Services.AddScoped<IApiOrchestrator, ApiOrchestrator>();

    // Sample API Client 등록
    builder.Services.AddScoped<ISampleApiClient, SampleApiClient>();

    // OllamaSettings 구성
    builder.Services.Configure<OllamaSettings>(
        builder.Configuration.GetSection("OllamaSettings"));

    // Controllers 등록
    builder.Services.AddControllers();

    // Razor Pages 등록
    builder.Services.AddRazorPages();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // Serilog 요청 로깅 미들웨어 추가
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} 응답됨 {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => ex != null
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode > 499
                ? LogEventLevel.Error
                : LogEventLevel.Information;
    });

    app.UseHttpsRedirection();

    // 정적 파일 제공
    app.UseStaticFiles();

    // 라우팅 활성화
    app.UseRouting();

    app.UseAuthorization();

    // Controllers 매핑
    app.MapControllers();

    // Razor Pages 매핑
    app.MapRazorPages();

    Log.Information("애플리케이션 구성 완료");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "애플리케이션 시작 실패");
}
finally
{
    Log.CloseAndFlush();
}
