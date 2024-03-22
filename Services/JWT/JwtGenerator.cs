using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace SchedulerApi.Services.JWT;

public class JwtGenerator : IJwtGenerator
{
    private readonly IConfigurationSection _jwtParams;
    private readonly IConfigurationSection _keyVaultJwtSecretNames;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SecretClient _secretClient;

    public JwtGenerator(IConfiguration configuration, UserManager<IdentityUser> userManager, SecretClient secretClient)
    {
        _jwtParams = configuration.GetSection("Jwt");
        _keyVaultJwtSecretNames = configuration.GetSection("KeyVault:SecretNames:Jwt");
        _userManager = userManager;
        _secretClient = secretClient;
    }
    
    public async Task<string> Generate(IdentityUser user)
    {
        var kvSecretName = _keyVaultJwtSecretNames["Secret"];
        var kvSecret = await _secretClient.GetSecretAsync(kvSecretName);
        if (kvSecret is null)
        {
            throw new SecurityTokenException("failed to retrieve secret.");
        }
        var secret = kvSecret.Value.Value;
        if (secret is null)
        {
            throw new SecurityTokenException( "secret is null.");
        }
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roleClaims);

        var token = new JwtSecurityToken(
            issuer: _jwtParams["Issuer"],
            audience: _jwtParams["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
