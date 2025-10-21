(() => {
    // Prefer configured API base, but if it points to the Docker DNS host 'api',
    // browsers on the host cannot resolve it. Fallback to current host with API port.
    const configured = (window.API_BASE_URL || '').trim();
    const fallback = `${window.location.protocol}//${window.location.hostname}:5163`;
    const apiBase = configured && !/^https?:\/\/api(?::|\/|$)/i.test(configured)
        ? configured.replace(/\/$/, '')
        : fallback;

    const hubUrl = `${apiBase}/hubs/notifications`;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();

    connection.on('FileProcessed', data => {
        console.info('FileProcessed', data);
        window.location.reload();
    });

    connection.start().catch(err => console.error('SignalR connection failed', err));
})();
