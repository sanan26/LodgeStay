using System;

namespace LodgeStay.Services
{
    public class ConnectivityService
    {
        public event Action? ConnectivityChanged;

        public bool IsConnected =>
            Connectivity.Current.NetworkAccess
            == NetworkAccess.Internet;

        public void StartMonitoring()
        {
            Connectivity.Current.ConnectivityChanged
                += OnConnectivityChanged;
        }

        public void StopMonitoring()
        {
            Connectivity.Current.ConnectivityChanged
                -= OnConnectivityChanged;
        }

        private void OnConnectivityChanged(
            object? sender, ConnectivityChangedEventArgs e)
        {
            ConnectivityChanged?.Invoke();
        }
    }
}