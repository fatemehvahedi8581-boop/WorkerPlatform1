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
                ${data.profileImageData
                    ? `<img src="${data.profileImageData}" alt="عکس پروفایل" class="rounded-circle object-fit-cover" style="width: 70px; height: 70px;">`
                    : `<div class="bg-primary text-white rounded-circle d-inline-flex justify-content-center align-items-center" style="width: 70px; height: 70px; font-size: 24px;">${escapeHtml(data.fullName.charAt(0))}</div>`}
            </div>
            <h5 class="fw-bold">${data.fullName}</h5>
            <p class="text-muted mb-2">${data.role === 'Worker' ? 'کارگر' : 'کارفرما'}</p>
            <div class="badge bg-warning text-dark px-3 py-2 fs-6">
                ⭐ میانگین امتیاز: ${data.averageRating} از ۵ (${data.totalReviews} نظر)
            </div>
        `;

        document.getElementById('birthYear').value = data.birthYear || '';
        document.getElementById('birthMonth').value = data.birthMonth || '';
        document.getElementById('birthDay').value = data.birthDay || '';
        document.getElementById('profileCity').value = data.city || '';

        const skillsCard = document.getElementById('skillsCard');
        if (data.role === 'Worker') {
            skillsCard.style.display = 'block';
            renderSkills(data.skills || []);
        }

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
                } else if (h.status === 'Accepted') {
                    actionButtons = `
                        <div class="mt-2">
                            <button class="btn btn-sm btn-primary" onclick="updateStatus(${h.requestId}, 'Completed')">اتمام کار</button>
                            <button class="btn btn-sm btn-warning ms-2" onclick="showReviewModal(${h.requestId})">ثبت امتیاز</button>
                        </div>
                    `;
                }
            }

            html += `
                <li class="list-group-item py-3">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="mb-1 fw-bold">${h.jobTitle}</h6>
                            ${data.role === 'Employer' ? renderWorkerSkills(h) : ''}
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

function escapeHtml(value) {
    return String(value || '').replace(/[&<>'"]/g, character => ({
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        "'": '&#39;',
        '"': '&quot;'
    }[character]));
}

function levelLabel(level) {
    return {
        Beginner: 'مبتدی',
        Intermediate: 'متوسط',
        Advanced: 'پیشرفته',
        Expert: 'حرفه‌ای'
    }[level] || level;
}

function renderSkills(skills) {
    const container = document.getElementById('skillsList');
    if (!skills.length) {
        container.innerHTML = '<p class="text-muted small mb-0">هنوز مهارتی ثبت نکرده‌اید.</p>';
        return;
    }

    container.innerHTML = skills.map(skill => `
        <div class="border rounded p-2 mb-2">
            <div class="d-flex justify-content-between align-items-center">
                <strong>${escapeHtml(skill.name)}</strong>
                <button class="btn btn-sm btn-outline-danger" onclick="deleteSkill(${skill.id})">حذف</button>
            </div>
            <span class="badge bg-info text-dark mt-1">${escapeHtml(levelLabel(skill.level))}</span>
            ${skill.description ? `<p class="small text-muted mb-0 mt-1">${escapeHtml(skill.description)}</p>` : ''}
            ${skill.imageData ? `<img src="${skill.imageData}" alt="نمونه‌کار ${escapeHtml(skill.name)}" class="img-fluid rounded mt-2" style="max-height: 120px;">` : ''}
        </div>
    `).join('');
}

function renderWorkerSkills(history) {
    const skills = history.workerSkills || [];
    if (!skills.length) {
        return '<div class="small text-muted mt-2">برای این کارگر مهارتی ثبت نشده است.</div>';
    }

    const skillItems = skills.map(skill => `
        <li class="mb-2"><strong>${escapeHtml(skill.name)}</strong> - ${escapeHtml(levelLabel(skill.level))}${skill.description ? `: ${escapeHtml(skill.description)}` : ''}${skill.imageData ? `<br><img src="${skill.imageData}" alt="نمونه‌کار ${escapeHtml(skill.name)}" class="img-fluid rounded mt-1" style="max-height: 100px;">` : ''}</li>
    `).join('');

    return `<div class="worker-skills mt-2"><div class="fw-bold small">مهارت‌های ${escapeHtml(history.workerName)}:</div><ul class="small mb-0">${skillItems}</ul></div>`;
}

function readImageData(file) {
    return new Promise((resolve, reject) => {
        if (!file) {
            resolve(null);
            return;
        }
        if (file.size > 2 * 1024 * 1024) {
            reject(new Error('حجم تصویر نباید بیشتر از ۲ مگابایت باشد.'));
            return;
        }
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = () => reject(new Error('خواندن تصویر انجام نشد.'));
        reader.readAsDataURL(file);
    });
}

async function updateProfile(event) {
    event.preventDefault();
    const token = localStorage.getItem('token');
    try {
        const imageData = await readImageData(document.getElementById('profileImage').files[0]);
        const response = await fetch(`${API_BASE_URL}/Users/${getUserIdFromToken(token)}/profile`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                profileImageData: imageData,
                birthYear: Number(document.getElementById('birthYear').value) || null,
                birthMonth: Number(document.getElementById('birthMonth').value) || null,
                birthDay: Number(document.getElementById('birthDay').value) || null,
                city: document.getElementById('profileCity').value
            })
        });
        const data = await response.json().catch(() => ({}));
        if (!response.ok) throw new Error(data.error || data.Error || 'ذخیره پروفایل انجام نشد.');
        Swal.fire('موفق!', 'اطلاعات پروفایل ذخیره شد.', 'success');
        loadProfile();
    } catch (error) {
        Swal.fire('خطا', error.message, 'error');
    }
}

async function addSkill(event) {
    event.preventDefault();
    const token = localStorage.getItem('token');
    let imageData;
    try {
        imageData = await readImageData(document.getElementById('skillImage').files[0]);
    } catch (error) {
        Swal.fire('خطا', error.message, 'error');
        return;
    }
    const response = await fetch(`${API_BASE_URL}/WorkerSkills`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({
            name: document.getElementById('skillName').value,
            level: document.getElementById('skillLevel').value,
            description: document.getElementById('skillDescription').value,
            imageData
        })
    });

    const data = await response.json().catch(() => ({}));
    if (!response.ok) {
        Swal.fire('خطا', data.error || data.Error || 'ثبت مهارت انجام نشد.', 'error');
        return;
    }

    document.getElementById('skillForm').reset();
    loadProfile();
}

async function deleteSkill(skillId) {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}/WorkerSkills/${skillId}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
    });

    if (!response.ok) {
        const data = await response.json().catch(() => ({}));
        Swal.fire('خطا', data.error || data.Error || 'حذف مهارت انجام نشد.', 'error');
        return;
    }

    loadProfile();
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
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('profileForm')?.addEventListener('submit', updateProfile);
    document.getElementById('skillForm')?.addEventListener('submit', addSkill);
});
