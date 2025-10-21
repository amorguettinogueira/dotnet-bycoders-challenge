using Microsoft.AspNetCore.SignalR;

namespace Bcp.API.Hubs;

/// <summary>
/// SignalR hub used to push notifications to connected clients.
/// </summary>
public class NotificationsHub : Hub;
