# EduQuiz API

API REST para sistema de quiz educacional desenvolvida em .NET 8 com Entity Framework Core e MySQL.

## 🚀 Funcionalidades

- **Autenticação JWT**: Sistema completo de login e registro de usuários
- **Gestão de Categorias**: Criação e gerenciamento de categorias de quiz
- **Gestão de Quizzes**: Criação, edição e exclusão de quizzes
- **Sistema de Questões**: Suporte a múltipla escolha, verdadeiro/falso, preenchimento e dissertativa
- **Tentativas de Quiz**: Sistema completo de tentativas com controle de tempo e tentativas máximas
- **Relatórios de Performance**: Análise detalhada de desempenho dos usuários
- **Controle de Acesso**: Diferentes níveis de permissão (Student, Teacher, Admin)

## 🛠️ Tecnologias Utilizadas

- **.NET 8**
- **Entity Framework Core 8.0**
- **MySQL** (Railway)
- **JWT Authentication**
- **Swagger/OpenAPI**
- **BCrypt** para hash de senhas

## 📋 Pré-requisitos

- .NET 8 SDK
- MySQL (já configurado no Railway)
- Visual Studio 2022 ou VS Code

## 🔧 Configuração

### 1. Clone o repositório
```bash
git clone <url-do-repositorio>
cd EduQuiz-Back-End/eduQuizApis
```

### 2. Restaure as dependências
```bash
dotnet restore
```

### 3. Configure a string de conexão
A string de conexão já está configurada no `appsettings.json` para o banco MySQL do Railway.

### 4. Execute o banco de dados
Execute o script SQL localizado em `database_script.sql` no seu banco MySQL do Railway.

### 5. Execute a aplicação
```bash
dotnet run
```

A API estará disponível em `https://localhost:7000` e o Swagger em `https://localhost:7000/swagger`.

## 📚 Documentação da API

### Autenticação

#### POST /api/auth/register
Registra um novo usuário.

**Body:**
```json
{
  "username": "usuario123",
  "email": "usuario@email.com",
  "password": "senha123",
  "firstName": "João",
  "lastName": "Silva",
  "role": "Student"
}
```

#### POST /api/auth/login
Realiza login do usuário.

**Body:**
```json
{
  "email": "usuario@email.com",
  "password": "senha123"
}
```

### Categorias

#### GET /api/categories
Lista todas as categorias ativas.

#### POST /api/categories
Cria uma nova categoria (requer autenticação de Teacher ou Admin).

**Body:**
```json
{
  "name": "Matemática",
  "description": "Quiz de matemática básica e avançada"
}
```

### Quizzes

#### GET /api/quizzes
Lista todos os quizzes públicos ou do usuário.

#### POST /api/quizzes
Cria um novo quiz (requer autenticação).

**Body:**
```json
{
  "title": "Quiz de Matemática Básica",
  "description": "Questões de matemática para iniciantes",
  "categoryId": 1,
  "timeLimit": 30,
  "maxAttempts": 3,
  "isPublic": true
}
```

### Questões

#### GET /api/questions/quiz/{quizId}
Lista questões de um quiz específico.

#### POST /api/questions
Cria uma nova questão (requer autenticação).

**Body:**
```json
{
  "quizId": 1,
  "questionText": "Qual é a capital do Brasil?",
  "questionType": "MultipleChoice",
  "points": 1,
  "orderIndex": 1,
  "options": [
    {
      "optionText": "São Paulo",
      "isCorrect": false,
      "orderIndex": 1
    },
    {
      "optionText": "Brasília",
      "isCorrect": true,
      "orderIndex": 2
    }
  ]
}
```

### Tentativas de Quiz

#### POST /api/quizattempts/start
Inicia uma nova tentativa de quiz.

**Body:**
```json
{
  "quizId": 1
}
```

#### POST /api/quizattempts/answer
Submete uma resposta durante a tentativa.

**Body:**
```json
{
  "attemptId": 1,
  "questionId": 1,
  "selectedOptionId": 2
}
```

#### POST /api/quizattempts/submit
Finaliza uma tentativa de quiz.

**Body:**
```json
{
  "attemptId": 1
}
```

### Relatórios

#### GET /api/reports/my-performance
Obtém relatórios de performance do usuário logado.

#### GET /api/reports/my-stats
Obtém estatísticas gerais do usuário.

## 🔐 Autenticação

A API utiliza JWT (JSON Web Tokens) para autenticação. Para acessar endpoints protegidos:

1. Faça login via `/api/auth/login`
2. Use o token retornado no header `Authorization: Bearer {token}`

## 👥 Roles de Usuário

- **Student**: Pode fazer quizzes e ver seus próprios relatórios
- **Teacher**: Pode criar quizzes e ver relatórios dos seus quizzes
- **Admin**: Acesso total ao sistema

## 🗄️ Estrutura do Banco de Dados

O banco de dados inclui as seguintes tabelas:

- **Users**: Usuários do sistema
- **Categories**: Categorias de quiz
- **Quizzes**: Quizzes criados
- **Questions**: Questões dos quizzes
- **QuestionOptions**: Opções de resposta (múltipla escolha)
- **QuizAttempts**: Tentativas de quiz dos usuários
- **Answers**: Respostas dos usuários
- **PerformanceReports**: Relatórios de performance
- **SystemSettings**: Configurações do sistema

## 🚀 Deploy

### Railway

1. Conecte seu repositório ao Railway
2. Configure as variáveis de ambiente:
   - `ConnectionStrings__DefaultConnection`: String de conexão do MySQL
   - `JwtSettings__SecretKey`: Chave secreta para JWT
3. Deploy automático

### Docker

```bash
docker build -t eduquiz-api .
docker run -p 8080:8080 eduquiz-api
```

## 📝 Exemplos de Uso

### Fluxo Completo de Uso

1. **Registrar usuário**:
   ```bash
   POST /api/auth/register
   ```

2. **Fazer login**:
   ```bash
   POST /api/auth/login
   ```

3. **Listar categorias**:
   ```bash
   GET /api/categories
   ```

4. **Criar quiz** (Teacher/Admin):
   ```bash
   POST /api/quizzes
   ```

5. **Adicionar questões**:
   ```bash
   POST /api/questions
   ```

6. **Fazer quiz** (Student):
   ```bash
   POST /api/quizattempts/start
   POST /api/quizattempts/answer
   POST /api/quizattempts/submit
   ```

7. **Ver relatórios**:
   ```bash
   GET /api/reports/my-performance
   ```

## 🐛 Troubleshooting

### Erro de Conexão com Banco
- Verifique se a string de conexão está correta
- Confirme se o banco MySQL está acessível

### Erro de Autenticação
- Verifique se o token JWT está sendo enviado corretamente
- Confirme se o token não expirou

### Erro de Permissão
- Verifique se o usuário tem a role necessária para a operação
- Confirme se o usuário é o criador do recurso (para edição/exclusão)

## 📞 Suporte

Para dúvidas ou problemas, entre em contato através dos issues do repositório.

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.


