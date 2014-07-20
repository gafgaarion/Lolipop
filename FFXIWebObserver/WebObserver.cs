using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;
using CommunicationHandler;
using FFXIWorldKnowledge;

namespace FFXIWebObserver
{
    public class WebObserverHttpServer : IObserver
    {
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private readonly Thread[] _workers;
        private readonly ManualResetEvent _stop, _ready;
        private bool stop;
        private Queue<HttpListenerContext> _queue;
        private ResponseProcessor responseProcessor;
        private IBus bus;

        public WebObserverHttpServer(IBus bus,
                                    ITruthRepository truthRepository)
        {
            this.responseProcessor = new ResponseProcessor(bus,  truthRepository);
            this.bus = bus;

            bus.Start();

            stop = false;
            _workers = new Thread[10];
            _queue = new Queue<HttpListenerContext>();
            _stop = new ManualResetEvent(false);
            _ready = new ManualResetEvent(false);
            ////_listener = new HttpListener();
            //_listenerThread = new Thread(HandleRequests);

            Start(8080);
        }

        public void Start(int port)
        {
            //_listener.Prefixes.Add(String.Format(@"http://+:{0}/", port));
            //_listener.Start();
            //_listenerThread.Start();

            //for (int i = 0; i < _workers.Length; i++)
            //{
            //    _workers[i] = new Thread(Worker);
            //    _workers[i].Start();
            //}
        }

        public void Stop()
        {
            stop = true;
            //_listenerThread.Join();
            //foreach (Thread worker in _workers)
            //    worker.Join();
            //_listener.Stop();
        }

        private void HandleRequests()
        {
            while (_listener.IsListening && !stop)
            {
                var context = _listener.BeginGetContext(ContextReady, null);

                if (0 == WaitHandle.WaitAny(new[] { _stop, context.AsyncWaitHandle }) || stop)
                    return;
            }
        }

        private void ContextReady(IAsyncResult ar)
        {
            try
            {
                lock (_queue)
                {
                    _queue.Enqueue(_listener.EndGetContext(ar));
                    _ready.Set();
                }
            }
            catch { return; }
        }

        private void Worker()
        {
            WaitHandle[] wait = new[] { _ready, _stop };
            while (0 == WaitHandle.WaitAny(wait))
            {
                HttpListenerContext context;
                lock (_queue)
                {
                    if (_queue.Count > 0)
                        context = _queue.Dequeue();
                    else
                    {
                        _ready.Reset();
                        continue;
                    }
                }

                try { responseProcessor.Process(context); }
                catch (Exception e) { Console.Error.WriteLine(e); }
            }
        }

        public void Terminate()
        {
            Stop();
            bus.Terminate();
        }
                
    }

}
