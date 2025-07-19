namespace Journey.Application.DTOs.Response;

public record SigninResponse(string tokenType, string accessToken, int expiresIn);