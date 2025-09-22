using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Domain.Repositories;
using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using StackoverflowService.Application.DTOs.Email;
using Newtonsoft.Json;

namespace StackoverflowService.Infrastructure.Email
{
    public class EmailClient : IEmailClient
    {
        private HttpClient _httpClient;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IUserRepository _userRepository;

        public EmailClient(
            HttpClient httpClient,
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository,
            IUserRepository users
        )
        {
            _httpClient = httpClient;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = users;
        }

        public async Task<bool> SendFinalAnswerEmail(string recipientUserId, string questionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(recipientUserId)) throw new ArgumentException("recipientUserId is required.", nameof(recipientUserId));
            if (string.IsNullOrWhiteSpace(questionId)) throw new ArgumentException("questionId is required.", nameof(questionId));

            var questionTask = _questionRepository.GetByIdAsync(questionId, cancellationToken);
            var finalAnswerTask = _answerRepository.GetFinalByQuestionAsync(questionId, cancellationToken);
            var recipientTask = _userRepository.GetAsync(recipientUserId, cancellationToken);

            await Task.WhenAll(questionTask, finalAnswerTask, recipientTask);

            var question = questionTask.Result;
            var finalAnswer = finalAnswerTask.Result;
            var recipient = recipientTask.Result;

            if (question == null) throw new InvalidOperationException($"Question '{questionId}' not found.");
            if (finalAnswer == null) throw new InvalidOperationException($"Final answer for question '{questionId}' not found.");
            if (recipient == null || string.IsNullOrWhiteSpace(recipient.Email))throw new InvalidOperationException($"Recipient '{recipientUserId}' not found.");

            var answerer = await _userRepository.GetAsync(finalAnswer.UserId, cancellationToken);
            var answeredBy = answerer != null ? $"{(answerer.Name ?? "").Trim()} {(answerer.Lastname ?? "").Trim()}".Trim() : "Unknown User";

            var template = LoadFinalAnswerTemplate();
            var htmlBody = BindTemplate(template,
                questionTitle: question.Title ?? "",
                finalAnswerText: finalAnswer.Text ?? "",
                answeredBy: answeredBy,
                questionUrl: $"http://localhost:4200/questions/{question.Id}"
            );

            var mail = new OutgoingMail
            {
                To = recipient.Email,
                Subject = $"[StackOverflow]: Final answer posted for \"{question.Title}\"",
                Body = htmlBody
            };

            var json = JsonConvert.SerializeObject(mail);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var resp = await _httpClient.PostAsync("Mail/send", content, cancellationToken);
            return resp.IsSuccessStatusCode;
        }

        #region Helpers

        private static string LoadFinalAnswerTemplate()
        {
            var asm = typeof(EmailClient).Assembly;
            var resourceName = asm
                .GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("Email.Templates.FinalAnswer.html", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
            {
                throw new InvalidOperationException(
                    "FinalAnswerEmail.html not found as an embedded resource. " +
                    "Ensure the file exists at Infrastructure/Email/Templates/FinalAnswerEmail.html " +
                    "and its Build Action is set to 'Embedded Resource'.");
            }

            using var stream = asm.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        private static string BindTemplate(string template, string questionTitle, string finalAnswerText, string answeredBy, string questionUrl)
        {
            string safeTitle = WebUtility.HtmlEncode(questionTitle ?? string.Empty);
            string safeAnswerer = WebUtility.HtmlEncode(answeredBy ?? string.Empty);
            string safeAnswer = WebUtility.HtmlEncode(finalAnswerText ?? string.Empty);
            string safeUrl = WebUtility.HtmlEncode(questionUrl ?? string.Empty);

            return template
                .Replace("{{QuestionTitle}}", safeTitle)
                .Replace("{{AnsweredBy}}", safeAnswerer)
                .Replace("{{FinalAnswer}}", safeAnswer)
                .Replace("{{QuestionUrl}}", string.IsNullOrWhiteSpace(safeUrl) ? "#" : safeUrl);
        }

        #endregion

    }
}
