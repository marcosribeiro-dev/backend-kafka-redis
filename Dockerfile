# Estágio 1: Build - Prepara o ambiente de compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia o arquivo da solução (.sln) para a raiz do WORKDIR (/source)
COPY backend-kafka-redis.sln .

# Copia a pasta 'src' (com todos os projetos da aplicação) para o contêiner
COPY src/ ./src/

# Copia a pasta 'tests' (com o projeto de teste) para o contêiner
# ESTA ERA A LINHA CRUCIAL QUE FALTAVA
COPY tests/ ./tests/

# Restaura os pacotes NuGet para a solução inteira. Agora ele encontrará TODOS os projetos.
RUN dotnet restore "backend-kafka-redis.sln"

# Publica apenas o projeto da API, que é o nosso ponto de entrada executável
RUN dotnet publish "src/Backend.API/Backend.API.csproj" -c Release -o /app/publish --no-restore

# ---

# Estágio 2: Final - Cria a imagem de produção final, leve e otimizada
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Backend.API.dll"]