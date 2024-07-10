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
    /// DBデータ出力処理のServiceインターフェース
    /// </summary>
    public interface IExportDataService : IServiceBase
    {
        /// <summary>
        /// テーブルアイテムをJSONファイルに出力します。
        /// </summary>
        /// <param name="tblNm">テーブル名</param>
        /// <param name="items">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        bool ExportData(string tblNm, List<Dictionary<string, object>> items);
    }

    /// <summary>
    /// DBデータ出力処理のServiceクラス
    /// </summary>
    public class ExportDataService : ServiceBase, IExportDataService
    {
        /// <summary>
        /// アプリケーション設定情報
        /// </summary>
        private ExportDataSrvConfig conf;

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
        public ExportDataService(IServiceProvider provider, IOptions<ExportDataSrvConfig> options, ILogger<ExportDataService> logger) : base(provider, logger)
        {
            SrvId = "B1100";
            SrvNm = "DBデータ出力";

            conf = options.Value;
        }

        /// <summary>
        /// DBデータ出力処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public override bool ExecuteService()
        {
            var result = true;
            var repository = provider.GetService<ISrcRepository>();

            // DB情報の読み込み
            var srcSettings = JsonConvert.DeserializeObject<List<SrcDbSetting>>(File.ReadAllText(conf.SrcSettings));

            // コピー元DB情報毎に処理
            foreach (var setting in srcSettings)
            {
                // テーブルアイテムの取得
                var items = repository.GetItems(setting.TblNm, setting.Columns);
                // テーブルアイテムの出力
                ExportData(setting.TblNm, items);
            }

            return result;
        }

        /// <summary>
        /// テーブルアイテムをJSONファイルに出力します。
        /// </summary>
        /// <param name="tblNm">テーブル名</param>
        /// <param name="items">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        public bool ExportData(string tblNm, List<Dictionary<string, object>> items)
        {
            var result = true;

            // エクスポートデータの抽出
            var data = items.Take(conf.MaxRows);
            var rows = data.Count();

            // 全てのアイテムをエクスポート
            var index = 1;
            while (rows < items.Count)
            {
                // JSONファイルの出力
                export($"{tblNm}_{index++}", data);

                // 次のエクスポートデータの抽出
                data = items.Skip(rows).Take(conf.MaxRows);
                rows += data.Count();
            }

            // JSONファイルの出力
            export($"{tblNm}_{index++}", data);

            return result;

            /// <summary>
            /// テーブルアイテムをJSONファイルに出力します。
            /// </summary>
            /// <param name="fileNm">出力ファイル名</param>
            /// <param name="data">エクスポートデータ</param>
            void export(string fileNm, IEnumerable<Dictionary<string, object>> data)
            {
                // JSON形式で出力
                var json = JsonConvert.SerializeObject(data);
                logger.LogDebug($"[export data]:{json}");

                var path = $"{conf.ExportFolderPath}\\{fileNm}.json";
                File.WriteAllText(path, json);
            }
        }
    }
}
