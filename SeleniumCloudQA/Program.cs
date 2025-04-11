using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

class ShadowDOMFormAutomation
{
    static void Main()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        // Uncomment the line below if you want to see what's happening
        // options.AddArgument("--auto-open-devtools-for-tabs");
        
        using IWebDriver driver = new ChromeDriver(options);
        try
        {
            driver.Navigate().GoToUrl("https://app.cloudqa.io/home/AutomationPracticeForm");
            
            // Use explicit wait
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            
            // Debug - print page title to verify we're on the right page
            Console.WriteLine($"Page title: {driver.Title}");
            
            // Wait for the shadow host to be present
            IWebElement shadowHost = wait.Until(drv => 
            {
                try {
                    var element = drv.FindElement(By.CssSelector("div#shadowDom"));
                    return element.Displayed ? element : null;
                }
                catch {
                    return null;
                }
            });
            
            if (shadowHost == null)
            {
                Console.WriteLine("Shadow host element not found. Checking for alternative selectors...");
                try {
                    // Try alternative selector
                    shadowHost = driver.FindElement(By.Id("shadowDom"));
                    Console.WriteLine("Found shadow host using ID selector");
                }
                catch (Exception ex) {
                    Console.WriteLine($"Alternative selector failed: {ex.Message}");
                    throw;
                }
            }
            
            // Get the shadow root using JavaScript
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            
            // Debug - check if element has shadowRoot
            bool hasShadowRoot = (bool)js.ExecuteScript("return arguments[0].shadowRoot != null", shadowHost);
            Console.WriteLine($"Element has shadowRoot: {hasShadowRoot}");
            
            object shadowRootObj = js.ExecuteScript("return arguments[0].shadowRoot", shadowHost);
            
            if (shadowRootObj == null)
            {
                Console.WriteLine("Shadow root is null. Trying alternative approach...");
                // Alternative approach using executeScript to interact directly
                js.ExecuteScript(@"
                    const host = document.querySelector('#shadowDom');
                    if (host && host.shadowRoot) {
                        const firstName = host.shadowRoot.querySelector('input[placeholder=""First Name""]');
                        const lastName = host.shadowRoot.querySelector('input[placeholder=""Last Name""]');
                        const genderRadio = host.shadowRoot.querySelector('label[for=""gender-radio-1""]');
                        
                        if (firstName) firstName.value = 'Amit';
                        if (lastName) lastName.value = 'Patil';
                        if (genderRadio) genderRadio.click();
                        
                        // Trigger events to ensure form recognizes changes
                        if (firstName) {
                            firstName.dispatchEvent(new Event('input', { bubbles: true }));
                            firstName.dispatchEvent(new Event('change', { bubbles: true }));
                        }
                        if (lastName) {
                            lastName.dispatchEvent(new Event('input', { bubbles: true }));
                            lastName.dispatchEvent(new Event('change', { bubbles: true }));
                        }
                    }
                ");
                Console.WriteLine("Attempted form fill using JavaScript directly");
            }
            else
            {
                Console.WriteLine("Shadow root found, attempting to interact with elements...");
                
                // Try a different approach to get elements in the shadow DOM
                IDictionary<string, object> params1 = new Dictionary<string, object>
                {
                    ["querySelector"] = "input[placeholder='First Name']",
                    ["root"] = shadowRootObj
                };
                
                IWebElement firstNameInput = (IWebElement)js.ExecuteScript(
                    "return arguments[0].querySelector(arguments[1])", 
                    shadowHost, "input[placeholder='First Name']");
                
                if (firstNameInput != null)
                {
                    Console.WriteLine("Found firstName input, sending keys...");
                    js.ExecuteScript("arguments[0].value = 'Amit';", firstNameInput);
                    js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", firstNameInput);
                }
                else
                {
                    Console.WriteLine("firstName input not found");
                }
                
                IWebElement lastNameInput = (IWebElement)js.ExecuteScript(
                    "return arguments[0].shadowRoot.querySelector(arguments[1])", 
                    shadowHost, "input[placeholder='Last Name']");
                
                if (lastNameInput != null)
                {
                    Console.WriteLine("Found lastName input, sending keys...");
                    js.ExecuteScript("arguments[0].value = 'Patil';", lastNameInput);
                    js.ExecuteScript("arguments[0].dispatchEvent(new Event('input', { bubbles: true }));", lastNameInput);
                }
                else
                {
                    Console.WriteLine("lastName input not found");
                }
                
                IWebElement genderRadioLabel = (IWebElement)js.ExecuteScript(
                    "return arguments[0].shadowRoot.querySelector(arguments[1])", 
                    shadowHost, "label[for='gender-radio-1']");
                
                if (genderRadioLabel != null)
                {
                    Console.WriteLine("Found gender radio label, clicking...");
                    js.ExecuteScript("arguments[0].click();", genderRadioLabel);
                }
                else
                {
                    Console.WriteLine("Gender radio label not found");
                }
            }
            
            // Print structure of the shadow DOM for debugging
            js.ExecuteScript(@"
                const host = document.querySelector('#shadowDom');
                if (host && host.shadowRoot) {
                    console.log('Shadow DOM structure:');
                    console.log(host.shadowRoot.innerHTML);
                }
            ");
            
            Console.WriteLine("Form fill attempt completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
        finally
        {
            // Keep the browser open for visibility
            Console.WriteLine("Keeping browser open for 20 seconds...");
            System.Threading.Thread.Sleep(20000);
            driver.Quit();
        }
    }
}