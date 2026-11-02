const API_BASE_URL = 'http://localhost:5067/api';

// ثبت‌نام
document.getElementById('registerForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const fullName = document.getElementById('regName').value;
    const phoneNumber = document.getElementById('regPhone').value;
    const password = document.getElementById('regPassword').value;
    const role = document.getElementById('regRole').value;

    try {
        const response = await fetch(`${API_BASE_URL}/Users/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName, phoneNumber, password, role })
        });

        const data = await response.json();
        if (response.ok) {
            Swal.fire({
                title: 'موفق!',
                text: 'ثبت‌نام موفقیت‌آمیز بود! حالا می‌توانید وارد شوید.',
                icon: 'success',
                confirmButtonText: 'باشه'
            });
            document.getElementById('login-tab').click();
        } else {
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
});

// ورود
document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const phoneNumber = document.getElementById('loginPhone').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch(`${API_BASE_URL}/Users/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ phoneNumber, password })
        });

        const data = await response.json();
        if (response.ok) {
            localStorage.setItem('token', data.token);
            Swal.fire({
                title: 'موفق!',
                text: 'ورود موفقیت‌آمیز بود!',
                icon: 'success',
                confirmButtonText: 'باشه'
            });
            window.location.href = 'index.html';
        } else {
            Swal.fire({
                title: 'خطا!',
                text: data.error || data.Error || 'شماره موبایل یا رمز عبور اشتباه است',
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
