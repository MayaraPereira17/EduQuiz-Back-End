# 📚 Guia da API - TecnicoFutebol

## 🎯 **Visão Geral**

API completa para o sistema de Técnico de Futebol do EduQuiz, permitindo gerenciar alunos, acompanhar desempenho e gerar relatórios educacionais.

---

## 🔐 **Autenticação**

Todos os endpoints requerem autenticação JWT:

```http
Authorization: Bearer {token}
```

---

## 📋 **Endpoints Disponíveis**

### **1. Dashboard do Técnico**

#### **Obter Dashboard**
```http
GET /api/tecnico/dashboard
```

**Descrição:** Retorna estatísticas gerais e melhores alunos do mês.

**Resposta:**
```json
{
  "nomeTecnico": "Gabriel Ronny",
  "emailTecnico": "gabs@email.com",
  "totalAlunos": 45,
  "performanceGeral": 76.5,
  "melhoresAlunos": [
    {
      "posicao": 1,
      "nome": "João Silva",
      "sequencia": 12,
      "performance": 92.0,
      "totalQuizzes": 15
    },
    {
      "posicao": 2,
      "nome": "Maria Santos",
      "sequencia": 8,
      "performance": 87.0,
      "totalQuizzes": 12
    },
    {
      "posicao": 3,
      "nome": "Ana Oliveira",
      "sequencia": 15,
      "performance": 85.0,
      "totalQuizzes": 14
    }
  ]
}
```

**Códigos de Resposta:**
- `200 OK` - Dashboard obtido com sucesso
- `400 Bad Request` - Técnico não encontrado ou sem permissão
- `401 Unauthorized` - Token inválido
- `500 Internal Server Error` - Erro interno do servidor

---

### **2. Gerenciar Alunos e Ranking**

#### **Listar Alunos com Ranking**
```http
GET /api/tecnico/alunos?busca={termo}
```

**Parâmetros:**
- `busca` (opcional) - Termo para filtrar alunos por nome ou email

**Exemplos:**
```http
GET /api/tecnico/alunos
GET /api/tecnico/alunos?busca=joão
GET /api/tecnico/alunos?busca=maria
```

**Resposta:**
```json
{
  "alunos": [
    {
      "id": 1,
      "posicao": 1,
      "nome": "João Silva",
      "email": "joao.silva@email.com",
      "idade": 16,
      "totalQuizzes": 24,
      "scoreGeral": 92.0,
      "ultimoQuiz": "2024-01-15T10:30:00Z"
    },
    {
      "id": 2,
      "posicao": 2,
      "nome": "Mariana Costa",
      "email": "mariana.costa@email.com",
      "idade": 17,
      "totalQuizzes": 20,
      "scoreGeral": 89.0,
      "ultimoQuiz": "2024-02-12T14:20:00Z"
    }
  ],
  "totalAlunos": 45
}
```

**Códigos de Resposta:**
- `200 OK` - Lista de alunos obtida com sucesso
- `400 Bad Request` - Técnico não encontrado ou sem permissão
- `401 Unauthorized` - Token inválido
- `500 Internal Server Error` - Erro interno do servidor

---

### **3. Relatório de Desempenho**

#### **Obter Relatório de Desempenho**
```http
GET /api/tecnico/relatorio-desempenho
```

**Descrição:** Retorna análise detalhada do desempenho de todos os alunos.

**Resposta:**
```json
{
  "alunos": [
    {
      "id": 1,
      "nome": "João Silva",
      "totalQuizzes": 15,
      "ultimoQuiz": "2024-01-19T10:30:00Z",
      "scoreGeral": 92.0
    },
    {
      "id": 2,
      "nome": "Mariana Costa",
      "totalQuizzes": 12,
      "ultimoQuiz": "2024-02-05T14:20:00Z",
      "scoreGeral": 88.0
    },
    {
      "id": 3,
      "nome": "Lucas Pereira",
      "totalQuizzes": 20,
      "ultimoQuiz": "2024-03-27T09:15:00Z",
      "scoreGeral": 95.0
    }
  ],
  "totalAlunos": 45,
  "mediaGeral": 76.5
}
```

**Códigos de Resposta:**
- `200 OK` - Relatório obtido com sucesso
- `400 Bad Request` - Técnico não encontrado ou sem permissão
- `401 Unauthorized` - Token inválido
- `500 Internal Server Error` - Erro interno do servidor

---

### **4. Perfil do Técnico**

#### **Obter Perfil**
```http
GET /api/tecnico/perfil
```

**Resposta:**
```json
{
  "id": 1,
  "nome": "Gabriel",
  "sobrenome": "Ronny",
  "email": "gabs@email.com",
  "funcao": "TecnicoFutebol",
  "instituicao": "Instituição de Ensino",
  "totalAlunos": 45,
  "mediaTurma": 76.5
}
```

#### **Atualizar Perfil**
```http
PUT /api/tecnico/perfil
Content-Type: application/json

{
  "nome": "Gabriel",
  "sobrenome": "Ronny",
  "email": "gabs@email.com"
}
```

**Resposta:**
```json
{
  "id": 1,
  "nome": "Gabriel",
  "sobrenome": "Ronny",
  "email": "gabs@email.com",
  "funcao": "TecnicoFutebol",
  "instituicao": "Instituição de Ensino",
  "totalAlunos": 45,
  "mediaTurma": 76.5
}
```

**Códigos de Resposta:**
- `200 OK` - Perfil obtido/atualizado com sucesso
- `400 Bad Request` - Dados inválidos ou técnico não encontrado
- `401 Unauthorized` - Token inválido
- `500 Internal Server Error` - Erro interno do servidor

---

## 🚀 **Exemplos de Integração Frontend**

### **JavaScript/TypeScript**

#### **1. Dashboard**
```javascript
async function obterDashboard() {
  try {
    const response = await fetch('/api/tecnico/dashboard', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (!response.ok) {
      throw new Error('Erro ao obter dashboard');
    }
    
    const dashboard = await response.json();
    console.log('Dashboard:', dashboard);
    return dashboard;
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}
```

#### **2. Listar Alunos com Busca**
```javascript
async function obterAlunos(termoBusca = '') {
  try {
    const url = `/api/tecnico/alunos${termoBusca ? `?busca=${encodeURIComponent(termoBusca)}` : ''}`;
    
    const response = await fetch(url, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (!response.ok) {
      throw new Error('Erro ao obter alunos');
    }
    
    const dados = await response.json();
    console.log('Alunos:', dados);
    return dados;
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}
```

#### **3. Relatório de Desempenho**
```javascript
async function obterRelatorioDesempenho() {
  try {
    const response = await fetch('/api/tecnico/relatorio-desempenho', {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (!response.ok) {
      throw new Error('Erro ao obter relatório');
    }
    
    const relatorio = await response.json();
    console.log('Relatório:', relatorio);
    return relatorio;
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}
```

#### **4. Atualizar Perfil**
```javascript
async function atualizarPerfil(dadosPerfil) {
  try {
    const response = await fetch('/api/tecnico/perfil', {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(dadosPerfil)
    });
    
    if (!response.ok) {
      throw new Error('Erro ao atualizar perfil');
    }
    
    const perfilAtualizado = await response.json();
    console.log('Perfil atualizado:', perfilAtualizado);
    return perfilAtualizado;
  } catch (error) {
    console.error('Erro:', error);
    throw error;
  }
}
```

### **React/Next.js**

#### **Hook Personalizado**
```typescript
import { useState, useEffect } from 'react';

export function useTecnicoAPI() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const apiCall = async (endpoint: string, options: RequestInit = {}) => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await fetch(endpoint, {
        ...options,
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
          ...options.headers
        }
      });
      
      if (!response.ok) {
        throw new Error(`Erro ${response.status}: ${response.statusText}`);
      }
      
      return await response.json();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    error,
    apiCall
  };
}
```

#### **Componente de Dashboard**
```typescript
import React, { useState, useEffect } from 'react';
import { useTecnicoAPI } from './hooks/useTecnicoAPI';

interface DashboardTecnicoDTO {
  nomeTecnico: string;
  emailTecnico: string;
  totalAlunos: number;
  performanceGeral: number;
  melhoresAlunos: Array<{
    posicao: number;
    nome: string;
    sequencia: number;
    performance: number;
    totalQuizzes: number;
  }>;
}

export function DashboardTecnico() {
  const { loading, error, apiCall } = useTecnicoAPI();
  const [dashboard, setDashboard] = useState<DashboardTecnicoDTO | null>(null);

  useEffect(() => {
    const carregarDashboard = async () => {
      try {
        const dados = await apiCall('/api/tecnico/dashboard');
        setDashboard(dados);
      } catch (err) {
        console.error('Erro ao carregar dashboard:', err);
      }
    };

    carregarDashboard();
  }, []);

  if (loading) return <div>Carregando...</div>;
  if (error) return <div>Erro: {error}</div>;
  if (!dashboard) return <div>Nenhum dado disponível</div>;

  return (
    <div>
      <h1>Dashboard Técnico</h1>
      <h2>Bem-vindo, {dashboard.nomeTecnico}</h2>
      <p>Email: {dashboard.emailTecnico}</p>
      
      <div className="stats">
        <div className="stat-card">
          <h3>Alunos Ativos</h3>
          <p>{dashboard.totalAlunos}</p>
        </div>
        <div className="stat-card">
          <h3>Performance Geral</h3>
          <p>{dashboard.performanceGeral}%</p>
        </div>
      </div>

      <div className="melhores-alunos">
        <h3>Melhores Alunos do Mês</h3>
        {dashboard.melhoresAlunos.map(aluno => (
          <div key={aluno.posicao} className="aluno-card">
            <span>#{aluno.posicao}</span>
            <span>{aluno.nome}</span>
            <span>{aluno.performance}%</span>
            <span>{aluno.totalQuizzes} quizzes</span>
          </div>
        ))}
      </div>
    </div>
  );
}
```

---

## 📊 **Estrutura de Dados**

### **DashboardTecnicoDTO**
```typescript
interface DashboardTecnicoDTO {
  nomeTecnico: string;
  emailTecnico: string;
  totalAlunos: number;
  performanceGeral: number;
  melhoresAlunos: MelhorAlunoDTO[];
}
```

### **AlunoRankingDTO**
```typescript
interface AlunoRankingDTO {
  id: number;
  posicao: number;
  nome: string;
  email: string;
  idade: number;
  totalQuizzes: number;
  scoreGeral: number;
  ultimoQuiz: string | null;
}
```

### **DesempenhoAlunoDTO**
```typescript
interface DesempenhoAlunoDTO {
  id: number;
  nome: string;
  totalQuizzes: number;
  ultimoQuiz: string | null;
  scoreGeral: number;
}
```

### **PerfilTecnicoDTO**
```typescript
interface PerfilTecnicoDTO {
  id: number;
  nome: string;
  sobrenome: string;
  email: string;
  funcao: string;
  instituicao: string;
  totalAlunos: number;
  mediaTurma: number;
}
```

---

## 🔧 **Configuração e Deploy**

### **1. Registrar Serviços**
O serviço já está registrado no `Program.cs`:
```csharp
builder.Services.AddScoped<ITecnicoFutebolService, TecnicoFutebolService>();
```

### **2. Autenticação**
Todos os endpoints requerem autenticação JWT válida.

### **3. CORS**
Configurado para permitir requisições do frontend.

---

## 🎯 **Funcionalidades Implementadas**

| Funcionalidade | Endpoint | Descrição |
|----------------|----------|-----------|
| **Dashboard** | `GET /api/tecnico/dashboard` | Estatísticas gerais + melhores alunos |
| **Ranking** | `GET /api/tecnico/alunos` | Lista alunos com ranking + busca |
| **Relatórios** | `GET /api/tecnico/relatorio-desempenho` | Análise detalhada de desempenho |
| **Perfil** | `GET /api/tecnico/perfil` | Dados do técnico |
| **Editar Perfil** | `PUT /api/tecnico/perfil` | Atualizar dados do técnico |

---

## ✅ **Status da Implementação**

- ✅ **Serviço** - Implementado
- ✅ **Controller** - Implementado  
- ✅ **DTOs** - Implementados
- ✅ **Registro** - Configurado
- ✅ **Documentação** - Completa

**API pronta para integração no frontend!** 🚀✨
