# Projeto: Motorent - Aluguel de Motos

Uma API REST para gerenciar aluguel de motos. Este projeto está sendo desenvolvido com ASP.NET Core, EF Core,
PostgreSQL, Docker/Docker Compose, Arquitetura Limpa, DDD, CQRS, Testes de Unidade, Testes de Integração, boas práticas
de programação e muito mais.

> Este projeto está sendo inspirado no [desafio backend da Mottu](https://github.com/Mottu-ops/Desafio-BackEnd), porém
> foi desenvolvido por interesse próprio, não estou participando ou participei de algum processo seletivo da empresa.

Este é um projeto feito por hobby. Se tiver alguma pergunta, recomendação ou qualquer outra questão, por favor abra uma
issue ou entre em contato.

## 🚀 Guia de Execução

### Pré-requisitos

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [AWS S3](https://aws.amazon.com/pt/s3/)
- [EF Core CLI](https://docs.microsoft.com/pt-br/ef/core/cli/dotnet)

### Executando o Projeto

1. Clone o repositório:

```bash
git clone https://github.com/pedrior/motorent-aluguel-de-motos.git
cd motorent-aluguel-de-motos
```

2. Você precisará definir algumas configurações da aplicação em `Motorent.Api > appsettings*.json`:

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

3. Crie um certificado para permitir a execução da API via HTTPS em contêineres Docker:

```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/motorent.pfx -p password
```

4. Execute o banco de dados para aplicar a migração:

```bash
docker compose up -d postgres
```

5. Após a completa inicialização do banco de dados, aplique a migração:

```bash
dotnet ef database update -s src/Motorent.Api -p src/Motorent.Infrastructure
```
6. Inicie a API:

```bash
docker compose up --build -d
```

## 🌐 API

A API está disponível em:

```plain
https://localhost:8081/api/{version}
```

### Endpoints

Acesse a documentação da API para obter informações detalhadas sobre os endpoints disponíveis.

```plain
https://localhost:8001/swagger/index.html
```

### Versionamento

A API é versionada via URL. Por padrão, todos os endpoints usam a versão `v1`.

```plain
https://localhost:8081/api/v1
```

### Limitação de Taxa

A maioria dos endpoints possui limitação de taxa para evitar abusos. Se esse limite for atingido em uma determinada
janela de tempo, você receberá uma resposta `429 Too Many Requests`.

### Autenticação

A API implementa autenticação via JWT para proteção de endpoints. Ao fazer solicitações autenticadas, é necessário
incluir o token de acesso no cabeçalho `Authorization` com o esquema `Bearer`.

```plain
Authorization: Bearer {token-de-acesso}
```

Se você não fornecer o token de acesso ou fornecer um inválido, você receberá uma resposta `401 Unauthorized`.

### Autorização

Alguns endpoints podem exigir requisitos específicos para serem executados com sucesso. Se você fizer uma requisição
autenticada, mas não atender aos requisitos de autorização, receberá uma resposta `403 Forbidden`.

### Erros

A API fornece respostas de erro em conforme o RFC 7807. A resposta pode conter campos adicionais para fornecer uma
descrição mais detalhada do erro.

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflict",
  "status": 409,
  "trace_id": "00-e1bc5d8bc5a9323b449fd98810d47b3d-9e93c3c8ce77e57c-00",
  "errors": [
    {
      "code": "motorcycle.license_plate_not_unique",
      "message": "There is already a motorcycle with the same license plate in the system.",
      "details": {
        "license_plate": "KPA7A55"
      }
    }
  ]
}
```

### Respostas

A API utiliza códigos de resposta HTTP padrão para indicar o sucesso ou falha de uma requisição.

| Código                      | Descrição                                                                                                         |
|-----------------------------|-------------------------------------------------------------------------------------------------------------------|
| 200 - OK                    | Tudo funcionou como esperado.                                                                                     |
| 201 - Created               | Tudo funcionou como esperado e, como resultado, foi criado um novo recurso.                                       |
| 204 - No Content            | Tudo funcionou como esperado, mas não retornou nenhum conteúdo.                                                   |
| 400 - Bad Request           | A solicitação foi inaceitável, frequentemente devido à falta de um parâmetro obrigatório ou parâmetro malformado. |
| 401 - Unauthorized          | A solicitação requer autenticação do usuário.                                                                     |
| 403 - Forbidden             | O usuário está autenticado, mas não autorizado a realizar a solicitação.                                          |
| 404 - Not Found             | O recurso solicitado não existe.                                                                                  |
| 409 - Conflict              | A solicitação não pôde ser concluída devido a um conflito com o estado atual do recurso.                          |
| 422 - Unprocessable Entity  | O corpo da solicitação era aceitável, mas não pôde ser processado.                                                |
| 429 - Too Many Requests     | Muitas solicitações atingiram a API muito rapidamente.                                                            |
| 500 - Internal Server Error | Ocorreu um erro inesperado.                                                                                       |
| 503 - Service Unavailable   | O serviço não está disponível no momento.                                                                         |

## Licença

Este repositório está licenciado sob
a [Licença MIT](https://github.com/pedrior/motorent-aluguel-de-motos/blob/master/LICENSE).
