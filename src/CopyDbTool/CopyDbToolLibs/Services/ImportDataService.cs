using CopyDbToolLibs.Configs;
using CopyDbToolLibs.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CopyDbToolLibs.Services
{
    /// <summary>
    /// DBデータ取込処理のServiceインターフェース
    /// </summary>
    public interface IImportDataService : IServiceBase
    {
        /// <summary>
        /// 取込データファイルをDBに取り込みます。
        /// </summary>
        /// <param name="setting">登録先DB情報</param>
        /// <returns>処理結果</returns>
        bool ImportData(DstDbSetting setting);
    }

    /// <summary>
    /// DBデータ取込処理のServiceクラス
    /// </summary>
    public class ImportDataService : ServiceBase, IImportDataService
    {
        /// <summary>
        /// アプリケーション設定情報
        /// </summary>
        private ImportDataSrvConfig conf;

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
        public ImportDataService(IServiceProvider provider, IOptions<ImportDataSrvConfig> options, ILogger<ImportDataService> logger) : base(provider, logger)
        {
            SrvId = "B1200";
            SrvNm = "DBデータ取込";

            conf = options.Value;
        }

        /// <summary>
        /// DBデータ取込処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public override bool ExecuteService()
        {
            var result = true;

            // DB情報の読み込み
            var dstSettings = JsonConvert.DeserializeObject<List<DstDbSetting>>(File.ReadAllText(conf.DstSettings));

            // コピー先DB情報毎に処理
            foreach (var setting in dstSettings)
            {
                if (!ImportData(setting))
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 取込データファイルをDBに取り込みます。
        /// </summary>
        /// <param name="setting">登録先DB情報</param>
        /// <returns>処理結果</returns>
        public bool ImportData(DstDbSetting setting)
        {
            var result = true;

            // 取込データファイルの取得
            var files = Directory.GetFiles(conf.DataFolderPath, $"{setting.SrcTblNm}*.json");

            // ファイル毎にDB登録
            foreach (var file in files)
            {
                // 取込データを取得
                var json = File.ReadAllText(file);
                var items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

                // 登録処理
                if (!RegisterDstItems(setting, items))
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
