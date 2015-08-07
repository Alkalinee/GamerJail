using System;
using System.Threading;
using System.Threading.Tasks;

namespace GamerJail.Logic
{
    public class ShutdownAction
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ShutdownAction()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public event EventHandler InvokeActions;

        public bool IsEnabled { get; private set; }

        public async void Start()
        {
            IsEnabled = true;
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(5), _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                IsEnabled = false;
            }

            InvokeActions?.Invoke(this, EventArgs.Empty);
            IsEnabled = false;
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
