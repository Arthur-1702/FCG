# FIAP Cloud Games (FCG)

[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-8.0-green)](https://learn.microsoft.com/en-us/ef/core/)
[![MSSQL](https://img.shields.io/badge/MSSQL-2022-blue)](https://www.microsoft.com/pt-br/sql-server/sql-server-2022)
[![xUnit](https://img.shields.io/badge/xUnit-2.9.0-blue)](https://xunit.net/releases/v2/2.9.0)

## Descrição

Uma API REST para gerenciamento de uma loja de jogos digitais com autenticação e autorização por níveis de acesso.

## Requisitos Funcionais (Principais Funcionalidades)

### 👤 Cadastro de Usuários

- ✔️ Cadastro com nome, e-mail e senha
- ✔️ Validação de formato de e-mail
- ✔️ Validação de senha segura (mínimo 8 caracteres com números, letras e caracteres especiais)

### 🔒 Autenticação e Autorização

- 🔑 Autenticação via JWT
- 🎚️ Dois níveis de acesso:
  - **👤 Usuário**: Acesso à plataforma e biblioteca de jogos
  - **👑 Administrador**: Cadastro de jogos, administração de usuários e criação de promoções

### 🎮 Gerenciamento de Jogos

- 🛠️ CRUD completo de jogos (apenas para administradores)
- 🔍 Consulta de jogos disponíveis (para todos os usuários autenticados)

## 🛠️ Requisitos Técnicos e Ferramentas Utilizadas

- **API RESTful** desenvolvida em **.NET 8**
- **ORM**: Entity Framework Core para mapeamento de entidades (usuários e jogos)
- **Banco de Dados**: MSSQL Server com sistema de migrations para versionamento do schema
- **Padrão Arquitetural**: Controllers MVC (Model-View-Controller) para organização dos endpoints
- **Tratamento de Erros**: Middleware customizado para padronização de respostas
- **Sistema de Logs**: Armazenamento estruturado em banco de dados para auditoria
- **Monitoramento**: Logs centralizados com captura de stack traces e contextos
- **Documentação API**: Swagger UI com descrição detalhada dos endpoints
- **Testes Automatizados**: Suíte de testes unitários implementada com xUnit
- **Validações**: Testes de integração para fluxos críticos da aplicação

## Event Storming

[Diagrama com fluxo do cadastro de Usuário e Cadastro de Jogos no Miro](https://miro.com/app/board/uXjVI-yGAVU=/)

## 🚀 Como executar

### 🗃️ Configuração do Banco de Dados (Migrations)

#### 📌 Pré-requisitos

- SQL Server instalado e rodando localmente ou remotamente
- Acesso de administrador ao banco de dados

#### ⚙️ Configuração

1. Edite o arquivo `appsettings.json` com suas credenciais MSSQL (ConnectionStrings > DefaultConnection)
2. A Migration inicial também irá criar um usuário administrador inicial (SeedAdmin) (que também pode ser configurado appsettings)
3. Quando o projeto for executado, ele fará as migrações automaticamente, montando o banco de dados FiapCloudGames

### 📚 Documentação e Uso

- Ao ser executado, a documentação e os seus endpoints são expostos no navegador através do Swagger

# 🔄 CI/CD Pipeline

Implementamos pipelines automatizadas para garantir qualidade e entrega contínua:

# 🛠️ Continuous Integration (CI)

Trigger: Pull Requests para branch dev

## Ações:

✅ Build da solução .NET

✅ Execução de testes unitários com xUnit

✅ Validação de qualidade de código

✅ Geração de artefatos de build

🚀 Continuous Deployment (CD)
Trigger: Merge na branch main

## Ações:

🐋 Build da imagem Docker (multi-stage)

📦 Push para Azure Container Registry (ACR)

☁️ Deploy automático no Azure Container Apps (ACA)

🔄 Atualização do ambiente de produção sem downtime

🐋 Configuração Docker
A imagem Docker está otimizada para produção com suporte a Azure SQL:

📦 Estrutura da Imagem
Base: mcr.microsoft.com/dotnet/aspnet:8.0

Multi-stage build para otimização

Configurações:

Variáveis de ambiente para conexão com Azure SQL

Healthcheck para monitoramento

Configuração de portas (80/tcp)

# ▶️ Como executar localmente

bash

## Build da imagem

docker build -t fiapcloudgames-api .

## Execução com Azure SQL

docker run -p 8080:80 fiapcloudgames-api
