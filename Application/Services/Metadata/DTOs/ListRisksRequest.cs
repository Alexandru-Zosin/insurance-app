using Application.Common;

namespace Application.Services.Metadata.DTOs;

public sealed record ListRisksRequest(bool? OnlyActive, PageRequest Page);
