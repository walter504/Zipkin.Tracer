using log4net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class SpanProcessorTaskFactory
    {
        private Task task;
        private CancellationTokenSource cancellationTokenSource;
        private ILog logger;

        public SpanProcessorTaskFactory(ILog logger, CancellationTokenSource cancellationTokenSource = null)
        {
            this.logger = logger;
            if (cancellationTokenSource == null)
            {
                this.cancellationTokenSource = new CancellationTokenSource();
            }
            else
            {
                this.cancellationTokenSource = cancellationTokenSource;
            }
        }

        [ExcludeFromCodeCoverage]  //excluded from code coverage since this class is a 1 liner that starts up a background thread
        public void CreateAndStart(Action action)
        {
            if (task == null || task.Status == TaskStatus.Faulted)
            {
                task = new Task(() => ActionWrapper(action), cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                task.Start();
            }
        }

        public void StopTask()
        {
            cancellationTokenSource.Cancel();
        }

        internal async void ActionWrapper(Action action)
        {
            while (!IsTaskCancelled())
            {
                int delayTime = 500;
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    logger.Error("Error in SpanProcessorTask", ex);
                    delayTime = 3000;
                }
                await Task.Delay(delayTime, cancellationTokenSource.Token);
            }
        }

        public bool IsTaskCancelled()
        {
            return cancellationTokenSource.IsCancellationRequested;
        }
    }
}
