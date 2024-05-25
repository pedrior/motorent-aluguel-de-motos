# Projeto: Motorent - Aluguel de Motos

O Motorent é uma API REST para gerenciar aluguel de motos e entregadores. Este projeto está sendo desenvolvido
com ASP.NET Core,EF Core, PostgreSQL, Docker/Docker Compose, RabbitMQ, MassTransit, Arquitetura Limpa, DDD, CQRS,
Testes de Unidade, Testes de Integração, boas práticas de programação e outras tecnologias e padrões.

> Este projeto é inspirado no [desafio backend da Mottu](https://github.com/Mottu-ops/Desafio-BackEnd), porém está
> sendo desenvolvido por interesse próprio, não estou participando ou participei de algum processo seletivo da empresa.

## :pushpin: Index

- [Casos de Uso](#triangular_flag_on_post-casos-de-uso)
- [Guia de Execução](#beginner-guia-de-execução)
- [API](#globe_with_meridians-api)
- [Tecnologias, Bibliotecas e Padrões Utilizados](#hammer_and_wrench-tecnologias-bibliotecas-e-padrões-utilizados)
- [Licença](#page_with_curl-licença)


## :triangular_flag_on_post: Casos de Uso

- ✅ Eu, como usuário administrador, desejo cadastrar uma nova moto.
  - ✅ Os dados obrigatórios da moto são: ID, Ano, Modelo e Placa.
  - ✅ A placa é um dado único e não pode se repetir.
  - ✅ Ao cadastrar a moto, a aplicação deve gerar um evento de moto cadastrada.
    - ✅ A notificação deve ser publicada por mensageria.
    - ✅ Criar um consumidor para notificar quando o ano da moto for nova (ano atual).
    - ✅ Assim que a mensagem for recebida, deve ser armazenada no banco de dados para consulta futura.
- ✅ Eu, como usuário administrador, desejo consultar as motos existentes na plataforma e conseguir filtrar pela placa.
- ✅ Eu, como usuário administrador, desejo modificar uma moto alterando apenas sua placa que foi cadastrada 
indevidamente.
- ✅ Eu, como usuário administrador, desejo remover uma moto cadastrada incorretamente, desde que não haja 
registro de locações.
- ✅ Eu, como entregador, desejo me cadastrar na plataforma para alugar motos.
  - ✅ Os dados do entregador são: ID, Nome, CNPJ, Data de Nascimento, Número da CNH, Tipo da CNH, Imagem da CNH.
  - ✅ Os tipos de CNH válidos são A, B ou ambos A+B.
  - ✅ O CNPJ é único e não pode se repetir.
  - ✅ O número da CNH é único e não pode se repetir.
- ✅ Eu, como entregador, desejo enviar a foto da minha CNH para atualizar meu cadastro.
  - ✅ O formato do arquivo deve ser PNG ou BMP.
  - ✅ A foto não deve ser armazenada no banco de dados; você pode utilizar um serviço de armazenamento.
- ✅ Eu como entregador quero alugar uma moto por um período.
  - ✅ Os planos disponíveis para locação são:
    - ✅ 7 dias com um custo de R$30,00 por dia
    - ✅ 15 dias com um custo de R$28,00 por dia
    - ✅ 30 dias com um custo de R$22,00 por dia
    - ✅ 45 dias com um custo de R$20,00 por dia
    - ✅ 50 dias com um custo de R$18,00 por dia
  - ✅ A locação obrigatóriamente tem que ter uma data de início e uma data de término e outra data de previsão de
    término.
  - ✅ O início da locação obrigatóriamente é o primeiro dia após a data de criação.
  - ✅ Somente entregadores habilitados na categoria A podem efetuar uma locação
- ✅ Eu como entregador quero informar a data que irei devolver a moto e consultar o valor total da locação.
  - ✅ Quando a data informada for inferior à data prevista do término, será cobrado o valor das diárias e uma multa
    adicional de 40% sobre o valor das diárias não efetivadas.
  - ✅ Quando a data informada for superior à data prevista do término, será cobrado um valor adicional de R$ 50,00 por
    diária adicional.

## :beginner: Guia de Execução

### Pré-requisitos

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [AWS S3](https://aws.amazon.com/pt/s3/)
- [EF Core CLI](https://docs.microsoft.com/pt-br/ef/core/cli/dotnet)

### Execução

1. Clone o repositório:

```bash
git clone https://github.com/pedrior/motorent-aluguel-de-motos.git
cd motorent-aluguel-de-motos
```

2. Forneça as configurações necessárias para a aplicação em `Motorent.Api > appsettings*.json`:

```json
{
  "Storage": {
    "BucketName": "Your unique AWS S3 bucket name"
  },
  "AWS": {
    "Profile": "Your AWS profile",
    "Region": "Your AWS region"
  }
}
```

3. Crie um certificado para permitir a execução da API via HTTPS no Docker:

```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/motorent.pfx -p password
```

4. Execute o banco de dados e aplique a migração:

```bash
docker compose up -d postgres
dotnet ef database update -s src/Motorent.Api -p src/Motorent.Infrastructure
```

5. Inicie a API e os serviços necessários via Docker Compose:

```bash
docker compose up --build -d
```

## :globe_with_meridians: API

> Alguns endpoints requerem um usuário administrador. Para isso, basta fazer login usando as credenciais:
> email: john@admin.com\
> password: John@123

### URLs

__API:__ [https://localhost:8001/api/v1](https://localhost:8001/api/v1)\
__Documentação:__ [https://localhost:8001/swagger/index.html](https://localhost:8001/swagger/index.html)

### Versionamento

A API é versionada via URL. Por padrão, todos os endpoints recebem a versão `v1`.

### Autenticação

A API implementa autenticação via JWT para proteção de endpoints. Ao fazer solicitações autenticadas, é necessário
incluir o token de acesso no cabeçalho `Authorization` com o esquema `Bearer`. Se o token de acesso não for fornecido 
ou for inválido, a API retornará uma resposta `401 Unauthorized`.

### Autorização

Alguns endpoints podem exigir requisitos específicos para serem executados com sucesso. Se você fizer uma requisição
autenticada, mas não atender aos requisitos de autorização, receberá uma resposta `403 Forbidden`.

### Respostas

A API utiliza códigos de resposta HTTP padrão para indicar o sucesso ou falha de uma requisição.

| Código                      | Descrição                                                                                |
|-----------------------------|------------------------------------------------------------------------------------------|
| 200 - OK                    | Tudo funcionou como esperado.                                                            |
| 201 - Created               | Tudo funcionou como esperado e, como resultado, foi criado um novo recurso.              |
| 204 - No Content            | Tudo funcionou como esperado, mas não retornou nenhum conteúdo.                          |
| 400 - Bad Request           | A solicitação contém um corpo ou parâmetro(s) inválido(s).                               |
| 401 - Unauthorized          | A solicitação requer autenticação de um usuário                                          |
| 403 - Forbidden             | O usuário está autenticado, mas não autorizado a realizar a solicitação.                 |
| 404 - Not Found             | O recurso não existe.                                                                    |
| 409 - Conflict              | A solicitação não pôde ser concluída devido a um conflito com o estado atual do recurso. |
| 422 - Unprocessable Entity  | O corpo da solicitação era aceitável, mas não pôde ser processado.                       |
| 429 - Too Many Requests     | Muitas solicitações atingiram a API muito rapidamente.                                   |
| 500 - Internal Server Error | Ocorreu um erro inesperado.                                                              |
| 503 - Service Unavailable   | Serviço temporariamente indisponível.                                                    |

### Erros

A API fornece respostas de erro em conforme o [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807).
A resposta pode conter campos adicionais para fornecer uma descrição mais detalhada do erro.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflict",
  "status": 409,
  "trace_id": "00-e1bc5d8bc5a9323b449fd98810d47b3d-9e93c3c8ce77e57c-00",
  "errors": [
    {
      "code": "motorcycle.license_plate_not_unique",
      "message": "Já existe uma moto com a mesma placa no sistema.",
      "details": {
        "license_plate": "KPA7A55"
      }
    }
  ]
}
```

## :hammer_and_wrench: Tecnologias, Bibliotecas e Padrões Utilizados

- ASP.NET Core 8
- Entity Framework Core 8
- PostgreSQL
- Docker/Docker Compose
- Amazon S3
- xUnit
- DDD
- Arquitetura Limpa
- Teste de Unidade
- Teste de Integração
- Swashbuckle
- MediatR
- FluentValidation
- Mapster
- Ulid
- Serilog
- Hangfire
- MassTransit
- RabbitMQ
- FluentAssertion
- FakeItEasy
- Testcontainers
- Respawn
- JWT
- Autenticação
- Autorização
- Versionamento
- Padrão CQRS
- Padrão Result
- Padrão Repository
- Padrão UnitOfWork


## :page_with_curl: Licença

Este repositório está licenciado sob a [Licença MIT](https://github.com/pedrior/motorent-aluguel-de-motos/blob/master/LICENSE).
