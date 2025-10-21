using BillingAgreementService.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StupidChat.Application.Common;
using StupidChat.Application.Common.Extensions;

namespace StupidChat.Application.User.EnsureUserExists;

public class EnsureUserExistsCommandHandler(
    IRepository<Domain.Entities.User> userRepository,
    IUnitOfWork unitOfWork,
    IDistributedCache cache
    ) : IRequestHandler<EnsureUserExistsCommand, Guid>
{
    public async Task<Guid> Handle(EnsureUserExistsCommand request, CancellationToken cancellationToken)
    {
        var user = await cache.GetObjectAsync<Domain.Entities.User>(request.UserName, cancellationToken)
            ?? await userRepository.AsQueryable().FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);

        if (user != null)
        {
            return user.Id;
        }

        var newUser = new Domain.Entities.User
        {
            UserName = request.UserName,
            Id = Guid.NewGuid(),
        };

        await userRepository.AddAsync(newUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.SetObjectAsync(request.UserName, newUser, new DistributedCacheEntryOptions(), cancellationToken);

        return newUser.Id;
    }
}