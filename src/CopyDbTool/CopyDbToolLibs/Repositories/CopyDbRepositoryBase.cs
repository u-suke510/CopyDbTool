using CopyDbToolLibs.Resources;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CopyDbToolLibs.Repositories
{
    /// <summary>
    /// DBデータコピーのRepositoryインターフェース
    /// </summary>
    public interface ICopyDbRepositoryBase
    {
        /// <summary>
        /// テーブルアイテムを取得します。
        /// </summary>
        /// <param name="tblNm">テーブル名</param>
        /// <param name="columns">列名</param>
        /// <returns>処理結果</returns>
        List<Dictionary<string, object>> GetItems(string tblNm, List<string> columns);

        /// <summary>
        /// テーブルアイテムを登録します。
        /// </summary>
        /// <param name="tblNm">テーブル名</param>
        /// <param name="item">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        bool InsTblItem(string tblNm, Dictionary<string, object> item);
    }

    /// <summary>
    /// DBデータコピーのRepositoryクラス
    /// </summary>
    public abstract class CopyDbRepositoryBase : ICopyDbRepositoryBase
    {
        /// <summary>
        /// 接続文字列
        /// </summary>
        private readonly string connStr;
        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="conf">設定情報</param>
        /// <param name="connStrNm">接続文字列名</param>
        /// <param name="logger">ロガー</param>
        public CopyDbRepositoryBase(IConfiguration conf, string connStrNm, ILogger logger)
        {
            connStr = conf.GetConnectionString(connStrNm);
            this.logger = logger;
        }

        /// <summary>
        /// テーブルアイテムを取得します。
        /// </summary>
        /// <param name="tblNm">テーブル名</param>
        /// <param name="columns">列名</param>
        /// <returns>処理結果</returns>
        public List<Dictionary<string, object>> GetItems(string tblNm, List<string> columns)
        {
            // クエリの生成
            var query = createQuery();

            var result = new List<Dictionary<string, object>>();
            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(createQuery(), conn))
            {
                ExecSqlLog(cmd);
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = new Dictionary<string, object>();
                    // 取得列名でアイテムを生成
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        item.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    result.Add(item);
                }
                conn.Close();
            }

            return result;

            /// <summary>
            /// SELECTのクエリ文を生成します。
            /// </summary>
            /// <returns>SELECT文</returns>
            string createQuery()
            {
                // 列名の指定が無い場合、全列を取得
                if (columns == null || !columns.Any())
                {
                    return $"SELECT * FROM {tblNm};";
                }

                return $"SELECT {string.Join(",", columns)} FROM {tblNm};";
            }
        }

        /// <summary>
        /// テーブルアイテムを登録します。
        /// </summary>
        /// <param name="tblNm">テーブル名</param>
        /// <param name="item">テーブルアイテム</param>
        /// <returns>処理結果</returns>
        public bool InsTblItem(string tblNm, Dictionary<string, object> item)
        {
            // クエリの生成
            var query = createQuery();

            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(query.str, conn))
            {
                // パラメータの設定
                foreach (var param in query.pVals)
                {
                    cmd.Parameters.Add(param);
                }

                // データの登録
                ExecSqlLog(cmd);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            return true;

            /// <summary>
            /// INSERTのクエリ文を生成します。
            /// </summary>
            /// <returns>INSERT文情報</returns>
            (string str, List<SqlParameter> pVals) createQuery()
            {
                var cols = new List<string>();
                var values = new List<SqlParameter>();
                foreach (var col in item.Select(x => x.Key))
                {
                    // テーブルアイテムから列名,登録値を取得
                    cols.Add(col);
                    values.Add(new SqlParameter($"@p{values.Count}", item[col]));
                }

                // INSERTクエリの生成
                var query = new StringBuilder();
                query.Append($"INSERT INTO {tblNm}({string.Join(",", cols)}) ");
                query.Append($"VALUES({string.Join(",", cols.Select((value, index) => $"@p{index}"))}); ");

                return (query.ToString(), values);
            }
        }

        /// <summary>
        /// 実行SQL情報をログ出力します。
        /// </summary>
        /// <param name="cmd">SQLコマンド</param>
        protected void ExecSqlLog(SqlCommand cmd)
        {
            try
            {
                var query = cmd.CommandText;
                var queryParams = new List<string>();
                foreach (var item in cmd.Parameters)
                {
                    var param = item as SqlParameter;
                    queryParams.Add($"{param.ParameterName}:{param.Value}");
                }
                logger.LogInformation(MsgResource.FmtMsgInfoExecSqlLog, string.Join(",", queryParams), query);
            }
            catch (Exception ex)
            {
                // 出力失敗時はWarningログとして出力
                logger.LogWarning(MsgResource.MsgErrExecSqlLog, ex);
            }
        }
    }
}
