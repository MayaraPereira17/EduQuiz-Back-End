# Teste do Sistema de Ranking

## Como o Ranking Funciona

### 1. **Estrutura do Ranking**
- **Tabela:** `RankingAlunos`
- **Campos principais:**
  - `UsuarioId` - ID do aluno
  - `CategoriaId` - ID da categoria do quiz
  - `PontuacaoTotal` - Soma de todos os pontos obtidos
  - `MediaPontuacao` - Média de acerto em porcentagem
  - `TotalQuizzes` - Quantidade de quizzes completados
  - `PontosExperiencia` - Pontos de experiência (igual à pontuação total)
  - `PosicaoRanking` - Posição no ranking (calculada dinamicamente)

### 2. **Como é Atualizado**
- **Quando:** Toda vez que um aluno completa um quiz
- **Método:** `AtualizarRankingAsync(usuarioId, categoriaId)`
- **Processo:**
  1. Busca ranking existente para o usuário/categoria
  2. Se não existir, cria novo registro
  3. Calcula estatísticas baseadas nas tentativas concluídas
  4. Atualiza o ranking
  5. Recalcula posições de todos os usuários na categoria

### 3. **Critérios de Ordenação**
1. **PontuacaoTotal** (decrescente) - Maior pontuação primeiro
2. **MediaPontuacao** (decrescente) - Maior média de acerto primeiro

### 4. **Endpoints Disponíveis**
- `GET /api/aluno/ranking` - Ranking completo
- `GET /api/aluno/ranking?busca=nome` - Ranking com busca por nome

### 5. **Dados Retornados**
```json
{
  "rankings": [
    {
      "posicao": 1,
      "usuarioId": 123,
      "nomeCompleto": "João Silva",
      "avatar": "",
      "pontos": 150,
      "quizzes": 5,
      "media": 85.5,
      "sequencia": 0
    }
  ],
  "meuRanking": {
    "posicao": 2,
    "usuarioId": 456,
    "nomeCompleto": "Maria Santos",
    "avatar": "",
    "pontos": 120,
    "quizzes": 3,
    "media": 80.0,
    "sequencia": 0
  }
}
```

## Teste Manual

### 1. **Criar Quiz e Responder**
1. Professor cria um quiz
2. Aluno responde o quiz
3. Sistema atualiza ranking automaticamente

### 2. **Verificar Ranking**
1. Acessar `GET /api/aluno/ranking`
2. Verificar se o aluno aparece na lista
3. Verificar se a posição está correta

### 3. **Testar Múltiplos Alunos**
1. Vários alunos respondem quizzes
2. Verificar se o ranking está ordenado corretamente
3. Verificar se as posições são recalculadas

## Possíveis Problemas

### 1. **Erro de Entity Framework (RESOLVIDO)**
- **Problema:** ID temporário ao tentar fazer Update
- **Solução:** Salvar entidade primeiro, depois atualizar

### 2. **Ranking Vazio**
- **Causa:** Nenhum aluno completou quizzes
- **Solução:** Completar pelo menos um quiz

### 3. **Posições Incorretas**
- **Causa:** RecalcularPosicoesRankingAsync não está sendo chamado
- **Solução:** Verificar se o método está sendo executado

### 4. **Dados Inconsistentes**
- **Causa:** TentativasQuiz não estão sendo salvas corretamente
- **Solução:** Verificar se Concluida = true e Pontuacao está preenchida

