# Rotas do Professor - EduQuiz API

Este documento descreve todas as rotas específicas para o professor no sistema EduQuiz.

## Autenticação

Todas as rotas do professor exigem:
- Token JWT válido no header `Authorization: Bearer {token}`
- Função "Professor" no token JWT

## Base URL

Todas as rotas seguem o padrão: `api/professor/{endpoint}`

---

## 📊 Dashboard

### `GET /api/professor/dashboard`
**O que faz:** Retorna os dados do dashboard do professor (estatísticas gerais, quizzes criados, etc.)

**Resposta:**
```json
{
  "quizzesCriados": 12,
  "mediaDosAlunos": 74.2,
  "totalAlunos": 45,
  "totalTentativas": 156,
  "quizzesRecentes": [
    {
      "id": 1,
      "titulo": "Equações do 2º Grau",
      "categoria": "Matemática",
      "totalTentativas": 17,
      "mediaPontuacao": 78.5,
      "dataCriacao": "2024-01-15T10:30:00Z",
      "publicado": true
    }
  ]
}
```

---

## 🎯 Gerenciamento de Quizzes

### `GET /api/professor/quizzes`
**O que faz:** Lista todos os quizzes criados pelo professor

**Parâmetros de query:**
- `busca` (opcional): Termo para buscar por título ou descrição

**Resposta:**
```json
[
  {
    "id": 1,
    "titulo": "Equações do 2º Grau",
    "descricao": "Quiz sobre resolução de equações quadráticas",
    "categoria": "Matemática",
    "dificuldade": "Médio",
    "tempoLimite": 30,
    "totalQuestoes": 2,
    "totalTentativas": 17,
    "publicado": true,
    "dataCriacao": "2024-01-15T10:30:00Z",
    "mediaPontuacao": 78.5
  }
]
```

### `GET /api/professor/quizzes/{quizId}`
**O que faz:** Obtém um quiz específico com todas as suas questões e opções

**Resposta:**
```json
{
  "id": 1,
  "titulo": "Equações do 2º Grau",
  "descricao": "Quiz sobre resolução de equações quadráticas",
  "categoriaId": 1,
  "categoria": "Matemática",
  "dificuldade": "Médio",
  "tempoLimite": 30,
  "maxTentativas": 1,
  "ativo": true,
  "publico": true,
  "dataCriacao": "2024-01-15T10:30:00Z",
  "dataAtualizacao": "2024-01-15T10:30:00Z",
  "totalQuestoes": 2,
  "questoes": [
    {
      "id": 1,
      "textoQuestao": "Qual é a fórmula de Bhaskara?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 1,
      "ordemIndice": 1,
      "ativo": true,
      "opcoes": [
        {
          "id": 1,
          "textoOpcao": "x = -b ± √(b²-4ac) / 2a",
          "correta": true,
          "ordemIndice": 1
        },
        {
          "id": 2,
          "textoOpcao": "x = -b ± √(b²+4ac) / 2a",
          "correta": false,
          "ordemIndice": 2
        }
      ]
    }
  ]
}
```

### `POST /api/professor/quizzes`
**O que faz:** Cria um novo quiz com suas questões e opções

**Corpo da requisição:**
```json
{
  "titulo": "Equações do 2º Grau",
  "descricao": "Quiz sobre resolução de equações quadráticas",
  "categoriaId": 1,
  "dificuldade": "Médio",
  "tempoLimite": 30,
  "maxTentativas": 1,
  "publico": true,
  "questoes": [
    {
      "textoQuestao": "Qual é a fórmula de Bhaskara?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 1,
      "ordemIndice": 1,
      "opcoes": [
        {
          "textoOpcao": "x = -b ± √(b²-4ac) / 2a",
          "correta": true,
          "ordemIndice": 1
        },
        {
          "textoOpcao": "x = -b ± √(b²+4ac) / 2a",
          "correta": false,
          "ordemIndice": 2
        }
      ]
    }
  ]
}
```

**Resposta:**
```json
{
  "id": 1,
  "titulo": "Equações do 2º Grau",
  "mensagem": "Quiz criado com sucesso!",
  "sucesso": true
}
```

### `PUT /api/professor/quizzes/{quizId}`
**O que faz:** Atualiza um quiz existente

**Corpo da requisição:**
```json
{
  "titulo": "Equações do 2º Grau - Atualizado",
  "descricao": "Quiz atualizado sobre resolução de equações quadráticas",
  "categoriaId": 1,
  "dificuldade": "Médio",
  "tempoLimite": 35,
  "maxTentativas": 2,
  "ativo": true,
  "publico": true,
  "questoes": [
    {
      "id": 1,
      "textoQuestao": "Qual é a fórmula de Bhaskara?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 1,
      "ordemIndice": 1,
      "opcoes": [
        {
          "id": 1,
          "textoOpcao": "x = -b ± √(b²-4ac) / 2a",
          "correta": true,
          "ordemIndice": 1
        }
      ]
    },
    {
      "textoQuestao": "O que é o discriminante?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 1,
      "ordemIndice": 2,
      "opcoes": [
        {
          "textoOpcao": "b² - 4ac",
          "correta": true,
          "ordemIndice": 1
        }
      ]
    }
  ]
}
```

### `DELETE /api/professor/quizzes/{quizId}`
**O que faz:** Deleta um quiz (ou desativa se houver tentativas)

**Resposta:**
```json
{
  "id": 1,
  "titulo": "Equações do 2º Grau",
  "mensagem": "Quiz deletado com sucesso!",
  "sucesso": true
}
```

### `POST /api/professor/quizzes/{quizId}/publicar`
**O que faz:** Publica um quiz (torna público para os alunos)

**Resposta:**
```json
{
  "id": 1,
  "titulo": "Equações do 2º Grau",
  "mensagem": "Quiz publicado com sucesso!",
  "sucesso": true
}
```

### `POST /api/professor/quizzes/{quizId}/despublicar`
**O que faz:** Despublica um quiz (remove da lista pública)

**Resposta:**
```json
{
  "id": 1,
  "titulo": "Equações do 2º Grau",
  "mensagem": "Quiz despublicado com sucesso!",
  "sucesso": true
}
```

---

## 📈 Estatísticas

### `GET /api/professor/quizzes/{quizId}/estatisticas`
**O que faz:** Obtém estatísticas detalhadas de um quiz específico

**Resposta:**
```json
{
  "quizId": 1,
  "tituloQuiz": "Equações do 2º Grau",
  "totalTentativas": 17,
  "totalAlunos": 15,
  "mediaPontuacao": 78.5,
  "mediaTempo": 1200.5,
  "totalQuestoes": 2,
  "estatisticasQuestoes": [
    {
      "questaoId": 1,
      "textoQuestao": "Qual é a fórmula de Bhaskara?",
      "totalRespostas": 17,
      "respostasCorretas": 14,
      "respostasErradas": 3,
      "percentualAcerto": 82.4
    }
  ],
  "tentativasRecentes": [
    {
      "tentativaId": 123,
      "nomeAluno": "Lucas Ribeiro",
      "pontuacao": 2,
      "pontuacaoMaxima": 2,
      "percentual": 100.0,
      "tempoGasto": 900,
      "dataConclusao": "2024-01-15T14:30:00Z"
    }
  ]
}
```

### `GET /api/professor/quizzes/{quizId}/estatisticas/questoes`
**O que faz:** Obtém estatísticas detalhadas das questões de um quiz

**Resposta:**
```json
[
  {
    "questaoId": 1,
    "textoQuestao": "Qual é a fórmula de Bhaskara?",
    "totalRespostas": 17,
    "respostasCorretas": 14,
    "respostasErradas": 3,
    "percentualAcerto": 82.4
  }
]
```

### `GET /api/professor/quizzes/{quizId}/tentativas`
**O que faz:** Obtém as tentativas recentes de um quiz

**Resposta:**
```json
[
  {
    "tentativaId": 123,
    "nomeAluno": "Lucas Ribeiro",
    "pontuacao": 2,
    "pontuacaoMaxima": 2,
    "percentual": 100.0,
    "tempoGasto": 900,
    "dataConclusao": "2024-01-15T14:30:00Z"
  }
]
```

---

## 👤 Perfil

### `GET /api/professor/perfil`
**O que faz:** Obtém os dados do perfil do professor

**Resposta:**
```json
{
  "id": 1,
  "nome": "João",
  "sobrenome": "Silva",
  "nomeCompleto": "João Silva",
  "email": "professor@demo.com",
  "funcao": "Professor",
  "cpf": "123.456.789-00",
  "dataNascimento": "1980-05-15T00:00:00Z",
  "escola": "Escola Estadual XYZ",
  "disciplina": "Matemática",
  "dataCriacao": "2023-01-01T10:00:00Z",
  "estatisticas": {
    "quizzesCriados": 12,
    "mediaDosAlunos": 74.2,
    "totalAlunos": 45,
    "totalTentativas": 156,
    "quizzesPublicados": 10
  }
}
```

### `PUT /api/professor/perfil`
**O que faz:** Atualiza os dados do perfil do professor

**Corpo da requisição:**
```json
{
  "nome": "João",
  "sobrenome": "Silva",
  "cpf": "123.456.789-00",
  "dataNascimento": "1980-05-15T00:00:00Z",
  "escola": "Escola Estadual XYZ",
  "disciplina": "Matemática"
}
```

---

## 📚 Categorias

### `GET /api/professor/categorias`
**O que faz:** Lista todas as categorias disponíveis

**Resposta:**
```json
[
  {
    "id": 1,
    "nome": "Matemática",
    "descricao": "Quizzes sobre matemática",
    "ativo": true
  },
  {
    "id": 2,
    "nome": "Português",
    "descricao": "Quizzes sobre língua portuguesa",
    "ativo": true
  }
]
```

### `GET /api/professor/categorias/{categoriaId}`
**O que faz:** Obtém uma categoria específica

**Resposta:**
```json
{
  "id": 1,
  "nome": "Matemática",
  "descricao": "Quizzes sobre matemática",
  "ativo": true
}
```

---

## 🔒 Segurança

### Regras de Autorização
- Todas as rotas exigem autenticação JWT
- Apenas usuários com função "Professor" podem acessar estas rotas
- O `professorId` é extraído automaticamente do token JWT

### Regras de Negócio
- Professor só pode editar/deletar seus próprios quizzes
- Quizzes com tentativas não podem ser deletados, apenas desativados
- Todas as questões valem 1 ponto
- Validação de categoria obrigatória ao criar/editar quiz
- Ordem das questões e opções é mantida conforme especificado

---

## 📝 Códigos de Status HTTP

- `200 OK`: Operação realizada com sucesso
- `201 Created`: Recurso criado com sucesso
- `400 Bad Request`: Dados inválidos na requisição
- `401 Unauthorized`: Token JWT inválido ou ausente
- `403 Forbidden`: Usuário não tem permissão (não é professor)
- `404 Not Found`: Recurso não encontrado
- `500 Internal Server Error`: Erro interno do servidor

---

## 🔧 Exemplos de Uso

### Fluxo Completo de Criação de Quiz

1. **Obter categorias disponíveis:**
   ```
   GET /api/professor/categorias
   ```

2. **Criar novo quiz:**
   ```
   POST /api/professor/quizzes
   {
     "titulo": "Equações do 2º Grau",
     "descricao": "Quiz sobre resolução de equações quadráticas",
     "categoriaId": 1,
     "dificuldade": "Médio",
     "tempoLimite": 30,
     "questoes": [...]
   }
   ```

3. **Publicar quiz:**
   ```
   POST /api/professor/quizzes/1/publicar
   ```

4. **Acompanhar estatísticas:**
   ```
   GET /api/professor/quizzes/1/estatisticas
   ```

### Gerenciamento de Quizzes

```
# Listar meus quizzes
GET /api/professor/quizzes

# Buscar quiz específico
GET /api/professor/quizzes?busca=equações

# Editar quiz
PUT /api/professor/quizzes/1

# Ver estatísticas
GET /api/professor/quizzes/1/estatisticas

# Ver tentativas
GET /api/professor/quizzes/1/tentativas

# Despublicar quiz
POST /api/professor/quizzes/1/despublicar
```

### Dashboard e Perfil

```
# Dashboard
GET /api/professor/dashboard

# Perfil
GET /api/professor/perfil
PUT /api/professor/perfil
```
