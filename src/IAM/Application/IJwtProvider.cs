using AlguienDijoChamba.Api.IAM.Domain;

namespace AlguienDijoChamba.Api.IAM.Application;

public interface IJwtProvider
{
    string Generate(User user);
}