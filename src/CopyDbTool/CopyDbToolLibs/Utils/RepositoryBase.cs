using Microsoft.Extensions.Logging;

namespace CopyDbToolLibs
{
    /// <summary>
    /// Reposityクラスのベースインターフェース
    /// </summary>
    public interface IRepositoryBase
    {
    }

    /// <summary>
    /// Reposityクラスのベースクラス
    /// </summary>
    public abstract class RepositoryBase : IRepositoryBase
    {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        protected AppDbContext context;
        /// <summary>
        /// ロガー
        /// </summary>
        protected ILogger logger;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="context">DBコンテキスト</param>
        /// <param name="logger">ロガー</param>
        public RepositoryBase(AppDbContext context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }
    }
}
