using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CopyDbToolLibs.Repositories
{
    /// <summary>
    /// コピー元DBのRepositoryインターフェース
    /// </summary>
    public interface ISrcRepository : ICopyDbRepositoryBase
    {
    }

    /// <summary>
    /// コピー元DBのRepositoryクラス
    /// </summary>
    public class SrcRepository : CopyDbRepositoryBase, ISrcRepository
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="conf">設定情報</param>
        /// <param name="logger">ロガー</param>
        public SrcRepository(IConfiguration conf, ILogger<SrcRepository> logger) : base(conf, "SrcConnString", logger)
        {
        }
    }
}
