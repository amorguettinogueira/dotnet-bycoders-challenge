(() => {
    const table = document.querySelector('table');
    if (!table) return;

    // Attach click to store rows to fetch transactions
    table.addEventListener('click', async (e) => {
        const tr = e.target.closest('tr[data-store-id]');
        if (!tr) return;
        const storeId = tr.getAttribute('data-store-id');
        const fileId = document.querySelector('[data-selected-file-id]')?.getAttribute('data-selected-file-id');
        if (!storeId || !fileId) return;

        // Toggle existing detail row
        const next = tr.nextElementSibling;
        if (next && next.classList.contains('drilldown')) {
            next.remove();
            return;
        }

        try {
            const url = `?handler=Transactions&fileId=${encodeURIComponent(fileId)}&storeId=${encodeURIComponent(storeId)}`;
            const res = await fetch(url, { headers: { 'Accept': 'application/json' } });
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            const items = await res.json();

            const detail = document.createElement('tr');
            detail.className = 'drilldown';
            const td = document.createElement('td');
            td.colSpan = 2;

            if (!items?.length) {
                td.innerHTML = '<div class="text-muted">No transactions.</div>';
            } else {
                const rows = items.map(i => `<tr><td>${i.transactionType}</td><td>${i.date}</td><td>${i.time}</td><td>${i.value.toLocaleString(undefined, { style: 'currency', currency: 'USD' })}</td></tr>`).join('');
                td.innerHTML = `
                    <table class="table table-sm mb-0">
                        <thead><tr><th>Type</th><th>Date</th><th>Time</th><th>Value</th></tr></thead>
                        <tbody>${rows}</tbody>
                    </table>`;
            }

            detail.appendChild(td);
            tr.parentNode.insertBefore(detail, tr.nextSibling);
        } catch (err) {
            console.error('Failed to load transactions', err);
        }
    });
})();
