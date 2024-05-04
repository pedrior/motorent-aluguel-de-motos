# Projeto: Motorent - Aluguel de Motos

Uma API REST para gerenciar aluguel de motos. Este projeto está sendo desenvolvido com ASP.NET Core, EF Core, PostgreSQL, Docker/Docker Compose,
Arquitetura Limpa, DDD, CQRS, Testes de Unidade, Testes de Integração, boas práticas de programação e muito mais.

> Este projeto está sendo inspirado no [desafio backend da Mottu](https://github.com/Mottu-ops/Desafio-BackEnd), porém foi desenvolvido
> por interesse próprio, não estou participando ou participei de algum processo seletivo da empresa.

Deixe uma ⭐ se você gostou deste projeto. Se tiver alguma pergunta, recomendação ou qualquer outra questão, abra uma issue ou entre em contato. 
Este é um projeto feito por hobby, mas se houver algo em que eu possa melhorar, ficaria muito grato em saber 😄.

## 🌐 API

A API está disponível em: `https://localhost:8081/api/{version}`.

__Versionamento__

A API é versionada via URL. Por padrão, todos os endpoints usam a versão `v1` da API.

__Limitação de Taxa__

A maioria dos endpoints tem limites de taxa para evitar abusos. Se esse limite for atingido dentro de um determinado período de tempo, você receberá
uma resposta `429 Too Many Requests`.

__Autenticação__

A API implementa autenticação via JWT para proteção de endpoints. Ao fazer solicitações autenticadas, é necessário incluir o token de acesso no 
cabeçalho `Authorization` com o esquema `Bearer`.

```plain
Authorization: Bearer {token-de-acesso}
```

Se você não fornecer o token de acesso ou fornecer um inválido, você receberá uma resposta `401 Unauthorized`.

__Autorização__

Alguns endpoints podem exigir requisitos específicos para serem executados com sucesso. Se você fizer uma chamada a um endpoint autenticado,
mas não atender aos requisitos de autorização, receberá uma resposta de `403 Forbidden`.

__Erros__

A API fornece respostas de erro em de acordo com o RFC 7807. A resposta pode conter campos adicionais para fornecer uma descrição mais detalhada do erro.

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

__Respostas__

O API utiliza códigos de resposta HTTP padrão para indicar o sucesso ou falha de uma solicitação de API.

| Código                      | Descrição                                                                                                                  |
|-----------------------------|----------------------------------------------------------------------------------------------------------------------------|
| 200 - OK                    | Tudo funcionou como esperado.                                                                                              |
| 201 - Created               | Tudo funcionou como esperado e, como resultado, foi criado um novo recurso.                                                |
| 204 - No Content            | Tudo funcionou como esperado, mas não retornou nenhum conteúdo.                                                            |
| 400 - Bad Request           | A solicitação foi inaceitável, frequentemente devido à falta de um parâmetro obrigatório ou parâmetro malformado.          |
| 401 - Unauthorized          | A solicitação requer autenticação do usuário.                                                                              |
| 403 - Forbidden             | O usuário está autenticado, mas não autorizado a realizar a solicitação.                                                   |
| 404 - Not Found             | O recurso solicitado não existe.                                                                                           |
| 409 - Conflict              | A solicitação não pôde ser concluída devido a um conflito com o estado atual do recurso.                                   |
| 422 - Unprocessable Entity  | O corpo da solicitação era aceitável, mas não pôde ser processado.                                                         |
| 429 - Too Many Requests     | Muitas solicitações atingiram a API muito rapidamente.                                                                     |
| 500 - Internal Server Error | Ocorreu um erro inesperado.                                                                                                |

## 🚀 Endpoints

### Auth

#### Login: Autentica um usuário no sistema.

```http request
POST https://localhost:8081/api/v1/auth/login
Accept: application/json
Content-Type: application/json

{
  "email": "john@doe.com",
  "password": "JohnDoe123"
}
```

Resposta: 200 OK

```json
{
  "type": "Bearer",
  "access_token": "eyJhbGciOiJIUzI1NiIsInR ...",
  "expires_in": 3600
}
```

#### Register: Registra um novo usuário no sistema.

```http request
POST https://localhost:8081/api/v1/auth/register
Accept: application/json
Content-Type: application/json

{
  "email": "john@doe.com",
  "password": "JohnDoe123",
  "given_name": "John",
  "family_name": "Doe",
  "birthdate": "2000-09-05",
  "cnpj": "18.864.014/0001-19",
  "cnh_number": "92353762700",
  "cnh_category": "ab",
  "cnh_exp_date": "2026-12-31"
}
```

Resposta: 201 Created

```json
{
  "type": "Bearer",
  "access_token": "eyJhbGciOiJIUzI1NiIsInR ...",
  "expires_in": 3600
}
```

### Motos

#### Listar Motos: Retorna uma lista paginada de motos.

```http request
GET https://localhost:8081/api/v1/motorcycles?page=1&limit=15&sort=model&order=asc
Accept: application/json
```

Essa requisição aceita os seguintes parâmetros:

| Parâmetro | Descrição                                                                                                |
|-----------|----------------------------------------------------------------------------------------------------------|
| page      | Número da página a ser retornada. Padrão: `1`.                                                           |
| limit     | Número de itens por página. Padrão: `15`.                                                                |
| sort      | Campo pelo qual os itens serão ordenados. Pode ser `model`, `brand` ou `license_plate`. Padrão: `model`. |
| order     | Ordem de classificação dos itens. Padrão: Pode ser  `asc` ou `desc`. Padrão `asc`.                       |


Resposta 200 OK

```json
{
  "page": 1,
  "size": 15,
  "total_items": 30,
  "total_pages": 2,
  "has_previous_page": false,
  "has_next_page": true,
  "items": [
    {
      "id": "01HWY421T6WQMPG7Q9Z6V9M2HP",
      "model": "Fazer 250 ABS",
      "brand": "yamaha",
      "year": 2024,
      "daily_price": 57.99,
      "license_plate": "KPA7A55"
    },
    {
      "id": "01HWXVQD61BVNZRJVD7QRDD3TJ",
      "model": "Titan 160cc ABS",
      "brand": "honda",
      "year": 2022,
      "daily_price": 38.99,
      "license_plate": "PIA2A91"
    },
    ...
  ]
}
```

#### Detalhes da Moto: Retorna os detalhes de uma moto específica por ID ou placa.

```http request
GET https://localhost:8081/api/v1/motorcycles/id-ou-placa
Accept: application/json
```

Resposta 200 OK

```json
{
  "id": "01HWY421T6WQMPG7Q9Z6V9M2HP",
  "model": "Fazer 250 ABS",
  "brand": "yamaha",
  "year": 2024,
  "daily_price": 57.99,
  "license_plate": "KPA7A55",
  "created_at": "2024-05-03T02:20:45.89759+00:00",
  "updated_at": null
}
```

#### Cadastrar Moto: Cadastra uma nova moto no sistema.

```http request
POST https://localhost:8081/api/v1/motorcycles
Accept: application/json
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR ...

{
  "model": "Fazer 250 ABS",
  "brand": "yamaha",
  "year": 2024,
  "daily_price": 57.99,
  "license_plate": "KPA7A55"
}
```

Resposta 201 Created

```json
{
  "id": "01HWY421T6WQMPG7Q9Z6V9M2HP",
  "model": "Fazer 250 ABS",
  "brand": "yamaha",
  "year": 2024,
  "daily_price": 57.99,
  "license_plate": "KPA7A55",
  "created_at": "2024-05-03T02:20:45.89759+00:00",
  "updated_at": null
}
```

#### Atualiza a placa de uma mota cadastrada.

```http request
POST https://localhost:8081/api/v1/motorcycles/id-da-moto/update-license-plate
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR ...

{
  "license_plate": "kOA-2A91"
}
```

Resposta 204 No Content

#### Exclui uma moto cadastrada.

```http request
DELETE https://localhost:8081/api/v1/motorcycles/id-da-moto
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR ...
```

Resposta 204 No Content

## Licença

Este repositório está licenciado sob a [Licença MIT](https://github.com/pedrior/motorent-aluguel-de-motos/blob/master/LICENSE).
