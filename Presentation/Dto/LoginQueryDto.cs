using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Presentation.Dto.Validation;

namespace Presentation.Dto;

public record LoginQueryDto(
    [param: Required, Url, WhitelistedDomain] Uri SuccessRedirectUri,
    [param: Required, Url, WhitelistedDomain] Uri ErrorRedirectUri);