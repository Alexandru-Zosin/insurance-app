using Application.Common;
using Domain.Policies;
namespace Application.Services.Policies.DTOs;

public sealed record ListPoliciesRequest(
    Guid? ClientId,
    Guid? BrokerId,
    PolicyStatus? Status,
    DateOnly? StartDate,
    DateOnly? EndDate,
    PageRequest Page);
