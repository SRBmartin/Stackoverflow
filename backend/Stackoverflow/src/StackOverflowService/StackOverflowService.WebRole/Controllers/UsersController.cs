using MediatR;
using StackoverflowService.Application.DTOs.File;
using StackoverflowService.Application.Features.Users.RegisterUser;
using StackoverflowService.Application.Features.Users.SetUserPhoto;
using StackOverflowService.WebRole.Http;
using StackOverflowService.WebRole.Requests.User;
using StackOverflowService.WebRole.Swagger;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

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

        [HttpPost, Route("{userId}/photo")]
        [SwaggerFileUpload]
        public async Task<IHttpActionResult> Upload(string userId, CancellationToken cancellationToken)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("multipart/form-data expected");

            var provider = await Request.Content.ReadAsMultipartAsync(
                new MultipartMemoryStreamProvider(), cancellationToken);

            var file = provider.Contents.FirstOrDefault(c =>
                (c.Headers.ContentDisposition?.Name ?? "").Trim('"') == "file")
                ?? provider.Contents.FirstOrDefault();

            if (file == null) return BadRequest("File part 'file' not found.");

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

    }
}
