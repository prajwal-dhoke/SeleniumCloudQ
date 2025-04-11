# Shadow DOM Form Automation with Selenium C#

## Project Overview

This project demonstrates how to automate form filling for web applications that utilize Shadow DOM encapsulation. Shadow DOM presents unique challenges for traditional Selenium automation, as elements inside the shadow tree are not directly accessible using standard Selenium selectors.

The implementation specifically targets the [CloudQA Automation Practice Form](https://app.cloudqa.io/home/AutomationPracticeForm) which contains form elements within Shadow DOM boundaries.

## Features

- Shadow DOM element identification and interaction
- Multiple fallback strategies for form filling
- JavaScript execution for direct DOM manipulation
- Comprehensive error handling and logging
- Screenshot capture for verification

## Prerequisites

- .NET 5.0 or higher
- Selenium WebDriver
- Selenium.Support
- ChromeDriver compatible with your installed Chrome version

## Installation

1. Clone this repository:
   ```
   git clone https://github.com/yourusername/SeleniumCloudQA.git
   cd SeleniumCloudQA
   ```

2. Install required NuGet packages:
   ```
   dotnet add package Selenium.WebDriver
   dotnet add package Selenium.Support
   ```

3. Download ChromeDriver that matches your Chrome browser version from [ChromeDriver Downloads](https://chromedriver.chromium.org/downloads) and place it in your project directory or ensure it's in your system PATH.

## Usage

Run the application:

```
dotnet run
```

The program will:
1. Launch Chrome browser
2. Navigate to the CloudQA practice form
3. Locate the shadow DOM elements
4. Fill in the form fields (First Name, Last Name, and select gender)
5. Keep the browser open for a short period for verification
6. Close the browser

## Technical Implementation

The application addresses several technical challenges:

### Shadow DOM Access

Shadow DOM creates a separate DOM tree that is isolated from the main document. To access elements within Shadow DOM:

```csharp
// Get the shadow root using JavaScript
object shadowRoot = ((IJavaScriptExecutor)driver).ExecuteScript(
    "return arguments[0].shadowRoot", shadowHost);
```

### Form Filling Strategies

The code implements three distinct strategies for filling forms:

1. **Standard Selenium approach with Shadow DOM**: Uses JavaScript to get the shadow root, then finds elements within it
2. **Direct JavaScript execution**: Interacts with form elements directly using JavaScript
3. **Fallback selectors**: Tries multiple possible selectors to find the form elements

### Event Dispatching

Merely setting input values sometimes isn't sufficient as forms often listen for input events:

```javascript
// Trigger input and change events to ensure the form recognizes the changes
element.dispatchEvent(new Event('input', { bubbles: true }));
element.dispatchEvent(new Event('change', { bubbles: true }));
```

## Troubleshooting

If the automation fails to fill the form:

1. **Check the console output** for error messages and debugging information
2. **View the screenshot** saved in your project directory
3. **Verify the form structure** by manually inspecting the page using Chrome DevTools
4. **Adjust selectors** based on the actual DOM structure
5. **Increase wait times** if the page or elements load slowly

## Common Issues and Solutions

| Issue | Solution |
|-------|----------|
| Shadow DOM not found | Verify the shadow host selector is correct |
| Form not filled | Check if the form requires authentication first |
| Timeouts | Increase wait times in the WebDriverWait constructor |
| Element selectors not matching | Inspect the page manually to find correct selectors |
| Chrome version mismatch | Update ChromeDriver to match your Chrome version |

## License

[MIT License](LICENSE)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
