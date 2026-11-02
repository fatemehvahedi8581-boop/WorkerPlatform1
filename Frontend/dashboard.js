const API_BASE_URL = 'http://localhost:5067/api';

function getUserIdFromToken(token) {
    const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(atob(base64));
    return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || payload.nameid;
}

async function loadProfile() {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = 'auth.html';
        return;
    }

    const userId = getUserIdFromToken(token);

    try {
        const response = await fetch(`${API_BASE_URL}/Users/${userId}/profile`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) throw new Error('خطا در دریافت پروفایل');
        const data = await response.json();

        document.getElementById('profileCard').innerHTML = `
            <div class="mb-3">
                <div class="bg-primary text-white rounded-circle d-inline-flex justify-content-center align-items-center" style="width: 70px; height: 70px; font-size: 24px;">
                    ${data.fullName.charAt(0)}
                </div>
            </div>
            <h5 class="fw-bold">${data.fullName}</h5>
            <p class="text-muted mb-2">${data.role === 'Worker' ? 'کارگر' : 'کارفرما'}</p>
            <div class="badge bg-warning text-dark px-3 py-2 fs-6">
                ⭐ میانگین امتیاز: ${data.averageRating} از ۵ (${data.totalReviews} نظر)
            </div>
        `;

        const historyContainer = document.getElementById('historyContainer');
        if (data.jobHistory.length === 0) {
            historyContainer.innerHTML = '<p class="text-muted">هیچ سابقه‌ای ثبت نشده است.</p>';
            return;
        }

        let html = '<ul class="list-group list-group-flush">';
        data.jobHistory.forEach(h => {
            let statusBadge = '';
            if (h.status === 'Pending') statusBadge = '<span class="badge bg-secondary">در انتظار</span>';
            if (h.status === 'Accepted') statusBadge = '<span class="badge bg-success">تأیید شده</span>';
            if (h.status === 'Rejected') statusBadge = '<span class="badge bg-danger">رد شده</span>';
            if (h.status === 'Completed') statusBadge = '<span class="badge bg-primary">تکمیل شده</span>';

            let actionButtons = '';
            if (data.role === 'Employer') {
                if (h.status === 'Pending') {
                    actionButtons = `
                        <div class="mt-2">
                            <button class="btn btn-sm btn-success" onclick="updateStatus(${h.requestId}, 'Accepted')">تأیید</button>
                            <button class="btn btn-sm btn-danger" onclick="updateStatus(${h.requestId}, 'Rejected')">رد</button>
                        </div>
                    `;
                } else if (h.status === 'Accepted' && !h.rating) {
                    actionButtons = `
                        <div class="mt-2">
                            <button class="btn btn-sm btn-warning" onclick="showReviewModal(${h.requestId})">ثبت امتیاز و نظر</button>
                        </div>
                    `;
                }
            }

            html += `
                <li class="list-group-item py-3">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="mb-1 fw-bold">${h.jobTitle}</h6>
                            <small class="text-muted">تاریخ: ${new Date(h.appliedAt).toLocaleDateString('fa-IR')}</small>
                        </div>
                        <div>${statusBadge}</div>
                    </div>
                    ${actionButtons}
                </li>
            `;
        });
        html += '</ul>';
        historyContainer.innerHTML = html;
    } catch (error) {
        document.getElementById('profileCard').innerHTML = `<div class="alert alert-danger">${error.message}</div>`;
        document.getElementById('historyContainer').innerHTML = '';
    }
}

async function updateStatus(requestId, newStatus) {
    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/JobRequests/${requestId}/status`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(newStatus)
        });

        if (response.ok) {
            Swal.fire({
                title: 'موفق!',
                text: 'وضعیت با موفقیت تغییر کرد.',
                icon: 'success',
                confirmButtonText: 'باشه'
            });
            loadProfile();
        } else {
            const data = await response.json();
            Swal.fire({
                title: 'خطا!',
                text: data.error || data.Error || 'مشکلی پیش آمد',
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

async function showReviewModal(requestId) {
    const { value: formValues } = await Swal.fire({
        title: 'ثبت امتیاز برای کارگر',
        html:
            '<div class="mb-3 text-start">' +
            '<label class="form-label">امتیاز (۱ تا ۵)</label>' +
            '<input id="swal-rating" type="number" min="1" max="5" class="form-control" placeholder="مثلاً 5">' +
            '</div>' +
            '<div class="text-start">' +
            '<label class="form-label">نظر شما</label>' +
            '<textarea id="swal-comment" class="form-control" rows="3" placeholder="کیفیت کار چطور بود؟"></textarea>' +
            '</div>',
        focusConfirm: false,
        showCancelButton: true,
        confirmButtonText: 'ثبت',
        cancelButtonText: 'انصراف',
        preConfirm: () => {
            const rating = document.getElementById('swal-rating').value;
            const comment = document.getElementById('swal-comment').value;
            if (!rating || rating < 1 || rating > 5) {
                Swal.showValidationMessage('امتیاز باید بین ۱ تا ۵ باشد');
                return false;
            }
            return { rating: parseInt(rating), comment: comment };
        }
    });

    if (formValues) {
        const token = localStorage.getItem('token');
        try {
            const response = await fetch(`${API_BASE_URL}/Reviews`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: JSON.stringify({
                    jobRequestId: requestId,
                    rating: formValues.rating,
                    comment: formValues.comment
                })
            });

            if (response.ok) {
                Swal.fire('موفق!', 'امتیاز شما ثبت شد.', 'success');
                loadProfile();
            } else {
                const data = await response.json();
                Swal.fire('خطا!', data.error || data.Error || 'مشکلی پیش آمد.', 'error');
            }
        } catch (err) {
            Swal.fire('خطا!', 'ارتباط با سرور برقرار نشد.', 'error');
        }
    }
}

document.addEventListener('DOMContentLoaded', loadProfile);
