using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CopyDbToolLibs.Repositories
{
    /// <summary>
    /// コピー先DBのRepositoryインターフェース
    /// </summary>
    public interface IDstRepository : ICopyDbRepositoryBase
    {
    }

    /// <summary>
    /// コピー先DBのRepositoryクラス
    /// </summary>
    public class DstRepository : CopyDbRepositoryBase, IDstRepository
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="conf">設定情報</param>
        /// <param name="logger">ロガー</param>
        public DstRepository(IConfiguration conf, ILogger<DstRepository> logger) : base(conf, "DstConnString", logger)
        {
        }
    }
}
