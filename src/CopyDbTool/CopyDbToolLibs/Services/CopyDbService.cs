using CopyDbToolLibs.Configs;
using CopyDbToolLibs.Repositories;
using CopyDbToolLibs.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyDbToolLibs.Services
{
    /// <summary>
    /// DBデータコピー処理のServiceインターフェース
    /// </summary>
    public interface ICopyDbService : IServiceBase
    {
        /// <summary>
        /// コピー先DBにテーブルアイテムをコピーします。
        /// </summary>
        /// <param name="setting">コピー先DB情報</param>
        /// <param name="items">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        bool CopyDstDb(DstDbSetting setting, List<Dictionary<string, object>> items);
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
                // DBのコピー
                CopyDstDb(dstSettings.Find(x => x.SrcTblNm == setting.TblNm), items);
            }

            return result;
        }

        /// <summary>
        /// コピー先DBにテーブルアイテムをコピーします。
        /// </summary>
        /// <param name="setting">コピー先DB情報</param>
        /// <param name="items">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        public bool CopyDstDb(DstDbSetting setting, List<Dictionary<string, object>> items)
        {
            var result = true;
            var repository = provider.GetService<IDstRepository>();

            // テーブルアイテム毎に処理
            foreach (var item in items)
            {
                repository.InsTblItem(setting.TblNm, createDstItem(item));
            }

            return result;

            /// <summary>
            /// コピー先DBに登録するテーブルアイテムを生成します。
            /// </summary>
            /// <param name="item">コピー元DBのテーブルアイテム</param>
            /// <returns>コピー先DBのテーブルアイテム</returns>
            Dictionary<string, object> createDstItem(Dictionary<string, object> item)
            {
                var result = new Dictionary<string, object>();
                foreach (var col in setting.Columns)
                {
                    // 固定値を保持しない場合、コピー元の列
                    if (string.IsNullOrEmpty(col.Value))
                    {
                        result.Add(col.Nm, item[col.SrcNm]);
                        continue;
                    }

                    result.Add(col.Nm, getFixedValue(col.Value));
                }

                return result;
            }

            /// <summary>
            /// 特殊固定値を変換します。
            /// </summary>
            /// <param name="value">固定値</param>
            /// <returns>固定値</returns>
            object getFixedValue(string value)
            {
                switch (value)
                {
                    // 実行日時の指定
                    case "$ExecDtm":
                        return ExecDtm;
                    default:
                        return value;
                }
            }
        }
    }
}
