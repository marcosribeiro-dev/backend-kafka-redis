# Desafio Técnico: Backend com .NET, Kafka e Redis 🚀



![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![Kafka](https://img.shields.io/badge/Apache%20Kafka-231F20?style=for-the-badge&logo=apachekafka)
![Redis](https://img.shields.io/badge/redis-%23DD0031.svg?&style=for-the-badge&logo=redis&logoColor=white)
![Status](https://img.shields.io/badge/status-concluído-success?style=for-the-badge)

Este repositório contém a solução completa para o desafio técnico de implementação de um serviço de backend que consome, processa e armazena pedidos utilizando .NET 8, Kafka e Redis, orquestrados com Docker Compose e estruturados com os princípios da **Clean Architecture**.

---

## 🏛️ Arquitetura

Este projeto foi refatorado para seguir os princípios da **Clean Architecture**, separando as responsabilidades em camadas distintas para garantir alta coesão, baixo acoplamento, testabilidade e manutenibilidade.

A regra de dependência é sempre do exterior para o interior:

**API (Apresentação) → Infrastructure → Application → Domain**

### 📁 Estrutura dos Projetos

A solução está organizada da seguinte forma:

-   **`src/`**: Contém todo o código-fonte da aplicação.
    -   **`Backend.Domain`**: O coração da aplicação. Contém as entidades de negócio (`Order`, `OrderResult`) e não possui dependências externas.
    -   **`Backend.Application`**: Define a lógica e os casos de uso. Contém as interfaces (`ICacheService`) que a aplicação necessita.
    -   **`Backend.Infrastructure`**: Implementa as tecnologias externas. Contém as classes que interagem com Kafka (`KafkaConsumerService`) e Redis (`RedisCacheService`).
    -   **`Backend.API`**: A camada de apresentação. Expõe os endpoints HTTP e configura a injeção de dependência.
-   **`tests/`**: Contém os projetos de teste.
    -   **`Backend.UnitTests`**: Testes unitários para validar a lógica de negócio da camada de Domínio.

---

## 🛠️ Tecnologias Utilizadas

* **Backend:** .NET 8, ASP.NET Core
* **Mensageria:** Apache Kafka
* **Cache:** Redis
* **Containerização:** Docker & Docker Compose
* **Testes:** xUnit
* **Interface Kafka:** Kafka-UI (Provectus)
* **Serialização:** Newtonsoft.Json

---

## 🚀 Como Executar

Certifique-se de que você tem o **.NET 8 SDK** e o **Docker Desktop** instalados e rodando na sua máquina.

1.  **Clone o repositório:**
    ```bash
    git clone <https://github.com/marcosribeiro-dev/backend-kafka-redis.git>
    ```

2.  **Navegue até a pasta raiz:**
    ```bash
    cd <backend-kafka-redis>
    ```

3.  **Suba o ambiente completo:**
    O `docker-compose` irá construir a imagem da API e iniciar todos os 6 contêineres na ordem correta, incluindo a criação automática do tópico `orders`.
    ```bash
    docker-compose up --build
    ```
    *Para rodar em segundo plano, adicione a flag `-d`.*

4.  **Aguarde a inicialização:** A primeira inicialização pode levar alguns minutos. Observe os logs até que todos os serviços se estabilizem.

---

## 🧪 Como Testar

Após o ambiente estar no ar, você pode testar o fluxo completo:

* **Interface do Kafka (Kafka-UI):** `http://localhost:8080`
* **Documentação da API (Swagger):** `http://localhost:8088/swagger`

**1. Envie uma mensagem para o Kafka:**

Abra um **novo terminal** e execute o comando abaixo para iniciar o produtor:
```bash
docker-compose exec kafka /opt/bitnami/kafka/bin/kafka-console-producer.sh --bootstrap-server localhost:9092 --topic orders
```
Cole um JSON de pedido e pressione Enter:
```json
{"OrderId":"order-001","CustomerName":"Marcos Ribeiro","Amount":700}
```
Pressione `Ctrl + C` para sair do modo produtor do Kafka.

**2. Verifique o processamento:**

* **No Kafka-UI:** Navegue até o tópico `orders` e veja a mensagem que você enviou.
* **Nos logs da API:** Você verá as mensagens "Pedido recebido..." e "Pedido ... processado e salvo no cache.".

**3. Consulte a API:**

Use o Swagger, o arquivo `.http` do projeto ou seu navegador para fazer uma requisição `GET` para o endpoint:
```
http://localhost:8088/order/order-001
```

**Resultado Esperado:** Uma resposta `200 OK` com o corpo do JSON processado.
```json
{
  "orderId": "order-001",
  "customerName": "Marcos Ribeiro",
  "amount": 700,
  "tax": 70,
  "total": 770
}
```

---
