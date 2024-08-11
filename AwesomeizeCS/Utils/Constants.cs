namespace AwesomeizeCS.Utils
{
    public class Constants
    {
        public const string TEST_SUCCESS = "Passed";
        public const string TEST_NONE_DEFINED = "There are no tests defined for this exercise.";
        public const string TIMEOUT_TEXT = "Timed out waiting for response";
        public const string FILE_NOT_FOUND = "File was not found at provided location";
        public const string OUTPUT_TESTING_SEPARATOR = "Output should be tested after this line";
        //public const string REGEX_STUDENT_EMAIL = @"^[a-z,A-Z]{2}[iI][erER]\d{4}@scs.ubbcluj.ro$";
        public const string REGEX_STUDENT_EMAIL = @"^[a-z,A-Z].*@scs.ubbcluj.ro$";
        public const string REGEX_STUDENT_GROUP = @"^\d{3}/\d$";
        public const string NARRATIVE_FANTASY_NAME = "Fantasy";
        public const string NARRATIVE_SCI_FI_NAME = "Sci-Fi";
        public const string NARRATIVE_MYSTERY_NAME = "Mystery";
        public const string NARRATIVE_HORROR_NAME = "Horror";
        public const string NARRATIVE_DEFAULT_NAME = "Default";
        public const string NARRATIVE_FANTASY_DESCRIPTION = "Walk the path of an apprentice mage and climb the ranks of an arcane guild bent on earning the trust of the local populace. Focused on steady progression and set in a generally calm environment, opt for this narrative if orderly accumulation and gradual improvement are traits that suit you.";
        public const string NARRATIVE_SCI_FI_DESCRIPTION = "Assume the role of a star ship technician, brought out of stasis by an emergency that has left your crew stranded among strange new stars. With a bigger emphasis on action and hectic progression, opt for this narrative if you feel like you thrive in fast-changing environments or prefer unforeseen twists and turns.";
        public const string NARRATIVE_MYSTERY_DESCRIPTION = "Embark on a nerve-wracking journey to the small town of Whaler's End, responding to the vaguely issued threats of a dangerous killer. This path may suit you best should you have a preference for atmospheric and macabre narratives. The flow of the story is fragmentary and often leaves you in the dark.";
        public const string NARRATIVE_HORROR_DESCRIPTION = "Step into the shoes of a security expert trying to protect his group of fellow survivors in the wake of a zombie apocalypse. If you respond well to stressful demands, timed events and the constant pressure of having to prevent misfortune, opt for this narrative. Focused on constantly building up tension, the story will often lead you from one harrowing event to the next.";
        public const string NARRATIVE_DEFAULT_DESCRIPTION = "Business as usual, you are not into narratives, don't want any more context than strictly nesessary. Technical details and that's it.";
        public const string PathToCppBuilder = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\VC\\Auxiliary\\Build\\vcvars32.bat";
        public const string PathToPythonInterpreter = "C:\\Python312\\python.exe";
    }
}
