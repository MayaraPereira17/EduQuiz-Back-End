# Guia de Integração Frontend - Dashboard do Aluno

Este documento detalha os endpoints e fluxos para implementar o dashboard do aluno, incluindo listagem de quizzes disponíveis e funcionalidade de responder quizzes.

## 📊 **Visão Geral do Dashboard do Aluno**

O dashboard do aluno deve exibir:
- **Estatísticas pessoais** (quizzes completos, média geral, total de pontos, tempo de estudo, posição no ranking)
- **Lista de quizzes disponíveis** para responder
- **Quizzes recentes** que o aluno já respondeu
- **Ranking geral** dos alunos

---

## 🔐 **1. Autenticação**

### Login do Aluno
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "aluno123",
  "password": "senha123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": 14,
    "username": "aluno123",
    "email": "aluno@escola.com",
    "role": "Aluno",
    "firstName": "João",
    "lastName": "Silva"
  }
}
```

---

## 📈 **2. Dashboard - Estatísticas do Aluno**

### Obter Estatísticas do Dashboard
```http
GET /api/aluno/dashboard
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "quizzesCompletos": 15,
  "mediaGeral": 85.5,
  "totalPontos": 2550,
  "tempoEstudo": 12.5,
  "posicaoRanking": 3,
  "quizzesDisponiveis": 8,
  "quizzesRecentes": [
    {
      "id": 1,
      "titulo": "Matemática Básica",
      "pontuacao": 90,
      "dataTentativa": "2024-01-15T10:30:00Z",
      "tempoGasto": 15.5
    }
  ]
}
```

---

## 📚 **3. Listagem de Quizzes Disponíveis**

### Listar Quizzes Públicos e Ativos
```http
GET /api/aluno/quizzes
Authorization: Bearer {token}
```

**Parâmetros de Query (opcionais):**
- `page=1` - Página atual
- `pageSize=10` - Itens por página
- `search=matemática` - Buscar por título/descrição
- `categoriaId=1` - Filtrar por categoria
- `dificuldade=Facil` - Filtrar por dificuldade

**Resposta:**
```json
{
  "currentPage": 1,
  "pageSize": 10,
  "totalPages": 3,
  "totalCount": 25,
  "items": [
    {
      "id": 1,
      "titulo": "Matemática Básica",
      "descricao": "Operações fundamentais de matemática",
      "categoria": {
        "id": 1,
        "nome": "Matemática"
      },
      "dificuldade": "Facil",
      "tempoLimite": 30,
      "maxTentativas": 3,
      "criadoPor": "Prof. Maria Silva",
      "dataCriacao": "2024-01-10T08:00:00Z",
      "totalQuestoes": 10,
      "mediaPontuacao": 75.5,
      "jaRespondido": false,
      "tentativasRestantes": 3
    },
    {
      "id": 2,
      "titulo": "História do Brasil",
      "descricao": "Conhecimentos sobre a história brasileira",
      "categoria": {
        "id": 2,
        "nome": "História"
      },
      "dificuldade": "Media",
      "tempoLimite": 45,
      "maxTentativas": 2,
      "criadoPor": "Prof. João Santos",
      "dataCriacao": "2024-01-12T14:30:00Z",
      "totalQuestoes": 15,
      "mediaPontuacao": 82.3,
      "jaRespondido": true,
      "tentativasRestantes": 1,
      "melhorPontuacao": 85
    }
  ]
}
```

---

## 🎯 **4. Detalhes do Quiz para Responder**

### Obter Quiz com Questões
```http
GET /api/aluno/quizzes/{quizId}
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "id": 1,
  "titulo": "Matemática Básica",
  "descricao": "Operações fundamentais de matemática",
  "categoria": {
    "id": 1,
    "nome": "Matemática"
  },
  "dificuldade": "Facil",
  "tempoLimite": 30,
  "maxTentativas": 3,
  "tentativasRestantes": 3,
  "totalQuestoes": 10,
  "questoes": [
    {
      "id": 101,
      "textoQuestao": "Quanto é 5 + 3?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 10,
      "ordemIndice": 1,
      "opcoes": [
        {
          "id": 1001,
          "textoOpcao": "7",
          "ordemIndice": 1
        },
        {
          "id": 1002,
          "textoOpcao": "8",
          "ordemIndice": 2
        },
        {
          "id": 1003,
          "textoOpcao": "9",
          "ordemIndice": 3
        }
      ]
    },
    {
      "id": 102,
      "textoQuestao": "Qual a capital do Brasil?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 15,
      "ordemIndice": 2,
      "opcoes": [
        {
          "id": 1004,
          "textoOpcao": "Rio de Janeiro",
          "ordemIndice": 1
        },
        {
          "id": 1005,
          "textoOpcao": "São Paulo",
          "ordemIndice": 2
        },
        {
          "id": 1006,
          "textoOpcao": "Brasília",
          "ordemIndice": 3
        }
      ]
    }
  ]
}
```

---

## 📝 **5. Responder Quiz**

### Enviar Respostas
```http
POST /api/aluno/quizzes/{quizId}/responder
Authorization: Bearer {token}
Content-Type: application/json

{
  "respostas": [
    {
      "questaoId": 101,
      "opcaoSelecionadaId": 1002
    },
    {
      "questaoId": 102,
      "opcaoSelecionadaId": 1006
    }
  ]
}
```

**Resposta de Sucesso:**
```json
{
  "tentativaId": 501,
  "quizId": 1,
  "alunoId": 14,
  "pontuacaoTotal": 25,
  "pontuacaoMaxima": 25,
  "percentualAcerto": 100.0,
  "dataTentativa": "2024-01-15T10:30:00Z",
  "tempoGasto": 12.5,
  "respostasCorretas": 2,
  "respostasIncorretas": 0,
  "respostas": [
    {
      "questaoId": 101,
      "opcaoSelecionadaId": 1002,
      "correta": true,
      "pontosObtidos": 10
    },
    {
      "questaoId": 102,
      "opcaoSelecionadaId": 1006,
      "correta": true,
      "pontosObtidos": 15
    }
  ],
  "message": "Quiz respondido com sucesso!",
  "novoRecorde": true
}
```

---

## 🏆 **6. Ranking Geral**

### Obter Ranking dos Alunos
```http
GET /api/aluno/ranking
Authorization: Bearer {token}
```

**Parâmetros de Query (opcionais):**
- `page=1` - Página atual
- `pageSize=10` - Itens por página
- `periodo=mes` - Período: "semana", "mes", "ano", "todos"

**Resposta:**
```json
{
  "currentPage": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalCount": 45,
  "minhaPosicao": 3,
  "items": [
    {
      "posicao": 1,
      "alunoId": 5,
      "nome": "Lucas Ribeiro",
      "avatar": "LR",
      "totalPontos": 3000,
      "quizzesCompletos": 30,
      "mediaGeral": 95.5,
      "sequenciaDias": 15
    },
    {
      "posicao": 2,
      "alunoId": 8,
      "nome": "Rafael Viera",
      "avatar": "RV",
      "totalPontos": 2480,
      "quizzesCompletos": 26,
      "mediaGeral": 85.5,
      "sequenciaDias": 8
    },
    {
      "posicao": 3,
      "alunoId": 14,
      "nome": "João Silva",
      "avatar": "JS",
      "totalPontos": 2200,
      "quizzesCompletos": 25,
      "mediaGeral": 88.0,
      "sequenciaDias": 12
    }
  ]
}
```

---

## 📋 **7. Histórico de Tentativas**

### Obter Histórico do Aluno
```http
GET /api/aluno/tentativas
Authorization: Bearer {token}
```

**Parâmetros de Query (opcionais):**
- `page=1` - Página atual
- `pageSize=10` - Itens por página
- `quizId=1` - Filtrar por quiz específico

**Resposta:**
```json
{
  "currentPage": 1,
  "pageSize": 10,
  "totalPages": 2,
  "totalCount": 15,
  "items": [
    {
      "tentativaId": 501,
      "quizId": 1,
      "quizTitulo": "Matemática Básica",
      "categoria": "Matemática",
      "pontuacao": 90,
      "pontuacaoMaxima": 100,
      "percentualAcerto": 90.0,
      "dataTentativa": "2024-01-15T10:30:00Z",
      "tempoGasto": 15.5,
      "respostasCorretas": 9,
      "respostasIncorretas": 1
    }
  ]
}
```

---

## 🏷️ **8. Categorias**

### Listar Categorias
```http
GET /api/categorias
Authorization: Bearer {token}
```

**Resposta:**
```json
[
  {
    "id": 1,
    "nome": "Matemática",
    "descricao": "Quizzes sobre matemática",
    "totalQuizzes": 15,
    "quizzesDisponiveis": 8
  },
  {
    "id": 2,
    "nome": "História",
    "descricao": "Quizzes sobre história",
    "totalQuizzes": 12,
    "quizzesDisponiveis": 5
  }
]
```

---

## 🎨 **9. Implementação Frontend - Exemplos**

### Dashboard Principal
```javascript
// Exemplo de como buscar dados do dashboard
async function carregarDashboard() {
  try {
    const response = await fetch('/api/aluno/dashboard', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const dashboard = await response.json();
    
    // Atualizar cards de estatísticas
    document.getElementById('quizzes-completos').textContent = dashboard.quizzesCompletos;
    document.getElementById('media-geral').textContent = `${dashboard.mediaGeral}%`;
    document.getElementById('total-pontos').textContent = dashboard.totalPontos;
    document.getElementById('tempo-estudo').textContent = `${dashboard.tempoEstudo}h`;
    document.getElementById('posicao-ranking').textContent = `#${dashboard.posicaoRanking}`;
    
  } catch (error) {
    console.error('Erro ao carregar dashboard:', error);
  }
}
```

### Listagem de Quizzes
```javascript
// Exemplo de como listar quizzes disponíveis
async function carregarQuizzes(filtros = {}) {
  const params = new URLSearchParams(filtros);
  
  try {
    const response = await fetch(`/api/aluno/quizzes?${params}`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const data = await response.json();
    
    // Renderizar lista de quizzes
    data.items.forEach(quiz => {
      const quizCard = criarCardQuiz(quiz);
      document.getElementById('lista-quizzes').appendChild(quizCard);
    });
    
  } catch (error) {
    console.error('Erro ao carregar quizzes:', error);
  }
}
```

### Responder Quiz
```javascript
// Exemplo de como enviar respostas
async function responderQuiz(quizId, respostas) {
  try {
    const response = await fetch(`/api/aluno/quizzes/${quizId}/responder`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ respostas })
    });
    
    const resultado = await response.json();
    
    if (response.ok) {
      // Exibir resultado do quiz
      exibirResultadoQuiz(resultado);
    } else {
      // Exibir erro
      alert(resultado.message);
    }
    
  } catch (error) {
    console.error('Erro ao responder quiz:', error);
  }
}
```

---

## ⚠️ **10. Considerações Importantes**

### Status dos Quizzes
- **Apenas quizzes com `Publico = true` e `Ativo = true`** aparecem para os alunos
- **Professores podem publicar/despublicar** quizzes a qualquer momento
- **Alunos só veem quizzes disponíveis** no momento da consulta

### Limites e Validações
- **Respeitar `maxTentativas`** de cada quiz
- **Validar `tempoLimite`** se especificado
- **Verificar se o quiz ainda está ativo** antes de responder

### Tratamento de Erros
- **401 Unauthorized**: Token inválido ou expirado
- **403 Forbidden**: Acesso negado
- **404 Not Found**: Quiz não encontrado ou não disponível
- **409 Conflict**: Limite de tentativas excedido
- **500 Internal Server Error**: Erro interno do servidor

### Performance
- **Usar paginação** para listas grandes
- **Implementar cache** para dados que mudam pouco
- **Carregamento assíncrono** para melhor UX

---

## 🚀 **11. Endpoints Resumidos**

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/aluno/dashboard` | Estatísticas do dashboard |
| `GET` | `/api/aluno/quizzes` | Listar quizzes disponíveis |
| `GET` | `/api/aluno/quizzes/{id}` | Detalhes do quiz |
| `POST` | `/api/aluno/quizzes/{id}/responder` | Responder quiz |
| `GET` | `/api/aluno/ranking` | Ranking geral |
| `GET` | `/api/aluno/tentativas` | Histórico de tentativas |
| `GET` | `/api/categorias` | Listar categorias |

---

Este guia fornece tudo que o frontend precisa para implementar o dashboard completo do aluno! 🎯
