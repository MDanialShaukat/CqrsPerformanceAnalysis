using System.Net;
using Microsoft.AspNetCore.Mvc;
using Traditional.Api.Common.Constants;
using Traditional.Api.Common.Endpoints;
using Traditional.Api.Common.ErrorHandling;
using Traditional.Api.UseCases.Attributes.Common.Responses;

namespace Traditional.Api.UseCases.Attributes.GetLeafAttributes;

/// <inheritdoc />
public class GetLeafAttributesEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet("attributes/leafAttributes", GetLeafAttributesAsync)
            .WithTags(EndpointTags.ATTRIBUTES)
            .WithSummary("Returns a list of category specific leaf attributes based on the request.")
            .Produces<IEnumerable<GetAttributesResponse>>()
            .ProducesProblem((int)HttpStatusCode.NotFound)
            .ProducesProblem((int)HttpStatusCode.BadRequest)
            .ProducesProblem((int)HttpStatusCode.InternalServerError)
            .AddEndpointFilter<ValidationFilter<GetLeafAttributesRequest>>()
            .WithOpenApi();
    }

    private static async Task<IResult> GetLeafAttributesAsync(
        [AsParameters] GetLeafAttributesRequest request,
        [FromServices] GetLeafAttributesHandler handler,
        [FromServices] HttpProblemDetailsService problemDetailsService)
    {
        var result = await handler.GetLeafAttributesAsync(request);

        return result.Match(
            Results.Ok,
            problemDetailsService.LogErrorsAndReturnProblem);
    }
}
