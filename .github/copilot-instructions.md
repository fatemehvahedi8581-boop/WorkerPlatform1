# Copilot Instructions

## Project Guidelines
- برای پیاده‌سازی Login در این پروژه، DTO باید با نام LoginUserDto در همان فایل LoginUserCommand.cs باشد؛ کلید JWT فعلاً می‌تواند مقدار موقت داشته باشد و در صورت خطای Build باید متن کامل خطا گزارش شود.
- برای API آگهی‌های شغلی، پروژه Application باید به Domain reference داشته باشد و endpoint ثبت آگهی در JobAdsController باید با [Authorize] فقط برای کاربران واردشده قابل دسترسی باشد؛ خطاهای Namespace یا Reference در Build باید کامل گزارش شوند.
- برای تغییرات این پروژه، باید Reference دسترسی Application به Domain و IApplicationDbContext بررسی شود؛ متدهای Register و Login در UsersController حفظ شوند و فقط endpoint جدید اضافه شود؛ در صورت خطای Build، متن کامل خطاهای Namespace یا Reference گزارش شود.

## Final Delivery Instructions
- برای تحویل نهایی WorkerPlatform، باید bin/obj پاک شوند، README بخش screenshots و checklist و اجرای داوران را داشته باشد، Presentation_Guide.md ساخته شود، و سپس با پیام docs مشخص commit و push انجام شود.