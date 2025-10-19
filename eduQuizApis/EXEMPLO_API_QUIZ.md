# 📚 Exemplo de Uso da API de Criação de Quiz

## Endpoint
```
POST /api/professor/quizzes
```

## Headers
```
Content-Type: application/json
Authorization: Bearer {seu_token_jwt}
```

## Exemplo de Payload Válido

### 1. Quiz Simples (Múltipla Escolha)
```json
{
  "titulo": "Matemática Básica",
  "descricao": "Quiz sobre operações matemáticas fundamentais",
  "categoriaId": 6,
  "dificuldade": "Media",
  "tempoLimite": 30,
  "maxTentativas": 1,
  "publico": true,
  "questoes": [
    {
      "textoQuestao": "Quanto é 2 + 2?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 1,
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
      "textoQuestao": "Qual é a raiz quadrada de 16?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 2,
      "ordemIndice": 2,
      "opcoes": [
        {
          "textoOpcao": "4",
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

### 2. Quiz com Dificuldades Diferentes
```json
{
  "titulo": "História do Brasil",
  "descricao": "Quiz sobre fatos históricos do Brasil",
  "categoriaId": 1,
  "dificuldade": "Dificil",
  "tempoLimite": 45,
  "maxTentativas": 3,
  "publico": false,
  "questoes": [
    {
      "textoQuestao": "Em que ano o Brasil foi descoberto?",
      "tipoQuestao": "MultiplaEscolha",
      "pontos": 1,
      "ordemIndice": 1,
      "opcoes": [
        {
          "textoOpcao": "1500",
          "correta": true,
          "ordemIndice": 1
        },
        {
          "textoOpcao": "1492",
          "correta": false,
          "ordemIndice": 2
        },
        {
          "textoOpcao": "1501",
          "correta": false,
          "ordemIndice": 3
        }
      ]
    }
  ]
}
```

## Valores Aceitos

### Dificuldade
- `"Facil"` ou `"Fácil"`
- `"Media"` ou `"Médio"` ou `"Média"`
- `"Dificil"` ou `"Difícil"`

### Tipo de Questão
- `"MultiplaEscolha"`
- `"VerdadeiroFalso"`
- `"Preenchimento"`
- `"Dissertativa"`

### Campos Obrigatórios
- `titulo` (string, máximo 200 caracteres)
- `categoriaId` (integer)
- `dificuldade` (string)
- `questoes` (array, pelo menos 1 questão)

### Campos Opcionais
- `descricao` (string)
- `tempoLimite` (integer, em minutos)
- `maxTentativas` (integer, padrão: 1)
- `publico` (boolean, padrão: false)

## Resposta de Sucesso
```json
{
  "id": 123,
  "titulo": "Matemática Básica",
  "mensagem": "Quiz criado com sucesso!"
}
```

## Possíveis Erros

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Titulo": ["O campo Titulo é obrigatório."],
    "CategoriaId": ["O campo CategoriaId é obrigatório."]
  }
}
```

### 500 Internal Server Error
- Verifique se a coluna `Dificuldade` foi adicionada no banco
- Verifique se a categoria existe
- Verifique os logs do servidor

## Script SQL para Adicionar Coluna Dificuldade
```sql
USE railway;

ALTER TABLE Quizzes 
ADD COLUMN Dificuldade VARCHAR(20) NOT NULL DEFAULT 'Media' 
AFTER CriadoPor;

UPDATE Quizzes SET Dificuldade = 'Media' WHERE Dificuldade IS NULL;

ALTER TABLE Quizzes 
ADD CONSTRAINT chk_dificuldade 
CHECK (Dificuldade IN ('Facil', 'Media', 'Dificil'));
```

## Teste com cURL
```bash
curl -X POST "https://sua-api.com/api/professor/quizzes" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "titulo": "Teste Quiz",
    "descricao": "Quiz de teste",
    "categoriaId": 6,
    "dificuldade": "Media",
    "tempoLimite": 30,
    "maxTentativas": 1,
    "publico": true,
    "questoes": [
      {
        "textoQuestao": "1 + 1 = ?",
        "tipoQuestao": "MultiplaEscolha",
        "pontos": 1,
        "ordemIndice": 1,
        "opcoes": [
          {
            "textoOpcao": "2",
            "correta": true,
            "ordemIndice": 1
          },
          {
            "textoOpcao": "3",
            "correta": false,
            "ordemIndice": 2
          }
        ]
      }
    ]
  }'
```
