using Microsoft.EntityFrameworkCore;
using BulkyBookWebApp.Models;

namespace BulkyBookWebApp.Data
{
    /// <summary>
    /// 應用程式的資料庫內容類別。
    /// :DbContext 代表能使用EF Core
    /// ApplicationDbContext 負責：
    /// 1. 連接資料庫。
    /// 2. 管理資料表。
    /// 3. 讓程式可以透過 Entity Framework Core 操作資料。
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// ApplicationDbContext 的建構子。
        /// 
        /// DbContextOptions 會接收 Program.cs 中設定的資料庫連線資訊，
        /// 例如使用哪一種資料庫以及 Connection String。
        /// 
        /// base(options) 會將設定傳給父類別 DbContext。
        /// </summary>
        /// <param name="options">資料庫連線與 EF Core 的相關設定</param>
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ) : base(options)
        {
        }

        /// <summary>
        /// 對應資料庫中的 Category 資料表。
        /// 
        /// Category 表示一筆分類資料。
        /// 
        /// 可以透過 Categories 執行查詢、新增、修改及刪除。
        /// 
        /// null! 表示此屬性會由 Entity Framework Core 自動初始化，
        /// 告訴編譯器不需要顯示 CS8618 警告。
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;
    }
}