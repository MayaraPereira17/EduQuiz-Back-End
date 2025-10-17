# 📸 Sistema de Avatar - EduQuiz API

## 🎯 Visão Geral

O sistema de avatar permite que usuários façam upload e gerenciem suas fotos de perfil no EduQuiz. O sistema suporta upload de arquivos de imagem e atualização via URL.

## 🗃️ Estrutura do Banco de Dados

### Tabela `users`
```sql
ALTER TABLE users ADD avatar_url VARCHAR(500) NULL;
```

## 🛣️ Rotas da API

### 1. **POST /api/auth/avatar** - Upload de Avatar

#### **Descrição**
Faz upload de um arquivo de imagem e atualiza automaticamente o avatar do usuário.

#### **Headers**
```
Authorization: Bearer {jwt_token}
Content-Type: multipart/form-data
```

#### **Body (FormData)**
```
file: [arquivo de imagem]
```

#### **Validações**
- ✅ **Tipos permitidos**: `image/jpeg`, `image/jpg`, `image/png`, `image/gif`
- ✅ **Tamanho máximo**: 5MB
- ✅ **Autenticação**: Obrigatória

#### **Resposta de Sucesso (200)**
```json
{
  "success": true,
  "message": "Avatar enviado com sucesso",
  "data": "/avatars/123_20241016164230.jpg",
  "timestamp": "2024-10-16T16:42:30Z"
}
```

#### **Resposta de Erro (400)**
```json
{
  "success": false,
  "message": "Tipo de arquivo inválido. Apenas imagens (JPEG, PNG, GIF) são permitidas",
  "errors": null,
  "timestamp": "2024-10-16T16:42:30Z"
}
```

---

### 2. **PUT /api/auth/profile** - Atualizar Perfil

#### **Descrição**
Atualiza o perfil do usuário, incluindo o avatar via URL.

#### **Headers**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

#### **Body (JSON)**
```json
{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@exemplo.com",
  "cpf": "123.456.789-00",
  "dataNascimento": "1990-05-15T00:00:00Z",
  "avatarUrl": "https://example.com/foto.jpg"
}
```

#### **Resposta de Sucesso (200)**
```json
{
  "success": true,
  "message": "Perfil atualizado com sucesso",
  "data": {
    "id": 123,
    "username": "joao123",
    "email": "joao@exemplo.com",
    "firstName": "João",
    "lastName": "Silva",
    "fullName": "João Silva",
    "cpf": "123.456.789-00",
    "dataNascimento": "1990-05-15T00:00:00Z",
    "avatarUrl": "https://example.com/foto.jpg",
    "role": "Aluno",
    "isActive": true,
    "createdAt": "2024-10-01T10:00:00Z"
  },
  "timestamp": "2024-10-16T16:42:30Z"
}
```

---

### 3. **GET /api/auth/profile** - Buscar Perfil

#### **Descrição**
Retorna o perfil completo do usuário, incluindo o avatar.

#### **Headers**
```
Authorization: Bearer {jwt_token}
```

#### **Resposta de Sucesso (200)**
```json
{
  "success": true,
  "message": "Perfil obtido com sucesso",
  "data": {
    "id": 123,
    "username": "joao123",
    "email": "joao@exemplo.com",
    "firstName": "João",
    "lastName": "Silva",
    "fullName": "João Silva",
    "cpf": "123.456.789-00",
    "dataNascimento": "1990-05-15T00:00:00Z",
    "avatarUrl": "/avatars/123_20241016164230.jpg",
    "role": "Aluno",
    "isActive": true,
    "createdAt": "2024-10-01T10:00:00Z"
  },
  "timestamp": "2024-10-16T16:42:30Z"
}
```

---

### 4. **POST /api/auth/register** - Registro com Avatar

#### **Body (JSON)**
```json
{
  "username": "joao123",
  "email": "joao@exemplo.com",
  "password": "senha123",
  "confirmPassword": "senha123",
  "firstName": "João",
  "lastName": "Silva",
  "cpf": "123.456.789-00",
  "dataNascimento": "1990-05-15T00:00:00Z",
  "role": "Aluno",
  "avatarUrl": "https://example.com/foto.jpg"
}
```

## 🌐 URLs dos Avatares

### **Desenvolvimento**
```
http://localhost:5034/avatars/123_20241016164230.jpg
```

### **Produção**
```
https://eduquiz-back-end-production.up.railway.app/avatars/123_20241016164230.jpg
```

## 💻 Implementação no Frontend

### 1. **Upload de Avatar (JavaScript/React)**

```javascript
const uploadAvatar = async (file, token) => {
  const formData = new FormData();
  formData.append('file', file);

  try {
    const response = await fetch('/api/auth/avatar', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });

    const result = await response.json();
    
    if (result.success) {
      console.log('Avatar URL:', result.data);
      return result.data;
    } else {
      throw new Error(result.message);
    }
  } catch (error) {
    console.error('Erro no upload:', error);
    throw error;
  }
};
```

### 2. **Atualizar Perfil (JavaScript/React)**

```javascript
const updateProfile = async (profileData, token) => {
  try {
    const response = await fetch('/api/auth/profile', {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(profileData)
    });

    const result = await response.json();
    
    if (result.success) {
      console.log('Perfil atualizado:', result.data);
      return result.data;
    } else {
      throw new Error(result.message);
    }
  } catch (error) {
    console.error('Erro na atualização:', error);
    throw error;
  }
};
```

### 3. **Buscar Perfil (JavaScript/React)**

```javascript
const getProfile = async (token) => {
  try {
    const response = await fetch('/api/auth/profile', {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    const result = await response.json();
    
    if (result.success) {
      console.log('Perfil obtido:', result.data);
      return result.data;
    } else {
      throw new Error(result.message);
    }
  } catch (error) {
    console.error('Erro ao buscar perfil:', error);
    throw error;
  }
};
```

### 4. **Componente React Completo**

```jsx
import React, { useState, useRef } from 'react';
import { Camera, Upload, User } from 'lucide-react';

const AvatarUpload = ({ currentAvatar, onAvatarUpdate, token }) => {
  const [preview, setPreview] = useState(currentAvatar);
  const [uploading, setUploading] = useState(false);
  const fileInputRef = useRef(null);

  const handleFileSelect = async (event) => {
    const file = event.target.files[0];
    if (!file) return;

    // Validar tipo
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
    if (!allowedTypes.includes(file.type)) {
      alert('Tipo de arquivo inválido. Use JPEG, PNG ou GIF.');
      return;
    }

    // Validar tamanho (5MB)
    if (file.size > 5 * 1024 * 1024) {
      alert('Arquivo muito grande. Máximo 5MB.');
      return;
    }

    // Preview
    const reader = new FileReader();
    reader.onload = (e) => setPreview(e.target.result);
    reader.readAsDataURL(file);

    // Upload
    await uploadAvatar(file);
  };

  const uploadAvatar = async (file) => {
    setUploading(true);
    
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await fetch('/api/auth/avatar', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        },
        body: formData
      });

      const result = await response.json();
      
      if (result.success) {
        setPreview(result.data);
        onAvatarUpdate(result.data);
      } else {
        alert('Erro: ' + result.message);
      }
    } catch (error) {
      alert('Erro no upload do avatar');
    } finally {
      setUploading(false);
    }
  };

  return (
    <div className="avatar-upload">
      <div className="avatar-preview">
        {preview ? (
          <img src={preview} alt="Avatar" className="avatar-image" />
        ) : (
          <User className="avatar-placeholder" />
        )}
        
        <button 
          className="upload-button"
          onClick={() => fileInputRef.current?.click()}
          disabled={uploading}
        >
          {uploading ? (
            <div className="spinner" />
          ) : (
            <Camera className="upload-icon" />
          )}
        </button>
      </div>
      
      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        onChange={handleFileSelect}
        style={{ display: 'none' }}
      />
    </div>
  );
};

export default AvatarUpload;
```

### 5. **CSS para o Avatar**

```css
.avatar-upload {
  position: relative;
  display: inline-block;
}

.avatar-preview {
  position: relative;
  width: 120px;
  height: 120px;
  border-radius: 50%;
  overflow: hidden;
  border: 3px solid #e5e7eb;
  background: #f9fafb;
}

.avatar-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.avatar-placeholder {
  width: 60px;
  height: 60px;
  color: #9ca3af;
  margin: 30px auto;
}

.upload-button {
  position: absolute;
  bottom: 5px;
  right: 5px;
  width: 35px;
  height: 35px;
  border-radius: 50%;
  background: #3b82f6;
  border: none;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.upload-button:hover {
  background: #2563eb;
}

.upload-button:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}

.spinner {
  width: 20px;
  height: 20px;
  border: 2px solid transparent;
  border-top: 2px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
```

## 🔧 Códigos de Resposta

| Código | Significado |
|--------|-------------|
| **200** | Sucesso |
| **400** | Dados inválidos |
| **401** | Não autenticado |
| **500** | Erro interno do servidor |

## 📁 Estrutura de Arquivos

```
eduQuizApis/
├── wwwroot/
│   └── avatars/          # Diretório dos avatares
│       ├── 123_20241016164230.jpg
│       └── 456_20241016164345.png
├── Domain/Entities/
│   └── User.cs           # Campo AvatarUrl adicionado
├── Application/DTOs/
│   └── AuthDTOs.cs       # DTOs atualizados
├── Application/Services/
│   └── AuthService.cs    # Lógica de avatar
└── Presentation/Controllers/
    └── AuthController.cs # Endpoints de avatar
```

## 🚀 Como Testar

### 1. **Via Swagger**
- Acesse: `http://localhost:5034`
- Teste o endpoint `POST /api/auth/avatar`

### 2. **Via cURL**
```bash
curl -X POST \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@avatar.jpg" \
  http://localhost:5034/api/auth/avatar
```

### 3. **Via Postman**
1. Crie uma requisição POST para `/api/auth/avatar`
2. Adicione o header `Authorization: Bearer {token}`
3. No body, selecione `form-data`
4. Adicione `file` como chave e selecione um arquivo de imagem

## ⚠️ Observações Importantes

1. **Autenticação**: Todos os endpoints requerem JWT válido
2. **Arquivos**: São salvos em `wwwroot/avatars/`
3. **Nomenclatura**: `{userId}_{timestamp}.{extensão}`
4. **Validação**: Apenas imagens JPEG, PNG e GIF são aceitas
5. **Tamanho**: Máximo 5MB por arquivo
6. **URLs**: Avatares são acessíveis via HTTP GET

---

**Versão**: 1.0  
**Última atualização**: 16/10/2024  
**Status**: ✅ Implementado e funcionando

