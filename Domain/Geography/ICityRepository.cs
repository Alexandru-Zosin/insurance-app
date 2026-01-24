namespace Domain.Geography;

public interface ICityRepository
{
    City? GetById(int cityId);
}
