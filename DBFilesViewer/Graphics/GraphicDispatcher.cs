using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DBFilesViewer.Graphics
{
    public sealed class GraphicsDispatcher
    {
        private BlockingCollection<Action> _queuedActions = new BlockingCollection<Action>(new ConcurrentQueue<Action>());

        private int AssignedThreadId { get; set; }
        public bool InvokeRequired => Thread.CurrentThread.ManagedThreadId != AssignedThreadId;
        public void AssignToThread()
        {
            AssignedThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public void ProcessFrame()
        {
            var start = Environment.TickCount;
            var numFrames = 0;

            do
            {
                // No action queued, immediately stop
                if (_queuedActions.Count == 0)
                    return;

                Action @delegate;
                if (_queuedActions.TryTake(out @delegate))
                    @delegate();

                ++numFrames;
            }
            while (Environment.TickCount - start < 30 && numFrames < 10);
        }

        public void Enqueue(Action @delegate)
        {
            if (!InvokeRequired)
                @delegate();
            else
                _queuedActions.Add(@delegate);
        }
    }
}
