// آدرس API (در صورت نیاز تغییر دهید)
const API_BASE_URL = 'http://localhost:5067/api';

async function loadJobAds() {
    const container = document.getElementById('jobAdsContainer');
    const loading = document.getElementById('loadingMessage');
    const searchQuery = document.getElementById('searchInput').value;

    // نمایش پیام لودینگ
    loading.style.display = 'block';
    container.innerHTML = '';
    container.appendChild(loading);

    try {
        // دریافت آگهی‌ها از API
        let url = `${API_BASE_URL}/JobAds`;
        if (searchQuery) {
            url += `?search=${encodeURIComponent(searchQuery)}`;
        }

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error('خطا در دریافت اطلاعات از سرور');
        }

        const result = await response.json();
        const jobAds = Array.isArray(result) ? result : (result.value || []);

        loading.style.display = 'none';

        if (jobAds.length === 0) {
            container.innerHTML = '<div class="col-12 text-center text-muted">هیچ آگهی‌ای یافت نشد.</div>';
            return;
        }

        // رندر کارت‌های آگهی
        jobAds.forEach(ad => {
            const card = document.createElement('div');
            card.className = 'col-md-6 col-lg-4';
            card.innerHTML = `
                <div class="card job-card shadow-sm h-100">
                    <div class="card-body">
                        <h5 class="card-title fw-bold text-primary">${ad.title}</h5>
                        <p class="card-text text-muted mb-2">
                            <small>📍 ${ad.city}</small>
                        </p>
                        <p class="card-text text-truncate">${ad.description || 'توضیحاتی ثبت نشده است.'}</p>
                        <div class="d-flex justify-content-between align-items-center mt-3">
                            <span class="price-badge">${ad.price.toLocaleString()} تومان</span>
                            <button class="btn btn-sm btn-outline-primary" onclick="window.location.href='job-detail.html?id=${ad.id}'">جزئیات</button>
                        </div>
                    </div>
                    <div class="card-footer bg-white border-0 text-muted small">
                        کارفرما: ${ad.employerName}
                    </div>
                </div>
            `;
            container.appendChild(card);
        });

    } catch (error) {
        loading.style.display = 'none';
        container.innerHTML = `<div class="col-12 text-center text-danger">خطا: ${error.message}</div>`;
    }
}

function updateNavbar() {
    const token = localStorage.getItem('token');
    const navActions = document.getElementById('navActions');

    if (token) {
        navActions.innerHTML = `
            <a href="dashboard.html" class="btn btn-info text-white me-2 fw-bold">داشبورد من</a>
            <a href="create-job.html" class="btn btn-warning me-2 fw-bold">+ ثبت آگهی</a>
            <button onclick="logout()" class="btn btn-outline-light">خروج</button>
        `;
    } else {
        navActions.innerHTML = `
            <a href="auth.html" class="btn btn-outline-light">ورود / ثبت‌نام</a>
        `;
    }
}

function logout() {
    localStorage.removeItem('token');
    window.location.reload();
}

// بارگذاری اولیه هنگام باز شدن صفحه
document.addEventListener('DOMContentLoaded', () => {
    loadJobAds();
    updateNavbar();
});
