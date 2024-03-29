using Newtonsoft.Json;

namespace CopyDbToolLibs.Settings
{
    /// <summary>
    /// コピー先DB情報クラス
    /// </summary>
    public class DstDbSetting
    {
        /// <summary>
        /// コピー元テーブル名
        /// </summary>
        [JsonProperty("src-table")]
        public string SrcTblNm { get; set; }

        /// <summary>
        /// テーブル名
        /// </summary>
        [JsonProperty("table")]
        public string TblNm { get; set; }

        /// <summary>
        /// 列情報
        /// </summary>
        [JsonProperty("columns")]
        public List<ColumnSetting> Columns { get; set; }

        /// <summary>
        /// キー項目名
        /// </summary>
        [JsonProperty("keys")]
        public List<string> Keys { get; set; }

        /// <summary>
        /// 列情報クラス
        /// </summary>
        public class ColumnSetting
        {
            /// <summary>
            /// 列名
            /// </summary>
            [JsonProperty("name")]
            public string Nm { get; set; }

            /// <summary>
            /// コピー元列名
            /// </summary>
            [JsonProperty("src")]
            public string SrcNm { get; set; }

            /// <summary>
            /// 固定値
            /// </summary>
            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
