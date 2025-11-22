using System.ComponentModel.DataAnnotations;
using Presentation.Dto.Validation;

namespace Presentation.Dto;

public record LoginQueryDto(
    [param: Required, Url, WhitelistedDomain] Uri SuccessRedirectUrl,
    [param: Required, Url, WhitelistedDomain] Uri ErrorRedirectUrl);