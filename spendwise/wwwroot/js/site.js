// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {

    // Role toggle — shared by Login and Register
    document.querySelectorAll('.sw-role-toggle').forEach(function (toggle) {
        const buttons = toggle.querySelectorAll('.sw-role-btn');
        const hiddenInput = toggle.closest('form').querySelector('[name="Role"]');
        buttons.forEach(function (btn) {
            btn.addEventListener('click', function () {
                buttons.forEach(function (b) { b.classList.remove('active'); });
                btn.classList.add('active');
                hiddenInput.value = btn.dataset.role;
            });
        });
    });

    // Password visibility toggle — shared by Login and Register
    document.querySelectorAll('.sw-pw-toggle').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const input = btn.closest('.input-group').querySelector('input');
            const icon = btn.querySelector('i');
            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.replace('bi-eye', 'bi-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.replace('bi-eye-slash', 'bi-eye');
            }
        });
    });

    // Password strength meter — Register page only
    const pwInput = document.getElementById('Password');
    if (pwInput) {
        pwInput.addEventListener('input', function () {
            const val = pwInput.value;
            const bar = document.getElementById('sw-pw-strength-bar');
            const label = document.getElementById('sw-pw-strength-label');
            let level = 0;
            if (val.length >= 6) level = 1;
            if (level && /[A-Z]/.test(val)) level = 2;
            if (level > 1 && /[0-9]/.test(val)) level = 3;
            if (level > 2 && /[^A-Za-z0-9]/.test(val)) level = 4;
            const map = ['', 'Weak', 'Fair', 'Good', 'Strong'];
            const cls = ['', 'bg-danger', 'bg-warning', 'bg-info', 'bg-success'];
            bar.style.width = (level * 25) + '%';
            bar.className = 'progress-bar ' + (cls[level] || '');
            label.textContent = map[level] || '';
        });
    }

    const canvas = document.getElementById('categoryChart');
    if (!canvas) return;

    const labels = JSON.parse(canvas.dataset.labels);
    const values = JSON.parse(canvas.dataset.values);
    const colors = JSON.parse(canvas.dataset.colors);

    new Chart(canvas, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: colors,
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { position: 'bottom' }
            }
        }
    });
});
