//using Application.Common;
//using Application.Repositories;
//using Application.Services.Metadata.DTOs;
//using Domain.Configurations;

//public sealed class RiskFactorService<TCfg>(
//    IRiskFactorRepository _risks,
//    IUnitOfWork _uow) : IRiskFactorService<T>
//    where TCfg : RiskFactorConfiguration<TCfg>
//{
//    public async Task<Result<CreateRiskResponse>> CreateAsync(
//        CreateRiskRequest<T> r, CancellationToken ct = default)
//    {
//        await _risks.AddAsync(r.Risk, ct);
//        await _uow.SaveChangesAsync(ct);
//        return Result<CreateRiskResponse>.Ok(new(r.Risk.Id));
//    }

//    public async Task<Result<UpdateRiskResponse>> UpdateAsync(
//        UpdateRiskRequest<T> r, CancellationToken ct = default)
//    {
//        var cfg = await _risks.GetByIdAsync<T>(r.RiskId, ct);
//        if (cfg is null) return NotFound<UpdateRiskResponse>();
//        r.Apply(cfg);                       // dto does the mutation
//        await _risks.UpdateAsync(cfg, ct);
//        await _uow.SaveChangesAsync(ct);
//        return Result<UpdateRiskResponse>.Ok(new(true));
//    }

//    public async Task<Result<SetRiskStatusResponse>> SetStatusAsync(
//        SetRiskStatusRequest r, CancellationToken ct = default)
//    {
//        var cfg = await _repo.GetByIdAsync<T>(r.RiskId, ct);
//        if (cfg is null) return NotFound<SetRiskStatusResponse>();
//        _ = r.Active ? cfg.Activate() : cfg.Deactivate();
//        await _uow.UpdateAsync(cfg, ct);
//        await _uow.SaveChangesAsync(ct);
//        return Result<SetRiskStatusResponse>.Ok(new(true));
//    }
//}