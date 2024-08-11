using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories.Interfaces;
using AwesomeizeCS.Services.Interfaces;

namespace AwesomeizeCS.Services
{
    public class CodeVersionsService : ICodeVersionsService
    {
        private readonly ICodeVersionsRepository _repository;

        public CodeVersionsService(ICodeVersionsRepository repository)
        {
            _repository = repository;
        }
        public bool CodeVersionExists(Guid id)
        {
            return _repository.CodeVersionExists(id);
        }

        public async Task CreateCodeVersionAsync(CodeVersion codeVersion)
        {
            await _repository.CreateCodeVersionAsync(codeVersion);
        }

        public async Task DeleteCodeVersionAsync(Guid id)
        {
           await _repository.DeleteCodeVersionAsync(id);
        }

        public Task<List<CodeVersion>> GetAllCodeVersionsAsync()
        {
            return _repository.GetAllCodeVersionsAsync();
        }

        public Task<CodeVersion> GetCodeVersionByIdAsync(Guid id)
        {
            return _repository.GetCodeVersionByIdAsync(id);
        }

        public async Task UpdateCodeVersionAsync(CodeVersion codeVersion)
        {
            await _repository.UpdateCodeVersionAsync(codeVersion);
        }
    }
}
