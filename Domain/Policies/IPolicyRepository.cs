namespace Domain.Policies;

public interface IPolicyRepository
{
    Policy? GetById(Guid policyId);
    IReadOnlyList<Policy> GetByClientId(Guid clientId);
    IReadOnlyList<Policy> GetByBuildingId(Guid buildingId);
    void Add(Policy policy);
}
