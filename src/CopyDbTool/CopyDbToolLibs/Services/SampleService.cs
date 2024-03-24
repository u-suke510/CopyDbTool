using Microsoft.Extensions.Logging;

namespace CopyDbToolLibs.Services
{
    /// <summary>
    /// バッチ処理サンプルのServiceインターフェース
    /// </summary>
    public interface ISampleService : IServiceBase
    {
    }

    /// <summary>
    /// バッチ処理サンプルのServiceクラス
    /// </summary>
    public class SampleService : ServiceBase, ISampleService
    {
        /// <summary>
        /// 処理ID
        /// </summary>
        protected override string SrvId
        {
            get;
        }
        /// <summary>
        /// 処理名
        /// </summary>
        protected override string SrvNm
        {
            get;
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="provider">サービスプロバイダー</param>
        /// <param name="logger">ロガー</param>
        public SampleService(IServiceProvider provider, ILogger<SampleService> logger) : base(provider, logger)
        {
            SrvId = "B0000";
            SrvNm = "バッチ処理サンプル";
        }

        /// <summary>
        /// バッチ処理サンプルを実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public override bool ExecuteService()
        {
            return true;
        }
    }
}
