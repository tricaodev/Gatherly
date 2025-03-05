using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using MediatR;

namespace Gatherly.Application.Gatherings.Commands.CreateGathering;

public class CreateGatheringCommandHandler : IRequestHandler<CreateGatheringCommand>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IGatheringRepository _gatheringRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGatheringCommandHandler(IMemberRepository memberRepository, IGatheringRepository gatheringRepository, IUnitOfWork unitOfWork)
    {
        _gatheringRepository = gatheringRepository;
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(CreateGatheringCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);

        if (member is null)
        {
            return Unit.Value;
        }

        var gathering = Gathering.Create(Guid.NewGuid(), member, request.Type, request.ScheduledAtUtc, request.Name, request.Location,
            request.MaximumNumberOfAttendees, request.InvitationsValidBeforeInHours);

        _gatheringRepository.Add(gathering);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
