# Guia Completo - Endpoints do Aluno (Frontend)

Este documento contém **TODOS** os endpoints disponíveis para o frontend do aluno, com exemplos práticos e detalhados.

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

**Resposta de Sucesso (200 OK):**
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

**Resposta de Erro (401 Unauthorized):**
```json
{
  "message": "Credenciais inválidas"
}
```

---

## 📊 **2. Dashboard do Aluno**

### Obter Estatísticas do Dashboard
```http
GET /api/aluno/dashboard
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "quizzesCompletos": 15,
  "mediaGeral": 85.5,
  "posicaoRanking": 3,
  "sequencia": 12,
  "pontos": 2550,
  "totalUsuarios": 45,
  "quizzesRecentes": [
    {
      "quizId": 1,
      "titulo": "Matemática Básica",
      "categoria": "Matemática",
      "percentualAcerto": 90.0,
      "dataConclusao": "2024-01-15T10:30:00Z"
    }
  ]
}
```

---

## 📚 **3. Quizzes Disponíveis**

### Listar Quizzes Disponíveis (Endpoint Principal)
```http
GET /api/aluno/quizzes
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "id": 1,
    "titulo": "Matemática Básica",
    "descricao": "Operações fundamentais de matemática",
    "categoria": "Matemática",
    "dificuldade": "Facil",
    "tempoLimite": 30,
    "totalQuestoes": 10,
    "pontuacaoTotal": 100,
    "disponivel": true
  },
  {
    "id": 2,
    "titulo": "História do Brasil",
    "descricao": "Conhecimentos sobre a história brasileira",
    "categoria": "História",
    "dificuldade": "Media",
    "tempoLimite": 45,
    "totalQuestoes": 15,
    "pontuacaoTotal": 150,
    "disponivel": true
  }
]
```

### Listar Quizzes Disponíveis (Endpoint Alternativo)
```http
GET /api/aluno/quizzes/disponiveis
Authorization: Bearer {seu_jwt_token}
```

*Mesma resposta do endpoint acima*

---

## 🎯 **4. Detalhes do Quiz**

### Obter Detalhes de um Quiz Específico
```http
GET /api/aluno/quizzes/{quizId}
Authorization: Bearer {seu_jwt_token}
```

**Exemplo:**
```http
GET /api/aluno/quizzes/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Resposta de Sucesso (200 OK):**
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
  "criadoPor": "Professor",
  "dataCriacao": "2024-01-10T08:00:00Z",
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

**Resposta de Erro (404 Not Found):**
```json
{
  "message": "Quiz não encontrado ou não disponível."
}
```

---

## 📝 **5. Responder Quiz**

### Responder Quiz Completo (Recomendado)
```http
POST /api/aluno/quizzes/{quizId}/responder
Authorization: Bearer {seu_jwt_token}
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

**Resposta de Sucesso (200 OK):**
```json
{
  "tentativaId": 501,
  "quizId": 1,
  "alunoId": 14,
  "pontuacaoTotal": 25,
  "pontuacaoMaxima": 25,
  "percentualAcerto": 100.0,
  "dataTentativa": "2024-01-15T10:30:00Z",
  "tempoGasto": 12,
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
  "novoRecorde": false
}
```

**Resposta de Erro (400 Bad Request):**
```json
{
  "message": "Não foi possível processar as respostas. Verifique os dados ou o status do quiz."
}
```

**Resposta de Erro (409 Conflict):**
```json
{
  "message": "Você já excedeu o número máximo de tentativas para este quiz."
}
```

---

## 🏆 **6. Ranking Geral**

### Obter Ranking dos Alunos
```http
GET /api/aluno/ranking
Authorization: Bearer {seu_jwt_token}
```

**Parâmetros de Query (Opcionais):**
- `busca=joão` - Buscar por nome do aluno

**Resposta de Sucesso (200 OK):**
```json
{
  "alunos": [
    {
      "posicao": 1,
      "usuarioId": 5,
      "nomeCompleto": "Lucas Ribeiro",
      "avatar": "LR",
      "pontos": 3000,
      "quizzes": 30,
      "media": 95.5,
      "sequencia": 15
    },
    {
      "posicao": 2,
      "usuarioId": 8,
      "nomeCompleto": "Rafael Viera",
      "avatar": "RV",
      "pontos": 2480,
      "quizzes": 26,
      "media": 85.5,
      "sequencia": 8
    },
    {
      "posicao": 3,
      "usuarioId": 14,
      "nomeCompleto": "João Silva",
      "avatar": "JS",
      "pontos": 2200,
      "quizzes": 25,
      "media": 88.0,
      "sequencia": 12
    }
  ],
  "totalAlunos": 45,
  "posicaoUsuarioLogado": 3
}
```

---

## 👤 **7. Perfil do Aluno**

### Obter Perfil Completo
```http
GET /api/aluno/perfil
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "id": 14,
  "nome": "João",
  "sobrenome": "Silva",
  "nomeCompleto": "João Silva",
  "email": "joao@escola.com",
  "funcao": "Aluno",
  "cpf": "123.456.789-00",
  "dataNascimento": "2005-03-15T00:00:00Z",
  "dataCriacao": "2024-01-01T08:00:00Z",
  "estatisticas": {
    "quizzesCompletos": 15,
    "mediaGeral": 85.5,
    "totalPontos": 2550,
    "posicaoRanking": 3,
    "sequenciaDias": 12
  }
}
```

### Atualizar Perfil
```http
PUT /api/aluno/perfil
Authorization: Bearer {seu_jwt_token}
Content-Type: application/json

{
  "nome": "João",
  "sobrenome": "Silva",
  "email": "joao.novo@escola.com",
  "dataNascimento": "2005-03-15T00:00:00Z"
}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "id": 14,
  "nome": "João",
  "sobrenome": "Silva",
  "nomeCompleto": "João Silva",
  "email": "joao.novo@escola.com",
  "funcao": "Aluno",
  "cpf": "123.456.789-00",
  "dataNascimento": "2005-03-15T00:00:00Z",
  "dataCriacao": "2024-01-01T08:00:00Z",
  "estatisticas": {
    "quizzesCompletos": 15,
    "mediaGeral": 85.5,
    "totalPontos": 2550,
    "posicaoRanking": 3,
    "sequenciaDias": 12
  }
}
```

---

## 📈 **8. Desempenho e Estatísticas**

### Obter Desempenho em Quizzes
```http
GET /api/aluno/desempenho
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "quizId": 1,
    "tituloQuiz": "Matemática Básica",
    "categoria": "Matemática",
    "percentualAcerto": 90.0,
    "pontuacao": 90,
    "tempoGasto": 15.5,
    "dataConclusao": "2024-01-15T10:30:00Z"
  },
  {
    "quizId": 2,
    "tituloQuiz": "História do Brasil",
    "categoria": "História",
    "percentualAcerto": 75.0,
    "pontuacao": 112,
    "tempoGasto": 22.3,
    "dataConclusao": "2024-01-14T14:20:00Z"
  }
]
```

### Obter Atividades Recentes
```http
GET /api/aluno/atividades-recentes
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
```json
[
  {
    "id": 1,
    "tipo": "Quiz Concluído",
    "descricao": "Você completou o quiz 'Matemática Básica' com 90% de acerto!",
    "data": "2024-01-15T10:30:00Z",
    "icone": "trophy",
    "cor": "green"
  },
  {
    "id": 2,
    "tipo": "Nova Posição no Ranking",
    "descricao": "Você subiu para a 3ª posição no ranking geral!",
    "data": "2024-01-15T10:25:00Z",
    "icone": "trending-up",
    "cor": "blue"
  }
]
```

---

## 🏷️ **9. Categorias**

### Listar Todas as Categorias
```http
GET /api/categorias
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
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
    "nome": "História",
    "descricao": "Quizzes sobre história",
    "ativo": true
  },
  {
    "id": 3,
    "nome": "Ciências",
    "descricao": "Quizzes sobre ciências naturais",
    "ativo": true
  }
]
```

---

## 🎮 **10. Sistema de Tentativas (Avançado)**

### Iniciar Quiz (Modo Questão por Questão)
```http
POST /api/aluno/quizzes/iniciar
Authorization: Bearer {seu_jwt_token}
Content-Type: application/json

{
  "quizId": 1
}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "tentativaId": 501,
  "quizId": 1,
  "tituloQuiz": "Matemática Básica",
  "questaoAtual": {
    "id": 101,
    "textoQuestao": "Quanto é 5 + 3?",
    "tipoQuestao": "MultiplaEscolha",
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
      }
    ],
    "pontos": 10,
    "ordemIndice": 1
  },
  "progresso": {
    "questaoAtual": 1,
    "totalQuestoes": 10,
    "percentualCompleto": 10.0,
    "pontuacaoAtual": 0,
    "tempoGasto": 0
  }
}
```

### Responder Questão Individual
```http
POST /api/aluno/tentativas/{tentativaId}/responder
Authorization: Bearer {seu_jwt_token}
Content-Type: application/json

{
  "questaoId": 101,
  "opcaoSelecionadaId": 1002
}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "respostaCorreta": true,
  "pontosGanhos": 10,
  "respostaCorretaTexto": "8",
  "feedback": "Parabéns! Resposta correta!",
  "proximaQuestao": {
    "id": 102,
    "textoQuestao": "Qual a capital do Brasil?",
    "tipoQuestao": "MultiplaEscolha",
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
      }
    ],
    "pontos": 15,
    "ordemIndice": 2
  },
  "quizConcluido": false,
  "resultadoFinal": null
}
```

### Obter Progresso da Tentativa
```http
GET /api/aluno/tentativas/{tentativaId}/progresso
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "questaoAtual": 3,
  "totalQuestoes": 10,
  "percentualCompleto": 30.0,
  "pontuacaoAtual": 20,
  "tempoGasto": 180
}
```

### Finalizar Quiz
```http
POST /api/aluno/tentativas/{tentativaId}/finalizar
Authorization: Bearer {seu_jwt_token}
```

**Resposta de Sucesso (200 OK):**
```json
{
  "tentativaId": 501,
  "pontuacaoFinal": 85,
  "pontuacaoMaxima": 100,
  "percentualAcerto": 85.0,
  "tempoGasto": 600,
  "totalQuestoes": 10,
  "respostasCorretas": 8,
  "respostasErradas": 2,
  "dataConclusao": "2024-01-15T10:30:00Z"
}
```

---

## 🚨 **11. Códigos de Status HTTP**

| Código | Significado | Quando Ocorre |
|--------|-------------|---------------|
| **200** | OK | Requisição bem-sucedida |
| **201** | Created | Recurso criado com sucesso |
| **400** | Bad Request | Dados inválidos ou malformados |
| **401** | Unauthorized | Token inválido ou expirado |
| **403** | Forbidden | Acesso negado (não é aluno) |
| **404** | Not Found | Recurso não encontrado |
| **409** | Conflict | Conflito (ex: limite de tentativas excedido) |
| **500** | Internal Server Error | Erro interno do servidor |

---

## 🔧 **12. Headers Obrigatórios**

### Para Todas as Requisições:
```
Authorization: Bearer {seu_jwt_token}
Content-Type: application/json
```

### Exemplo de Headers:
```http
GET /api/aluno/dashboard
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxNCIsIm5hbWUiOiJhbHVubzEyMyIsInJvbGUiOiJBbHVubyIsImV4cCI6MTcwNTQzMjAwMH0.abc123def456
Content-Type: application/json
```

---

## 📱 **13. Exemplos de Implementação Frontend**

### JavaScript/TypeScript - Listar Quizzes
```javascript
async function carregarQuizzes() {
  try {
    const token = localStorage.getItem('jwt_token');
    
    const response = await fetch('/api/aluno/quizzes', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (!response.ok) {
      throw new Error(`Erro ${response.status}: ${response.statusText}`);
    }
    
    const quizzes = await response.json();
    
    // Renderizar lista de quizzes
    quizzes.forEach(quiz => {
      console.log(`Quiz: ${quiz.titulo} - ${quiz.categoria}`);
    });
    
  } catch (error) {
    console.error('Erro ao carregar quizzes:', error);
  }
}
```

### JavaScript/TypeScript - Responder Quiz
```javascript
async function responderQuiz(quizId, respostas) {
  try {
    const token = localStorage.getItem('jwt_token');
    
    const response = await fetch(`/api/aluno/quizzes/${quizId}/responder`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ respostas })
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message);
    }
    
    const resultado = await response.json();
    
    // Exibir resultado
    console.log(`Pontuação: ${resultado.pontuacaoTotal}/${resultado.pontuacaoMaxima}`);
    console.log(`Percentual: ${resultado.percentualAcerto}%`);
    
    return resultado;
    
  } catch (error) {
    console.error('Erro ao responder quiz:', error);
    throw error;
  }
}
```

### React Hook - Dashboard
```javascript
import { useState, useEffect } from 'react';

function useDashboard() {
  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function carregarDashboard() {
      try {
        const token = localStorage.getItem('jwt_token');
        
        const response = await fetch('/api/aluno/dashboard', {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        });
        
        if (!response.ok) {
          throw new Error(`Erro ${response.status}`);
        }
        
        const data = await response.json();
        setDashboard(data);
        
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }
    
    carregarDashboard();
  }, []);

  return { dashboard, loading, error };
}
```

---

## ⚠️ **14. Considerações Importantes**

### Segurança
- **Sempre validar o token JWT** antes de fazer requisições
- **Implementar refresh token** para renovação automática
- **Nunca expor tokens** no código frontend

### Performance
- **Implementar cache** para dados que mudam pouco (categorias, perfil)
- **Usar paginação** para listas grandes
- **Carregamento assíncrono** para melhor UX

### Tratamento de Erros
- **Sempre tratar erros 401** (token expirado) - redirecionar para login
- **Implementar retry** para erros 500 (servidor)
- **Feedback visual** para o usuário em caso de erro

### Validações Frontend
- **Validar dados** antes de enviar
- **Verificar tentativas restantes** antes de permitir responder quiz
- **Validar tempo limite** se especificado

---

## 🎯 **15. Resumo dos Endpoints**

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/aluno/dashboard` | Estatísticas do dashboard |
| `GET` | `/api/aluno/quizzes` | Listar quizzes disponíveis |
| `GET` | `/api/aluno/quizzes/{id}` | Detalhes do quiz |
| `POST` | `/api/aluno/quizzes/{id}/responder` | Responder quiz completo |
| `GET` | `/api/aluno/ranking` | Ranking geral |
| `GET` | `/api/aluno/perfil` | Perfil do aluno |
| `PUT` | `/api/aluno/perfil` | Atualizar perfil |
| `GET` | `/api/aluno/desempenho` | Desempenho em quizzes |
| `GET` | `/api/aluno/atividades-recentes` | Atividades recentes |
| `POST` | `/api/aluno/quizzes/iniciar` | Iniciar quiz (questão por questão) |
| `POST` | `/api/aluno/tentativas/{id}/responder` | Responder questão individual |
| `GET` | `/api/aluno/tentativas/{id}/progresso` | Progresso da tentativa |
| `POST` | `/api/aluno/tentativas/{id}/finalizar` | Finalizar quiz |
| `GET` | `/api/categorias` | Listar categorias |

---

**🎉 Este guia contém tudo que o frontend precisa para implementar a funcionalidade completa do aluno!**
