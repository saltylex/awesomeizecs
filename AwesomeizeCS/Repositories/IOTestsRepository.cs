using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using AwesomeizeCS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AwesomeizeCS.Repositories
{
    public class IOTestsRepository : IIOTestsRepository
    {
        private readonly ApplicationDbContext _context;

        public IOTestsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IOTest>> GetAllTestsAsync()
        {
            return await _context.IOTest.ToListAsync();
        }

        public async Task<IOTest> GetTestByIdAsync(Guid id)
        {
            return await _context.IOTest.FindAsync(id);
        } 
  

        public async Task<IOTest> GetTestByIdWithStepsAsync(Guid id)
        {
            return await _context.IOTest.Include(t => t.Steps)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateTestAsync(Guid? assignmentId, IOTest test)
        {
            if (assignmentId.HasValue)
            {
                _context.IOTest.Add(test);
                //_context.Assignment.First(a => a.Id == assignmentId).Tests.Add(iOTest);
                var assignment = _context.Assignment.First(a => a.Id == assignmentId);

                // Add the IOTest to the assignment
                if (assignment.Tests == null)
                {
                    assignment.Tests = new List<IOTest>();
                }
                assignment.Tests.Add(test);

                // Add the IOTest to the context
                _context.IOTest.Add(test);

                await _context.SaveChangesAsync();
                
            }
            else
            {
                _context.IOTest.Add(test);
                await _context.SaveChangesAsync();
                
            }
        }

        public async Task UpdateTestAsync(IOTest test)
        {
            _context.Entry(test).State = EntityState.Modified;
            _context.Update(test);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestAsync(Guid id)
        {
            var test = await _context.IOTest.FindAsync(id);
            if (test != null)
            {
                _context.IOTest.Remove(test);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddStepToTestAsync(Guid testId, TestStep step)
        {

            step.Id = Guid.NewGuid();

            var test = _context.IOTest.First(t => t.Id == testId);
            if (test.Steps == null)
            {
                test.Steps = new List<TestStep>();
                step.IOTest = test;
            }
            //test.Steps.Add(step);
            _context.TestStep.Add(step);

             await _context.SaveChangesAsync();

            //var test = await _context.IOTest.Include(t => t.Steps).FirstOrDefaultAsync(t => t.Id == testId);
            //if (test != null)
            //{
            //    step.Id = Guid.NewGuid();
            //    test.Steps.Add(step);
            //    await _context.SaveChangesAsync();
            //}
        }

        public async Task TextToStepView(Guid testId, string step)
        {
            
            var steps = step.Split(new[] { "; \r\n" }, StringSplitOptions.RemoveEmptyEntries);
            //step.Id = Guid.NewGuid();

            var test = _context.IOTest.Include(t => t.Steps).First(t => t.Id == testId);
            if (test.Steps == null)
            {
                test.Steps = new List<TestStep>();
            }
            test.Steps.RemoveAll(s => true);

            for (int i = 0; i < steps.Length; i++)
            {
                var parts = steps[i].Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                test.Steps.Add(new TestStep
                {
                    Id = Guid.NewGuid(),
                    Order = i,
                    ProvidedInput = parts[0],
                    ExpectedOutput = parts.Length == 1 ? null : parts[1]
                });
            }

            await _context.SaveChangesAsync();
                
            
        }

        public async Task DeleteStepFromTestAsync(Guid testId, Guid stepId)
        {
            var test = await _context.IOTest.Include(t => t.Steps).FirstOrDefaultAsync(t => t.Id == testId);
            if (test != null)
            {
                var step = test.Steps.FirstOrDefault(s => s.Id == stepId);
                if (step != null)
                {
                    test.Steps.Remove(step);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task MoveStepUpAsync(Guid id, Guid testId)
        {
            List<TestStep> steps = _context.IOTest.Include(t => t.Steps).First(t => t.Id == testId).Steps;
            var stepToMoveUp = steps.First(s => s.Id == id);
            steps.First(s => s.Order == stepToMoveUp.Order - 1).Order++;
            stepToMoveUp.Order--;
            await _context.SaveChangesAsync();
        }

        public async Task MoveStepDownAsync(Guid id, Guid testId)
        {
            List<TestStep> steps = _context.IOTest.Include(t => t.Steps).First(t => t.Id == testId).Steps;
            var stepToMoveDown = steps.First(s => s.Id == id);
            steps.First(s => s.Order == stepToMoveDown.Order + 1).Order--;
            stepToMoveDown.Order++;

            await _context.SaveChangesAsync();
        }

        public bool IOTestExists(Guid id)
        {
            return _context.IOTest.Any(e => e.Id == id);
        }
    }
}
