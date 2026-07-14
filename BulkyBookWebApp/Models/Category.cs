using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBookWebApp.Models
{
    /// <summary>
    /// 商品分類資料模型。
    /// 此類別會透過 Entity Framework Core 對應到資料庫中的分類資料表。
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 分類編號。
        /// [Key] 表示此欄位是資料表的主索引鍵。
        /// int 型別的主索引鍵通常會被設定為自動遞增。
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 分類名稱。
        /// [Required] 表示此欄位為必填。
        /// 如果沒有輸入名稱，ModelState 驗證會失敗。
        /// </summary>
        [Required(ErrorMessage = "請輸入分類名稱")]
        ///View中
        ///<label asp-for="Name" class="text-dark"></label>
        ///預設名會變成<label>Name</label>
        ///
        ///可以使用 [DisplayName] 設定欄位顯示名稱。
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 分類的顯示順序。
        /// [DisplayName] 設定欄位顯示名稱。
        /// [Range] 限制輸入值必須介於 1 到 100。
        /// </summary>
        [DisplayName("顯示順序")]
        [Range(1, 100, ErrorMessage = "顯示順序必須介於 1 到 100 之間")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 分類資料的建立日期與時間。
        /// 建立 Category 物件時，預設使用目前的系統時間。
        /// </summary>
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}