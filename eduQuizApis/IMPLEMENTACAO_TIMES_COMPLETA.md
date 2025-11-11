# ✅ Implementação Completa: Gerenciamento de Times pelo Técnico

## 📋 Resumo

Implementados **5 endpoints** para permitir que o técnico gerencie times de futebol, escalando jogadores, criando múltiplos times, excluindo times e visualizando informações dos times.

---

## 🎯 Endpoints Implementados

### 1. **GET /api/tecnico/times** - Listar Times
**Descrição:** Lista todos os times criados pelo técnico.

**Response (200 OK):**
```json
{
  "times": [
    {
      "id": 1,
      "nome": "Time Principal",
      "dataCriacao": "2024-01-15T10:30:00Z",
      "jogadores": [
        {
          "id": 1,
          "alunoId": 5,
          "nome": "João Silva",
          "email": "joao@email.com",
          "posicao": 1,
          "scoreGeral": 92.0
        }
      ]
    }
  ],
  "totalTimes": 1
}
```

---

### 2. **POST /api/tecnico/times** - Criar Time
**Descrição:** Cria um novo time com os jogadores especificados.

**Request Body:**
```json
{
  "nome": "Time Principal",
  "jogadoresIds": [5, 8, 12, 15, 20, 22, 25, 30, 35, 40, 45]
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "nome": "Time Principal",
  "dataCriacao": "2024-01-15T10:30:00Z",
  "jogadores": [...]
}
```

**Validações:**
- ✅ Nome do time é obrigatório
- ✅ Deve ter pelo menos 1 jogador
- ✅ Alunos devem existir e estar ativos
- ✅ Evita duplicatas de jogadores no mesmo time

---

### 3. **POST /api/tecnico/times/{timeId}/jogadores** - Adicionar Jogador ao Time
**Descrição:** Adiciona um jogador a um time existente.

**Request Body:**
```json
{
  "alunoId": 10
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "nome": "Time Principal",
  "dataCriacao": "2024-01-15T10:30:00Z",
  "jogadores": [...]
}
```

**Validações:**
- ✅ Time deve existir e pertence ao técnico
- ✅ Aluno deve existir e estar ativo
- ✅ Aluno não deve estar no time

---

### 4. **DELETE /api/tecnico/times/{timeId}/jogadores/{jogadorId}** - Remover Jogador do Time
**Descrição:** Remove um jogador de um time.

**Response (200 OK):**
```json
{
  "message": "Jogador removido do time com sucesso"
}
```

**Validações:**
- ✅ Time deve existir e pertence ao técnico
- ✅ Jogador deve existir no time

---

### 5. **DELETE /api/tecnico/times/{timeId}** - Deletar Time
**Descrição:** Deleta um time (soft delete).

**Response (200 OK):**
```json
{
  "message": "Time excluído com sucesso",
  "timeId": 1
}
```

**Validações:**
- ✅ Time deve existir e pertence ao técnico
- ✅ Soft delete (marca como inativo)

---

## 📁 Arquivos Criados/Modificados

### **1. Entidades** (`Domain/Entities/`)
- ✅ **`Time.cs`** - Entidade que representa um time de futebol
- ✅ **`JogadorTime.cs`** - Entidade que representa a relação entre time e jogador

### **2. DbContext** (`Infrastructure/Data/EduQuizContext.cs`)
- ✅ Adicionado `DbSet<Time> Times`
- ✅ Adicionado `DbSet<JogadorTime> JogadoresTime`
- ✅ Configurados relacionamentos e constraints
- ✅ Configurado soft delete para times

### **3. DTOs** (`Application/DTOs/TecnicoFutebolDTOs.cs`)
- ✅ `GerenciarTimesDTO` - Lista de times
- ✅ `TimeDTO` - Dados do time
- ✅ `JogadorTimeDTO` - Dados do jogador no time
- ✅ `CriarTimeRequestDTO` - Request para criar time
- ✅ `AdicionarJogadorRequestDTO` - Request para adicionar jogador
- ✅ `RemoverJogadorResponseDTO` - Response para remover jogador
- ✅ `DeletarTimeResponseDTO` - Response para deletar time

### **4. Interface** (`Application/Interfaces/ITecnicoFutebolService.cs`)
- ✅ `ObterTimesAsync` - Listar times
- ✅ `CriarTimeAsync` - Criar time
- ✅ `AdicionarJogadorAoTimeAsync` - Adicionar jogador
- ✅ `RemoverJogadorDoTimeAsync` - Remover jogador
- ✅ `DeletarTimeAsync` - Deletar time

### **5. Service** (`Application/Services/TecnicoFutebolService.cs`)
- ✅ Implementado `ObterTimesAsync` - Lista times com jogadores e scores
- ✅ Implementado `CriarTimeAsync` - Cria time com validações
- ✅ Implementado `AdicionarJogadorAoTimeAsync` - Adiciona jogador
- ✅ Implementado `RemoverJogadorDoTimeAsync` - Remove jogador
- ✅ Implementado `DeletarTimeAsync` - Soft delete
- ✅ Método auxiliar `ObterTimePorIdAsync` - Otimizado para performance
- ✅ **Otimização:** Cálculo de scores e posições do ranking em batch

### **6. Controller** (`Presentation/Web/Controllers/TecnicoFutebolController.cs`)
- ✅ `GET /api/tecnico/times` - Listar times
- ✅ `POST /api/tecnico/times` - Criar time
- ✅ `POST /api/tecnico/times/{timeId}/jogadores` - Adicionar jogador
- ✅ `DELETE /api/tecnico/times/{timeId}/jogadores/{jogadorId}` - Remover jogador
- ✅ `DELETE /api/tecnico/times/{timeId}` - Deletar time
- ✅ Tratamento de erros completo
- ✅ Autorização `TecnicoFutebolOnly`

### **7. Script SQL** (`migration_times.sql`)
- ✅ Script para criar tabelas `Times` e `JogadoresTime`
- ✅ Configurado para MySQL
- ✅ Índices para performance
- ✅ Foreign keys e constraints

---

## 🗃️ Estrutura do Banco de Dados

### **Tabela: Times**
```sql
CREATE TABLE IF NOT EXISTS Times (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    TecnicoId INT NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY (TecnicoId) REFERENCES Usuarios(Id) ON DELETE RESTRICT
);
```

### **Tabela: JogadoresTime**
```sql
CREATE TABLE IF NOT EXISTS JogadoresTime (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TimeId INT NOT NULL,
    AlunoId INT NOT NULL,
    DataEscalacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TimeId) REFERENCES Times(Id) ON DELETE CASCADE,
    FOREIGN KEY (AlunoId) REFERENCES Usuarios(Id) ON DELETE CASCADE
);
```

**Nota:** Um aluno pode estar em múltiplos times simultaneamente. Se quiser permitir apenas um time por aluno, adicione um índice único na coluna `AlunoId`.

---

## 🔧 Funcionalidades Implementadas

### **Listar Times**
1. **Busca:** Retorna todos os times do técnico
2. **Ordenação:** Ordenado por data de criação (mais recente primeiro)
3. **Jogadores:** Inclui lista de jogadores com:
   - Nome e email
   - Posição no ranking
   - Score geral
4. **Otimização:** Calcula scores e posições em batch (não individualmente)

### **Criar Time**
1. **Validações:**
   - Nome do time é obrigatório
   - Deve ter pelo menos 1 jogador
   - Alunos devem existir e estar ativos
   - Evita duplicatas
2. **Criação:** Cria time e adiciona jogadores
3. **Retorno:** Retorna time criado com todos os dados

### **Adicionar Jogador**
1. **Validações:**
   - Time deve existir e pertence ao técnico
   - Aluno deve existir e estar ativo
   - Aluno não deve estar no time
2. **Adição:** Adiciona jogador ao time
3. **Retorno:** Retorna time atualizado

### **Remover Jogador**
1. **Validações:**
   - Time deve existir e pertence ao técnico
   - Jogador deve existir no time
2. **Remoção:** Remove jogador do time
3. **Retorno:** Mensagem de sucesso

### **Deletar Time**
1. **Validações:**
   - Time deve existir e pertence ao técnico
2. **Soft Delete:** Marca time como inativo
3. **Preservação:** Mantém dados no banco para histórico
4. **Retorno:** Mensagem de sucesso

---

## 🔐 Segurança

- ✅ **Autorização:** Apenas técnicos podem acessar (`TecnicoFutebolOnly`)
- ✅ **Validação:** Validações de permissão em cada método
- ✅ **Isolamento:** Técnico só pode gerenciar seus próprios times
- ✅ **Autenticação:** Requer token JWT válido
- ✅ **Auditoria:** Logs de ações importantes

---

## ⚡ Otimizações de Performance

### **Cálculo de Scores e Posições**
- ✅ **Antes:** Calculava score e posição para cada jogador individualmente (N queries)
- ✅ **Agora:** Calcula scores de todos os alunos de uma vez (2 queries)
- ✅ **Melhoria:** Reduz drasticamente o número de queries ao banco

### **Uso de Dicionários**
- ✅ Usa dicionários para lookup rápido de scores e posições
- ✅ Evita múltiplas iterações sobre a lista de alunos

---

## 📊 Estrutura de Dados

### **TimeDTO**
```csharp
public class TimeDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<JogadorTimeDTO> Jogadores { get; set; }
}
```

### **JogadorTimeDTO**
```csharp
public class JogadorTimeDTO
{
    public int Id { get; set; }           // ID do registro JogadorTime
    public int AlunoId { get; set; }      // ID do aluno
    public string Nome { get; set; }      // Nome do aluno
    public string Email { get; set; }     // Email do aluno
    public int Posicao { get; set; }      // Posição no ranking
    public decimal ScoreGeral { get; set; } // Score geral do aluno
}
```

### **CriarTimeRequestDTO**
```csharp
public class CriarTimeRequestDTO
{
    public string Nome { get; set; }
    public List<int> JogadoresIds { get; set; }
}
```

---

## 🧪 Testes

### **Teste 1: Criar Time**
```http
POST /api/tecnico/times
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Time Titular",
  "jogadoresIds": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11]
}
```

### **Teste 2: Listar Times**
```http
GET /api/tecnico/times
Authorization: Bearer {token}
```

### **Teste 3: Adicionar Jogador**
```http
POST /api/tecnico/times/1/jogadores
Authorization: Bearer {token}
Content-Type: application/json

{
  "alunoId": 12
}
```

### **Teste 4: Remover Jogador**
```http
DELETE /api/tecnico/times/1/jogadores/5
Authorization: Bearer {token}
```

### **Teste 5: Deletar Time**
```http
DELETE /api/tecnico/times/1
Authorization: Bearer {token}
```

---

## ⚠️ Observações Importantes

### **1. Múltiplos Times por Aluno**
- ✅ **Implementado:** Um aluno pode estar em múltiplos times simultaneamente
- ✅ **Razão:** Flexibilidade para o técnico criar diferentes formações
- ✅ **Limitação:** Se quiser permitir apenas um time por aluno, adicione índice único

### **2. Soft Delete**
- ✅ Times são marcados como inativos, não deletados
- ✅ Dados são preservados para histórico
- ✅ Jogadores não são removidos automaticamente

### **3. Validações**
- ✅ Validações de permissão em cada método
- ✅ Validações de dados de entrada
- ✅ Validações de existência de entidades
- ✅ Validações de estado (ativo/inativo)

### **4. Performance**
- ✅ Otimizado para reduzir queries ao banco
- ✅ Usa eager loading para carregar relacionamentos
- ✅ Calcula scores em batch

---

## 📝 Próximos Passos (Opcional)

### **1. Notificações**
- [ ] Enviar notificação quando aluno é escalado
- [ ] Enviar email quando aluno é escalado
- [ ] Notificar quando time é excluído

### **2. Exportar Relatório**
- [ ] Implementar exportação de relatório em PDF
- [ ] Implementar exportação de relatório em Excel
- [ ] Filtro por quantidade de alunos

### **3. Melhorias**
- [ ] Adicionar limite de jogadores por time
- [ ] Adicionar validação de duplicatas de nomes de times
- [ ] Adicionar histórico de mudanças no time

---

## ✅ Checklist de Implementação

- [x] Criar entidades Time e JogadorTime
- [x] Adicionar DbSets ao EduQuizContext
- [x] Configurar relacionamentos no DbContext
- [x] Criar DTOs para gerenciamento de times
- [x] Atualizar interface ITecnicoFutebolService
- [x] Implementar métodos no TecnicoFutebolService
- [x] Adicionar endpoints no TecnicoFutebolController
- [x] Adicionar validações de permissão
- [x] Adicionar validações de dados de entrada
- [x] Implementar soft delete
- [x] Otimizar cálculos de scores e posições
- [x] Adicionar logs de auditoria
- [x] Criar script SQL para migração
- [x] Testar compilação
- [x] Documentar implementação

---

## 🚀 Status

**✅ Implementação Completa**

Todos os 5 endpoints foram implementados conforme a especificação e estão prontos para uso.

---

**Data de Implementação:** Janeiro 2024  
**Versão:** 1.0  
**Status:** ✅ Completo

