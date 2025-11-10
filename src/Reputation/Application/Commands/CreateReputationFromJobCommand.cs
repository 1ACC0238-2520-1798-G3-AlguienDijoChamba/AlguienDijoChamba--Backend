using MediatR;

namespace AlguienDijoChamba.Api.Reputation.Application.Commands;

public class CreateReputationFromJobCommand : IRequest<object>
{
    public Guid JobId { get; }
    public int Rating { get; }
    public string Review { get; }

    public CreateReputationFromJobCommand(Guid jobId, int rating, string review)
    {
        JobId = jobId;
        Rating = rating;
        Review = review;
    }
}
