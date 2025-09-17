using MediatR;
using StackoverflowService.Application.DTOs.File;
using StackoverflowService.Application.Features.Users.GetUserProfile;
using StackoverflowService.Application.Features.Users.Login;
using StackoverflowService.Application.Features.Users.RegisterUser;
using StackoverflowService.Application.Features.Users.SetUserPhoto;
using StackoverflowService.Application.Features.Users.UpdateUserProfile;
using StackOverflowService.WebRole.Http;
using StackOverflowService.WebRole.Requests.User;
using StackOverflowService.WebRole.Security;
using StackOverflowService.WebRole.Swagger;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using StackoverflowService.Application.Features.Users.GetUserPhoto;
using System.Net.Http.Headers;

namespace StackOverflowService.WebRole.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Register(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterUserCommand(request.Name, request.Lastname, request.Email, request.Password, request.Gender, request.State, request.City, request.Address);

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpPost, Route("login")]
        public async Task<IHttpActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.Email, request.Password);

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpPost, Route("photo")]
        [SwaggerFileUpload]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> Upload(CancellationToken cancellationToken)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("multipart/form-data expected");

            var provider = await Request.Content.ReadAsMultipartAsync(
                new MultipartMemoryStreamProvider(), cancellationToken);

            var file = provider.Contents.FirstOrDefault(c =>
                (c.Headers.ContentDisposition?.Name ?? "").Trim('"') == "file")
                ?? provider.Contents.FirstOrDefault();

            if (file == null) return BadRequest("File part 'file' not found.");

            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));

            var dto = new FileUploadDto
            {
                Content = await file.ReadAsByteArrayAsync(),
                ContentType = file.Headers.ContentType?.MediaType ?? "application/octet-stream",
                FileName = file.Headers.ContentDisposition?.FileName?.Trim('"') ?? "upload"
            };

            var cmd = new SetUserPhotoCommand { UserId = userId, File = dto };
            var result = await _mediator.Send(cmd, cancellationToken);
            return this.ToActionResult(result);
        }

        [HttpGet, Route("{id}/photo")]
        [RequireJwtAuth]
        [SwaggerFileResponse("image/jpeg", "image/png", "image/gif", "image/webp", "application/octet-stream")]
        public async Task<IHttpActionResult> DownloadAsync(string id, CancellationToken cancellationToken)
        {
            var query = new GetUserPhotoQuery(id);

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

        [HttpGet, Route("profile")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));

            var query = new GetUserProfileQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);

            return this.ToActionResult(result);
        }

        [HttpPut, Route("profile")]
        [RequireJwtAuth]
        public async Task<IHttpActionResult> UpdateProfile(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var principal = User as ClaimsPrincipal;
            var userId = principal?.FindFirst("sub")?.Value ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));

            var command = new UpdateUserProfileCommand(
                userId: userId,
                name: request.Name,
                lastname: request.Lastname,
                state: request.State,
                city: request.City,
                address: request.Address
            );

            var result = await _mediator.Send(command, cancellationToken);
            return this.ToActionResult(result);
        }

    }
}
