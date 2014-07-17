using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Counter
{
    internal class ThreadSyncTester
    {
        private readonly CancellationToken _token;
        private volatile int _aCounter;
        private volatile int _bCounter;

        public ThreadSyncTester(CancellationToken token)
        {
            _token = token;
            _aCounter = 0;
            _bCounter = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ThreadA()
        {
            while (!_token.IsCancellationRequested)
            {
                if (_bCounter == _aCounter)
                {
                    _aCounter = _aCounter + 1;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ThreadB()
        {
            while (!_token.IsCancellationRequested)
            {
                if (_aCounter == _bCounter + 1)
                {
                    _bCounter = _bCounter + 1;
                }
            }
        }

        public void PrintResult(TextWriter output)
        {
            output.WriteLine("A counter: " + _aCounter);
            output.WriteLine("B counter: " + _bCounter);
        }
    }
}