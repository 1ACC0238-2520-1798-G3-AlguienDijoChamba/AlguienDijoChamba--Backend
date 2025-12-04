using AlguienDijoChamba.Api.Reputation.Domain;
using AlguienDijoChamba.Api.Reputation.Interfaces.Dtos;
using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Queries
{
    public class GetReputationByProfessionalIdQueryHandler : IRequestHandler<GetReputationByProfessionalIdQuery, ReputationResponse?>
    {
        private readonly IReputationRepository _repository;

        public GetReputationByProfessionalIdQueryHandler(IReputationRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReputationResponse?> Handle(GetReputationByProfessionalIdQuery request, CancellationToken cancellationToken)
        {
            var rep = await _repository.GetByProfessionalIdAsync(request.ProfessionalId, cancellationToken);
            if (rep is null) return null;

            return new ReputationResponse(
                rep.ProfessionalId,
                rep.StarRating,        
                rep.CompletedJobs,   
                rep.ProfessionalLevel,  
                rep.HourlyRate
            );
        }
    }
}