window.initCharts = {
    initTimelineChart: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Shipments Created',
                    data: data,
                    borderColor: '#4f46e5',
                    backgroundColor: 'rgba(79, 70, 229, 0.1)',
                    borderWidth: 2,
                    tension: 0.4,
                    fill: true,
                    pointRadius: 4,
                    pointBackgroundColor: '#4f46e5',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            color: '#6b7280',
                            font: { size: 12, weight: '500' },
                            padding: 15,
                            usePointStyle: true,
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(15, 23, 42, 0.8)',
                        padding: 12,
                        titleColor: '#fff',
                        bodyColor: '#fff',
                        borderColor: '#334155',
                        borderWidth: 1,
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(229, 231, 235, 0.5)' },
                        ticks: { color: '#6b7280', font: { size: 11 } },
                    },
                    x: {
                        grid: { display: false },
                        ticks: { color: '#6b7280', font: { size: 11 } },
                    }
                }
            }
        });
    },

    initStatusChart: function (canvasId, labels, data) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        const colors = ['#fef3c7', '#cffafe', '#dcfce7', '#f3e8ff'];
        const borderColors = ['#fde68a', '#a5f3fc', '#bbf7d0', '#e9d5ff'];

        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: colors.slice(0, labels.length),
                    borderColor: borderColors.slice(0, labels.length),
                    borderWidth: 2,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            color: '#6b7280',
                            font: { size: 12, weight: '500' },
                            padding: 15,
                            usePointStyle: true,
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(15, 23, 42, 0.8)',
                        padding: 12,
                        titleColor: '#fff',
                        bodyColor: '#fff',
                        borderColor: '#334155',
                        borderWidth: 1,
                        callbacks: {
                            label: function (context) {
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((context.parsed / total) * 100).toFixed(1);
                                return context.label + ': ' + context.parsed + ' (' + percentage + '%)';
                            }
                        }
                    }
                }
            }
        });
    }
};
