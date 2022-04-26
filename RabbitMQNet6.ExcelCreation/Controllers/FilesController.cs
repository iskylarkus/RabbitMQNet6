using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQNet6.ExcelCreation.Models;

namespace RabbitMQNet6.ExcelCreation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public FilesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, int fileId)
        {
            if (file is not { Length: > 0})
            {
                return BadRequest();
            }


            var userFile = await _appDbContext.UserFiles.FirstAsync(x => x.Id == fileId);

            var filePath = userFile?.FileName + Path.GetExtension(file.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/", filePath);


            using FileStream fileStream = new(path, FileMode.Create);

            await file.CopyToAsync(fileStream);


            userFile.CreatedDate = DateTime.Now;

            userFile.FilePath = filePath;

            userFile.FileStatus = FileStatus.Completed;

            await _appDbContext.SaveChangesAsync();


            return Ok();
        }
    }
}
