using HiveGameWPFApp.Logic;
using System.Diagnostics.Contracts;
using Xunit;

namespace ClientTest
{
    public class ValidatorTest
    {
        [Fact]
        public void ValidateEmailTestSucces()
        {
            string email = "chris.12@gmail.com.mx";
            bool validationResult = Validator.ValidateEmail(email);
            Assert.True(validationResult);
        }

        [Fact]
        public void ValidateEmailFailTest()
        {
            string email = "chris-vasquez.12@gmail.com_mx";
            bool validationResult = Validator.ValidateEmail(email);
            Assert.False(validationResult);
        }

        [Fact]
        public void ValidateDescriptionTestSuccess()
        {
            string description = "Hello I'm christopher and this is a test, Nice to meet you! ¿How are you? :D";
            bool validationResult = Validator.ValidateDescription(description);
            Assert.True(validationResult);
        }

        [Fact]
        public void ValidateDescriptionFailTestSuccess()
        {
            string description = " ";
            bool validationResult = Validator.ValidateDescription(description); 
            Assert.False(validationResult);
        }

        [Fact]
        public void ValidatePasswordTestSuccess()
        {
            string password = "TheMostSecurePassword123*?";
            bool validationResult = Validator.ValidatePassword(password);
            Assert.True(validationResult);
        }

        [Fact]
        public void ValidatePasswordFailTestSuccess()
        {
            string password = "Hello 123*";
            bool validatorResult = Validator.ValidatePassword(password); 
            Assert.False(validatorResult);
        }

        [Fact]
        public void ValidateUsernameTestSuccess()
        {
            string username = "Chris2500";
            bool validationResult = Validator.ValidateUsername(username);
            Assert.True(validationResult);
        }

        [Fact]
        public void ValidateUsernameFailTestSuccess()
        {
            string username = "Chris2500*";
            bool validationResult = Validator.ValidateUsername(username);
            Assert.False(validationResult);
        }

        [Fact]
        public void ValidateNickNameTestSuccess()
        {
            string nickname = "The_Destroyer69";
            bool validationResult = Validator.ValidateNickName(nickname);
            Assert.True(validationResult);
        }

        [Fact]
        public void ValidateNickNameFailTest()
        {
            string nickname = "¿The_Destroyer69?";
            bool validationResult = Validator.ValidateNickName(nickname);
            Assert.False(validationResult);
        }

        [Fact]
        public void ValidateDateTestSuccess()
        {
            string date = "2024-10-23";
            bool validationResult = Validator.ValidateDate(date);
            Assert.True(validationResult);
        }

        [Fact]
        public void ValidateDateFailTestSuccess()
        {
            string date = "23-10-2024";
            bool validationResult = Validator.ValidateDate(date);
            Assert.False(validationResult);
        }

        [Fact]
        public void ValidateCodeTestSuccess()
        {
            string code = "578522";
            bool validationResult = Validator.ValidateCode(code);
            Assert.True(validationResult);  
        }

        [Fact]
        public void ValidateCodeFailTestSuccess()
        {
            string code = "j78k22";
            bool validationResult = Validator.ValidateCode(code);
            Assert.False(validationResult);
        }

        [Fact]
        public void ValidateStateTestSuccess()
        {
            string state = "Accepted";
            bool validatorResult = Validator.ValidateState(state);
            Assert.True(validatorResult);
        }

        [Fact]
        public void ValidateStateFailTestSuccess()
        {
            string state = "Declined!";
            bool validatorResult = Validator.ValidateState(state); 
            Assert.False(validatorResult);
        }

    }
}
