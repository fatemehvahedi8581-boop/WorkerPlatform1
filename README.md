# WorkerPlatform (پلتفرم اتصال کارگران و کارفرمایان)

پروژه‌ای برای اتصال کارگران فصلی/روزمزد به کارفرمایان پروژه‌های کوچک و خانگی.

## 📸 نمای پروژه (Screenshots)
*(برای اضافه کردن تصاویر، یک پوشه به نام `screenshots` در ریشه پروژه بسازید و عکس‌های صفحات را با نام‌های زیر در آن قرار دهید)*
- `home.png`: صفحه اصلی و لیست آگهی‌ها
- `auth.png`: صفحه ورود و ثبت‌نام
- `dashboard.png`: پنل کاربری و مدیریت درخواست‌ها
- `swagger.png`: مستندات API

## تکنولوژی‌های استفاده‌شده
- **Backend:** ASP.NET Core 10 (Web API)
- **Database:** SQLite + Entity Framework Core
- **Architecture:** Clean Architecture + CQRS (MediatR)
- **Authentication:** JWT (JSON Web Tokens)

## پیش‌نیازها
- .NET 10 SDK
- Visual Studio 2026

## نحوه‌ی اجرا
1. پروژه را کلون کنید.
2. در Package Manager Console دستور `Update-Database` را برای ساخت دیتابیس اجرا کنید.
3. پروژه `WorkerPlatform.API` را به عنوان Startup Project تنظیم کرده و `F5` بزنید.
4. مستندات Swagger در آدرس `https://localhost:port/swagger` در دسترس است.

## ساختار پروژه
- `Domain`: مدل‌های اصلی (User, JobAd, JobRequest, Review)
- `Application`: منطق کسب‌وکار (CQRS Commands & Queries)
- `Infrastructure`: دسترسی به دیتابیس (EF Core, AppDbContext)
- `WorkerPlatform.API`: کنترلرها و تنظیمات API

## ویژگی‌های پیاده‌سازی‌شده
- ثبت‌نام و ورود با JWT
- ثبت آگهی شغلی (فقط کارفرمایان)
- مشاهده و جستجوی آگهی‌ها
- ارسال درخواست توسط کارگر
- تأیید/رد درخواست توسط کارفرما
- ثبت امتیاز و مشاهده پروفایل با میانگین امتیازات

## ✅ چک‌لیست تحویل
- [x] پیاده‌سازی احراز هویت و مدیریت کاربران
- [x] ثبت، مشاهده و مدیریت آگهی‌های شغلی
- [x] ثبت و مدیریت درخواست‌های کاری
- [x] مستندسازی API با Swagger
- [x] افزودن راهنمای مشارکت و راهنمای ارائه پروژه

## 🧑‍⚖️ اجرای پروژه برای داوران
1. مخزن را کلون کنید و در پوشه پروژه قرار بگیرید.
2. دستور `dotnet restore WorkerPlatform.sln` را اجرا کنید.
3. دستور `dotnet build WorkerPlatform.sln` را برای بررسی صحت Build اجرا کنید.
4. پروژه `WorkerPlatform.API` را اجرا کنید و Swagger را در مسیر `/swagger` باز کنید.
5. ابتدا ثبت‌نام و ورود را آزمایش کنید، سپس ایجاد آگهی، جستجوی آگهی و ارسال درخواست کاری را بررسی کنید.
6. برای مشاهده تصاویر نمایشی، فایل‌های مربوطه را در پوشه `screenshots` قرار دهید.
