using MediatR;
using StackoverflowService.Application.Common.Results;
using StackoverflowService.Application.DTOs.Common;
using StackoverflowService.Application.DTOs.Questions;
using StackoverflowService.Domain.Repositories;
using System.Threading.Tasks;
using System.Threading;
using StackoverflowService.Application.Features.Questions.GetQuestions.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using StackoverflowService.Domain.Enums;
using StackoverflowService.Domain.Entities;
using StackoverflowService.Application.DTOs.Users;

namespace StackoverflowService.Application.Features.Questions.GetQuestions
{
    public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, Result<PagedResult<QuestionDto>>>
    {
        private const int PageSize = 25;

        private readonly IQuestionRepository _questionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IVoteRepository _voteRepository;

        public GetQuestionsQueryHandler(
            IQuestionRepository questionRepository,
            IUserRepository userRepository,
            IAnswerRepository answerRepository,
            IVoteRepository voteRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _answerRepository = answerRepository;
            _voteRepository = voteRepository;
        }

        public async Task<Result<PagedResult<QuestionDto>>> Handle(GetQuestionsQuery query, CancellationToken cancellationToken)
        {
            var all = await _questionRepository.GetAllFilteredAsync(query.TitleStartsWith, cancellationToken);

            Dictionary<string, int> voteScores;
            if (query.SortBy == QuestionsSortBy.Votes)
            {
                voteScores = await ComputeVoteScoresAsync(all.Select(q => q.Id).ToList(), cancellationToken);
            }
            else
            {
                voteScores = new Dictionary<string, int>();
            }

            IEnumerable<Question> ordered = query.SortBy switch
            {
                QuestionsSortBy.Votes => all.OrderBy(q => voteScores.TryGetValue(q.Id, out var s) ? s : 0),
                _ => all.OrderBy(q => q.CreationDate)
            };
            if (query.Direction == SortDirection.Desc)
                ordered = ordered.Reverse();

            var totalItems = all.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            var page = Math.Min(Math.Max(query.Page, 1), Math.Max(totalPages, 1));
            var slice = ordered.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            if (query.SortBy != QuestionsSortBy.Votes)
            {
                var pageScores = await ComputeVoteScoresAsync(slice.Select(q => q.Id).ToList(), cancellationToken);
                foreach (var kv in pageScores) voteScores[kv.Key] = kv.Value;
            }

            var items = new List<QuestionDto>(slice.Count);
            foreach (var question in slice)
            {
                var user = await _userRepository.GetAsync(question.UserId, cancellationToken);

                items.Add(new QuestionDto
                {
                    Id = question.Id,
                    UserId = question.UserId,
                    Title = question.Title,
                    Description = question.Description,
                    PhotoBlobName = question.Photo?.BlobName,
                    PhotoContainer = question.Photo?.Container,
                    CreationDate = question.CreationDate,
                    IsClosed = question.IsClosed,
                    IsDeleted = question.IsDeleted,
                    VoteScore = voteScores.TryGetValue(question.Id, out var s) ? s : 0,
                    User = new UserPreviewDto
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Lastname = user.Lastname,
                            Email = user.Email,
                            PhotoBlobName = user.Photo?.BlobName,
                            PhotoContainer = user.Photo?.Container
                        }
                });
            }

            var result = new PagedResult<QuestionDto>
            {
                Page = page,
                PageSize = PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };

            return Result.Ok(result);
        }

        #region Helpers
        private async Task<Dictionary<string, int>> ComputeVoteScoresAsync(IReadOnlyList<string> questionIds, CancellationToken cancellationToken)
        {
            var scores = new Dictionary<string, int>(StringComparer.Ordinal);
            var tasks = questionIds.ToDictionary(id => id, id => _voteRepository.CountByQuestionAsync(id, cancellationToken));
            await Task.WhenAll(tasks.Values);

            foreach (var kv in tasks)
            {
                var (up, down) = kv.Value.Result;
                scores[kv.Key] = up - down;
            }
            return scores;
        }

        #endregion

    }
}
