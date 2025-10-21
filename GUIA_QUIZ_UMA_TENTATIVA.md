# 🎯 Guia: Sistema de Quiz com Uma Tentativa

## 📋 Visão Geral

O sistema EduQuiz foi configurado para permitir que cada aluno faça cada quiz **apenas uma vez**. Após a conclusão, o quiz é marcado como "feito" e não pode ser realizado novamente.

---

## 🔧 Funcionalidades Implementadas

### ✅ **1. Limitação de Tentativas**
- Cada usuário pode fazer cada quiz **apenas 1 vez**
- Sistema bloqueia automaticamente tentativas duplicadas
- Mensagens de erro claras quando usuário tenta refazer

### ✅ **2. Status de Conclusão**
- Quizzes concluídos são marcados como "feito"
- Frontend recebe status `QuizConcluido: true`
- Campo `TentativasRestantes: 0` para quizzes já feitos

### ✅ **3. Bloqueio de Acesso**
- Endpoints verificam se quiz já foi realizado
- Retornam erro 400/409 com mensagem explicativa
- Impedem início de nova tentativa

---

## 📡 Endpoints da API

### **1. Listar Quizzes com Status**
```http
GET /api/aluno/quizzes
Authorization: Bearer {token}
```

**Resposta:**
```json
[
  {
    "id": 1,
    "titulo": "Quiz de Matemática Básica",
    "descricao": "Operações fundamentais",
    "categoria": "Matemática Básica",
    "dificuldade": "Media",
    "tempoLimite": 30,
    "totalQuestoes": 10,
    "pontuacaoTotal": 10,
    "disponivel": true,
    "quizConcluido": false,    // ← NOVO: false = pode fazer
    "tentativasRestantes": 1   // ← NOVO: sempre 1 para novos
  },
  {
    "id": 2,
    "titulo": "Quiz de Álgebra",
    "descricao": "Equações lineares",
    "categoria": "Álgebra",
    "dificuldade": "Dificil",
    "tempoLimite": 45,
    "totalQuestoes": 15,
    "pontuacaoTotal": 15,
    "disponivel": false,       // ← NOVO: false = já feito
    "quizConcluido": true,     // ← NOVO: true = já concluído
    "tentativasRestantes": 0   // ← NOVO: 0 = não pode mais
  }
]
```

### **2. Iniciar Quiz (Bloqueado se já feito)**
```http
POST /api/aluno/quizzes/iniciar
Authorization: Bearer {token}
Content-Type: application/json

{
  "quizId": 1
}
```

**Se já foi feito:**
```json
{
  "error": "Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez."
}
```

### **3. Responder Quiz (Bloqueado se já feito)**
```http
POST /api/aluno/quizzes/{quizId}/responder
Authorization: Bearer {token}
Content-Type: application/json

{
  "respostas": [...]
}
```

**Se já foi feito:**
```json
{
  "error": "Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez."
}
```

### **4. Ver Detalhes do Quiz (Bloqueado se já feito)**
```http
GET /api/aluno/quizzes/{quizId}
Authorization: Bearer {token}
```

**Se já foi feito:**
```json
{
  "error": "Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez."
}
```

---

## 🎨 Implementação no Frontend

### **Status dos Quizzes:**

| Campo | Valor | Significado |
|-------|-------|-------------|
| `quizConcluido` | `false` | Quiz não foi feito ainda |
| `quizConcluido` | `true` | Quiz já foi concluído |
| `tentativasRestantes` | `1` | Pode fazer o quiz |
| `tentativasRestantes` | `0` | Não pode mais fazer |
| `disponivel` | `true` | Quiz disponível |
| `disponivel` | `false` | Quiz não disponível (já feito) |

### **Lógica de Exibição:**

```javascript
// Verificar status do quiz
if (quiz.quizConcluido) {
  // Quiz já foi feito
  showQuizCompleted(quiz);
  disableQuizButton();
} else {
  // Quiz pode ser feito
  showQuizAvailable(quiz);
  enableQuizButton();
}

function showQuizCompleted(quiz) {
  // Mostrar status "FEITO" ou "CONCLUÍDO"
  // Exibir pontuação obtida (se disponível)
  // Desabilitar botão "Fazer Quiz"
}

function showQuizAvailable(quiz) {
  // Mostrar botão "Fazer Quiz"
  // Permitir acesso ao quiz
  // Mostrar informações do quiz
}
```

### **Componente React Exemplo:**

```jsx
function QuizCard({ quiz }) {
  const isCompleted = quiz.quizConcluido;
  const canTake = quiz.tentativasRestantes > 0;

  return (
    <div className={`quiz-card ${isCompleted ? 'completed' : 'available'}`}>
      <h3>{quiz.titulo}</h3>
      <p>{quiz.descricao}</p>
      
      {isCompleted ? (
        <div className="quiz-completed">
          <span className="status">✅ FEITO</span>
          <button disabled>Já Concluído</button>
        </div>
      ) : (
        <div className="quiz-available">
          <span className="status">🎯 Disponível</span>
          <button onClick={() => startQuiz(quiz.id)}>
            Fazer Quiz
          </button>
        </div>
      )}
    </div>
  );
}
```

---

## 🔒 Regras de Negócio

### **Antes de Iniciar Quiz:**
1. ✅ Verificar se `quizConcluido = false`
2. ✅ Verificar se `tentativasRestantes > 0`
3. ✅ Verificar se `disponivel = true`

### **Após Concluir Quiz:**
1. ✅ Sistema marca `Concluida = true`
2. ✅ Próxima consulta retorna `quizConcluido = true`
3. ✅ Próxima consulta retorna `tentativasRestantes = 0`
4. ✅ Próxima consulta retorna `disponivel = false`

### **Tentativa de Refazer:**
1. ❌ API retorna erro 400/409
2. ❌ Mensagem: "Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez."
3. ❌ Frontend deve tratar o erro e mostrar status correto

---

## 🚨 Tratamento de Erros

### **Erro 400 - Bad Request:**
```json
{
  "error": "Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez."
}
```

### **Erro 409 - Conflict:**
```json
{
  "error": "Você já realizou este quiz. Cada quiz pode ser feito apenas uma vez."
}
```

### **Tratamento no Frontend:**
```javascript
try {
  const response = await startQuiz(quizId);
  // Quiz iniciado com sucesso
} catch (error) {
  if (error.status === 400 || error.status === 409) {
    // Quiz já foi feito
    showQuizCompleted();
    updateQuizStatus(quizId, { quizConcluido: true });
  }
}
```

---

## 📊 Fluxo Completo

### **1. Usuário Acessa Lista de Quizzes:**
```
GET /api/aluno/quizzes
↓
Frontend recebe lista com status
↓
Mostra quizzes disponíveis e concluídos
```

### **2. Usuário Tenta Fazer Quiz Novo:**
```
POST /api/aluno/quizzes/iniciar
↓
Sistema permite início
↓
Usuário faz o quiz
↓
Sistema marca como concluído
```

### **3. Usuário Tenta Fazer Quiz Já Feito:**
```
POST /api/aluno/quizzes/iniciar
↓
Sistema bloqueia com erro
↓
Frontend mostra "Já Concluído"
```

---

## 🎯 Benefícios da Implementação

### **Para o Sistema:**
- ✅ Evita spam de tentativas
- ✅ Dados mais confiáveis no ranking
- ✅ Performance melhorada
- ✅ Lógica de negócio clara

### **Para o Usuário:**
- ✅ Interface clara sobre status dos quizzes
- ✅ Não perde tempo tentando refazer
- ✅ Foco em novos conteúdos
- ✅ Sensação de progresso

### **Para o Frontend:**
- ✅ Estados bem definidos
- ✅ Tratamento de erro simples
- ✅ UX consistente
- ✅ Fácil implementação

---

## 🔧 Configurações Técnicas

### **Banco de Dados:**
- `MaxTentativas` = 1 (padrão)
- Verificação de tentativas existentes
- Índices para performance

### **Backend:**
- Validação em todos os endpoints de quiz
- Mensagens de erro padronizadas
- Status de conclusão no DTO

### **Frontend:**
- Campos `quizConcluido` e `tentativasRestantes`
- Tratamento de erros 400/409
- Estados visuais claros

---

## 📝 Resumo

O sistema agora garante que cada aluno faça cada quiz apenas uma vez, com:

1. **Bloqueio automático** de tentativas duplicadas
2. **Status claro** para o frontend
3. **Mensagens de erro** informativas
4. **Interface intuitiva** para o usuário

**Resultado:** Sistema mais justo, confiável e com melhor experiência do usuário! 🚀
