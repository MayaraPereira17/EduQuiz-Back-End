namespace eduQuizApis.Application.DTOs
{
    /// <summary>
    /// DTO para respostas padronizadas da API
    /// </summary>
    /// <typeparam name="T">Tipo dos dados retornados</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem de resposta
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dados retornados (opcional)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Lista de erros (opcional)
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Timestamp da resposta
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Cria uma resposta de sucesso
        /// </summary>
        public static ApiResponse<T> SuccessResponse(string message, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Cria uma resposta de erro
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// DTO para respostas sem dados específicos
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Cria uma resposta de sucesso sem dados
        /// </summary>
        public static ApiResponse SuccessResponse(string message)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Cria uma resposta de erro sem dados
        /// </summary>
        public new static ApiResponse ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
