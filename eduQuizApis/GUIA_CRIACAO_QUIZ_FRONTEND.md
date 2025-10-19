# Guia de Criação de Quiz - Frontend

## 📋 Visão Geral

Este documento explica como o frontend deve enviar os dados para criar um quiz com questões e opções, evitando erros 500.

## 🚀 Endpoints Disponíveis

### 1. Criar Quiz
```
POST /api/professor/quizzes
```

### 2. Listar Quizzes do Professor
```
GET /api/professor/quizzes
```

### 3. Obter Quiz por ID
```
GET /api/professor/quizzes/{id}
```

### 4. Atualizar Quiz
```
PUT /api/professor/quizzes/{id}
```

### 5. Deletar Quiz
```
DELETE /api/professor/quizzes/{id}
```

### 6. Dashboard do Professor
```
GET /api/professor/dashboard
```

### 7. Listar Categorias
```
GET /api/categorias
```

### 8. Obter Estatísticas
```
GET /api/professor/estatisticas
```

### 9. Obter Quizzes Recentes
```
GET /api/professor/quizzes/recentes
```

## 🔐 Headers Obrigatórios

```http
Content-Type: application/json
Authorization: Bearer {seu_jwt_token}
```

## 📚 Exemplos de Uso dos Endpoints

### 1. Listar Categorias (para popular dropdown)
```javascript
// GET /api/categorias
const categorias = await fetch('/api/categorias', {
  headers: { 'Authorization': `Bearer ${token}` }
});
// Retorna: [{ id: 1, nome: "Matemática Básica", ativo: true }, ...]
```

### 2. Dashboard do Professor
```javascript
// GET /api/professor/dashboard
const dashboard = await fetch('/api/professor/dashboard', {
  headers: { 'Authorization': `Bearer ${token}` }
});
// Retorna: { totalQuizzes, totalTentativas, mediaPontuacao, ... }
```

### 3. Listar Quizzes do Professor
```javascript
// GET /api/professor/quizzes
const quizzes = await fetch('/api/professor/quizzes', {
  headers: { 'Authorization': `Bearer ${token}` }
});
// Retorna: [{ id, titulo, descricao, categoriaId, ... }, ...]
```

### 4. Obter Quiz Específico
```javascript
// GET /api/professor/quizzes/123
const quiz = await fetch('/api/professor/quizzes/123', {
  headers: { 'Authorization': `Bearer ${token}` }
});
// Retorna: { id, titulo, questoes: [...], ... }
```

### 5. Atualizar Quiz
```javascript
// PUT /api/professor/quizzes/123
const updatedQuiz = await fetch('/api/professor/quizzes/123', {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    titulo: "Novo Título",
    descricao: "Nova Descrição",
    // ... outros campos
  })
});
```

### 6. Deletar Quiz
```javascript
// DELETE /api/professor/quizzes/123
const result = await fetch('/api/professor/quizzes/123', {
  method: 'DELETE',
  headers: { 'Authorization': `Bearer ${token}` }
});
```

## 🎯 Endpoint Principal: Criar Quiz

### POST /api/professor/quizzes

Este é o endpoint principal para criar um quiz completo com questões e opções.

**Headers:**
```http
Content-Type: application/json
Authorization: Bearer {seu_jwt_token}
```

**Método:** POST  
**URL:** `/api/professor/quizzes`  
**Autenticação:** Obrigatória (Professor)

## 📝 Estrutura do Payload

### 1. Dados Básicos do Quiz

```json
{
  "titulo": "Quiz de Matemática Básica",
  "descricao": "Teste seus conhecimentos em operações básicas",
  "categoriaId": 1,
  "tempoLimite": 30,
  "maxTentativas": 3,
  "publico": true,
  "dificuldade": "Media"
}
```

### 2. Questões e Opções

```json
{
  "questoes": [
    {
      "enunciado": "Qual é o resultado de 2 + 2?",
      "tipoQuestao": "MultiplaEscolha",
      "pontuacao": 10,
      "ordemIndice": 1,
      "opcoes": [
        {
          "textoOpcao": "3",
          "correta": false,
          "ordemIndice": 1
        },
        {
          "textoOpcao": "4",
          "correta": true,
          "ordemIndice": 2
        },
        {
          "textoOpcao": "5",
          "correta": false,
          "ordemIndice": 3
        }
      ]
    },
    {
      "enunciado": "Calcule: 5 × 3",
      "tipoQuestao": "MultiplaEscolha",
      "pontuacao": 10,
      "ordemIndice": 2,
      "opcoes": [
        {
          "textoOpcao": "15",
          "correta": true,
          "ordemIndice": 1
        },
        {
          "textoOpcao": "8",
          "correta": false,
          "ordemIndice": 2
        }
      ]
    }
  ]
}
```

## 📋 Payload Completo

```json
{
  "titulo": "Quiz de Matemática Básica",
  "descricao": "Teste seus conhecimentos em operações básicas",
  "categoriaId": 1,
  "tempoLimite": 30,
  "maxTentativas": 3,
  "publico": true,
  "dificuldade": "Media",
  "questoes": [
    {
      "enunciado": "Qual é o resultado de 2 + 2?",
      "tipoQuestao": "MultiplaEscolha",
      "pontuacao": 10,
      "ordemIndice": 1,
      "opcoes": [
        {
          "textoOpcao": "3",
          "correta": false,
          "ordemIndice": 1
        },
        {
          "textoOpcao": "4",
          "correta": true,
          "ordemIndice": 2
        },
        {
          "textoOpcao": "5",
          "correta": false,
          "ordemIndice": 3
        }
      ]
    }
  ]
}
```

## ✅ Validações Obrigatórias

### Quiz
- `titulo`: **OBRIGATÓRIO** - String, máximo 200 caracteres
- `categoriaId`: **OBRIGATÓRIO** - ID de categoria existente no banco
- `dificuldade`: **OBRIGATÓRIO** - Aceita: "Facil", "Media", "Dificil" (com ou sem acentos)
- `descricao`: Opcional - String
- `tempoLimite`: Opcional - Número em minutos
- `maxTentativas`: Opcional - Número, padrão 1
- `publico`: Opcional - Boolean, padrão false

### Questões
- `questoes`: **OBRIGATÓRIO** - Array com pelo menos 1 questão
- `enunciado`: **OBRIGATÓRIO** - String
- `tipoQuestao`: **OBRIGATÓRIO** - "MultiplaEscolha" ou "Dissertativa"
- `pontuacao`: **OBRIGATÓRIO** - Número maior que 0
- `ordemIndice`: **OBRIGATÓRIO** - Número sequencial (1, 2, 3...)

### Opções (para MultiplaEscolha)
- `opcoes`: **OBRIGATÓRIO** - Array com pelo menos 2 opções
- `textoOpcao`: **OBRIGATÓRIO** - String
- `correta`: **OBRIGATÓRIO** - Boolean (deve ter pelo menos 1 opção correta)
- `ordemIndice`: **OBRIGATÓRIO** - Número sequencial (1, 2, 3...)

## 🎯 Valores Aceitos

### Dificuldade
```javascript
// Aceita qualquer uma dessas variações:
"Facil"     // ✅
"facil"     // ✅
"Fácil"     // ✅
"fácil"     // ✅

"Media"     // ✅
"media"     // ✅
"Média"     // ✅
"média"     // ✅
"Médio"     // ✅
"médio"     // ✅

"Dificil"   // ✅
"dificil"   // ✅
"Difícil"   // ✅
"difícil"   // ✅
```

### Tipo de Questão
```javascript
"MultiplaEscolha"  // ✅
"Dissertativa"     // ✅
```

### Categorias Disponíveis
```javascript
// IDs válidos (verificar no banco):
1  // Matemática Básica
2  // Álgebra
3  // Geometria
4  // Trigonometria
5  // Cálculo
6  // Estatística
```

## 🚨 Erros Comuns que Causam 500

### 1. Categoria Inexistente
```json
// ❌ ERRO - Categoria ID 999 não existe
{
  "categoriaId": 999
}

// ✅ CORRETO - Usar ID válido
{
  "categoriaId": 1
}
```

### 2. Questão sem Opções (MultiplaEscolha)
```json
// ❌ ERRO - MultiplaEscolha precisa de opções
{
  "tipoQuestao": "MultiplaEscolha",
  "opcoes": []
}

// ✅ CORRETO - Pelo menos 2 opções
{
  "tipoQuestao": "MultiplaEscolha",
  "opcoes": [
    {"textoOpcao": "Opção A", "correta": true, "ordemIndice": 1},
    {"textoOpcao": "Opção B", "correta": false, "ordemIndice": 2}
  ]
}
```

### 3. Nenhuma Opção Correta
```json
// ❌ ERRO - Todas as opções marcadas como false
{
  "opcoes": [
    {"textoOpcao": "A", "correta": false, "ordemIndice": 1},
    {"textoOpcao": "B", "correta": false, "ordemIndice": 2}
  ]
}

// ✅ CORRETO - Pelo menos 1 opção correta
{
  "opcoes": [
    {"textoOpcao": "A", "correta": true, "ordemIndice": 1},
    {"textoOpcao": "B", "correta": false, "ordemIndice": 2}
  ]
}
```

### 4. Dificuldade Inválida
```json
// ❌ ERRO - Dificuldade não reconhecida
{
  "dificuldade": "SuperDificil"
}

// ✅ CORRETO - Usar valores aceitos
{
  "dificuldade": "Dificil"
}
```

## 🔧 Exemplo de Implementação JavaScript

```javascript
// Função para criar quiz
async function criarQuiz(quizData) {
  try {
    const response = await fetch('/api/professor/quizzes', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: JSON.stringify(quizData)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Erro ao criar quiz');
    }

    const result = await response.json();
    console.log('Quiz criado com sucesso:', result);
    return result;
  } catch (error) {
    console.error('Erro:', error.message);
    throw error;
  }
}

// Exemplo de uso
const novoQuiz = {
  titulo: "Meu Quiz",
  descricao: "Descrição do quiz",
  categoriaId: 1,
  dificuldade: "Media",
  tempoLimite: 30,
  maxTentativas: 3,
  publico: true,
  questoes: [
    {
      enunciado: "Pergunta 1?",
      tipoQuestao: "MultiplaEscolha",
      pontuacao: 10,
      ordemIndice: 1,
      opcoes: [
        {
          textoOpcao: "Resposta A",
          correta: true,
          ordemIndice: 1
        },
        {
          textoOpcao: "Resposta B",
          correta: false,
          ordemIndice: 2
        }
      ]
    }
  ]
};

criarQuiz(novoQuiz);
```

## 📊 Resposta de Sucesso

```json
{
  "success": true,
  "message": "Quiz criado com sucesso",
  "data": {
    "id": 123,
    "titulo": "Quiz de Matemática Básica",
    "descricao": "Teste seus conhecimentos em operações básicas",
    "categoriaId": 1,
    "dificuldade": "Media",
    "tempoLimite": 30,
    "maxTentativas": 3,
    "publico": true,
    "dataCriacao": "2024-01-15T10:30:00Z",
    "questoes": [
      {
        "id": 456,
        "enunciado": "Qual é o resultado de 2 + 2?",
        "tipoQuestao": "MultiplaEscolha",
        "pontuacao": 10,
        "ordemIndice": 1,
        "opcoes": [
          {
            "id": 789,
            "textoOpcao": "4",
            "correta": true,
            "ordemIndice": 2
          }
        ]
      }
    ]
  }
}
```

## 🚨 Resposta de Erro

```json
{
  "success": false,
  "message": "Erro de validação",
  "errors": [
    "Categoria não encontrada",
    "Pelo menos uma questão é obrigatória"
  ]
}
```

## 💡 Dicas Importantes

1. **Sempre valide os dados no frontend** antes de enviar
2. **Verifique se a categoria existe** consultando `/api/categorias`
3. **Para MultiplaEscolha, sempre inclua opções**
4. **Pelo menos uma opção deve ser correta**
5. **Use ordemIndice sequencial** (1, 2, 3...)
6. **Teste com dados simples primeiro**

## 🔍 Debug

Se ainda der erro 500, verifique os logs do servidor que mostrarão exatamente onde está falhando:

- ✅ "Professor validado: [nome]"
- ✅ "Categoria validada: [nome]"
- ✅ "Quiz criado com ID: [id]"
- ✅ "Questão criada com ID: [id]"
- ✅ "Todas as opções salvas com sucesso"
