using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _task8.Domain
{
    public abstract class ConveyorBase : IConveyor
    {
        public bool IsStarted { get; set; } = false;
        public abstract string Name { get; }

        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        public virtual int ProcessingTime { get; } = 2000;
        public virtual int SugarPerCall { get; } = 10;
        public bool IsCrashed { get; protected set; }

        public virtual double SuccessProbability { get; } = 0.9;
        public void Start(Factory factory)
        {
            ThreadPool.QueueUserWorkItem(
                async o =>
                {
                    while (true)
                    {
                        if (cancelTokenSource.IsCancellationRequested || IsCrashed)
                        {
                            cancelTokenSource = new CancellationTokenSource();
                            IsStarted = false;

                            return;
                        }
                        await Process(factory);
                    }
                });

            IsStarted = true;
        }

        public void Stop()
        {
            cancelTokenSource.Cancel();
            
            IsStarted = false;
        }

        public async Task Process(Factory factory)
        {
            if (factory.TakeSugar(this))
            {

                Random random = new Random();
                if (random.NextDouble() > SuccessProbability)
                {
                    IsCrashed = true;
                    OnCrashChanged?.Invoke(this);
                    return;
                }

                await Task.Delay(ProcessingTime);
            }
        }

        public void Repair()
        {
            IsCrashed = false;
        }

        public event Action<IConveyor> OnCrashChanged;
    }
    public interface IConveyor
    {
        public bool IsStarted { get; set; }
        public string Name { get; }
        public bool IsCrashed { get; }
        public int SugarPerCall { get; }
        public int ProcessingTime { get; }
        public void Start(Factory factory);
        public void Stop();
        public Task Process(Factory factory);
        public void Repair();

        public event Action<IConveyor> OnCrashChanged;

    }
}
