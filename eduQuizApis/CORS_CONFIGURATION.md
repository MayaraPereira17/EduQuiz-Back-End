# Configuração de CORS - EduQuiz API

## 🚀 Configuração Atual

A API EduQuiz foi configurada para permitir acesso do frontend em **desenvolvimento**.

### 📋 URLs Permitidas

**Desenvolvimento:**
- `http://localhost:5173` ✅
- `https://localhost:5173` ✅

**Produção (Railway):**
- `https://eduquiz-back-end-production.up.railway.app` ✅
- `http://localhost:5173` ✅ (para testes locais)
- `https://localhost:5173` ✅ (para testes locais)

### 🔧 Configuração Técnica

**Desenvolvimento:**
```csharp
// Política para desenvolvimento - permite frontend local
options.AddPolicy("Development", policy =>
{
    policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials(); // Permite cookies e headers de autenticação
});
```

**Produção (Railway):**
```csharp
// Política para produção - Railway
options.AddPolicy("Production", policy =>
{
    policy.WithOrigins(
            "https://eduquiz-back-end-production.up.railway.app",
            "http://localhost:5173",  // Para testes locais em produção
            "https://localhost:5173"  // Para testes locais em produção
          )
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
});
```

### 🛡️ Recursos Permitidos

- ✅ **Métodos HTTP**: Todos (GET, POST, PUT, DELETE, etc.)
- ✅ **Headers**: Todos os headers
- ✅ **Credentials**: Cookies e headers de autenticação (JWT)
- ✅ **CORS Preflight**: Suportado

### 🌐 Como Testar

1. **Inicie a API:**
   ```bash
   dotnet run
   ```

2. **A API estará disponível em:**
   - `http://localhost:5034` (HTTP)
   - `https://localhost:5035` (HTTPS)

3. **Teste do Frontend:**
   ```javascript
   // Exemplo de requisição do frontend
   fetch('http://localhost:5034/api/auth/login', {
     method: 'POST',
     headers: {
       'Content-Type': 'application/json',
     },
     credentials: 'include', // Para incluir cookies
     body: JSON.stringify({
       username: 'seu_usuario',
       password: 'sua_senha'
     })
   });
   ```

### 🔒 Segurança

- **Desenvolvimento**: Acesso liberado para `localhost:5173`
- **Produção**: Apenas domínios específicos serão permitidos

### 🚨 Troubleshooting

Se ainda houver problemas de CORS:

1. **Verifique se a API está rodando:**
   ```bash
   curl http://localhost:5034/health
   ```

2. **Verifique os headers de resposta:**
   ```bash
   curl -H "Origin: http://localhost:5173" \
        -H "Access-Control-Request-Method: POST" \
        -H "Access-Control-Request-Headers: X-Requested-With" \
        -X OPTIONS \
        http://localhost:5034/api/auth/login
   ```

3. **Headers esperados na resposta:**
   ```
   Access-Control-Allow-Origin: http://localhost:5173
   Access-Control-Allow-Methods: GET, POST, PUT, DELETE, OPTIONS
   Access-Control-Allow-Headers: *
   Access-Control-Allow-Credentials: true
   ```

### 📝 Notas Importantes

- A configuração atual permite acesso apenas em **desenvolvimento**
- Para produção, será necessário configurar o domínio específico
- O `AllowCredentials()` é essencial para autenticação JWT
- Todas as rotas da API estão protegidas por CORS

### 🔄 Atualizações Futuras

Para adicionar novos domínios:

1. Edite o arquivo `Program.cs`
2. Adicione a nova URL na política `Development`:
   ```csharp
   policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://novodominio.com")
   ```
3. Reinicie a aplicação

---

**Status**: ✅ Configurado e funcionando  
**Última atualização**: $(Get-Date)  
**Versão da API**: v1.0
