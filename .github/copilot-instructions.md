# Copilot Instructions

## Project Guidelines
- برای پیاده‌سازی Login در این پروژه، DTO باید با نام LoginUserDto در همان فایل LoginUserCommand.cs باشد؛ کلید JWT فعلاً می‌تواند مقدار موقت داشته باشد و در صورت خطای Build باید متن کامل خطا گزارش شود.
- برای API آگهی‌های شغلی، پروژه Application باید به Domain reference داشته باشد و endpoint ثبت آگهی در JobAdsController باید با [Authorize] فقط برای کاربران واردشده قابل دسترسی باشد؛ خطاهای Namespace یا Reference در Build باید کامل گزارش شوند.
- برای تغییرات این پروژه، باید Reference دسترسی Application به Domain و IApplicationDbContext بررسی شود؛ متدهای Register و Login در UsersController حفظ شوند و فقط endpoint جدید اضافه شود؛ در صورت خطای Build، متن کامل خطاهای Namespace یا Reference گزارش شود.
- برای آماده‌سازی پروژه WorkerPlatform، CORS باید در Program.cs با policy به نام AllowAll فعال شود، Swagger باید عنوان و توضیحات مشخص داشته باشد، و README.md در ریشه پروژه ایجاد شود؛ سپس build، commit با پیام مشخص و push انجام شود.