using HabitTracker.FelipeGirardi.Helpers;

namespace HabitTracker.UnitTests
{
    public class ValidatorHelperTests
    {
        private ValidatorService _validatorService;
        private static readonly object[] isValidDateCases =
        {
            new TestCaseData("18-12-25", true),
            new TestCaseData("31-01-25", true),
            new TestCaseData("   20-12-25   ", true),
            new TestCaseData("18-12", false),
            new TestCaseData("33-12-25", false),
            new TestCaseData("21-13-25", false)
        };
        private static readonly object[] isValidIntegerCases =
        {
            new TestCaseData("1", true),
            new TestCaseData("10", true),
            new TestCaseData("  5  ", true),
            new TestCaseData("hello", false),
            new TestCaseData("", false),
            new TestCaseData("...", false)
        };

        [SetUp]
        public void SetupValidatorHelper()
        {
            _validatorService = new ValidatorService();
        }

        [TestCaseSource(nameof(isValidDateCases))]
        public void CorrectInput_ReturnsBool_ResultingDateIsValidOrInvalid(string input, bool expectedResult)
        {
            var isValidDate = _validatorService.IsDateValid(input);

            Assert.That(isValidDate, Is.EqualTo(expectedResult));
        }

        [TestCaseSource(nameof(isValidIntegerCases))]
        public void CorrectInput_ReturnsBool_ResultingIntegerIsValidOrInvalid(string input, bool expectedResult)
        {
            var isValidDate = _validatorService.IsNumberValid(input);

            Assert.That(isValidDate, Is.EqualTo(expectedResult));
        }
    }
}
