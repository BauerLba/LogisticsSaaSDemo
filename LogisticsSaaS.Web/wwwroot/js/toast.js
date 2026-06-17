class ToastManager {
    constructor() {
        this.toasts = [];
        this.container = null;
        this.initContainer();
    }

    initContainer() {
        if (!document.getElementById('toast-container')) {
            const container = document.createElement('div');
            container.id = 'toast-container';
            container.style.cssText = `
                position: fixed;
                top: 1.5rem;
                right: 1.5rem;
                z-index: 9999;
                display: flex;
                flex-direction: column;
                gap: 0.75rem;
                max-width: 400px;
            `;
            document.body.appendChild(container);
            this.container = container;
        } else {
            this.container = document.getElementById('toast-container');
        }
    }

    show(message, type = 'info', duration = 4000) {
        const id = Date.now();
        const toast = document.createElement('div');

        const bgColor = {
            success: '#10b981',
            error: '#ef4444',
            warning: '#f59e0b',
            info: '#3b82f6'
        }[type] || '#3b82f6';

        const bgLight = {
            success: 'rgba(16, 185, 129, 0.1)',
            error: 'rgba(239, 68, 68, 0.1)',
            warning: 'rgba(245, 158, 11, 0.1)',
            info: 'rgba(59, 130, 246, 0.1)'
        }[type] || 'rgba(59, 130, 246, 0.1)';

        const icon = {
            success: 'fa-circle-check',
            error: 'fa-circle-xmark',
            warning: 'fa-triangle-exclamation',
            info: 'fa-circle-info'
        }[type] || 'fa-circle-info';

        toast.id = `toast-${id}`;
        toast.style.cssText = `
            background: ${bgLight};
            border: 1px solid ${bgColor}80;
            border-radius: 0.5rem;
            padding: 1rem 1.25rem;
            color: ${bgColor};
            font-size: 0.875rem;
            display: flex;
            align-items: center;
            gap: 0.75rem;
            animation: slideInRight 0.3s ease-out;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        `;

        toast.innerHTML = `
            <i class="fas ${icon}" style="flex-shrink: 0; font-size: 1rem;"></i>
            <span style="flex: 1;">${message}</span>
            <button onclick="document.getElementById('toast-${id}').remove()"
                    style="background: transparent; border: none; color: ${bgColor}; cursor: pointer; padding: 0; width: 1.25rem; height: 1.25rem; display: flex; align-items: center; justify-content: center; opacity: 0.7;">
                <i class="fas fa-xmark"></i>
            </button>
        `;

        this.container.appendChild(toast);

        if (duration > 0) {
            setTimeout(() => {
                const el = document.getElementById(`toast-${id}`);
                if (el) {
                    el.style.animation = 'slideOutRight 0.3s ease-out';
                    setTimeout(() => el.remove(), 300);
                }
            }, duration);
        }
    }

    success(message, duration = 3000) {
        this.show(message, 'success', duration);
    }

    error(message, duration = 5000) {
        this.show(message, 'error', duration);
    }

    warning(message, duration = 4000) {
        this.show(message, 'warning', duration);
    }

    info(message, duration = 3000) {
        this.show(message, 'info', duration);
    }
}

window.toast = new ToastManager();

// Add animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(400px);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }

    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(400px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);
