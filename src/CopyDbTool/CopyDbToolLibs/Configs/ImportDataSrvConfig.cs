namespace CopyDbToolLibs.Configs
{
    /// <summary>
    /// DBデータ取込処理の設定情報クラス
    /// </summary>
    public class ImportDataSrvConfig
    {
        /// <summary>
        /// コピー先DB情報情報ファイルパス
        /// </summary>
        public string DstSettings { get; set; }
        /// <summary>
        /// 取込データファイルの格納フォルダパス
        /// </summary>
        public string DataFolderPath { get; set; }
    }
}
