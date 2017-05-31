using Microsoft.IdentityModel.Tokens;

namespace ArchitectNow.Models.Security
{
	public class JwtSigningKey: SymmetricSecurityKey
    {
	    public JwtSigningKey(byte[] key) : base(key)
	    {
	    }
    }
}
