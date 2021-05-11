using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using System;

namespace AndroidAppTest_AppiumNUnit
{
    public class AppiumTests_AndroidVivinoApp
    {
        private AndroidDriver<AndroidElement> driver;

        private const string AppiumServerUrl = "http://[::1]:4723/wd/hub";
        private const string PlatformName = "Android";
        private const string VivinoAppPath =
            @"D:\ComputerScience\Java_Studies\QA\QA_Automation\04. AppiumAndMobileTesting\Exercise\vivino_8.18.11-8181203.apk";
        private const string VivinoTestEmail = "testera@abv.bg";
        private const string VivinoTestPass = "pass1234";
        private const string VivinoAppPackage = "vivino.web.app";
        private const string VivinoAppActivity = "com.sphinx_solution.activities.SplashActivity";

        [OneTimeSetUp]
        public void Setup()
        {
            var appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("platformName", PlatformName);
            appiumOptions.AddAdditionalCapability("app", VivinoAppPath);
            appiumOptions.AddAdditionalCapability("appPackage", VivinoAppPackage);
            appiumOptions.AddAdditionalCapability("appActivity", VivinoAppActivity);

            driver = new AndroidDriver<AndroidElement>(new Uri(AppiumServerUrl), appiumOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(25);

        }

        [Test]
        public void TestVivinoAndroidApp()
        {
            // Login in VivinoApp using existing user
            var getStartedButton = driver.FindElementById("vivino.web.app:id/getstarted_layout");
            getStartedButton.Click();
            var emailTextBox = driver.FindElementById("vivino.web.app:id/edtEmail");
            emailTextBox.SendKeys(VivinoTestEmail);
            var passwordTextBox = driver.FindElementById("vivino.web.app:id/edtPassword");
            passwordTextBox.SendKeys(VivinoTestPass);
            var signInButton = driver.FindElementById("vivino.web.app:id/action_next");
            signInButton.Click();

            // Search "Katarzyna Reserve Red 2006" and open the top result
            var searchTab = driver.FindElementById("vivino.web.app:id/wine_explorer_tab");
            searchTab.Click();
            var findWineTextBox = driver.FindElementById("vivino.web.app:id/search_vivino");
            findWineTextBox.Click();
            var searchForWineTextBox = driver.FindElementById("vivino.web.app:id/editText_input");
            searchForWineTextBox.SendKeys("Katarzyna Reserve Red 2006");
            var firstSearchResult = driver.FindElementById("vivino.web.app:id/wineimage");
            firstSearchResult.Click();
            // Assert wine name is "Reserve Red 2006"
            var expectedWineName = driver.FindElementById("vivino.web.app:id/wine_name").Text;
            Assert.AreEqual("Reserve Red 2006", expectedWineName);

            // Assert wine rating is a number in the range [1.00 ... 5.00]
            string ratingText = driver.FindElementById("vivino.web.app:id/rating").Text;
            double ratingNumber = double.Parse(ratingText);
            var rating = (ratingNumber > 1.00 && ratingNumber <= 5.00);
            Assert.IsTrue(rating);

            // Assert wine highlights contain "Among top 1% of all wines in the world"(SCROLLING needed!)
            // we need to use scrolable selector to scroll down!
            var summaryBox = driver.FindElementByXPath("/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout[2]/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.HorizontalScrollView/android.widget.LinearLayout/android.widget.TextView[1]");
            var highlightsTabScrollable = driver.FindElementByAndroidUIAutomator
                ("new UiScrollable(new UiSelector().scrollable(true))" + 
                ".scrollIntoView(new UiSelector().resourceIdMatches(" +
                "\"vivino.web.app:id/highlight_description\"));");

            Assert.AreEqual("Among top 1% of all wines in the world", highlightsTabScrollable.Text);

            // Assert the wine facts hold "Grapes: Cabernet Sauvignon, Merlot"
            var factsTab = driver.FindElementByXPath
                ("/hierarchy/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.FrameLayout/android.view.ViewGroup/androidx.recyclerview.widget.RecyclerView/android.widget.FrameLayout[2]/android.widget.FrameLayout/android.widget.LinearLayout/android.widget.HorizontalScrollView/android.widget.LinearLayout/android.widget.TextView[2]");
            factsTab.Click();
            string factText = driver.FindElementById("vivino.web.app:id/wine_fact_text").Text;

            Assert.AreEqual("Cabernet Sauvignon,Merlot", factText);
        }

        [OneTimeTearDown]
        public void CloseUp()
        {
            driver.Quit();
        }
    }
}