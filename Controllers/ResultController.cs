using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoccerQuizApi.Helper;
using SoccerQuizApi.Models;
using SoccerQuizApi.Services;

namespace SoccerQuizApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly QuizService _quizService;
        private readonly ResultService _resultService;
        private readonly AdminHelper _adminHelper;

        public ResultController(UserService userService, QuizService quizService, ResultService resultService, AdminHelper adminHelper)
        {
            _userService = userService;
            _quizService = quizService;
            _resultService = resultService;
            _adminHelper = adminHelper;
        }

        [HttpGet]
        public async Task<IEnumerable<Result>> Get(string userId)
        {
            var result = await _resultService.GetAsync();

            return result.Where(w => w.UserId == userId);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IEnumerable<Result>> GetAll(string adminId)
        {
            if (await _adminHelper.NotAdmin(adminId))
            {
                return new List<Result>();
            }

            return await _resultService.GetAsync();
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<Quiz> GetQuizByResult(string id,string userId)
        {
            var result = await _resultService.GetAsync(id);

            if (result is null)
            {
                return new Quiz();
            }
            if (result.UserId != userId && await _adminHelper.NotAdmin(userId))
            {
                return new Quiz();
            }

            var quiz = await _quizService.GetAsync(result.QuizId);

            if (quiz is null)
            {
                return new Quiz();
            }

            return quiz;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CalculateResult(UserQuiz quizResult)
        {
            var quiz = await _quizService.GetAsync(quizResult.Quiz.Id);
            var user = await _userService.GetAsync(quizResult.UserId);

            if (quiz is null || user is null)
            {
                return NotFound();
            }

            var answers = new List<int>();
            var score = 0;

            var qna = quizResult.Quiz.QuestionAndAnswers;
            for (int i = 0; i < qna.Count; i++)
            {
                answers.Add(qna[i].CorrectAnswer);
                if (qna[i].CorrectAnswer == quiz.QuestionAndAnswers[i].CorrectAnswer)
                {
                    score++;
                }
            }

            var result = new Result
            {
                QuizId = quiz.Id,
                UserId = user.Id,
                QuizName = quiz.QuizName,
                UserName = user.UserName,
                Answers = answers,
                Score = score,
                Created = DateTime.Now,           
            };

            await _resultService.CreateAsync(result);

            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(string id, string adminId)
        {
            if (await _adminHelper.NotAdmin(adminId))
            {
                return Unauthorized();
            }

            var result = await _resultService.GetAsync(id);

            if (result is null)
            {
                return NotFound();
            }

            await _resultService.RemoveAsync(id);

            return NoContent();
        }
    }
}
