using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;

namespace ForesterAPI
{
    public static class JWTManager
    {
        const string secret = "6y78ice3fuj8ideogvj4frgt3irfg8hs";

        public static string Encode(IEnumerable<KeyValuePair<string, object>> claims) 
            => JwtBuilder.Create().ExpirationTime(DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds()).WithAlgorithm(new HMACSHA256Algorithm()).WithSecret(secret).AddClaims(claims).Encode();

        public static string Decode(string encodedMessage)
        {
            try
            {
                return JwtBuilder.Create().WithAlgorithm(new HMACSHA256Algorithm()).WithSecret(secret).MustVerifySignature().Decode(encodedMessage);
            }
            catch (TokenExpiredException)
            {
                return "";
            }
            catch (SignatureVerificationException)
            {
                return "";
            }
        }
    }
}
