using Application.Common;
namespace Application.Services.Metadata.DTOs;
public sealed record ListFeeConfigsRequest(bool? OnlyActive, PageRequest Page);
