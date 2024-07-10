namespace CopyDbToolLibs.Configs
{
    /// <summary>
    /// DBデータ出力処理の設定情報クラス
    /// </summary>
    public class ExportDataSrvConfig
    {
        /// <summary>
        /// コピー元DB情報情報ファイルパス
        /// </summary>
        public string SrcSettings { get; set; }
        /// <summary>
        /// 1ファイルの最大出力行数
        /// </summary>
        public int MaxRows { get; set; } = 1000;
        /// <summary>
        /// 出力先フォルダパス
        /// </summary>
        public string ExportFolderPath { get; set; }
    }
}
