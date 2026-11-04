const THEME_STORAGE_KEY = 'worker-platform-theme';

function applyTheme(theme) {
    const selectedTheme = theme === 'dark' || theme === 'light' ? theme : 'default';
    document.documentElement.dataset.theme = selectedTheme;
    localStorage.setItem(THEME_STORAGE_KEY, selectedTheme);

    const themeSelect = document.getElementById('themeSelect');
    if (themeSelect) {
        themeSelect.value = selectedTheme;
    }
}

function addThemeControl() {
    const navContainer = document.querySelector('nav .container');
    const host = navContainer || document.body;
    const control = document.createElement('label');
    control.className = 'theme-control';
    control.innerHTML = `
        <span>تم</span>
        <select id="themeSelect" aria-label="انتخاب تم سایت">
            <option value="default">پیش‌فرض</option>
            <option value="light">روشن</option>
            <option value="dark">شب</option>
        </select>
    `;

    if (navContainer) {
        control.classList.add('ms-2');
        navContainer.appendChild(control);
    } else {
        control.classList.add('theme-control-floating');
        host.appendChild(control);
    }

    document.getElementById('themeSelect').addEventListener('change', (event) => {
        applyTheme(event.target.value);
    });
}

const savedTheme = localStorage.getItem(THEME_STORAGE_KEY) || 'default';
applyTheme(savedTheme);
document.addEventListener('DOMContentLoaded', addThemeControl);
