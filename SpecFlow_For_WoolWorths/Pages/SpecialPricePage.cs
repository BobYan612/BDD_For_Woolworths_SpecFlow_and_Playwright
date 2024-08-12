/********************************************************************************
# This is a practice project from Biao Yan (Bob Yan), and it's free to be 
# downloaded for study and test project.
*********************************************************************************/

using Microsoft.Playwright;


namespace PlaywrightTest.pages
{
    /// <summary>
    /// The page object for special & offer, it has same attributes as the Browse page.
    /// </summary>
    public class SpecialPricePage : BrowsePage
    {
        public SpecialPricePage(IPage page) : base(page) { }
    }
}
