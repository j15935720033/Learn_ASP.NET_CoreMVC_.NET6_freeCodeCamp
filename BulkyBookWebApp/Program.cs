using BulkyBookWebApp.Data;
using Microsoft.EntityFrameworkCore;

// 建立 WebApplicationBuilder 物件。
// builder 負責讀取設定檔、註冊服務，以及建立 Web 應用程式。
var builder = WebApplication.CreateBuilder(args);

// 將 ASP.NET Core MVC 的 Controller 與 View 服務註冊到依賴注入容器中。
// 註冊後，專案才能使用 Controller、View、Model 等 MVC 功能。
builder.Services.AddControllersWithViews();

// 將 ApplicationDbContext 註冊到依賴注入容器中。
// ASP.NET Core 會在需要 ApplicationDbContext 時，自動建立並注入物件。
builder.Services.AddDbContext<ApplicationDbContext>(
    // 設定 Entity Framework Core 使用 SQL Server 資料庫。
    // DefaultConnection 是 appsettings.json 中設定的資料庫連線字串名稱。
    options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// 啟用 Razor Pages 服務，並加入 Razor Runtime Compilation。
// Razor Runtime Compilation 可以讓開發期間修改 .cshtml 檔案後，
// 不需要重新啟動應用程式就能看到修改結果。
builder.Services
    .AddRazorPages()
    .AddRazorRuntimeCompilation();

// 根據前面完成的設定與服務註冊，建立 WebApplication 物件。
var app = builder.Build();

// 設定 HTTP 請求的處理流程（Middleware Pipeline）。
// 當目前環境不是開發環境時，執行正式環境的錯誤處理設定。
if (!app.Environment.IsDevelopment())
{
    // 發生未處理的例外時，將使用者導向 HomeController 的 Error Action。
    app.UseExceptionHandler("/Home/Error");

    // 啟用 HSTS（HTTP Strict Transport Security）。
    // HSTS 會要求瀏覽器在一段時間內只能使用 HTTPS 連線到網站。
    app.UseHsts();
}

// 將所有 HTTP 請求重新導向成 HTTPS，提高連線安全性。
app.UseHttpsRedirection();

// 啟用靜態檔案服務。
// 允許瀏覽器存取 wwwroot 資料夾中的 CSS、JavaScript、圖片等檔案。
app.UseStaticFiles();

// 啟用路由功能。
// ASP.NET Core 會根據網址判斷要執行哪一個 Controller 與 Action。
app.UseRouting();

// 啟用授權功能。
// 如果 Controller 或 Action 使用 [Authorize]，會在此檢查使用者權限。
app.UseAuthorization();

// 設定 MVC 預設路由規則。
app.MapControllerRoute(
    // 路由名稱。
    name: "default",

    // 路由格式：
    // controller 預設為 HomeController。
    // action 預設為 Index 方法。
    // id 後面的 ? 表示 id 是選填參數。
    //
    // 例如：
    // /Category/Index
    // /Category/Edit/1
    // /
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// 啟動 ASP.NET Core Web 應用程式，開始接收 HTTP 請求。
app.Run();