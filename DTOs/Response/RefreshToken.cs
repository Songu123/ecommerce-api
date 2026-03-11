namespace API.DTOs.Response
{
    /// <summary>
    /// Refresh Token DTO for response
    /// </summary>
    public class RefreshTokenResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
