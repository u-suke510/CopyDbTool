using Newtonsoft.Json;

namespace CopyDbToolLibs.Settings
{
    /// <summary>
    /// コピー元DB情報クラス
    /// </summary>
    public class SrcDbSetting
    {
        /// <summary>
        /// テーブル名
        /// </summary>
        [JsonProperty("table")]
        public string TblNm { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        [JsonProperty("columns")]
        public List<string> Columns { get; set; }
    }
}
