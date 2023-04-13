using OpenDentBusiness;
using System;

namespace UnitTestsCore.TableTypes
{
    public class SessionTokenT
    {
        public static SessionToken CreateToken(SessionTokenType tokenType, long fkey, DateTime expiration = default)
        {
            SessionToken token = SessionTokens.GenerateToken(tokenType, fkey);
            if (expiration.Year > 1880)
            {
                token.Expiration = expiration;
                OpenDentBusiness.Crud.SessionTokenCrud.Update(token);
            }
            return token;
        }
    }
}
