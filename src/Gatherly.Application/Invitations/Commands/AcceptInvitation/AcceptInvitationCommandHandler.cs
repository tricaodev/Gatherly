﻿using Gatherly.Application.Abstractions;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;
using Gatherly.Domain.Shared.Enums;
using MediatR;

namespace Gatherly.Application.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IGatheringRepository _gatheringRepository;
    private readonly IAttendeeRepository _attendeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public AcceptInvitationCommandHandler(IInvitationRepository invitationRepository, IMemberRepository memberRepository, 
        IGatheringRepository gatheringRepository, IAttendeeRepository attendeeRepository, IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _invitationRepository = invitationRepository;
        _memberRepository = memberRepository;
        _gatheringRepository = gatheringRepository;
        _attendeeRepository = attendeeRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository
            .GetByIdAsync(request.InvitationId, cancellationToken);

        if (invitation is null || invitation.Status != InvitationStatus.Pending)
        {
            return Unit.Value;
        }

        var member = await _memberRepository.GetByIdAsync(invitation.MemberId, cancellationToken);

        var gathering = await _gatheringRepository
            .GetByIdWithCreatorAsync(invitation.GatheringId, cancellationToken);

        if (member is null || gathering is null)
        {
            return Unit.Value;
        }

        Result<Attendee> attendeeResult = gathering.AcceptInvitation(invitation);

        if (attendeeResult is not null)
        {
            _attendeeRepository.Add(attendeeResult.Value);
        }

        // Send email
        if (invitation.Status == InvitationStatus.Accepted)
        {
            await _emailService.SendInvitationAcceptedEmailAsync(gathering, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
