﻿namespace NTDLS.Katzebase.Engine.Library
{
    internal class CriticalSection : IDisposable
    {
        private readonly object _lockObject;

        public bool IsLockHeld { get; private set; }

        /// <summary>
        /// Enters a critical section.
        /// </summary>
        /// <param name="lockObject"></param>
        public CriticalSection(object lockObject)
        {
            _lockObject = lockObject;
            Monitor.Enter(_lockObject);
            IsLockHeld = true;
        }

        /// <summary>
        /// Tries to enter a critical section.
        /// </summary>
        /// <param name="lockObject"></param>
        public CriticalSection(object lockObject, int timeout)
        {
            _lockObject = lockObject;
            IsLockHeld = Monitor.TryEnter(_lockObject, timeout);
        }

        public void Dispose()
        {
            if (IsLockHeld)
            {
                Monitor.Exit(_lockObject);
            }
        }
    }
}
