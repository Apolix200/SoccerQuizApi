using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using SoccerQuizApi.Helper;
using SoccerQuizApi.Models;
using SoccerQuizApi.Services;
using System.Text.RegularExpressions;

namespace SoccerQuizApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {

        private readonly QuizService _quizService;
        private readonly AdminHelper _adminHelper;
        private readonly ResultService _resultService;

        public QuizController(QuizService quizService, AdminHelper adminHelper, ResultService resultService)
        {
            _quizService = quizService;
            _adminHelper = adminHelper;
            _resultService = resultService;
        }

        [HttpGet]
        public async Task<IEnumerable<Quiz>> Get(string adminId)
        {
            var quiz = await _quizService.GetAsync();

            if (await _adminHelper.NotAdmin(adminId))
            {
                return new List<Quiz>();
            }

            return quiz;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IEnumerable<Quiz>> GetActiveQuiz()
        {
            var quizes = await _quizService.GetAsync();

            var activeQuizes = quizes.Where(w => w.IsActive == true);

            foreach (var quiz in activeQuizes) {
                foreach (var qna in quiz.QuestionAndAnswers)
                {
                    qna.CorrectAnswer = -1;
                }
            }

            activeQuizes.FirstOrDefault().QuizName = "Micsoda?";

            return activeQuizes;
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserQuiz newQuiz)
        {
            if (await _adminHelper.NotAdmin(newQuiz.UserId))
            {
                return Unauthorized();
            }

            await _quizService.UpdateAsync(newQuiz.Quiz.Id, newQuiz.Quiz);

            return Ok();
        }

        [Route("[action]")]
        [HttpPut]
        public async Task<IActionResult> ActivateQuiz(string id, string adminId, bool isActive)
        {
            if (await _adminHelper.NotAdmin(adminId))
            {
                return Unauthorized();
            }

            var quiz = await _quizService.GetAsync(id);

            if (quiz is null)
            {
                return NotFound();
            }

            quiz.IsActive = !isActive;

            await _quizService.UpdateAsync(id, quiz);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserQuiz newQuiz)
        {
            if (await _adminHelper.NotAdmin(newQuiz.UserId))
            {
                return Unauthorized();
            }

            await _quizService.CreateAsync(newQuiz.Quiz);

            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> ImportQuiz([FromForm] FileModel file)
        {
            if (await _adminHelper.NotAdmin(file.AdminId))
            {
                return Unauthorized();
            }
            if (!file.FormFile.FileName.Contains("xlsx"))
            {
                return StatusCode(420);
            }

            Stream fileStream = file.FormFile.OpenReadStream();
            fileStream.Position = 0;
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileStream, false))
            {
                var workbookPart = document.WorkbookPart;
                var workbook = workbookPart.Workbook;

                var sheets = workbook.Descendants<Sheet>();

                foreach (var sheet in sheets)
                {
                    var quiz = new Quiz();
                    var qnaList = new List<QuestionAndAnswer>();
                    var qna = new QuestionAndAnswer();

                    var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    var sharedStringPart = workbookPart.SharedStringTablePart;
                    var stringValues = sharedStringPart.SharedStringTable.Elements<SharedStringItem>().ToArray();

                    var rows = worksheetPart.Worksheet.Descendants<Row>().ToList();

                    int qnaIndex = 0;
                    foreach (var row in rows)
                    {
                        var cellsInRow = row.Descendants<Cell>();

                        foreach (var cell in cellsInRow)
                        {
                            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                            {
                                var index = int.Parse(cell.CellValue.Text);

                                switch (Regex.Replace(cell.CellReference, "[0-9]", ""))
                                {
                                    case "B":
                                        qnaIndex = qnaIndex > 0 ? qnaIndex : 4;
                                        qna.Question += qna.Question == "" ? stringValues[index].InnerText : " " + stringValues[index].InnerText;
                                        break;
                                    case "D":
                                        qna.Answers.Add(stringValues[index].InnerText);
                                        break;
                                    case "E":
                                        qna.CorrectAnswer = 4 - qnaIndex;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (cell.CellValue != null)
                            {
                                if (Regex.Replace(cell.CellReference, "[0-9]", "") == "D")
                                {
                                    qna.Answers.Add(cell.CellValue.Text);
                                }                               
                            }
                        }
                        qnaIndex--;

                        if(qnaIndex == 0)
                        {
                            qnaList.Add(qna);
                            qna = new QuestionAndAnswer();
                        }              
                    }

                    quiz.QuizName = sheet.Name;
                    quiz.QuestionAndAnswers = qnaList;

                    await _quizService.CreateAsync(quiz);
                }
            }

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id, string adminId)
        {
            if (await _adminHelper.NotAdmin(adminId))
            {
                return Unauthorized();
            }

            var quiz = await _quizService.GetAsync(id);

            if (quiz is null)
            {
                return NotFound();
            }

            await _resultService.RemoveManyByQuizAsync(id);
            await _quizService.RemoveAsync(id);

            return NoContent();
        }

    }
}
