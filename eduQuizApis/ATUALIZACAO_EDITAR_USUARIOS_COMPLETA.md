# ✅ Atualização: Edição Completa de Dados de Usuários pelo Técnico

## 📋 Resumo

Atualizados os endpoints de edição de alunos e professores para permitir que o técnico edite **todos os campos editáveis** dos usuários, **exceto a senha**.

---

## 🎯 Campos Editáveis

### **Campos que podem ser editados:**
- ✅ **Username** - Nome de usuário único
- ✅ **Nome** - Primeiro nome e sobrenome (FirstName e LastName)
- ✅ **Email** - Email do usuário
- ✅ **CPF** - CPF do usuário (opcional)
- ✅ **DataNascimento** - Data de nascimento
- ✅ **Idade** - Idade (apenas para alunos, converte para DataNascimento)
- ✅ **AvatarUrl** - URL do avatar/foto do usuário (opcional)
- ✅ **Instituicao** - Instituição (apenas para professores, não salva no banco)
- ✅ **AreaEspecializacao** - Área de especialização (apenas para professores, não salva no banco)

### **Campos que NÃO podem ser editados:**
- ❌ **Senha (PasswordHash)** - A senha não pode ser editada pelo técnico
- ❌ **Role** - O papel do usuário não pode ser alterado
- ❌ **Id** - ID do usuário não pode ser alterado
- ❌ **CreatedAt** - Data de criação não pode ser alterada
- ❌ **IsActive** - Status ativo/inativo é gerenciado pelo soft delete

---

## 📝 Mudanças nos DTOs

### **AtualizarAlunoRequestDTO**
```csharp
public class AtualizarAlunoRequestDTO
{
    public string? Username { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? CPF { get; set; }
    public int? Idade { get; set; }              // Converte para DataNascimento
    public DateTime? DataNascimento { get; set; }
    public string? AvatarUrl { get; set; }
}
```

### **AtualizarProfessorRequestDTO**
```csharp
public class AtualizarProfessorRequestDTO
{
    public string? Username { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? CPF { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Instituicao { get; set; }        // Não salva no banco
    public string? AreaEspecializacao { get; set; } // Não salva no banco
}
```

### **AlunoRankingDTO** (Response)
```csharp
public class AlunoRankingDTO
{
    public int Id { get; set; }
    public int Posicao { get; set; }
    public string Username { get; set; }           // ✅ Novo
    public string Nome { get; set; }
    public string Email { get; set; }
    public string? CPF { get; set; }               // ✅ Novo
    public int Idade { get; set; }
    public DateTime? DataNascimento { get; set; }  // ✅ Novo
    public string? AvatarUrl { get; set; }         // ✅ Novo
    public int TotalQuizzes { get; set; }
    public decimal ScoreGeral { get; set; }
    public DateTime? UltimoQuiz { get; set; }
}
```

### **ProfessorDTO** (Response)
```csharp
public class ProfessorDTO
{
    public int Id { get; set; }
    public string Username { get; set; }           // ✅ Novo
    public string Nome { get; set; }
    public string Email { get; set; }
    public string? CPF { get; set; }               // ✅ Novo
    public DateTime? DataNascimento { get; set; }  // ✅ Novo
    public string? AvatarUrl { get; set; }         // ✅ Novo
    public string? Instituicao { get; set; }
    public string? AreaEspecializacao { get; set; }
    public int TotalQuizzes { get; set; }
    public DateTime DataCadastro { get; set; }
}
```

---

## 🔧 Validações Implementadas

### **Username**
- ✅ Máximo de 50 caracteres
- ✅ Validação de unicidade (não pode estar em uso por outro usuário)
- ✅ Trim automático de espaços

### **Email**
- ✅ Validação de formato usando regex
- ✅ Máximo de 100 caracteres
- ✅ Validação de unicidade (não pode estar em uso por outro usuário)
- ✅ Trim automático de espaços

### **Nome**
- ✅ Não pode estar vazio
- ✅ Divide em FirstName e LastName automaticamente
- ✅ Trim automático de espaços

### **CPF**
- ✅ Máximo de 14 caracteres
- ✅ Opcional (pode ser null ou string vazia para remover)
- ✅ Trim automático de espaços

### **DataNascimento**
- ✅ Aceita DateTime direto
- ✅ Para alunos: também aceita Idade (converte para DataNascimento)
- ✅ Opcional

### **Idade** (apenas para alunos)
- ✅ Deve ser um número positivo
- ✅ Converte para DataNascimento automaticamente
- ✅ Opcional

### **AvatarUrl**
- ✅ Máximo de 500 caracteres
- ✅ Opcional (pode ser null ou string vazia para remover)
- ✅ Trim automático de espaços

---

## 📊 Exemplos de Uso

### **Atualizar Aluno - Exemplo 1: Todos os campos**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "joao_silva",
  "nome": "João Silva",
  "email": "joao.silva@email.com",
  "cpf": "123.456.789-00",
  "dataNascimento": "2008-05-15T00:00:00Z",
  "avatarUrl": "https://example.com/avatar.jpg"
}
```

### **Atualizar Aluno - Exemplo 2: Apenas alguns campos**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "novo.email@email.com",
  "idade": 16
}
```

### **Atualizar Aluno - Exemplo 3: Remover CPF e Avatar**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "cpf": "",
  "avatarUrl": ""
}
```

### **Atualizar Professor - Exemplo 1: Todos os campos**
```http
PUT /api/tecnico/professores/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "maria_santos",
  "nome": "Maria Santos",
  "email": "maria.santos@email.com",
  "cpf": "987.654.321-00",
  "dataNascimento": "1985-03-20T00:00:00Z",
  "avatarUrl": "https://example.com/avatar.jpg",
  "instituicao": "Escola XYZ",
  "areaEspecializacao": "Matemática"
}
```

### **Atualizar Professor - Exemplo 2: Apenas email**
```http
PUT /api/tecnico/professores/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "novo.email@email.com"
}
```

---

## 🔍 Busca Atualizada

A busca agora também inclui o **username**:

### **Buscar Alunos**
```http
GET /api/tecnico/alunos?busca=joao
Authorization: Bearer {token}
```

**Busca em:**
- ✅ FirstName
- ✅ LastName
- ✅ Email
- ✅ **Username** (novo)

### **Buscar Professores**
```http
GET /api/tecnico/professores?busca=maria
Authorization: Bearer {token}
```

**Busca em:**
- ✅ FirstName
- ✅ LastName
- ✅ Email
- ✅ **Username** (novo)

---

## 📁 Arquivos Modificados

### **1. DTOs** (`Application/DTOs/TecnicoFutebolDTOs.cs`)
- ✅ Adicionado `Username` em `AtualizarAlunoRequestDTO`
- ✅ Adicionado `CPF` em `AtualizarAlunoRequestDTO`
- ✅ Adicionado `DataNascimento` em `AtualizarAlunoRequestDTO`
- ✅ Adicionado `AvatarUrl` em `AtualizarAlunoRequestDTO`
- ✅ Adicionado `Username` em `AtualizarProfessorRequestDTO`
- ✅ Adicionado `CPF` em `AtualizarProfessorRequestDTO`
- ✅ Adicionado `DataNascimento` em `AtualizarProfessorRequestDTO`
- ✅ Adicionado `AvatarUrl` em `AtualizarProfessorRequestDTO`
- ✅ Adicionado campos em `AlunoRankingDTO` (Username, CPF, DataNascimento, AvatarUrl)
- ✅ Adicionado campos em `ProfessorDTO` (Username, CPF, DataNascimento, AvatarUrl)

### **2. Service** (`Application/Services/TecnicoFutebolService.cs`)
- ✅ Implementada validação e atualização de `Username`
- ✅ Implementada validação e atualização de `CPF`
- ✅ Implementada validação e atualização de `DataNascimento`
- ✅ Implementada validação e atualização de `AvatarUrl`
- ✅ Atualizado método `AtualizarAlunoAsync` para incluir todos os campos
- ✅ Atualizado método `AtualizarProfessorAsync` para incluir todos os campos
- ✅ Atualizado método `ObterAlunosAsync` para retornar todos os campos
- ✅ Atualizado método `ObterProfessoresAsync` para retornar todos os campos
- ✅ Atualizada busca para incluir `Username`

---

## 🔐 Segurança

- ✅ **Autorização:** Apenas técnicos podem editar (`TecnicoFutebolOnly`)
- ✅ **Validação de Unicidade:** Username e Email são únicos no sistema
- ✅ **Validação de Formato:** Email e outros campos são validados
- ✅ **Senha Protegida:** Senha não pode ser editada pelo técnico
- ✅ **Role Protegido:** Papel do usuário não pode ser alterado

---

## ⚠️ Observações Importantes

### **1. Senha Não Editável**
- ❌ A senha **não pode** ser editada pelo técnico
- ✅ Se necessário, o usuário deve alterar a senha através do endpoint de recuperação de senha

### **2. Campos Opcionais**
- ✅ Todos os campos são opcionais (exceto validações quando fornecidos)
- ✅ Campos podem ser enviados como `null` ou omitidos
- ✅ CPF e AvatarUrl podem ser enviados como string vazia (`""`) para remover

### **3. Compatibilidade com Idade**
- ✅ Para alunos, ainda é possível usar `idade` em vez de `dataNascimento`
- ✅ O sistema converte automaticamente `idade` para `dataNascimento`
- ✅ Se ambos forem fornecidos, `dataNascimento` tem prioridade

### **4. Instituição e Área de Especialização (Professores)**
- ⚠️ Esses campos **não existem** no banco de dados
- ✅ São aceitos no request e retornados no response
- ❌ **Não são salvos no banco de dados**
- 📝 Se no futuro forem adicionados ao banco, devem ser implementados

### **5. Validação de Unicidade**
- ✅ Username e Email são únicos no sistema
- ✅ Ao atualizar, o próprio usuário é excluído da verificação
- ✅ Se o username/email já está em uso por outro usuário, retorna erro

---

## 🧪 Testes

### **Teste 1: Atualizar todos os campos de um aluno**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "joao_silva",
  "nome": "João Silva",
  "email": "joao.silva@email.com",
  "cpf": "123.456.789-00",
  "dataNascimento": "2008-05-15T00:00:00Z",
  "avatarUrl": "https://example.com/avatar.jpg"
}
```

### **Teste 2: Atualizar apenas email**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "novo.email@email.com"
}
```

### **Teste 3: Remover CPF e Avatar**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "cpf": "",
  "avatarUrl": ""
}
```

### **Teste 4: Username duplicado (deve falhar)**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "usuario_existente"
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Username já está em uso por outro usuário"
}
```

### **Teste 5: Email duplicado (deve falhar)**
```http
PUT /api/tecnico/alunos/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "email.existente@email.com"
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Email já está em uso por outro usuário"
}
```

---

## 📊 Códigos de Erro

| Código | Descrição |
|--------|-----------|
| `200` | Sucesso |
| `400` | Dados inválidos (email inválido, username duplicado, etc.) |
| `401` | Não autenticado |
| `403` | Sem permissão (não é técnico) |
| `404` | Usuário não encontrado |
| `500` | Erro interno do servidor |

---

## ✅ Checklist de Implementação

- [x] Adicionar `Username` aos DTOs de request
- [x] Adicionar `CPF` aos DTOs de request
- [x] Adicionar `DataNascimento` aos DTOs de request
- [x] Adicionar `AvatarUrl` aos DTOs de request
- [x] Adicionar campos aos DTOs de response
- [x] Implementar validação de `Username`
- [x] Implementar validação de `CPF`
- [x] Implementar validação de `DataNascimento`
- [x] Implementar validação de `AvatarUrl`
- [x] Atualizar método `AtualizarAlunoAsync`
- [x] Atualizar método `AtualizarProfessorAsync`
- [x] Atualizar método `ObterAlunosAsync`
- [x] Atualizar método `ObterProfessoresAsync`
- [x] Atualizar busca para incluir `Username`
- [x] Testar validações
- [x] Testar unicidade de username e email
- [x] Testar remoção de CPF e AvatarUrl

---

## 🚀 Status

**✅ Implementação Completa**

Todos os campos editáveis foram adicionados e estão funcionando corretamente. O técnico agora pode editar qualquer campo dos usuários, exceto a senha.

---

**Data de Implementação:** Janeiro 2024  
**Versão:** 2.0  
**Status:** ✅ Completo

