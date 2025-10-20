<h1>Desafio Pague veloz</h1>
O desafio consiste em realizar uma api com gerenciamento de contas conseguindo realizar operações de, credito, debito, reserva e transferencia.

Tecnologia e frameworks utilizados:
 - EntityFramwork - para persistencia e manipulação de dados ao banco de dados.
 - MediatR - para facilitar a separação de command e queries.
 - Swagger - para documentação da API.

## ⚙️ Como Rodar Localmente (sem Docker)

### 🔧 Pré-requisitos

- [.NET SDK 9.0+](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/)

### 🪜 Passo a Passo
1. Configure a connection string do arquivo:
  - DesafioPagueVeloz.API/appsettings.Development.json
  - exemplo:
  ```
  {
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres"
    }
  }
  ```
2. Execute o comando de incialização:
  - ```dotnet run --project DesafioPagueVeloz.API```
3. Acesse a aplicação:
  - Swagger: http://localhost:5000/swagger/index.html
  - Api: http://localhost:5000

## ⚙️ Como Rodar Localmente (com Docker)

### 🔧 Pré-requisitos
 - [Docker]https://www.docker.com/
 - [Docker componse]https://docs.docker.com/compose/
### 🪜 Passo a Passo
1. Suba os containers:
```docker-compose up --build````
2. Acesse a aplicação:
  - Swagger: http://localhost:5000/swagger/index.html
  - Api: http://localhost:5000

##Informações adicionais e decisões técnicas:
 - Cada operação realizada é executada de forma assíncrona pelo DesafioPagueVeloz.Persistense/Workers/OperationWorkerService.cs. Para escalabilidade na execução, ele simula uma fila de pagamentos a serem processados, o que poderia ser substituído por um consumer integrado com RabbitMQ ou Apache Kafka. Também foi implementado o ciclo de vida de cada operação, que é executada apenas três vezes e finalizada com erro para investigação.
 - Implementado middlewares como Filters e Behaviors do mediatR para captação de erros inesperados do sistema e salvos em uma planilha de erros para investigação para caso a aplicação possua erros não mapeados.

 🧑‍💻 Autor
 
Erick Allan Moraes de Oliveira<br>
💻 Desenvolvedor .NET & Angular<br>
📧 [LinkedIn](https://www.linkedin.com/in/erick-allan-moraes/)<br>
📍 Brasil



