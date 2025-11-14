using MyApi.Dtos;

    public record LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
