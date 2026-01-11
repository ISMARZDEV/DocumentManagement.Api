using Bhd.Domain.Entities;

namespace Bhd.Application.Interfaces;

public interface IJwtGenerator
{
    string CreateToken(User user);
}