const API_BASE_URL = 'http://localhost:5067/api';

function getTokenRole(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
        return payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    } catch {
        return null;
    }
}

const urlParams = new URLSearchParams(window.location.search);
const jobId = urlParams.get('id');

async function loadJobDetail() {
    try {
        const response = await fetch(`${API_BASE_URL}/JobAds`);
        if (!response.ok) throw new Error('خطا در دریافت اطلاعات آگهی‌ها');

        const result = await response.json();
        const jobAds = Array.isArray(result) ? result : (result.value || []);
        const ad = jobAds.find(j => j.id == jobId);

        if (!ad) {
            document.getElementById('jobDetailCard').innerHTML = '<div class="alert alert-danger">آگهی یافت نشد.</div>';
            return;
        }

    if (getTokenRole(token) !== 'Worker') {
        Swal.fire({
            title: 'دسترسی غیرمجاز',
            text: 'کارفرما نمی‌تواند برای کارفرمایان دیگر درخواست انجام کار بدهد. فقط کارگر می‌تواند درخواست ارسال کند.',
            icon: 'error',
            confirmButtonText: 'باشه'
        });
        return;
    }

        document.getElementById('jobDetailCard').innerHTML = `
            <div class="card-body p-5">
                <h2 class="text-primary mb-3">${ad.title}</h2>
                <div class="d-flex justify-content-between mb-4 border-bottom pb-3">
                    <span class="badge bg-secondary fs-6">📍 ${ad.city}</span>
                    <span class="badge bg-success fs-6">💰 ${ad.price.toLocaleString()} تومان</span>
                </div>
                <h5>توضیحات:</h5>
                <p class="text-muted lh-lg">${ad.description || 'بدون توضیحات'}</p>
                <hr>
                <p class="text-muted">کارفرما: <strong>${ad.employerName}</strong></p>
                <div class="mt-4 text-center">
                    <button class="btn btn-lg btn-primary px-5" onclick="sendRequest(${ad.id})">ارسال درخواست برای این کار</button>
                </div>
            </div>
        `;
    } catch (error) {
        document.getElementById('jobDetailCard').innerHTML = `<div class="alert alert-danger">خطا: ${error.message}</div>`;
    }
}

async function sendRequest(adId) {
    const token = localStorage.getItem('token');
    if (!token) {
        Swal.fire({
            title: 'نیاز به ورود',
            text: 'برای ارسال درخواست باید وارد حساب خود شوید.',
            icon: 'warning',
            confirmButtonText: 'باشه'
        });
        window.location.href = 'auth.html';
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/JobRequests`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(adId)
        });

        const data = await response.json().catch(() => ({}));
        if (response.ok) {
            Swal.fire({
                title: 'موفق!',
                text: 'درخواست شما با موفقیت ارسال شد!',
                icon: 'success',
                confirmButtonText: 'باشه'
            });
        } else {
            Swal.fire({
                title: 'خطا!',
                text: data.error || data.Error || 'ارسال درخواست مجاز نیست.',
                icon: 'error',
                confirmButtonText: 'باشه'
            });
        }
    } catch (err) {
        Swal.fire({
            title: 'خطا!',
            text: 'خطا در ارتباط با سرور',
            icon: 'error',
            confirmButtonText: 'باشه'
        });
    }
}

document.addEventListener('DOMContentLoaded', loadJobDetail);
