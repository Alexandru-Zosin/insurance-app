using Domain.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Shared.DTOs.MetadataDTOs
{
    public sealed record FeeConfigDetailedDto(
    Guid Id,
    FeeConfigCoreDto Core)
    {
        public static FeeConfigDetailedDto From(FeeConfiguration e) =>
            new(e.Id, FeeConfigCoreDto.From(e));
    }
}
