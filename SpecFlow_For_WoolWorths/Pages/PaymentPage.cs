/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace PlaywrightTest.pages
{
    /// <summary>
    /// The class is used for Payment Page
    /// </summary>
    public class PaymentPage : PageBase
    {

        public PaymentPage(IPage page) : base(page) { }

        //The below are the location definition for all the elements in payment page

        private string pickupWholeFrameTag = null!;
        private string pickupOptionText = null!;
        private string selectDateWholeFrameTag = null!;
        private string selectTimeslotWholeFrameTag = null!;
        private string commonSingleOptionFrameTag = null!;
        private string timeslotSingleOptionFrameTag = null!;
        private string selectOptionRadioType = null!;
        private string selectTimeslotOptionTag = null!;
        private string continueButton = null!;
        private string continuePaymentButton = null!;
        private string confirmCheckBoxID = null!;
        private string confirmInputCheckBox = null!;
        private string confirmButton = null!;
        private string creditCardNumber = null!;
        private string creditExpireMonth = null!;
        private string creditExpireYear = null!;
        private string creditCVVNumber = null!;
        private string submitPaymentButton = null!;
        private string paymentLogoPageText = null!;
        private string paymentReviewOrderPageText = null!;
        private string paymentReviewChangeButton = null!;
        private string paymentURL = null!;
        private string creditIframeLocation = null!;
        private string creditIframeParent = null!;

        private string creditCardNumberValue = null!;
        private string creditExpireMonthValue = null!;
        private string creditExpireYearValue = null!;
        private string creditCVVNumberValue = null!;


        /// <summary>
        /// The method is used to read the definition from user configuration file.
        /// </summary>
        protected override void InitLocationDefinition()
        {
            pickupWholeFrameTag = userSettings.GetAppParameter("paymentpage.pickupframe_tag");
            pickupOptionText = userSettings.GetAppParameter("paymentpage.pickupOption_text");
            selectDateWholeFrameTag = userSettings.GetAppParameter("paymentpage.dateframe_tag");
            selectTimeslotWholeFrameTag = userSettings.GetAppParameter("paymentpage.timeslotframe_tag");
            commonSingleOptionFrameTag = userSettings.GetAppParameter("paymentpage.common_single_option_frame_tag");
            timeslotSingleOptionFrameTag = userSettings.GetAppParameter("paymentpage.timeslot_single_option_frame_tag");
            selectOptionRadioType = userSettings.GetAppParameter("paymentpage.radio_input_type");
            selectTimeslotOptionTag = userSettings.GetAppParameter("paymentpage.timeslot_option_tag");
            continueButton = userSettings.GetAppParameter("paymentpage.continue_button_text");
            continuePaymentButton = userSettings.GetAppParameter("paymentpage.term_continue_button_text");
            confirmCheckBoxID = userSettings.GetAppParameter("paymentpage.term_confirm_label");
            confirmInputCheckBox = userSettings.GetAppParameter("paymentpage.term_confirm_input");
            confirmButton = userSettings.GetAppParameter("paymentpage.term_continue_button_text");
            creditCardNumber = userSettings.GetAppParameter("paymentpage.credit_card_number_id");
            creditExpireMonth = userSettings.GetAppParameter("paymentpage.credit_card_month_id");
            creditExpireYear = userSettings.GetAppParameter("paymentpage.credit_card_year_id");
            creditCVVNumber = userSettings.GetAppParameter("paymentpage.credit_card_cvv_csv_id");
            submitPaymentButton = userSettings.GetAppParameter("paymentpage.credit_card_submit_button_text");
            paymentLogoPageText = userSettings.GetAppParameter("paymentpage.transaction_page_logo_text");
            creditCardNumberValue = userSettings.GetAppParameter("credit_card_number");
            creditExpireMonthValue = userSettings.GetAppParameter("credit_card_exp_mm");
            creditExpireYearValue = userSettings.GetAppParameter("credit_card_exp_yy");
            creditCVVNumberValue = userSettings.GetAppParameter("credit_card_cvv");
            paymentReviewOrderPageText = userSettings.GetAppParameter("paymentpage.review_order_page_text");
            paymentReviewChangeButton = userSettings.GetAppParameter("paymentpage.review_button_change");
            paymentURL = userSettings.GetAppParameter("paymentpage.payment_url");
            creditIframeLocation = userSettings.GetAppParameter("paymentpage.credit_iframe_location");
            creditIframeParent = userSettings.GetAppParameter("paymentpage.credit_iframe_parent");
        }

        /// <summary>
        /// The method is used to complete one transaction
        /// </summary>
        /// <returns></returns>
        public async Task CompleteTransaction()
        {
            await FillCreditCard();
            ILocator submitButtonElement = await LocateElement(submitPaymentButton, SearchType.Text);
            Boolean isEnabled = await submitButtonElement.IsEnabledAsync();
            if (isEnabled) { await submitButtonElement.ClickAsync(); }
            else { logger.LogInformation("The credit information is not valid, skip the submittion"); };
        }

        /// <summary>
        /// The method is used to verify the result of payment
        /// </summary>
        /// <param name="expectSucceeded">expecting succeeded or not</param>
        /// <returns></returns>
        public async Task AssertPaymentIsSucceeded(bool expectSucceeded)
        {
            //Will finish it if I know the atuall return status.
            ILocator successPage = await LocateElement(paymentLogoPageText, SearchType.XPath, false);
            bool isSuccess = false;

            if (successPage != null)
            {
                isSuccess = await successPage.IsVisibleAsync();
            } 
            Assert.AreEqual(isSuccess, expectSucceeded);
        }

        /// <summary>
        /// The method is used to select a radio option on the payment page.
        /// </summary>
        /// <param name="optionText">The option text, such as "Delivery" or "Saturday" or "10:00am - 10:30am"</param>
        /// <returns>Task</returns>
        public async Task SelectRadioOption(string optionText)
        {
            ILocator cdxRadioFrameElement = await LocateElementByChildText(optionText, commonSingleOptionFrameTag);
            ILocator radioInput = cdxRadioFrameElement.Locator(selectOptionRadioType);
            Assert.IsNotNull(radioInput);
            await radioInput.ClickAsync();
        }

       
        /// <summary>
        /// The method is used to complete a random option in booking page.
        /// </summary>
        /// <returns></returns>
        public async Task CompleteRandomBookSelection()
        {
            ILocator reviewPage = await LocateElement(paymentReviewOrderPageText, SearchType.Text);
            //If review page is appeared, it means the deliver/pickup options is effective
            //So, we may skip the deliver or pickup stage.
            if (reviewPage != null)
            {
                bool reviewPageIsVisible = await reviewPage.IsVisibleAsync();
                if (reviewPageIsVisible)
                {
                    ILocator changeBookButton = await LocateElement(paymentReviewChangeButton);
                    await changeBookButton.ClickAsync();
                }
            }
            await ClickPickupOption();
            Thread.Sleep(1000);

            await ClickRandomDateOption();
            Thread.Sleep(1000);

            await ClickRandomTimeslotOption();
            Thread.Sleep(1000);

            ILocator continueButtonElement = await LocateElement(continueButton, SearchType.Text);
            await continueButtonElement.ClickAsync();


            //Wait 2 seconds to appear the confirm box
            Thread.Sleep(2000);
            await AcceptConfirmTerm();

/*            ILocator continuePaymentButtonElement = await LocateElement(continuePaymentButton, SearchType.Text);
            if (continuePaymentButtonElement != null)
            {
                await continuePaymentButtonElement.ClickAsync();
            }*/
        }

        /// <summary>
        /// The method is used to fill the credit card information
        /// </summary>
        /// <returns></returns>
        public async Task FillCreditCard()
        {
            //Sleep 2 seconds for the input of credit cards
            Thread.Sleep(2000);
            logger.LogInformation(this._page.Url);

            /*            ILocator creditCardNumberInput = _page.FrameLocator(creditIframeClassname).First.Locator(creditCardNumber);
                        ILocator creditCardExpMMInput = _page.FrameLocator(creditIframeClassname).First.Locator(creditExpireMonth);
                        ILocator creditCardExpYYInput = _page.FrameLocator(creditIframeClassname).First.Locator(creditExpireYear);
                        ILocator creditCardCVVInput = _page.FrameLocator(creditIframeClassname).First.Locator(creditCVVNumber);
                        */
            //var locator = _page.Locator("iframe[name=\"embedded\"]");
            var iframeParent = await LocateElement(creditIframeParent);
            var frame = iframeParent.FrameLocator(creditIframeLocation);
            if (frame != null)
            {
                await frame.Locator("#" + creditCardNumber).FillAsync(creditCardNumber);
                await frame.Locator("#" + creditExpireMonth).FillAsync(creditExpireMonthValue);
                await frame.Locator("#" + creditExpireYear).FillAsync(creditExpireYearValue);
                await frame.Locator("#" + creditCVVNumber).FillAsync(creditCVVNumberValue);
            }
            else
            {
                logger.LogInformation("The iframe is null!");
            }

        }

        /// <summary>
        /// The method is used to click a random pickup option
        /// </summary>
        /// <returns>Task</returns>
        public async Task ClickRandomDileverOrPickupOption()
        {
            ILocator pickupFrame = await LocateElement(pickupWholeFrameTag);
            IReadOnlyList<ILocator> pickupOptinonFrames = await pickupFrame.Locator(commonSingleOptionFrameTag).Locator("visible=true").AllAsync();
            Random r = new Random();
            int index = r.Next(0, pickupOptinonFrames.Count);            
            ILocator selectedOptionElement = pickupOptinonFrames[index];
            ILocator radioInput = await LocateElementFromParent(selectedOptionElement, selectOptionRadioType);
            await radioInput.ClickAsync();            
        }

        /// <summary>
        /// The method is used to click a random pickup option
        /// </summary>
        /// <returns>Task</returns>
        public async Task ClickPickupOption()
        {
            await SelectRadioOption(pickupOptionText);
        }

        /// <summary>
        /// The method is used to click a random date option
        /// </summary>
        /// <returns>Task</returns>
        public async Task ClickRandomDateOption()
        {
            ILocator dateFrame = await LocateElement(selectDateWholeFrameTag);
            IReadOnlyList<ILocator> dateOptinonFrames = await dateFrame.Locator(commonSingleOptionFrameTag).Locator("visible=true").AllAsync();
            Random r = new Random();
            int index = r.Next(0, dateOptinonFrames.Count);
            logger.LogInformation("Got index is " + index.ToString());
            ILocator selectedOptionElement = dateOptinonFrames[index];
            ILocator radioInput = await LocateElementFromParent(selectedOptionElement, selectOptionRadioType);
            await radioInput.ClickAsync();
        }

        /// <summary>
        /// The method is used to click a random timeslot option
        /// </summary>
        /// <returns>Task</returns>
        public async Task ClickRandomTimeslotOption()
        {
            ILocator timeslotFrame = await LocateElement(selectTimeslotWholeFrameTag);
            IReadOnlyList<ILocator> timeslotOptinonFrames = await timeslotFrame.Locator(timeslotSingleOptionFrameTag).Locator("visible=true").AllAsync();
            Random r = new Random();
            int index = r.Next(0, timeslotOptinonFrames.Count);
            ILocator selectedOptionElement = timeslotOptinonFrames[index];
            ILocator optionLabel = await LocateElementFromParent(selectedOptionElement, selectTimeslotOptionTag);
            await optionLabel.ClickAsync();
        }

        /// <summary>
        /// The method is used to accept the confirm terms
        /// </summary>
        /// <returns></returns>
        public async Task AcceptConfirmTerm()
        {
            ILocator confirmCheckbox = await LocateElement(confirmCheckBoxID);
            ILocator confirmButtonElement;            
            bool isVisible = await confirmCheckbox.IsVisibleAsync();
            if (isVisible) 
            {
                ILocator checkBoxElement = await LocateElement(confirmInputCheckBox, SearchType.XPath, false, false);
                bool isChecked = await confirmCheckbox.IsCheckedAsync();
                if (!isChecked)
                {
                    await confirmCheckbox.ClickAsync();
                }
                Thread.Sleep(1000);
                confirmButtonElement = await LocateElement(confirmButton, SearchType.Text);
                await confirmButtonElement.ClickAsync();
            }
            
        }
    }
}
