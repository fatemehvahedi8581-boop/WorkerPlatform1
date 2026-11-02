# WorkerPlatform (پلتفرم اتصال کارگران و کارفرمایان)

پروژه‌ای برای اتصال کارگران فصلی/روزمزد به کارفرمایان پروژه‌های کوچک و خانگی.

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
