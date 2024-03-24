using System.ComponentModel.DataAnnotations.Schema;

namespace CopyDbToolLibs
{
    /// <summary>
    /// Entityクラスのベースクラス
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// 登録日時
        /// </summary>
        [Column("pms_i_ymd")]
        public DateTime PmsIYmd
        {
            get;
            set;
        }

        /// <summary>
        /// 登録ユーザー
        /// </summary>
        [Column("pms_i_usr")]
        public string PmsIUsr
        {
            get;
            set;
        }

        /// <summary>
        /// 登録処理
        /// </summary>
        [Column("pms_i_class")]
        public string? PmsIClass
        {
            get;
            set;
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Column("pms_u_ymd")]
        public DateTime? PmsUYmd
        {
            get;
            set;
        }

        /// <summary>
        /// 更新ユーザー
        /// </summary>
        [Column("pms_u_usr")]
        public string? PmsUUsr
        {
            get;
            set;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        [Column("pms_u_class")]
        public string? PmsUClass
        {
            get;
            set;
        }

        /// <summary>
        /// Table属性に指定されているテーブル名を取得します。
        /// </summary>
        /// <returns>テーブル名</returns>
        public string GetTableNameByTableAttr()
        {
            var type = GetType();
            dynamic attr = type.GetCustomAttributes(false).SingleOrDefault(x => x.GetType().Name == "TableAttribute");

            return attr?.Name;
        }
    }
}
