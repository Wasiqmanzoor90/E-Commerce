using E_Commerce.Types;

namespace E_Commerce.Interface
{
    public interface IJasonToken
    {
        string CreateToken(Guid Id, string Email, string Name, Role role);
        Guid VerifyToken(string id);
    }
}
