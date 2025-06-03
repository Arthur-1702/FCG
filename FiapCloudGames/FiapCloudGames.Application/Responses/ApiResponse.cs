namespace FiapCloudGames.Application.Responses
{
    public class ApiResponse<T>
    {
        public T? Dados { get; set; }
        public bool Sucesso { get; set; }
        public ErroResposta? Erro { get; set; }


        public static ApiResponse<T> Ok(T dados)
            => new() { Sucesso = true, Dados = dados };

        public static ApiResponse<T> Error(int statusCode, string mensagem)
            => new() { Sucesso = false, Erro = new ErroResposta { StatusCode = statusCode, Mensagem = mensagem } };
    }
}
