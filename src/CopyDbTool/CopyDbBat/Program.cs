using CopyDbBat.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CopyDbToolLibs;
using CopyDbToolLibs.Services;
using CopyDbToolLibs.Repositories;
using CopyDbToolLibs.Configs;

// 処理対象
List<string> targets = null;
// appsettings.jsonのファイルパス
var settingPath = string.Empty;
// ロガー
ILogger<Program> logger;
// サービスプロバイダー
IServiceProvider provider;

// 初期化
Setup();

var datetime = DateTime.Now;
logger.LogInformation(Resources.MsgBatStartLog);

// 各処理の実行
var isAbort = false;
foreach (var target in targets)
{
    // サービスクラスの取得
    var service = getService(target);
    if (service == null)
    {
        logger.LogError(Resources.MsgErrServiceNotFound, target);
        continue;
    }

    // サービスクラスの実行
    if (!service.Execute())
    {
        isAbort = true;
    }
    Thread.Sleep(1000);
}

// 全処理の終了ログ
if (isAbort)
{
    logger.LogError(Resources.MsgBatEndLog, DateTime.Now.Subtract(datetime));
}
else
{
    logger.LogInformation(Resources.MsgBatEndLog, DateTime.Now.Subtract(datetime));
}
Thread.Sleep(1000);


/// <summary>
/// バッチ処理を初期化します。
/// </summary>
void Setup()
{
    // アプリケーション設定
    if (string.IsNullOrEmpty(settingPath))
    {
        // 既定のアプリケーション設定ファイルを設定
        settingPath = "appsettings.json";
    }
    var confBuilder = new ConfigurationBuilder().AddJsonFile(settingPath, true, true);
    var configuration = confBuilder.Build();
    if (targets == null)
    {
        // 処理対象の取得
        targets = configuration.GetSection("Targets").Get<List<string>>();
    }

    // サービスの生成
    var services = new ServiceCollection();
    services.AddScoped<IConfiguration>(_ => configuration);
    services.AddLogging(builder =>
    {
        builder.AddConfiguration(configuration.GetSection("Logging"));
        builder.AddConsole();
        builder.AddLog4Net();
    });
    services.AddDbContext<AppDbContext>(option =>
    {
        option.UseSqlServer(configuration.GetConnectionString("ConnString"));
        option.EnableSensitiveDataLogging();
    });
    services.AddSingleton<ICopyDbService, CopyDbService>().AddOptions<CopyDbSrvConfig>().Configure<IConfiguration>((x, y) => {
        y.GetSection("ServiceSettings:B1000").Bind(x);
    });
    services.AddSingleton<IExportDataService, ExportDataService>().AddOptions<ExportDataSrvConfig>().Configure<IConfiguration>((x, y) => {
        y.GetSection("ServiceSettings:B1100").Bind(x);
    });
    services.AddSingleton<ISampleService, SampleService>();
    services.AddSingleton<IDstRepository, DstRepository>();
    services.AddSingleton<ISrcRepository, SrcRepository>();
    provider = services.BuildServiceProvider();

    // ロガーの生成
    var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
    logger = loggerFactory.CreateLogger<Program>();
}

/// <summary>
/// 処理名から対象のサービスクラスを取得します。
/// </summary>
/// <param name="target">処理名</param>
/// <returns>サービスクラス</returns>
IServiceBase getService(string target)
{
    switch (target)
    {
        case "B0000":
            return provider.GetService<ISampleService>();
        case "B1000":
            return provider.GetService<ICopyDbService>();
        case "B1100":
            return provider.GetService<IExportDataService>();
        default:
            return null;
    }
}
