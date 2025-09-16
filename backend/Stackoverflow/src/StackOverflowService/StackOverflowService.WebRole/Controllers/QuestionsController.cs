using MediatR;

using System.Web.Http;
using StackOverflowService.WebRole.Security;
using System.Threading.Tasks;
using StackOverflowService.WebRole.Requests.Questions;
using System.Threading;
using System.Net;
using System.Security.Claims;
using StackoverflowService.Application.Features.Questions.CreateQuestion;
using StackOverflowService.WebRole.Http;
using StackOverflowService.WebRole.Swagger;
using StackoverflowService.Application.DTOs.File;
using StackoverflowService.Application.Features.Questions.SetQuestionPhoto;
using System.Net.Http;
using System.Linq;
using StackoverflowService.Application.Features.Questions.GetQuestions.Enums;
using StackoverflowService.Application.Features.Questions.GetQuestions;
using StackoverflowService.Application.Features.Questions.GetQuestionById;
using StackoverflowService.Application.Features.Questions.UpdateQuestion;
using StackoverflowService.Application.Features.Questions.DeleteQuestion;
using StackoverflowService.Application.Features.Questions.GetQuestionPhoto;
using System.Net.Http.Headers;

namespace StackOverflowService.WebRole.Controllers
{
    [RoutePrefix("api/questions")]
    public class QuestionsController : ApiController
    {
        private readonly IMediator _mediator;

        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet, Route("")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> List([FromUri] GetQuestionsRequest request, CancellationToken cancellationToken)
        {
            if (request == null) request = new GetQuestionsRequest(); //take all default fields

            var sortBy = request?.SortBy?.ToLowerInvariant() == "votes"
                ? QuestionsSortBy.Votes
                : QuestionsSortBy.Date;

            var dir = request?.Direction?.ToLowerInvariant() == "asc"
                ? SortDirection.Asc
                : SortDirection.Desc;

            var query = new GetQuestionsQuery(
                page: request.Page <= 0 ? 1 : request.Page,
                titleStartsWith: string.IsNullOrWhiteSpace(request.Title) ? string.Empty : request.Title,
                sortBy: sortBy,
                direction: dir
            );

            var result = await _mediator.Send(query, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpGet, Route("{id}")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var command = new GetQuestionByIdQuery(id);

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpPost, Route("")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> Create(CreateQuestionRequest request, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Content(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            var command = new CreateQuestionCommand(userId, request.Title, request.Description);

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpPut, Route("{id}")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> Update(string id, UpdateQuestionRequest request, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Content(HttpStatusCode.Unauthorized, "Unauthorized");
            }

            var command = new UpdateQuestionCommand(userId, id, request.Title, request.Description);

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpDelete, Route("{id}")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Content(HttpStatusCode.Unauthorized, "Unauthorized");

            var command = new DeleteQuestionCommand(userId, id);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsSuccess && result.Value)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return this.ToActionResult(result);
        }

        [HttpPost, Route("{id}/photo")]
        [SwaggerFileUpload]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> UploadPhoto(string id, CancellationToken cancellationToken)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("multipart/form-data expected");

            var provider = await Request.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider(), cancellationToken);

            var file = provider.Contents.FirstOrDefault(c =>
                (c.Headers.ContentDisposition?.Name ?? "").Trim('"') == "file")
                ?? provider.Contents.FirstOrDefault();

            if (file == null) return BadRequest("File part 'file' not found.");

            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value
                      ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));

            var dto = new FileUploadDto
            {
                Content = await file.ReadAsByteArrayAsync(),
                ContentType = file.Headers.ContentType?.MediaType ?? "application/octet-stream",
                FileName = file.Headers.ContentDisposition?.FileName?.Trim('"') ?? "upload"
            };

            var command = new SetQuestionPhotoCommand
            {
                UserId = userId,
                QuestionId = id,
                File = dto
            };

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpGet, Route("{id}/photo")]
        [RequireJwtAuth]
        [SwaggerFileResponse("image/jpeg", "image/png", "image/gif", "image/webp", "application/octet-stream")]
        public async Task<IHttpActionResult> DownloadPhoto(string id, CancellationToken cancellationToken)
        {
            var query = new GetQuestionPhotoQuery(id);

            var result = await _mediator.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return this.ToActionResult(result);

            var file = result.Value;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(file.Content)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileName = file.FileName
            };

            return ResponseMessage(response);
        }

    }
}
