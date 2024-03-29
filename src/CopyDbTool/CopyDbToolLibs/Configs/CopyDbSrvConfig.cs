namespace CopyDbToolLibs.Configs
{
    /// <summary>
    /// DBデータコピー処理の設定情報クラス
    /// </summary>
    public class CopyDbSrvConfig
    {
        /// <summary>
        /// コピー元DB情報情報ファイルパス
        /// </summary>
        public string SrcSettings { get; set; }

        /// <summary>
        /// コピー先DB情報情報ファイルパス
        /// </summary>
        public string DstSettings { get; set; }
    }
}
