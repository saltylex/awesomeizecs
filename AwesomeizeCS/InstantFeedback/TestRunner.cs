using AwesomeizeCS.Data;
using AwesomeizeCS.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AwesomeizeCS.Utils;
using System.Configuration;
using System.Collections.Specialized;

namespace AwesomeizeCS.InstantFeedback
{   
    public class TestRunner 
    {

        private ApplicationDbContext _db;
        private StringBuilder logger = new StringBuilder();
        public ApplicationDbContext Db
        {
            get
            {
                return _db;
            }
            private set
            {
                _db = value;
            }
        }
        public TestRunner(ApplicationDbContext db)
        {
            _db = db;

        }


        public async Task RunTests(string pathToSource, Guid assignmnetId, Guid studentId, Guid codeVersionId) {

            try {
                ProcessStartInfo processToTest = null;
                var assignments = _db.StudentAssignment.Include(sa => sa.Assignment).ThenInclude(a => a.Tests).ThenInclude(t => t.Steps).First(sa => sa.Assignment.Id == assignmnetId && sa.Student.Id == studentId);
                //here it saves  code version for statistics
                CodeVersion codeVersion = new CodeVersion { Id = codeVersionId, UploadDate = DateTime.Now, CodeFor = assignments, Location = pathToSource, Results = new List<TestResult>() };
                var testsToRun = assignments.Assignment.Tests.ToList();
                

                if(testsToRun == null || testsToRun.Count == 0)
                {
                    //handle error
                    SaveErrorResult(pathToSource, codeVersion, Constants.TEST_NONE_DEFINED);
                    //all tests passed bool
                    return;
                   
                }

                if (GetSourceFiles(pathToSource).Count == 0) //maybe use this to get the extension for right compiler
                {
                    SaveErrorResult (pathToSource, codeVersion, "Upload error: no source files found!");
                    return;
                }

                try
                {
                    processToTest = PrepareProcess(pathToSource, "c++"); //add python implementation here
                }
                catch(Exception e)
                {
                    //handle error
                    SaveErrorResult( pathToSource, codeVersion, "Compile Error: " + e.InnerException?.Message.Replace(pathToSource.ToLower(), string.Empty));
                    return;
                }

                int count = 0;
                while(!File.Exists(processToTest.FileName) && count < 40) // count<40 to wait for compiler, there is no way to know if it crashed, finished, or stuck
                {
                    count++;
                    Thread.Sleep(50); //wait for count and compiler 
                }
                Thread.Sleep(100); 

                if (!File.Exists(processToTest.FileName))
                {
                    SaveErrorResult(pathToSource, codeVersion, "Could not find compiled file, the server might be overloaded at the moment, re-try in a couple of minutes");
                    return;
                }
                //May need to use SeriLogger or implement a singleton logger 
                logger.AppendLine();
                logger.AppendLine($"Testing started for exercise: {assignments.Assignment.Name}");
                foreach (var test in testsToRun) 
                {
                    //Store results 
                    //added the function return value to results
                    codeVersion.Results.Add(RunTest(processToTest, test, pathToSource));
                    
                }
                //fill in lines 89-100, uses codeVersion in TestRunner class OG
                if(codeVersion.Results.Count == 0)
                {
                    codeVersion.Results = null;
                }
                else
                {
                    var resultsWhereTheTestRan = codeVersion.Results.Where(r => r.Test != null && r.Result == Constants.TEST_SUCCESS).ToList();
                    //if(resultsWhereTheTestRan.Count == testsToRun.Count)
                    //{
                        //have a bool for allTestsPassed? possible extension for improvement
                    //}
                }

                logger.AppendLine($"Test result count: {codeVersion.Results?.Count ?? 0}");
                _db.CodeVersion.Add(codeVersion);
                _db.SaveChanges();
                File.WriteAllText(Path.Combine(pathToSource, "log.txt"), logger.ToString());
            }
            catch(Exception e) 
            {
                logger.AppendLine($"RunTests exception: {e.ToString()}");
                File.WriteAllText(Path.Combine(pathToSource, "log.txt"), logger.ToString());
                throw e;
            }
        }

        private void SaveErrorResult(string pathToSource, CodeVersion results, string message)
        {

            results.Results.Add(new TestResult { Id = Guid.NewGuid(), Result = message, Test = null });
            _db.CodeVersion.Add(results);
            _db.SaveChanges();
            File.WriteAllText(Path.Combine(pathToSource, "log.txt"), logger.ToString());
            

        }

        private static List<string> GetSourceFiles(string pathToSource)
        {
            var allowedExtensions = new List<string> { ".c", ".cpp", ".py"/*, ".h"*/ };
            var directories = Directory.GetDirectories(pathToSource, "*.*", SearchOption.AllDirectories);
            var files = new List<string>();
            foreach (var directory in directories)
            {
                files.AddRange(Directory.GetFiles(directory));
            }
            files.AddRange(Directory.GetFiles(pathToSource));
            var validSourceFilePaths = files.Where(f => allowedExtensions.Contains(Path.GetExtension(f))).ToList();
            return validSourceFilePaths;
        }

        object lockObject = new object();
        List<Todo> todos = new List<Todo>();

        private TestResult RunTest(ProcessStartInfo startInfo, IOTest test, string pathToSource)
        {
            //startinfo values. debugging
            //logger.AppendLine(string.Format("At: {0} RunTest Started for {1}: with steps: {3} {2}", DateTime.Now, test, Environment.NewLine)); 
            TestResult testResult = new TestResult
            {
                Id = Guid.NewGuid(),
                Test = test
            };

            string outputAsString ="";

            if (test.Steps.Any(s => s.ProvidedInput.Contains("Location")))
            {
                outputAsString = RunFileBasedTest(startInfo, test, pathToSource); // file based test
            }
            else
            {
                outputAsString = RunStdIOBasedTest(startInfo, test); // IO based test
            }
            logger.AppendLine(string.Format("At: {0} Output was: {2} {1} {2}", DateTime.Now, outputAsString, Environment.NewLine));


            if (outputAsString.Contains(Constants.TIMEOUT_TEXT) || outputAsString.Contains(Constants.FILE_NOT_FOUND))
            {
                testResult.Result = outputAsString.Contains(Constants.TIMEOUT_TEXT) ? Constants.TIMEOUT_TEXT : Constants.FILE_NOT_FOUND; ;
                testResult.Output = outputAsString.Replace(Constants.TIMEOUT_TEXT, String.Empty);
                testResult.Output = outputAsString.Replace(Constants.OUTPUT_TESTING_SEPARATOR + Environment.NewLine, String.Empty);
            }
            else
            {
                var allStepsDone = true;
                try
                {
                    var outputLines = outputAsString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    var outputsToTest = new StringBuilder();
                    bool separatorReached = false;
                    foreach (var line in outputLines)
                    {
                        if (line == Constants.OUTPUT_TESTING_SEPARATOR)
                        {
                            separatorReached = true;
                            continue;
                        }
                        if (separatorReached)
                        {
                            outputsToTest.AppendLine(line);
                        }
                    }

                    outputAsString = outputAsString.Replace(Constants.OUTPUT_TESTING_SEPARATOR + Environment.NewLine, String.Empty);

                    var stepWithExpectedOutput = test.Steps.FirstOrDefault(s => !string.IsNullOrEmpty(s.ExpectedOutput) && s.ExpectedOutput.Contains("regex"));
                    if (stepWithExpectedOutput == null)
                    {
                        stepWithExpectedOutput = test.Steps.FirstOrDefault(s => !string.IsNullOrEmpty(s.ExpectedOutput) && s.ExpectedOutput.Length > 0);
                    }

                    int resultCount = 1;
                    string regularExpression = stepWithExpectedOutput.ExpectedOutput;
                    if (stepWithExpectedOutput.ExpectedOutput.Contains("<regex>"))
                    {
                        var regexSeparatorIndex = stepWithExpectedOutput.ExpectedOutput.IndexOf("<regex>");
                        if (!int.TryParse(stepWithExpectedOutput.ExpectedOutput.Substring(0, regexSeparatorIndex), out resultCount))
                        {
                            resultCount = 1;
                        }
                        regularExpression = stepWithExpectedOutput.ExpectedOutput.Substring(regexSeparatorIndex + 7);
                    }
                    Regex regex = new Regex(regularExpression, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (regex.Matches(outputsToTest.ToString()).Count != resultCount)
                    {
                        allStepsDone = false;
                    }
                }
                catch (Exception ex)
                {
                    logger.AppendLine($"At: {DateTime.Now} Exception while regex testing: {ex}");
                    testResult.Result = "Not expected result";
                    testResult.Output = outputAsString;
                    return testResult;
                }
                testResult.Result = allStepsDone ? Constants.TEST_SUCCESS : "Not expected result";
                testResult.Output = outputAsString;
            }
            logger.AppendLine("***********************************************************");
            logger.AppendLine($"At: {DateTime.Now} Test result: *{testResult.Result}*");
            logger.AppendLine("***********************************************************");

            return testResult;
        }

        private string RunStdIOBasedTest(ProcessStartInfo startInfo, IOTest test)
        {
            int timeout = 1000;

            foreach (var step in test.Steps.OrderBy(s => s.Order))
            {
                if (step.ExpectedOutput != null && step.ExpectedOutput.Contains("regex"))
                {
                    todos.Add(new Todo { StepNumber = step.Order, Operation = "read", Value = step.ExpectedOutput });
                }
                todos.Add(new Todo { StepNumber = step.Order, Operation = "write", Value = step.ProvidedInput });
            }
            
            //logger.AppendLine(string.Format("At: {0} RunTest Process Started with todolist: {1}", DateTime.Now, string.Join(" ", todos)));
            bool hasTimedOut = false;

            StringBuilder output = new StringBuilder();
            using (Process process = Process.Start(startInfo))
            {
                Thread.Sleep(200);
                int pid = process.Id;
                //process.ProcessorAffinity = (IntPtr)2;
                process.OutputDataReceived += (sender, dataReceived) =>
                {
                    if (dataReceived.Data != null)
                    {
                        lock (lockObject)
                        {
                            output.AppendLine($"<p class=\"studentCommand\">{dataReceived.Data}</p>");
                        }
                    }
                };
                //process.ErrorDataReceived += (sender, e) =>
                //{
                //    if (e.Data != null)
                //    {
                //        error.AppendLine(e.Data);
                //        todos = null;
                //    }
                //};

                //process.Start();
                process.BeginOutputReadLine();
                //process.BeginErrorReadLine();

                var writerCancellationToken = new CancellationTokenSource();
                var writer = new Task(() =>
                {
                    while (todos.Count > 0)
                    {
                        lock (lockObject)
                        {
                            if (todos[0].Operation == "read")
                            {
                                output.AppendLine(Constants.OUTPUT_TESTING_SEPARATOR);
                            }
                            else
                            {
                                output.AppendLine($"<p class=\"serverCommand\">{todos[0].Value}</p>");
                                process.StandardInput.WriteLine(todos[0].Value);
                            }

                            todos.RemoveAt(0);
                        }
                        Thread.Sleep(100);
                    }

                }, writerCancellationToken.Token);
                writer.Start();
                writer.Wait();
                if (process.WaitForExit(timeout))
                {
                    // Process completed. Check process.ExitCode here.
                    //logger.AppendLine(string.Format("At: {0} RunTest Process completed Output: {4}\"{1}\" Error: \"{2}\" Todos left: {3}", DateTime.Now, output.ToString(), error.ToString(), string.Join(" ", todos), Environment.NewLine));
                    logger.AppendLine($"At: {DateTime.Now} RunTest Process completed, Todos left: {string.Join(" ", todos)}");
                    writerCancellationToken.Cancel();
                }
                else
                {
                    // Timed out.
                    //logger.AppendLine(string.Format("At: {0} Process timed out Output: {4}\"{1}\" Error: \"{2}\" Todos left: {3}", DateTime.Now, output.ToString(), error.ToString(), string.Join(" ", todos), Environment.NewLine));
                    logger.AppendLine($"At: {DateTime.Now} Process timed out, Todos left: {string.Join(" ", todos)}");
                    hasTimedOut = true;
                    writerCancellationToken.Cancel();
                    process.Close();

                }
                logger.AppendLine($"At: {DateTime.Now} writer.IsCanceled: {writer.IsCanceled}, writer.IsCompleted: {writer.IsCompleted}");

                try
                {
                    if (!process.HasExited)
                    {
                        var processFromOs = Process.GetProcessById(pid);
                        if (!processFromOs.HasExited)
                        {
                            processFromOs.Kill();
                            logger.AppendLine($"At: {DateTime.Now} Process killed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.AppendLine(string.Format("At: {0} Exception while trying to kill process: \"{1}\"", DateTime.Now, ex));
                }
                //process.CancelErrorRead();
                //process.CancelOutputRead();

            }



            //var errorAsString = error.ToString();
            //logger.AppendLine(string.Format("At: {0} RunTest Process error: {1}", DateTime.Now, errorAsString));
            //if (!string.IsNullOrEmpty(errorAsString))
            //{
            //    testResult.Result = string.Format("Error occured: {0}", errorAsString);
            //}
            if (hasTimedOut)
            {
                return output.AppendLine(Constants.TIMEOUT_TEXT).ToString();
            }
            return output.ToString();
            //logger.AppendLine(string.Format("At: {0} RunTest Process output: {1}", DateTime.Now, outputAsString));

        }

        private string RunFileBasedTest(ProcessStartInfo startInfo, IOTest test, string pathToSource)
        {
            int timeout = 1000;
            bool hasTimedOut = false;
            StringBuilder output = new StringBuilder();
            var inputWithFile = test.Steps.OrderBy(s => s.Order).Last(s => s.ProvidedInput.Contains("Location")).ProvidedInput;
            var filePathToExpectedOutput = Path.Combine(pathToSource, Path.GetExtension(inputWithFile.Contains("fileLocation") ? inputWithFile.Substring(13) : inputWithFile.Substring(15)));
            if (File.Exists(filePathToExpectedOutput))
            {
                File.Delete(filePathToExpectedOutput);
            }
            int pid;

            using (Process process = Process.Start(startInfo))
            {
                Thread.Sleep(200);
                pid = process.Id;

                foreach (var step in test.Steps.OrderBy(s => s.Order))
                {
                    if (step.ProvidedInput.Contains("fileLocation"))
                    {
                        process.StandardInput.WriteLine($"fileLocation {Path.Combine(pathToSource, step.ProvidedInput.Substring(13))}");
                        output.AppendLine($"<p class=\"serverCommand\">fileLocation {Path.Combine(pathToSource, step.ProvidedInput.Substring(13))}</p>");
                    }
                    else
                    {
                        if (step.ProvidedInput.Contains("mylistLocation"))
                        {
                            process.StandardInput.WriteLine($"mylistLocation {Path.Combine(pathToSource, step.ProvidedInput.Substring(15))}");
                            output.AppendLine($"<p class=\"serverCommand\">mylistLocation {Path.Combine(pathToSource, step.ProvidedInput.Substring(15))}</p>");
                        }
                        else

                        {
                            process.StandardInput.WriteLine(step.ProvidedInput);
                            output.AppendLine($"<p class=\"serverCommand\">{step.ProvidedInput}</p>");
                        }
                    }
                    Thread.Sleep(100);
                }
                process.StandardInput.WriteLine("exit");
                output.AppendLine("exit");

                if (process.WaitForExit(timeout))
                {
                    // Process completed. Check process.ExitCode here.
                    logger.AppendLine($"At: {DateTime.Now} RunFileBasedTest Process completed.{Environment.NewLine}");
                }
                else
                {
                    // Timed out.
                    logger.AppendLine($"At: {DateTime.Now} RunFileBasedTest timed out.{Environment.NewLine}");
                    hasTimedOut = true;
                    process.Close();
                }

                try
                {
                    if (!process.HasExited)
                    {
                        var processFromOs = Process.GetProcessById(pid);
                        if (!processFromOs.HasExited)
                        {
                            processFromOs.Kill();
                            logger.AppendLine($"At: {DateTime.Now} Process killed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.AppendLine($"At: {DateTime.Now} Exception while trying to kill process: \"{ex}\"");
                }
            }

            if (hasTimedOut)
            {
                return Constants.TIMEOUT_TEXT;
            }

            output.AppendLine("File contents:");
            if (File.Exists(filePathToExpectedOutput))
            {
                output.AppendLine(Constants.OUTPUT_TESTING_SEPARATOR + Environment.NewLine);
                foreach (var line in File.ReadAllLines(filePathToExpectedOutput))
                {
                    output.AppendLine($"<p class=\"studentCommand\">{line}</p>");
                }

            }
            else
            {
                output.AppendLine(Constants.FILE_NOT_FOUND);
            }
            return output.ToString();
        }

        public ProcessStartInfo PrepareProcess(string pathToSource, string programmingLanguage)
        {
            try
            {
                switch (programmingLanguage.ToLower())
                {
                    case "python":
                        return PreparePython(pathToSource);
                    case "c++":
                        return PrepareCpp(pathToSource);
                    default:
                        throw new ArgumentException("Programming language is not set correctly");
                }
            }
            catch (Exception e)
            {
                logger.AppendLine($"PrepareProcess exception thrown: {e}");
                throw new Exception("Compile error", e);
            }
        }

        private ProcessStartInfo PrepareCpp(string pathToSource)
        {
            CompileUsingCl(pathToSource, logger);
            var startInfo = new ProcessStartInfo(pathToSource + "\\program.exe")
            {
                CreateNoWindow = true,
                //FileName = pathToSource + "\\program.exe",
                //FileName = Path.Combine(pathToSource, "program.exe"),
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Normal,
            };
            return startInfo;
        }

        private ProcessStartInfo PreparePython(string pathToSource)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = Constants.PathToPythonInterpreter,
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = " -i " + pathToSource
            };

            return startInfo;
        }
        public void CompileUsingCl(string pathToSource, StringBuilder logger)
        {
                
            ProcessStartInfo info = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false, // %comspec% /k "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\VC\Auxiliary\Build\vcvars32.bat"
                FileName = @"cmd.exe",
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = "/k \"" + Constants.PathToCppBuilder + "\""  // +1 here 
               
            };
            logger.AppendLine();
            logger.AppendLine("Compilation started");
            logger.AppendLine($"At: {DateTime.Now} CompileUsingCL arguments: {"/k \"" + /*WebConfigurationManager.AppSettings["PathToCppBuilder"] +*/ "\""}"); 

            var fileNamesWithPath = GetSourceFiles(pathToSource);
            //var fileNames = fileNamesWithPath.Select(f => Path.GetFileName(f));
            string compileParameter =
                //string.Format("cl /permissive- /GS- /analyze- /w /Zc:wchar_t /Z7 /FS /Gm- /Od /sdl /Zc:inline /fp:precise /D \"WIN32\" /D \"_DEBUG\" /D \"_CONSOLE\" /D \"_UNICODE\" /D \"UNICODE\" /D \"_CRT_SECURE_NO_WARNINGS\" /errorReport:none /WX- /Zc:forScope /RTC1 /Gd /Oy- /MDd /FC /EHsc /nologo /diagnostics:column {0} /Fe:\"{1}\" /Fo\"{2}\\\\\"", 
                string.Format("cl /permissive- /GS- /analyze- /w /Zc:wchar_t /ZI /Gm- /Od /sdl /Zc:inline /fp:precise /D \"WIN32\" /D \"_DEBUG\" /D \"_CONSOLE\" /D \"_UNICODE\" /D \"UNICODE\" /D \"_CRT_SECURE_NO_WARNINGS\" /errorReport:none /WX- /Zc:forScope /RTC1 /Gd /Oy- /MDd /FC /EHsc /nologo /diagnostics:column {0}  /Fa.\\ /Fe:.\\program.exe /Fo.\\",
                string.Join(" ", fileNamesWithPath.Select(f => $"\"{f}\"")),
                Path.Combine(pathToSource, "program.exe"),
                pathToSource);
            logger.AppendLine($"CompileUsingCL compile command: {compileParameter}");
            string message = string.Empty;

            using (Process process = Process.Start(info))
            {
                process.StandardInput.WriteLine($"cd {pathToSource}");
                process.StandardInput.WriteLine(compileParameter);
                process.StandardInput.WriteLine(compileParameter);
                StringBuilder fullMessageOutput = new StringBuilder();
                process.StandardInput.WriteLine("exit");
                while (!process.StandardOutput.EndOfStream)
                {
                    var buffer = process.StandardOutput.ReadLine();
                    fullMessageOutput.AppendLine(buffer);
                }

                message = string.Join("</br>", CleanCompileOutput(fullMessageOutput.ToString()));
            }
            logger.AppendLine(string.Format("CompileUsingCL compile response: {0}", message));
            if (message.Contains("error"))
            {
                throw new Exception(message);
            }
        }

        private static List<string> CleanCompileOutput(string raw)
        {
            var lines = raw.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var errors = lines.Where(l => l.Substring(l.Length - 2) != ".c" && l.Substring(l.Length - 2) != ".cpp" && !l.Contains("Generating Code...") && !l.Contains(">cl ") && !l.Contains(">cd") && !l.Contains(">exit") && !string.IsNullOrWhiteSpace(l) && l[0] != '*' && !l.Contains("[vcvarsall.bat]")).ToList();

            return errors;
        }

        public static void OldCompileUsingCl(string pathToSource, StringBuilder logger)
        {

            ProcessStartInfo info = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false, // %comspec% /k "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\VC\Auxiliary\Build\vcvars32.bat"
                FileName = @"cmd.exe",
                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = "/k \"" + Constants.PathToCppBuilder + "\"" //web
                //Arguments = "/k \"" + /*WebConfigurationManager.AppSettings["PathToCppBuilder"] +*/ "\"" //web
            };
            logger.AppendLine($"At: {DateTime.Now} CompileUsingCL arguments: {"/k \"" + /*WebConfigurationManager.AppSettings["PathToCppBuilder"] +*/ "\""}"); //figure this out


            var fileNamesWithPath = GetSourceFiles(pathToSource);
            var fileNames = fileNamesWithPath.Select(Path.GetFileName);
            string compileParameter = $"cl \"{string.Join(" ", fileNamesWithPath)}\" /Fe:\"{Path.Combine(pathToSource, "program.exe")}\" /Fo{pathToSource}\\";
            logger.AppendLine($"CompileUsingCL compile command: {compileParameter}");

            using (Process process = Process.Start(info))
            {
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();
                    process.StandardInput.WriteLine(compileParameter);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    int timeout = 300;
                    if (process.WaitForExit(timeout) &&
                        outputWaitHandle.WaitOne(timeout) &&
                        errorWaitHandle.WaitOne(timeout))
                    {
                        logger.AppendLine($"At: {DateTime.Now} CompileUsingCL Process completed Output: {output.ToString()} Error: {error.ToString()}");
                        // Process completed. Check process.ExitCode here.
                        process.Close();
                        return;
                    }
                    else
                    {
                        logger.AppendLine($"At: {DateTime.Now} CompileUsingCL Process timed out Output: {output.ToString()} Error: {error.ToString()}");
                        // Timed out.
                        process.Close();
                        return;
                    }
                    //process.WaitForExit();
                }
            }
        }

    }
}
