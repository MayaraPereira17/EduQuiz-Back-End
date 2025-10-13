# Rotas do Aluno - EduQuiz API

Este documento descreve todas as rotas específicas para o aluno no sistema EduQuiz.

## Autenticação

Todas as rotas do aluno exigem:
- Token JWT válido no header `Authorization: Bearer {token}`
- Função "Aluno" no token JWT

## Base URL

Todas as rotas seguem o padrão: `api/aluno/{endpoint}`

---

## 📊 Dashboard

### `GET /api/aluno/dashboard`
**O que faz:** Retorna os dados do dashboard do aluno (estatísticas gerais, quizzes recentes, etc.)

**Resposta:**
```json
{
  "quizzesCompletos": 24,
  "mediaGeral": 78.5,
  "posicaoRanking": 15,
  "sequencia": 7,
  "pontos": 1890,
  "totalUsuarios": 245,
  "quizzesRecentes": [
    {
      "quizId": 1,
      "titulo": "Matemática - Adição",
      "categoria": "Matemática",
      "percentualAcerto": 90.0,
      "dataConclusao": "2024-01-15T10:30:00Z"
    }
  ]
}
```

---

## 🎯 Quiz

### `GET /api/aluno/quizzes/disponiveis`
**O que faz:** Lista todos os quizzes disponíveis para o aluno

**Resposta:**
```json
[
  {
    "id": 1,
    "titulo": "Equações do 1º Grau",
    "descricao": "Teste seus conhecimentos sobre equações lineares",
    "categoria": "Matemática",
    "dificuldade": "Fácil",
    "tempoLimite": 20,
    "totalQuestoes": 10,
    "pontuacaoTotal": 10,
    "disponivel": true
  }
]
```

### `POST /api/aluno/quizzes/iniciar`
**O que faz:** Inicia uma nova tentativa de quiz para o aluno

**Corpo da requisição:**
```json
{
  "quizId": 1
}
```

**Resposta:**
```json
{
  "tentativaId": 123,
  "quizId": 1,
  "tituloQuiz": "Equações do 1º Grau",
  "questaoAtual": {
    "id": 1,
    "textoQuestao": "Resolva a equação: 2x + 5 = 13",
    "tipoQuestao": "MultiplaEscolha",
    "pontos": 1,
    "ordemIndice": 1,
    "opcoes": [
      {
        "id": 1,
        "textoOpcao": "x = 4",
        "ordemIndice": 1
      },
      {
        "id": 2,
        "textoOpcao": "x = 9",
        "ordemIndice": 2
      }
    ]
  },
  "progresso": {
    "questaoAtual": 1,
    "totalQuestoes": 10,
    "percentualCompleto": 0,
    "pontuacaoAtual": 0,
    "tempoGasto": 0
  }
}
```

### `POST /api/aluno/tentativas/{tentativaId}/responder`
**O que faz:** Responde a uma questão específica de um quiz em andamento

**Corpo da requisição:**
```json
{
  "questaoId": 1,
  "opcaoSelecionadaId": 1,
  "textoResposta": null
}
```

**Resposta:**
```json
{
  "respostaCorreta": true,
  "pontosGanhos": 1,
  "respostaCorretaTexto": "x = 4",
  "feedback": "Correta!",
  "proximaQuestao": {
    "id": 2,
    "textoQuestao": "Qual é o valor de y em 3y - 7 = 14?",
    "tipoQuestao": "MultiplaEscolha",
    "pontos": 1,
    "ordemIndice": 2,
    "opcoes": [...]
  },
  "quizConcluido": false,
  "resultadoFinal": null
}
```

### `GET /api/aluno/tentativas/{tentativaId}/progresso`
**O que faz:** Obtém o progresso atual de uma tentativa de quiz

**Resposta:**
```json
{
  "questaoAtual": 3,
  "totalQuestoes": 10,
  "percentualCompleto": 30.0,
  "pontuacaoAtual": 2,
  "tempoGasto": 180
}
```

### `POST /api/aluno/tentativas/{tentativaId}/finalizar`
**O que faz:** Finaliza uma tentativa de quiz e calcula o resultado final

**Resposta:**
```json
{
  "tentativaId": 123,
  "pontuacaoFinal": 8,
  "pontuacaoMaxima": 10,
  "percentualAcerto": 80.0,
  "tempoGasto": 900,
  "totalQuestoes": 10,
  "respostasCorretas": 8,
  "respostasErradas": 2,
  "dataConclusao": "2024-01-15T10:45:00Z"
}
```

---

## 🏆 Ranking

### `GET /api/aluno/ranking`
**O que faz:** Obtém o ranking completo de alunos com opção de busca por nome

**Parâmetros de query:**
- `busca` (opcional): Termo para filtrar por nome

**Resposta:**
```json
{
  "alunos": [
    {
      "posicao": 1,
      "usuarioId": 1,
      "nomeCompleto": "Lucas Ribeiro",
      "avatar": "",
      "pontos": 3000,
      "quizzes": 30,
      "media": 95.5,
      "sequencia": 15
    },
    {
      "posicao": 2,
      "usuarioId": 2,
      "nomeCompleto": "Rafael Viera",
      "avatar": "",
      "pontos": 2480,
      "quizzes": 26,
      "media": 85.5,
      "sequencia": 8
    }
  ],
  "totalAlunos": 245,
  "posicaoUsuarioLogado": 15
}
```

---

## 👤 Perfil

### `GET /api/aluno/perfil`
**O que faz:** Obtém os dados completos do perfil do aluno

**Resposta:**
```json
{
  "id": 1,
  "nome": "Lucas",
  "sobrenome": "Ribeiro",
  "nomeCompleto": "Lucas Ribeiro",
  "email": "lucas.ribeiro@gmail.com",
  "funcao": "Aluno",
  "cpf": "123.456.789-00",
  "dataNascimento": "2000-01-15T00:00:00Z",
  "dataCriacao": "2023-01-01T10:00:00Z",
  "estatisticas": {
    "quizzesCompletos": 24,
    "mediaGeral": 78.5,
    "sequencia": 7,
    "pontos": 1890
  }
}
```

### `PUT /api/aluno/perfil`
**O que faz:** Atualiza os dados do perfil do aluno

**Corpo da requisição:**
```json
{
  "nome": "Lucas",
  "sobrenome": "Ribeiro",
  "cpf": "123.456.789-00",
  "dataNascimento": "2000-01-15T00:00:00Z"
}
```

### `GET /api/aluno/perfil/desempenho`
**O que faz:** Obtém o desempenho do aluno em todos os quizzes realizados

**Resposta:**
```json
[
  {
    "quizId": 1,
    "tituloQuiz": "Matemática - Adição",
    "categoria": "Matemática",
    "percentualAcerto": 90.0,
    "pontuacao": 9,
    "pontuacaoMaxima": 10,
    "dataConclusao": "2024-01-15T10:30:00Z",
    "tempoGasto": 600
  }
]
```

### `GET /api/aluno/perfil/atividades-recentes`
**O que faz:** Obtém as atividades recentes do aluno (quizzes concluídos, iniciados, conquistas, etc.)

**Resposta:**
```json
[
  {
    "id": 123,
    "tipo": "QuizConcluido",
    "descricao": "Você concluiu o 'Matemática - Adição' com 9/10 pontos",
    "data": "2024-01-15T10:30:00Z",
    "icone": "check-circle",
    "cor": "green"
  },
  {
    "id": 124,
    "tipo": "QuizIniciado",
    "descricao": "Você iniciou o 'Matemática - Subtração'",
    "data": "2024-01-15T09:15:00Z",
    "icone": "play-circle",
    "cor": "blue"
  }
]
```

---

## 🔒 Segurança

### Regras de Autorização
- Todas as rotas exigem autenticação JWT
- Apenas usuários com função "Aluno" podem acessar estas rotas
- O `usuarioId` é extraído automaticamente do token JWT

### Regras de Negócio
- Todas as questões valem 1 ponto
- Um aluno só pode ter uma tentativa em andamento por quiz
- As tentativas são limitadas conforme configuração do quiz
- O ranking é calculado automaticamente após cada quiz concluído

---

## 📝 Códigos de Status HTTP

- `200 OK`: Operação realizada com sucesso
- `201 Created`: Recurso criado com sucesso
- `400 Bad Request`: Dados inválidos na requisição
- `401 Unauthorized`: Token JWT inválido ou ausente
- `403 Forbidden`: Usuário não tem permissão (não é aluno)
- `404 Not Found`: Recurso não encontrado
- `409 Conflict`: Conflito (ex: tentativa já existe)
- `500 Internal Server Error`: Erro interno do servidor

---

## 🔧 Exemplos de Uso

### Fluxo Completo de um Quiz

1. **Listar quizzes disponíveis:**
   ```
   GET /api/aluno/quizzes/disponiveis
   ```

2. **Iniciar um quiz:**
   ```
   POST /api/aluno/quizzes/iniciar
   {
     "quizId": 1
   }
   ```

3. **Responder questões (repetir para cada questão):**
   ```
   POST /api/aluno/tentativas/123/responder
   {
     "questaoId": 1,
     "opcaoSelecionadaId": 1
   }
   ```

4. **Finalizar quiz (quando todas as questões foram respondidas):**
   ```
   POST /api/aluno/tentativas/123/finalizar
   ```

### Verificar Progresso Durante o Quiz

```
GET /api/aluno/tentativas/123/progresso
```

### Visualizar Dashboard

```
GET /api/aluno/dashboard
```

### Ver Ranking

```
GET /api/aluno/ranking
GET /api/aluno/ranking?busca=Lucas
```

### Gerenciar Perfil

```
GET /api/aluno/perfil
PUT /api/aluno/perfil
GET /api/aluno/perfil/desempenho
GET /api/aluno/perfil/atividades-recentes
```
