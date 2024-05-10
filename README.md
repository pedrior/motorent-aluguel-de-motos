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


## Licença

Este repositório está licenciado sob a [Licença MIT](https://github.com/pedrior/motorent-aluguel-de-motos/blob/master/LICENSE).
