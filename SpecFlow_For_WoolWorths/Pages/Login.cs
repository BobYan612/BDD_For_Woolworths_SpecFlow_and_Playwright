/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;

namespace PlaywrightTest.pages
{
    
    /// <summary>
    /// The class is used for Login Page
    /// </summary>
    public class Login : PageBase
    {

        public Login(IPage user) : base(user) { }

        //The below variables are for the elements and user information 

        private string loginEmailInput = null!;
        private string loginEmailContinueButton = null!;
        private string loginPasswordInput = null!;
        private string loginPasswordSignButton = null!;
        private string username = "45139921@qq.com";
        private string password = "Bobyan_417";

        /// <summary>
        /// The method is used to initialize the location variables from the user configuration file
        /// </summary>
        protected override void InitLocationDefinition()
        {
            loginEmailInput = userSettings.GetAppParameter("loginpage.email_input_location");
            loginEmailContinueButton = userSettings.GetAppParameter("loginpage.email_continue_button_text");
            loginPasswordInput = userSettings.GetAppParameter("loginpage.password_input_location");
            loginPasswordSignButton = userSettings.GetAppParameter("loginpage.password_sign_button_text");
            username = userSettings.GetAppParameter("username");
            password = userSettings.GetAppParameter("password");
        }

        /// <summary>
        /// The method is used to completed the login process
        /// </summary>
        /// <returns>Task</returns>
        public async Task LoginIn()
        {
            ILocator emailInput = await LocateElement(loginEmailInput);
            await emailInput.FillAsync(username);

            var emailContinueButton = await LocateElement(loginEmailContinueButton, SearchType.Text);
            await emailContinueButton.ClickAsync();

            ILocator passwordInput = await LocateElement(loginPasswordInput);
            await passwordInput.FillAsync(password);
            var passwordContinueButton = await LocateElementByChildText(loginPasswordSignButton, "button");
            await passwordContinueButton.ClickAsync();
        }

    }
}
