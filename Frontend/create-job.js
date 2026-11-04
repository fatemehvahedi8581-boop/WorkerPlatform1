const API_BASE_URL = 'http://localhost:5067/api';

function getTokenRole(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
        return payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    } catch {
        return null;
    }
}

document.getElementById('createJobForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const token = localStorage.getItem('token');
    if (!token) {
        Swal.fire({
            title: 'نیاز به ورود',
            text: 'برای ثبت آگهی باید وارد حساب کاربری خود شوید.',
            icon: 'warning',
            confirmButtonText: 'باشه'
        });
        window.location.href = 'auth.html';
        return;
    }

    if (getTokenRole(token) !== 'Employer') {
        Swal.fire({
            title: 'دسترسی غیرمجاز',
            text: 'فقط کارفرما می‌تواند آگهی ثبت کند.',
            icon: 'error',
            confirmButtonText: 'باشه'
        });
        window.location.href = 'index.html';
        return;
    }

    const title = document.getElementById('jobTitle').value;
    const city = document.getElementById('jobCity').value;
    const price = parseFloat(document.getElementById('jobPrice').value);
    const description = document.getElementById('jobDescription').value;

    try {
        const response = await fetch(`${API_BASE_URL}/JobAds`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({ title, description, city, price })
        });

        const data = await response.json().catch(() => ({}));

        if (response.ok) {
            Swal.fire({
                title: 'موفق!',
                text: 'آگهی شما با موفقیت ثبت شد!',
                icon: 'success',
                confirmButtonText: 'باشه'
            });
            window.location.href = 'index.html';
        } else {
            Swal.fire({
                title: 'خطا!',
                text: data.error || data.Error || 'کارگر اجازه ثبت آگهی ندارد. فقط کارفرما می‌تواند آگهی ثبت کند.',
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
});
