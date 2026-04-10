namespace Backend.Dtos;

public class TokenResponseDto
{
    public string TokenType { get; set; } = "Bearer";
    public string AccessToken { get; set; } = string.Empty;
}