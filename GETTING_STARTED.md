# Getting Started with Noundry UI

Noundry UI is a modern C# ASP.NET TagHelper library that provides beautiful, interactive components with Alpine.js and Tailwind CSS integration.

## 🚀 Quick Start (5 minutes)

### 1. Install the Package

Add Noundry UI to your ASP.NET Core project:

```bash
dotnet add package Noundry.UI
```

### 2. Register the Services

In your `Program.cs`, add Noundry UI services:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddNoundryUI(options =>
{
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
});

var app = builder.Build();
// ... rest of your configuration
```

### 3. Add TagHelpers

In your `Views/_ViewImports.cshtml`, add:

```csharp
@addTagHelper *, Noundry.UI
```

### 4. Include Required Scripts

In your `Views/Shared/_Layout.cshtml`, add the required scripts:

```html
<!DOCTYPE html>
<html>
<head>
    <!-- Your existing head content -->
    
    <!-- Tailwind CSS -->
    <script src="https://cdn.tailwindcss.com"></script>
    
    <!-- Alpine.js Plugins -->
    <script defer src="https://unpkg.com/@@alpinejs/collapse@@3.x.x/dist/cdn.min.js"></script>
    <script defer src="https://unpkg.com/@@alpinejs/focus@@3.x.x/dist/cdn.min.js"></script>
    
    <!-- Alpine.js -->
    <script defer src="https://unpkg.com/alpinejs@@3.x.x/dist/cdn.min.js"></script>
    
    <style>
        [x-cloak] { display: none !important; }
    </style>
</head>
<body>
    <!-- Your content -->
</body>
</html>
```

### 5. Start Using Components

That's it! You can now use Noundry UI components in your Razor views:

```html
<!-- Beautiful alert -->
<noundry-alert type="success" dismissible="true" title="Welcome!">
    You're now using Noundry UI components!
</noundry-alert>

<!-- Interactive button -->
<noundry-button variant="primary">Click Me</noundry-button>

<!-- Modal dialog -->
<noundry-modal title="Hello World" button-text="Open Modal">
    <p>This is your first Noundry UI modal!</p>
</noundry-modal>
```

## 📖 Your First Form

Let's create a complete form using Noundry UI components:

### 1. Create a Model

```csharp
public class ContactFormViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Subscribe to Newsletter")]
    public bool Subscribe { get; set; }

    [Display(Name = "Preferred Contact Date")]
    public DateTime? ContactDate { get; set; }
}
```

### 2. Create the Form View

```html
@model ContactFormViewModel

<div class="max-w-2xl mx-auto p-6">
    <h1 class="text-2xl font-bold mb-6">Contact Us</h1>
    
    <form asp-action="Contact" method="post" class="space-y-6">
        <!-- Name Input -->
        <noundry-text-input asp-for="Name" 
                            icon="user" 
                            placeholder="Enter your full name" />
        
        <!-- Email Input -->
        <noundry-text-input asp-for="Email" 
                            type="email" 
                            icon="email"
                            placeholder="your.email@example.com" />
        
        <!-- Newsletter Switch -->
        <noundry-switch asp-for="Subscribe" />
        
        <!-- Date Picker -->
        <noundry-date-picker asp-for="ContactDate" 
                             placeholder="Select preferred date" />
        
        <!-- Submit Button -->
        <div class="flex gap-4">
            <noundry-button type="submit" variant="primary">
                Send Message
            </noundry-button>
            <noundry-button type="button" variant="secondary">
                Cancel
            </noundry-button>
        </div>
    </form>
</div>
```

### 3. Handle the Form Submission

```csharp
[HttpPost]
public async Task<IActionResult> Contact(ContactFormViewModel model)
{
    if (ModelState.IsValid)
    {
        // Process the form
        TempData["Success"] = "Thank you! We'll be in touch soon.";
        return RedirectToAction("Contact");
    }
    
    return View(model);
}
```

## 🎨 Available Components

### Layout & Navigation
- **`<noundry-accordion>`** - Collapsible content sections
- **`<noundry-tabs>`** - Tab-based content switching
- **`<noundry-dropdown-menu>`** - Context menus and user menus

### Feedback & Status
- **`<noundry-alert>`** - Notification messages
- **`<noundry-badge>`** - Status indicators
- **`<noundry-toast>`** - Temporary notifications

### Form Controls
- **`<noundry-button>`** - Interactive buttons
- **`<noundry-text-input>`** - Text input fields
- **`<noundry-switch>`** - Toggle controls
- **`<noundry-date-picker>`** - Calendar date selection
- **`<noundry-select>`** - Advanced dropdown selection

### Overlays
- **`<noundry-modal>`** - Dialog windows
- **`<noundry-tooltip>`** - Hover information display

## ⚙️ Configuration Options

You can customize Noundry UI behavior during service registration:

```csharp
builder.Services.AddNoundryUI(options =>
{
    // Auto-include dependencies
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
    
    // Default component settings
    options.Defaults.ButtonSize = "md";
    options.Defaults.ButtonVariant = "primary";
    options.Defaults.ToastPosition = "top-right";
    options.Defaults.ModalMaxWidth = "sm:max-w-lg";
    
    // Custom CSS classes
    options.CustomClasses["button"] = "my-custom-button-class";
    options.CustomClasses["alert"] = "my-custom-alert-class";
});
```

## 🎯 Next Steps

### Learn More
1. **[Complete Documentation](README.md)** - Detailed overview and features
2. **[Usage Guide](Noundry.UI/USAGE.md)** - Comprehensive component reference
3. **[Demo Application](Noundry.UI.Demo/)** - Live examples of all components

### Explore Components
- Check out the **[Demo Application](Noundry.UI.Demo/)** for working examples
- Browse the **[Component Gallery](Noundry.UI/USAGE.md)** for detailed usage
- Review the **[API Reference](Noundry.UI/USAGE.md#component-properties-reference)** for all properties

### Common Patterns

#### Dynamic Forms
```html
<noundry-select asp-for="Category" placeholder="Choose category">
    <noundry-option value="general">General Inquiry</noundry-option>
    <noundry-option value="support">Technical Support</noundry-option>
    <noundry-option value="sales">Sales Question</noundry-option>
</noundry-select>
```

#### Interactive Dashboards
```html
<noundry-accordion>
    <noundry-accordion-item title="User Statistics">
        <!-- Dashboard content -->
    </noundry-accordion-item>
    <noundry-accordion-item title="System Health">
        <!-- Health metrics -->
    </noundry-accordion-item>
</noundry-accordion>
```

#### User Notifications
```html
<noundry-toast type="success" 
               title="Success!" 
               position="top-right"
               auto-dismiss="true"
               delay="3000">
    Your changes have been saved successfully.
</noundry-toast>
```

## 🛠️ Development Setup

If you want to contribute or run the demo locally:

### Prerequisites
- .NET 8.0 SDK
- Node.js (for Tailwind CSS, optional)

### Clone and Run
```bash
git clone https://github.com/plsft/noundry.ui.git
cd noundry.ui

# Build the library
dotnet build Noundry.UI/

# Run the demo application
dotnet run --project Noundry.UI.Demo/
```

### Project Structure
```
noundry.ui/
├── Noundry.UI/              # Main library
│   ├── Components/          # TagHelper components
│   ├── Core/               # Base classes and utilities
│   └── Extensions/         # Service registration
├── Noundry.UI.Demo/        # Demo application
└── Documentation/          # Guides and examples
```

## 🤝 Support & Community

- **Issues**: [GitHub Issues](https://github.com/plsft/noundry.ui/issues)
- **Discussions**: [GitHub Discussions](https://github.com/plsft/noundry.ui/discussions)
- **Documentation**: [Full Documentation](README.md)

## 📄 License

Noundry UI is open source and available under the [MIT License](LICENSE.md).

---

**Ready to build beautiful web applications with Noundry UI? [Check out the full documentation](README.md) for more advanced features and examples!**