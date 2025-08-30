# Noundry UI

A modern C# ASP.NET TagHelper library that provides server-side components with Alpine.js and Tailwind CSS integration for building beautiful, interactive web applications.

## ✨ Features

- **🎯 54 UI Components** - Complete set of interactive components
- **🔗 Full Model Binding** - Seamless ASP.NET model binding support  
- **♿ Accessibility Ready** - ARIA attributes and keyboard navigation
- **🎨 Tailwind CSS Integration** - Beautiful, consistent styling
- **⚡ Alpine.js Powered** - Lightweight client-side interactivity
- **📱 Responsive Design** - Mobile-first approach
- **🛡️ Type Safe** - Strong typing throughout
- **🔧 Highly Configurable** - Extensive customization options

## 🚀 Quick Start

### Installation

```bash
dotnet add package Noundry.UI
```

### Setup

1. Add the TagHelper to your `_ViewImports.cshtml`:
```csharp
@addTagHelper *, Noundry.UI
```

2. Register services in your `Program.cs`:
```csharp
builder.Services.AddNoundryUI(options =>
{
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
});
```

3. Include required scripts in your layout:
```html
<!DOCTYPE html>
<html>
<head>
    <!-- Tailwind CSS -->
    <script src="https://cdn.tailwindcss.com"></script>
    
    <!-- Alpine.js Plugins -->
    <script defer src="https://unpkg.com/@alpinejs/collapse@3.x.x/dist/cdn.min.js"></script>
    <script defer src="https://unpkg.com/@alpinejs/focus@3.x.x/dist/cdn.min.js"></script>
    
    <!-- Alpine.js -->
    <script defer src="https://unpkg.com/alpinejs@3.x.x/dist/cdn.min.js"></script>
    
    <style>
        [x-cloak] { display: none !important; }
    </style>
</head>
<body>
    <!-- Your content -->
</body>
</html>
```

## 📖 Basic Usage

### Alerts & Notifications

```html
<!-- Success alert -->
<noundry-alert type="success" dismissible="true" title="Success!">
    Your action was completed successfully.
</noundry-alert>

<!-- Toast notifications -->
<noundry-toast type="info" title="Information" trigger-text="Show Info">
    This is an informational message.
</noundry-toast>
```

### Interactive Components

```html
<!-- Modal dialog -->
<noundry-modal title="Confirmation" button-text="Open Modal">
    <p>Are you sure you want to continue?</p>
    <div class="flex gap-2 mt-4">
        <noundry-button type="submit">Confirm</noundry-button>
        <noundry-button variant="secondary">Cancel</noundry-button>
    </div>
</noundry-modal>

<!-- Collapsible accordion -->
<noundry-accordion>
    <noundry-accordion-item title="What is Noundry UI?">
        Noundry UI is a modern C# ASP.NET TagHelper library designed for building beautiful, interactive web applications.
    </noundry-accordion-item>
    <noundry-accordion-item title="How do I use it?">
        Simply add the NuGet package, include Alpine.js and Tailwind CSS, then use the noundry-* tags in your Razor views.
    </noundry-accordion-item>
</noundry-accordion>
```

### Form Controls with Model Binding

```html
<!-- Text inputs with validation -->
<noundry-text-input asp-for="Name" label="Full Name" placeholder="Enter your name" />
<noundry-text-input asp-for="Email" type="email" placeholder="user@example.com" />

<!-- Toggle switch -->
<noundry-switch asp-for="EnableNotifications" label="Enable Notifications" />

<!-- Date picker -->
<noundry-date-picker asp-for="BirthDate" placeholder="Select date" />

<!-- Advanced select with search -->
<noundry-select asp-for="Country" placeholder="Select country" searchable="true">
    <noundry-option value="us">United States</noundry-option>
    <noundry-option value="uk">United Kingdom</noundry-option>
    <noundry-option value="ca">Canada</noundry-option>
</noundry-select>
```

## 📦 Available Components

### Layout & Navigation
- **Accordion** - Collapsible content sections
- **Tabs** - Tab-based content switching  
- **Dropdown Menu** - Context menus and user menus
- **Breadcrumbs** - Navigation paths

### Feedback & Status
- **Alert** - Notification messages with dismiss functionality
- **Badge** - Status indicators and labels
- **Toast** - Temporary notification messages
- **Progress** - Progress bars and loading indicators

### Form Controls  
- **Button** - Interactive buttons with loading states
- **Text Input** - Text fields with validation support
- **Switch** - Toggle controls for boolean values
- **Date Picker** - Calendar-based date selection
- **Select** - Dropdown selection with search capability
- **Textarea** - Multi-line text input
- **Checkbox** - Checkbox inputs
- **Radio Group** - Radio button selections

### Overlays & Modals
- **Modal** - Dialog windows with backdrop
- **Slide Over** - Side panel overlays
- **Tooltip** - Hover information display
- **Popover** - Click-activated content overlay

### Data Display
- **Table** - Data tables with sorting
- **Card** - Content containers
- **Image Gallery** - Image viewer with navigation
- **Rating** - Star rating system

### Advanced Components
- **Command** - Command palette interface
- **Copy to Clipboard** - Text copy functionality
- **Text Animation** - Animated text effects
- **Navigation Menu** - Advanced navigation with dropdowns

## 🎨 Styling & Theming

Noundry UI uses Tailwind CSS for styling. You can customize components by:

1. **Using CSS Classes**: Add custom classes via the `css-class` attribute
```html
<noundry-button css-class="shadow-lg border-2">Custom Button</noundry-button>
```

2. **Theme Configuration**: Configure default styles in your service registration
```csharp
builder.Services.AddNoundryUI(options =>
{
    options.Defaults.ButtonVariant = "primary";
    options.Defaults.AlertType = "info";
    options.CustomClasses.Add("button", "custom-button-class");
});
```

## 🔧 Advanced Features

### Form Integration

```html
<form asp-action="Create" method="post">
    <div class="space-y-4">
        <noundry-text-input asp-for="Name" />
        <noundry-switch asp-for="IsActive" />
        
        <div class="flex gap-2">
            <noundry-button type="submit" loading="true" loading-text="Saving...">
                Save
            </noundry-button>
            <noundry-button type="button" variant="secondary">
                Cancel
            </noundry-button>
        </div>
    </div>
</form>
```

### Alpine.js Integration

Components automatically integrate with Alpine.js. You can extend functionality:

```html
<div x-data="{ customData: 'Hello World!' }">
    <noundry-button x-on:click="alert(customData)">Show Alert</noundry-button>
</div>
```

### Configuration Options

```csharp
builder.Services.AddNoundryUI(options =>
{
    // Auto-include dependencies
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
    
    // Default component settings
    options.Defaults.ButtonSize = "md";
    options.Defaults.ToastPosition = "top-right";
    options.Defaults.ModalMaxWidth = "sm:max-w-lg";
    
    // Custom CSS classes
    options.CustomClasses["alert"] = "my-alert-styles";
});
```

## 🌟 Examples

Check out our [comprehensive demo application](https://github.com/noundry/noundry-ui/tree/main/Noundry.UI.Demo) showcasing all components with working examples.

## 📄 Documentation

- [Usage Guide](USAGE.md) - Detailed component documentation
- [API Reference](https://noundry.github.io/noundry-ui/) - Complete API documentation
- [Migration Guide](MIGRATION.md) - Upgrading from other libraries

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

## 📝 License

This project is licensed under the [MIT License](LICENSE.md).

## 🙏 Acknowledgments

Built with inspiration from the excellent PinesUI component library, adapted for the ASP.NET ecosystem with modern tooling and best practices.

---

**Made with ❤️ by the Noundry UI team**