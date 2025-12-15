# 📘 Bookstore API Automation Tests

## 📌 Descrição

Este repositório contém um projeto de **automação de testes de API** para a aplicação **[Bookstore API](https://bookstore.toolsqa.com/swagger/)**.  
O objetivo do projeto é validar os principais fluxos funcionais e contratuais da API, garantindo confiabilidade, previsibilidade e rápida detecção de regressões.

O foco principal está em:
- Validações de **Bookshelf (coleção do usuário)**
- Validações de **Catálogo de livros**
- Validações de **consulta individual por ISBN**
- Validações de **cenários de erro e regras de negócio**

O projeto foi estruturado seguindo **boas práticas de organização, legibilidade e escalabilidade**, pensando em evolução futura (ex.: relatórios com Allure).

---

## 🧰 Tecnologias Utilizadas

- **.NET (C#)** – Linguagem principal do projeto
- **xUnit** – Framework de testes
- **RestSharp** – Cliente HTTP para consumo da API
- **System.Text.Json** – Serialização e desserialização de JSON
- **dotenv (.env)** – Gerenciamento de variáveis de ambiente
- **Git** – Controle de versão

---

## 🎯 Objetivos do Projeto

- Validar **fluxos felizes (happy path)** da API
- Validar **cenários negativos** (erros de negócio)
- Garantir a **estrutura dos contratos de resposta**
- Manter testes **independentes, legíveis e confiáveis**
- Facilitar futura integração com **relatórios automatizados**

---

## ✅ Funcionalidades Testadas

### 1️⃣ Bookshelf – Inserção de Livros

- 1.1. Inserir um livro com ISBN válido
- 1.2. Inserir múltiplos livros na coleção
- 1.3. Tentar inserir um ISBN duplicado
- 1.4. Tentar inserir um ISBN inválido

---

### 2️⃣ Catálogo de Livros

- 2.1. Buscar lista completa de livros
- 2.2. Garantir que o catálogo não esteja vazio
- 2.3. Validar a **estrutura de cada livro retornado**
  - ISBN
  - Title
  - Author
  - Publisher
  - Pages
  - Publish Date
  - Description
  - Website

---

### 3️⃣ Consulta por ISBN

- 3.1. Buscar livro por ISBN válido
- 3.2. Buscar livro por ISBN inválido (Not Found)

---

## 📂 Estrutura do Projeto

```text
Bookstore.Api.Automation/
│
├── Clients/                  # Clients responsáveis por chamadas HTTP
│   ├── BookshelfClient.cs
│   └── CatalogClient.cs
│
├── Fixture/                  # Fixtures compartilhadas (ex: autenticação)
│   └── AuthFixture.cs
│
├── Models/                   # Modelos de request e response
│   ├── Bookshelf/
│   │   ├── AddBookRequest.cs
│   │   ├── AddBookResponse.cs
│   │   └── ErrorResponse.cs
│   └── Catalog/
│       └── BookListResponse.cs
│
├── Tests/			# Arquivos responsáveis pela implementação dos testes
│   ├── Bookshelf/
│   │   └── AddBookTests.cs
│   ├── Catalog/
│   │   ├── GetAllBooksTests.cs
│   │   └── GetBookByIsbnTests.cs
│   └── Builders/
│       └── AddBookRequestBuilder.cs
│
├── Utils/                    # Utilitários compartilhados
│
├── .env                      # Variáveis de ambiente (não versionado)
├── README.md
└── Bookstore.Api.Automation.csproj
```
## ⚙️ Pré-requisitos

Para executar este projeto localmente, é necessário:

- **.NET SDK 8.0 ou superior**
- **Acesso à [Bookstore API](https://bookstore.toolsqa.com/swagger/)**
- **Usuário criado na API através do método POST /Account/v1/User**
- **Armazenar userName, password e userId previamente criados na API**

---

## 🔐 Variáveis de Ambiente

O projeto utiliza variáveis de ambiente para evitar dados sensíveis hardcoded no código.

Crie um arquivo `.env` na raiz do projeto com o seguinte conteúdo:

```env
BASE_URL_ACCOUNT=https://bookstore.toolsqa.com/Account/v1
BASE_URL_BOOKSTORE=https://bookstore.toolsqa.com/BookStore/v1

BOOKSTORE_USERNAME=your_username_here
BOOKSTORE_PASSWORD=your_password_here
BOOKSTORE_USER_ID=your_user_id_here
```
⚠️ **O arquivo `.env` não deve ser versionado.**

## ▶️ Execução dos Testes

### Executar todos os testes

```bash
dotnet test
```

### Executar um conjunto específico de testes

Para executar apenas um conjunto específico de testes, utilize o filtro por nome totalmente qualificado:

```bash
dotnet test --filter "FullyQualifiedName~AddBookTests"
```
Exemplos para outros módulos de teste:

```bash
dotnet test --filter "FullyQualifiedName~GetAllBooksTests"
```
```bash
dotnet test --filter "FullyQualifiedName~GetBookByIsbnTests"
```
## 🧪 Logs de Execução

Os testes utilizam **logs padronizados no console** para facilitar a leitura, entendimento do fluxo e análise de falhas.

### Padrão de Logs

- `[SETUP]` → Preparação do estado do teste
- `[STEP]` → Ação executada
- `[ASSERT]` → Validações realizadas
- `[INFO]` → Informações adicionais

### Exemplo de Saída no Console

```text
[SETUP] Clearing user's bookshelf before test execution
---------------------------------------------------
[STEP] Building request body
[STEP] Calling POST /Book endpoint
[ASSERT] Expected Status: Created
[ASSERT] Actual Status: Created
[STEP] Deserializing response
[ASSERT] Checking response object is not null
[ASSERT] Checking ISBN in response
[ASSERT] Expected ISBN: 9781449325862
[ASSERT] Actual ISBN: 9781449325862
[INFO] Test finished successfully
---------------------------------------------------
```
## 🚀 Próximos Passos

Possíveis melhorias e evoluções futuras:

- Integração com **Allure Report** para geração de relatórios avançados
- Execução automática dos testes em pipelines de **CI/CD**
- Introdução de uma camada de **Service** para melhor separação de responsabilidades
- Expansão das validações de contrato da API
- Separação entre testes funcionais e testes de contrato

---

## 👤 Autor

**Vinicius Rafael**  
QA Analyst / Test Automation Engineer
