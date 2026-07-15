using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BulkyBookWebApp.Data;
using BulkyBookWebApp.Models;

namespace BulkyBookWebApp.Controllers
{
    /// <summary>
    /// 商品分類控制器。
    ///
    /// 負責處理 Category（分類）相關的 HTTP 請求，
    /// 並在 Category Model、資料庫與 View 之間傳遞資料。
    ///
    /// 對應的 View 資料夾：
    /// Views/Category/
    /// </summary>
    public class CategoryController : Controller
    {
        /// <summary>
        /// 資料庫操作物件。
        ///
        /// ApplicationDbContext 繼承自 DbContext，
        /// 可以透過它操作 Categories 資料表。
        /// </summary>
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// CategoryController 建構子。
        ///
        /// ASP.NET Core 會透過依賴注入（Dependency Injection），
        /// 自動將 ApplicationDbContext 物件傳入。
        /// </summary>
        /// <param name="db">資料庫操作物件</param>
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 顯示所有分類資料。
        ///
        /// HTTP 方法：GET
        /// 網址：/Category/Index
        /// 簡寫網址：/Category
        ///
        /// 對應 View：
        /// Views/Category/Index.cshtml
        /// </summary>
        /// <returns>包含所有分類資料的 Index View</returns>
        [HttpGet]
        public IActionResult Index()
        {
            // 從 Categories 資料表取得所有分類資料。
            IEnumerable<Category> categoryList = _db.Categories.ToList();

            // 將分類集合傳給 Views/Category/Index.cshtml。
            return View(categoryList);
        }

        /// <summary>
        /// 顯示新增分類的表單。
        ///
        /// HTTP 方法：GET
        /// 網址：/Category/Create
        ///
        /// 對應 View：
        /// Views/Category/Create.cshtml
        /// </summary>
        /// <returns>新增分類表單</returns>
        [HttpGet]
        public IActionResult Create()
        {
            // 沒有指定 View 名稱時，
            // ASP.NET Core 會尋找 Views/Category/Create.cshtml。
            return View();
        }

        /// <summary>
        /// 接收新增分類表單送出的資料。
        ///
        /// HTTP 方法：POST
        /// 網址：/Category/Create
        /// </summary>
        /// <param name="obj">由表單欄位繫結產生的 Category 物件</param>
        /// <returns>
        /// 驗證成功：重新導向 Index。
        /// 驗證失敗：回到 Create View 並顯示錯誤。
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            // 自訂驗證：
            // 分類名稱不能和顯示順序完全相同。
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                // 第一個參數必須對應 Category 的屬性名稱。
                // 使用 nameof 可以避免手動輸入字串時拼錯。
                ModelState.AddModelError(
                    nameof(obj.Name),
                    "分類名稱不能與顯示順序完全相同。"
                );
            }

            /*
                ModelState 
                是 ASP.NET Core MVC 用來保存「表單資料綁定結果」與「驗證結果」的物件。

                可以把它理解成：
                MVC 幫你記錄使用者送進來的資料有沒有成功轉換，以及資料是否符合驗證規則。
            */
            // ModelState.IsValid 會檢查：
            // 1. Category Model 中的 DataAnnotations 驗證。
            // 2. 上方使用 AddModelError() 加入的自訂錯誤。
            if (ModelState.IsValid)
            {
                // 將新的 Category 物件加入資料庫追蹤。
                _db.Categories.Add(obj);

                // 實際執行 INSERT，將資料寫入資料庫。
                _db.SaveChanges();

                // TempData 可以在重新導向後保留一次資料，
                // 通常用來顯示操作成功訊息。
                TempData["success"] = "分類新增成功。";

                // 重新導向至 CategoryController 的 Index Action。
                return RedirectToAction(nameof(Index));
            }

            // 驗證失敗時，將使用者輸入的 obj 傳回 Create View。
            //
            // ASP.NET Core 會因目前 Action 名稱是 Create，
            // 自動尋找 Views/Category/Create.cshtml。
            //
            // 傳入 obj 可保留使用者剛才填寫的資料，
            // ModelState 中的錯誤也可以顯示在畫面上。
            return View(obj);
        }

        /// <summary>
        /// 顯示編輯分類的表單。
        ///
        /// HTTP 方法：GET
        /// 網址：/Category/Edit/1
        ///
        /// 對應 View：
        /// Views/Category/Edit.cshtml
        /// </summary>
        /// <param name="id">要編輯的分類編號</param>
        /// <returns>
        /// 找到資料：顯示 Edit View。
        /// 找不到資料：回傳 404 Not Found。
        /// </returns>
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            // id 是 nullable int，因此可能沒有值。
            // 分類編號通常也不會是 0。
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Find() 會使用主鍵查詢資料。
            // 如果資料已經存在於 EF Core 的追蹤快取中，
            // 可能不需要再次查詢資料庫。
            Category? categoryFromDb = _db.Categories.Find(id);

            // 其他常見查詢方式：
            //
            // FirstOrDefault：
            // 找到第一筆符合條件的資料，找不到回傳 null。
            //
            // Category? categoryFromDbFirst =
            //     _db.Categories.FirstOrDefault(category => category.Id == id);
            //
            // SingleOrDefault：
            // 預期最多只有一筆資料。
            // 如果找到多筆符合資料，會發生例外。
            //
            // Category? categoryFromDbSingle =
            //     _db.Categories.SingleOrDefault(category => category.Id == id);

            // 查不到指定分類時回傳 HTTP 404。
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            // 將查到的分類資料傳給 Views/Category/Edit.cshtml。
            return View(categoryFromDb);
        }

        /// <summary>
        /// 接收編輯分類表單送出的資料。
        ///
        /// HTTP 方法：POST
        /// 網址：/Category/Edit
        /// </summary>
        /// <param name="obj">修改後的 Category 物件</param>
        /// <returns>
        /// 驗證成功：更新資料並重新導向 Index。
        /// 驗證失敗：回到 Edit View。
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            // 自訂驗證：
            // 分類名稱不能和顯示順序完全相同。
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError(
                    nameof(obj.Name),
                    "分類名稱不能與顯示順序完全相同。"
                );
            }

            // 驗證失敗時不執行資料庫更新。
            if (!ModelState.IsValid)
            {
                // 將使用者輸入的資料傳回 Edit View，
                // 並顯示 ModelState 中的驗證錯誤。
                return View(obj);
            }

            // 先確認這筆分類是否仍然存在。
            bool categoryExists = _db.Categories.Any(
                category => category.Id == obj.Id
            );

            if (!categoryExists)
            {
                return NotFound();
            }

            // 將 obj 標記為已修改。
            _db.Categories.Update(obj);

            // 實際執行 UPDATE，將變更寫入資料庫。
            _db.SaveChanges();

            TempData["success"] = "分類修改成功。";

            // 修改完成後回到分類列表。
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 顯示刪除分類的確認頁面。
        ///
        /// HTTP 方法：GET
        /// 網址：/Category/Delete/1
        ///
        /// 對應 View：
        /// Views/Category/Delete.cshtml
        /// </summary>
        /// <param name="id">要刪除的分類編號</param>
        /// <returns>
        /// 找到資料：顯示 Delete View。
        /// 找不到資料：回傳 404 Not Found。
        /// </returns>
        /*
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // 取得準備刪除的分類資料。
            Category? categoryFromDb = _db.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            // 將資料傳給刪除確認頁面，
            // 此時尚未真正刪除資料。
            return View(categoryFromDb);
        }
        */

        // 直接刪除
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // 使用主鍵取得要刪除的分類。
            Category? categoryFromDb = _db.Categories.Find(id);

            // 找不到資料時回傳 HTTP 404，
            // 避免原本使用 Single() 時因查不到資料而發生例外。
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            // 將資料標記為刪除。
            _db.Categories.Remove(categoryFromDb);

            // 實際執行 DELETE。
            _db.SaveChanges();

            TempData["success"] = "分類刪除成功。";

            return RedirectToAction(nameof(Index));
        }




        /// <summary>
        /// 確認並執行刪除分類。
        ///
        /// HTTP 方法：POST
        /// 網址：/Category/Delete/1
        ///
        /// ActionName("Delete") 表示：
        /// 雖然 C# 方法名稱是 DeletePost，
        /// 但對外的 Action 名稱仍然是 Delete。
        /// </summary>
        /// <param name="id">要刪除的分類編號</param>
        /// <returns>刪除完成後重新導向 Index</returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // 使用主鍵取得要刪除的分類。
            Category? categoryFromDb = _db.Categories.Find(id);

            // 找不到資料時回傳 HTTP 404，
            // 避免原本使用 Single() 時因查不到資料而發生例外。
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            // 將資料標記為刪除。
            _db.Categories.Remove(categoryFromDb);

            // 實際執行 DELETE。
            _db.SaveChanges();

            TempData["success"] = "分類刪除成功。";

            return RedirectToAction(nameof(Index));
        }
    }
}