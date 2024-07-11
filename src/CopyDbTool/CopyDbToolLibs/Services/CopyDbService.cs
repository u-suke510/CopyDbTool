using CopyDbToolLibs.Configs;
using CopyDbToolLibs.Repositories;
using CopyDbToolLibs.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CopyDbToolLibs.Services
{
    /// <summary>
    /// DBデータコピー処理のServiceインターフェース
    /// </summary>
    public interface ICopyDbService : IServiceBase
    {
    }

    /// <summary>
    /// DBデータコピー処理のServiceクラス
    /// </summary>
    public class CopyDbService : ServiceBase, ICopyDbService
    {
        /// <summary>
        /// アプリケーション設定情報
        /// </summary>
        private CopyDbSrvConfig conf;

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
        /// <param name="options">設定情報</param>
        /// <param name="logger">ロガー</param>
        public CopyDbService(IServiceProvider provider, IOptions<CopyDbSrvConfig> options, ILogger<CopyDbService> logger) : base(provider, logger)
        {
            SrvId = "B1000";
            SrvNm = "DBデータコピー処理";

            conf = options.Value;
        }

        /// <summary>
        /// DBデータコピー処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public override bool ExecuteService()
        {
            var result = true;
            var repository = provider.GetService<ISrcRepository>();

            // DB情報の読み込み
            var srcSettings = JsonConvert.DeserializeObject<List<SrcDbSetting>>(File.ReadAllText(conf.SrcSettings));
            var dstSettings = JsonConvert.DeserializeObject<List<DstDbSetting>>(File.ReadAllText(conf.DstSettings));

            // コピー元DB情報毎に処理
            foreach (var setting in srcSettings)
            {
                // テーブルアイテムの取得
                var items = repository.GetItems(setting.TblNm, setting.Columns);
                // テーブルアイテムの登録
                RegisterDstItems(dstSettings.Find(x => x.SrcTblNm == setting.TblNm), items);
            }

            return result;
        }
    }
}
