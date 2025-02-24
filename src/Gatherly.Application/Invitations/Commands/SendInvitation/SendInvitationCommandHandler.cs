﻿using Gatherly.Application.Abstractions;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared.Enums;
using MediatR;

namespace Gatherly.Application.Invitations.Commands.SendInvitation;

public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IGatheringRepository _gatheringRepository;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public SendInvitationCommandHandler(IMemberRepository memberRepository, IGatheringRepository gatheringRepository,
        IInvitationRepository invitationRepository, IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _memberRepository = memberRepository;
        _gatheringRepository = gatheringRepository;
        _invitationRepository = invitationRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);

        var gathering = await _gatheringRepository
            .GetByIdWithCreatorAsync(request.GatheringId, cancellationToken);

        if (member is null || gathering is null)
        {
            return Unit.Value;
        }

        var invitation = gathering.SendInvitation(Guid.NewGuid(), gathering.Id, member.Id, InvitationStatus.Pending, DateTime.UtcNow);

        _invitationRepository.Add(invitation);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send email
        await _emailService.SendInvitationSentEmailAsync(member, gathering, cancellationToken);

        return Unit.Value;
    }
}
