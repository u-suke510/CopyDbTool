using CopyDbToolLibs.Repositories;
using CopyDbToolLibs.Resources;
using CopyDbToolLibs.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CopyDbToolLibs
{
    /// <summary>
    /// Serviceクラスのベースインターフェース
    /// </summary>
    public interface IServiceBase
    {
        /// <summary>
        /// メイン処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        bool Execute();

        /// <summary>
        /// 各種処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        bool ExecuteService();

        /// <summary>
        /// 登録先DBへアイテムを登録します。
        /// </summary>
        /// <param name="setting">登録先DB情報</param>
        /// <param name="items">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        bool RegisterDstItems(DstDbSetting setting, List<Dictionary<string, object>> items);
    }

    /// <summary>
    /// Serviceクラスのベースクラス
    /// </summary>
    public abstract class ServiceBase : IServiceBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        protected ILogger logger;
        /// <summary>
        /// サービスプロバイダー
        /// </summary>
        protected IServiceProvider provider;

        /// <summary>
        /// 処理開始日時
        /// </summary>
        protected DateTime ExecDtm
        {
            get;
            private set;
        }
        /// <summary>
        /// 処理ID
        /// </summary>
        protected abstract string SrvId
        {
            get;
        }
        /// <summary>
        /// 処理名
        /// </summary>
        protected abstract string SrvNm
        {
            get;
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="provider">サービスプロバイダー</param>
        /// <param name="logger">ロガー</param>
        public ServiceBase(IServiceProvider provider, ILogger logger)
        {
            this.logger = logger;
            this.provider = provider;
        }

        /// <summary>
        /// メイン処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public bool Execute()
        {
            try
            {
                ExecDtm = DateTime.Now;

                // 開始ログ
                logger.LogInformation(MsgResource.MsgExecStartLog, SrvId, SrvNm);

                // 各種処理の実施
                var result = ExecuteService();

                // 終了ログ
                endLog(result);
                return result;
            }
            catch (Exception ex)
            {
                // 終了ログ
                endLog(false, ex);
                return false;
            }

            /// <summary>
            /// 終了ログの出力処理を実施します。
            /// </summary>
            /// <param name="result">処理結果</param>
            /// <param name="ex">例外</param>
            void endLog(bool result, Exception ex = null)
            {
                // 終了ログの出力
                if (result)
                {
                    logger.LogInformation(MsgResource.MsgExecSuccessLog, SrvId, SrvNm);
                }
                else
                {
                    logger.LogError(MsgResource.MsgExecFailedLog, SrvId, SrvNm, ex == null ? string.Empty : ex);
                }
            }
        }

        /// <summary>
        /// 各種処理を実装します。
        /// </summary>
        /// <returns>処理結果</returns>
        public abstract bool ExecuteService();

        /// <summary>
        /// 登録先DBへアイテムを登録します。
        /// </summary>
        /// <param name="setting">登録先DB情報</param>
        /// <param name="items">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        public bool RegisterDstItems(DstDbSetting setting, List<Dictionary<string, object>> items)
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
