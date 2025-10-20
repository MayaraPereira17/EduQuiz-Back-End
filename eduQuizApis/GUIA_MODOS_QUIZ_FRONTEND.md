# Guia Completo - Modos de Responder Quiz (Frontend)

Este documento detalha os **dois modos** disponíveis para responder quizzes no EduQuiz, com exemplos práticos de implementação.

---

## 🎯 **Visão Geral dos Modos**

### **Modo 1: Quiz Completo (Tradicional)**
- Responde **todas as questões** de uma vez
- Recebe **resultado completo** no final
- Ideal para: Quizzes rápidos, formulários simples

### **Modo 2: Questão por Questão (Dinâmico)**
- Responde **uma questão por vez**
- Recebe **feedback imediato** após cada resposta
- Ideal para: Experiência interativa, gamificação

---

## 📚 **Modo 1: Quiz Completo (Tradicional)**

### **Fluxo do Modo Completo:**
1. **Obter quiz** → `GET /api/aluno/quizzes/{id}`
2. **Exibir todas as questões** para o usuário
3. **Coletar todas as respostas** do formulário
4. **Enviar tudo de uma vez** → `POST /api/aluno/quizzes/{id}/responder`
5. **Exibir resultado completo**

### **Endpoint Principal:**
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

### **Resposta do Modo Completo:**
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
      "textoRespostaSelecionada": "8",
      "opcaoCorretaId": 1002,
      "textoRespostaCorreta": "8",
      "correta": true,
      "pontosObtidos": 10
    },
    {
      "questaoId": 102,
      "opcaoSelecionadaId": 1006,
      "textoRespostaSelecionada": "Brasília",
      "opcaoCorretaId": 1006,
      "textoRespostaCorreta": "Brasília",
      "correta": true,
      "pontosObtidos": 15
    }
  ],
  "message": "Quiz respondido com sucesso!",
  "novoRecorde": false
}
```

### **Implementação JavaScript - Modo Completo:**
```javascript
class QuizCompleto {
  constructor(quizId, token) {
    this.quizId = quizId;
    this.token = token;
    this.respostas = [];
  }

  // 1. Carregar quiz
  async carregarQuiz() {
    const response = await fetch(`/api/aluno/quizzes/${this.quizId}`, {
      headers: {
        'Authorization': `Bearer ${this.token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (!response.ok) throw new Error('Erro ao carregar quiz');
    
    this.quiz = await response.json();
    this.exibirQuiz();
  }

  // 2. Exibir todas as questões
  exibirQuiz() {
    const container = document.getElementById('quiz-container');
    
    this.quiz.questoes.forEach((questao, index) => {
      const questaoDiv = document.createElement('div');
      questaoDiv.className = 'questao';
      questaoDiv.innerHTML = `
        <h3>Questão ${index + 1}: ${questao.textoQuestao}</h3>
        <div class="opcoes">
          ${questao.opcoes.map(opcao => `
            <label>
              <input type="radio" 
                     name="questao_${questao.id}" 
                     value="${opcao.id}"
                     data-questao-id="${questao.id}">
              ${opcao.textoOpcao}
            </label>
          `).join('')}
        </div>
      `;
      container.appendChild(questaoDiv);
    });

    // Adicionar botão de enviar
    const enviarBtn = document.createElement('button');
    enviarBtn.textContent = 'Enviar Quiz';
    enviarBtn.onclick = () => this.enviarQuiz();
    container.appendChild(enviarBtn);
  }

  // 3. Coletar respostas e enviar
  async enviarQuiz() {
    // Coletar todas as respostas
    this.respostas = [];
    const radios = document.querySelectorAll('input[type="radio"]:checked');
    
    radios.forEach(radio => {
      this.respostas.push({
        questaoId: parseInt(radio.dataset.questaoId),
        opcaoSelecionadaId: parseInt(radio.value)
      });
    });

    // Enviar para o servidor
    try {
      const response = await fetch(`/api/aluno/quizzes/${this.quizId}/responder`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ respostas: this.respostas })
      });

      if (!response.ok) throw new Error('Erro ao enviar quiz');

      const resultado = await response.json();
      this.exibirResultado(resultado);
      
    } catch (error) {
      console.error('Erro:', error);
      alert('Erro ao enviar quiz: ' + error.message);
    }
  }

  // 4. Exibir resultado completo
  exibirResultado(resultado) {
    const container = document.getElementById('resultado-container');
    container.innerHTML = `
      <h2>Resultado do Quiz</h2>
      <p>Pontuação: ${resultado.pontuacaoTotal}/${resultado.pontuacaoMaxima}</p>
      <p>Percentual: ${resultado.percentualAcerto}%</p>
      <p>Respostas Corretas: ${resultado.respostasCorretas}</p>
      <p>Respostas Incorretas: ${resultado.respostasIncorretas}</p>
      
      <h3>Detalhamento por Questão:</h3>
      ${resultado.respostas.map(resposta => `
        <div class="resposta ${resposta.correta ? 'correta' : 'incorreta'}">
          <p><strong>Questão ${resposta.questaoId}:</strong></p>
          <p>Sua resposta: ${resposta.textoRespostaSelecionada}</p>
          <p>Resposta correta: ${resposta.textoRespostaCorreta}</p>
          <p>Pontos: ${resposta.pontosObtidos}</p>
        </div>
      `).join('')}
    `;
  }
}

// Uso
const quiz = new QuizCompleto(1, 'seu_jwt_token');
quiz.carregarQuiz();
```

---

## 🎮 **Modo 2: Questão por Questão (Dinâmico)**

### **Fluxo do Modo Dinâmico:**
1. **Iniciar quiz** → `POST /api/aluno/quizzes/iniciar`
2. **Exibir primeira questão**
3. **Coletar resposta** da questão atual
4. **Enviar resposta** → `POST /api/aluno/tentativas/{tentativaId}/responder`
5. **Receber feedback imediato** (pontos, se acertou, próxima questão)
6. **Repetir** até finalizar

### **Endpoints do Modo Dinâmico:**

#### **1. Iniciar Quiz:**
```http
POST /api/aluno/quizzes/iniciar
Authorization: Bearer {token}
Content-Type: application/json

{
  "quizId": 1
}
```

**Resposta:**
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

#### **2. Responder Questão:**
```http
POST /api/aluno/tentativas/{tentativaId}/responder
Authorization: Bearer {token}
Content-Type: application/json

{
  "questaoId": 101,
  "opcaoSelecionadaId": 1002
}
```

**Resposta:**
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
      },
      {
        "id": 1006,
        "textoOpcao": "Brasília",
        "ordemIndice": 3
      }
    ],
    "pontos": 15,
    "ordemIndice": 2
  },
  "quizConcluido": false,
  "resultadoFinal": null
}
```

### **Implementação JavaScript - Modo Dinâmico:**
```javascript
class QuizDinamico {
  constructor(quizId, token) {
    this.quizId = quizId;
    this.token = token;
    this.tentativaId = null;
    this.pontuacaoTotal = 0;
    this.questaoAtual = null;
    this.progresso = null;
  }

  // 1. Iniciar quiz
  async iniciarQuiz() {
    try {
      const response = await fetch('/api/aluno/quizzes/iniciar', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ quizId: this.quizId })
      });

      if (!response.ok) throw new Error('Erro ao iniciar quiz');

      const resultado = await response.json();
      this.tentativaId = resultado.tentativaId;
      this.questaoAtual = resultado.questaoAtual;
      this.progresso = resultado.progresso;
      
      this.exibirQuestao();
      this.atualizarProgresso();
      
    } catch (error) {
      console.error('Erro:', error);
      alert('Erro ao iniciar quiz: ' + error.message);
    }
  }

  // 2. Exibir questão atual
  exibirQuestao() {
    const container = document.getElementById('quiz-container');
    container.innerHTML = `
      <div class="questao-atual">
        <h2>${this.questaoAtual.textoQuestao}</h2>
        <p>Pontos: ${this.questaoAtual.pontos}</p>
        
        <div class="opcoes">
          ${this.questaoAtual.opcoes.map(opcao => `
            <label class="opcao">
              <input type="radio" 
                     name="resposta" 
                     value="${opcao.id}">
              ${opcao.textoOpcao}
            </label>
          `).join('')}
        </div>
        
        <button onclick="quiz.responderQuestao()">Responder</button>
      </div>
    `;
  }

  // 3. Responder questão
  async responderQuestao() {
    const opcaoSelecionada = document.querySelector('input[name="resposta"]:checked');
    
    if (!opcaoSelecionada) {
      alert('Selecione uma opção!');
      return;
    }

    try {
      const response = await fetch(`/api/aluno/tentativas/${this.tentativaId}/responder`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          questaoId: this.questaoAtual.id,
          opcaoSelecionadaId: parseInt(opcaoSelecionada.value)
        })
      });

      if (!response.ok) throw new Error('Erro ao responder questão');

      const resultado = await response.json();
      this.processarResposta(resultado);
      
    } catch (error) {
      console.error('Erro:', error);
      alert('Erro ao responder questão: ' + error.message);
    }
  }

  // 4. Processar resposta e feedback
  processarResposta(resultado) {
    // Atualizar pontuação
    this.pontuacaoTotal += resultado.pontosGanhos;
    
    // Exibir feedback
    this.exibirFeedback(resultado);
    
    // Verificar se quiz terminou
    if (resultado.quizConcluido) {
      this.exibirResultadoFinal(resultado.resultadoFinal);
    } else {
      // Preparar próxima questão
      setTimeout(() => {
        this.questaoAtual = resultado.proximaQuestao;
        this.atualizarProgresso();
        this.exibirQuestao();
      }, 2000); // 2 segundos para mostrar feedback
    }
  }

  // 5. Exibir feedback imediato
  exibirFeedback(resultado) {
    const feedbackDiv = document.createElement('div');
    feedbackDiv.className = `feedback ${resultado.respostaCorreta ? 'correto' : 'incorreto'}`;
    feedbackDiv.innerHTML = `
      <h3>${resultado.respostaCorreta ? '✅ Correto!' : '❌ Incorreto!'}</h3>
      <p>Pontos ganhos: ${resultado.pontosGanhos}</p>
      <p>Resposta correta: ${resultado.respostaCorretaTexto}</p>
      <p>Feedback: ${resultado.feedback}</p>
      <p>Pontuação total: ${this.pontuacaoTotal}</p>
    `;
    
    document.getElementById('quiz-container').appendChild(feedbackDiv);
  }

  // 6. Atualizar barra de progresso
  atualizarProgresso() {
    const progressoDiv = document.getElementById('progresso');
    if (progressoDiv) {
      progressoDiv.innerHTML = `
        <div class="barra-progresso">
          <div class="progresso" style="width: ${this.progresso.percentualCompleto}%"></div>
        </div>
        <p>Questão ${this.progresso.questaoAtual} de ${this.progresso.totalQuestoes}</p>
        <p>Pontuação: ${this.pontuacaoTotal}</p>
      `;
    }
  }

  // 7. Exibir resultado final
  exibirResultadoFinal(resultado) {
    const container = document.getElementById('quiz-container');
    container.innerHTML = `
      <div class="resultado-final">
        <h2>🎉 Quiz Concluído!</h2>
        <p>Pontuação Final: ${resultado.pontuacaoFinal}/${resultado.pontuacaoMaxima}</p>
        <p>Percentual: ${resultado.percentualAcerto}%</p>
        <p>Respostas Corretas: ${resultado.respostasCorretas}</p>
        <p>Respostas Incorretas: ${resultado.respostasErradas}</p>
        <p>Tempo Gasto: ${resultado.tempoGasto} segundos</p>
      </div>
    `;
  }
}

// Uso
const quiz = new QuizDinamico(1, 'seu_jwt_token');
quiz.iniciarQuiz();
```

---

## 🎨 **Exemplo de Interface HTML**

### **HTML Base:**
```html
<!DOCTYPE html>
<html>
<head>
    <title>EduQuiz - Modo Dinâmico</title>
    <style>
        .questao-atual {
            border: 1px solid #ddd;
            padding: 20px;
            margin: 20px 0;
            border-radius: 8px;
        }
        
        .opcao {
            display: block;
            margin: 10px 0;
            padding: 10px;
            border: 1px solid #eee;
            border-radius: 4px;
            cursor: pointer;
        }
        
        .opcao:hover {
            background-color: #f5f5f5;
        }
        
        .feedback {
            padding: 15px;
            margin: 10px 0;
            border-radius: 4px;
        }
        
        .feedback.correto {
            background-color: #d4edda;
            border: 1px solid #c3e6cb;
            color: #155724;
        }
        
        .feedback.incorreto {
            background-color: #f8d7da;
            border: 1px solid #f5c6cb;
            color: #721c24;
        }
        
        .barra-progresso {
            width: 100%;
            height: 20px;
            background-color: #e9ecef;
            border-radius: 10px;
            overflow: hidden;
        }
        
        .progresso {
            height: 100%;
            background-color: #007bff;
            transition: width 0.3s ease;
        }
        
        .resultado-final {
            text-align: center;
            padding: 40px;
            background-color: #f8f9fa;
            border-radius: 8px;
        }
    </style>
</head>
<body>
    <div id="progresso"></div>
    <div id="quiz-container"></div>
    <div id="resultado-container"></div>
    
    <script>
        // Código JavaScript aqui
    </script>
</body>
</html>
```

---

## ⚖️ **Comparação dos Modos**

| Aspecto | Modo Completo | Modo Dinâmico |
|---------|---------------|---------------|
| **Feedback** | Apenas no final | Imediato por questão |
| **Pontuação** | Calculada no final | Acumulada em tempo real |
| **Experiência** | Formulário tradicional | Gamificada e interativa |
| **Controle** | Usuário vê todas as questões | Sistema controla o fluxo |
| **Pausar/Retomar** | Não | Sim (via tentativaId) |
| **Validação** | No envio | A cada resposta |
| **Performance** | 1 requisição | N requisições (1 por questão) |
| **Complexidade** | Simples | Mais complexa |

---

## 🎯 **Recomendações de Uso**

### **Use Modo Completo quando:**
- ✅ Quiz é curto (≤ 10 questões)
- ✅ Formulário tradicional é adequado
- ✅ Performance é crítica (menos requisições)
- ✅ Usuário prefere ver todas as questões

### **Use Modo Dinâmico quando:**
- ✅ Quiz é longo (> 10 questões)
- ✅ Quer gamificar a experiência
- ✅ Precisa de feedback imediato
- ✅ Quer controlar o fluxo (timer, pausar, etc.)
- ✅ Quer mostrar progresso em tempo real

---

## 🚀 **Próximos Passos**

1. **Escolha o modo** baseado nas suas necessidades
2. **Implemente a interface** usando os exemplos fornecidos
3. **Teste os endpoints** com dados reais
4. **Personalize** a experiência conforme necessário
5. **Adicione funcionalidades** extras (timer, animações, etc.)

---

**🎉 Agora você tem tudo que precisa para implementar ambos os modos de quiz no frontend!**
