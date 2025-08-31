# Noundry UI

![Noundry UI](https://img.shields.io/badge/Noundry_UI-1.0.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Alpine.js](https://img.shields.io/badge/Alpine.js-3.x-green.svg)
![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-3.x-blue.svg)
![License](https://img.shields.io/badge/license-MIT-green.svg)

A modern C# ASP.NET TagHelper library that provides server-side components with Alpine.js and Tailwind CSS integration for building beautiful, interactive web applications.

## ✨ Features

- **🎯 62 UI Components** - Complete set of interactive components
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

```csharp
// Program.cs
builder.Services.AddNoundryUI(options =>
{
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
});
```

```csharp
// _ViewImports.cshtml
@addTagHelper *, Noundry.UI
```

### Usage

```html
<!-- Beautiful components with zero JavaScript -->
<noundry-alert type="success" dismissible="true" title="Welcome!">
    You're now using Noundry UI components!
</noundry-alert>

<noundry-modal title="Hello World" button-text="Open Modal">
    <p>This modal works with Alpine.js integration!</p>
</noundry-modal>

<noundry-text-input asp-for="Name" label="Full Name" icon="user" />
<noundry-switch asp-for="EnableNotifications" label="Enable Notifications" />
```

## 📖 Documentation

- **[Getting Started Guide](GETTING_STARTED.md)** - Complete setup guide with examples
- **[Component Library Documentation](Noundry.UI/README.md)** - Detailed component reference
- **[Usage Guide](Noundry.UI/USAGE.md)** - Comprehensive usage examples
- **[Demo Application](Noundry.UI.Demo/)** - Live examples of all components

## 📦 Available Components

### Layout & Navigation (14 TagHelpers)
- **Accordion** - Collapsible content sections (2 TagHelpers)
- **Tabs** - Tab-based content switching (2 TagHelpers)  
- **Dropdown Menu** - Context menus and user menus (2 TagHelpers)
- **Breadcrumbs** - Navigation path components (2 TagHelpers)
- **Context Menu** - Right-click context menus (2 TagHelpers)
- **Command** - Advanced command palette with search (3 TagHelpers)

### Feedback & Status (8 TagHelpers)
- **Alert** - Notification messages with dismiss functionality
- **Badge** - Status indicators and labels
- **Toast** - Temporary notification messages
- **Banner** - Dismissible notification banners
- **Progress** - Progress bars with animation
- **Rating** - Interactive star rating system

### Form Controls (15 TagHelpers)
- **Button** - Interactive buttons with loading states
- **Text Input** - Text fields with validation support
- **Textarea** - Multi-line text inputs with auto-resize
- **Switch** - Toggle controls for boolean values
- **Checkbox** - Checkbox controls with model binding
- **Radio Group** - Radio button groups with validation (2 TagHelpers)
- **Date Picker** - Calendar-based date selection
- **Date Range Picker** - Advanced date range selection with quick presets
- **Select** - Advanced dropdown with search capability (2 TagHelpers)
- **Combobox** - Searchable combo box selection (2 TagHelpers)
- **Multi-Select** - Advanced multi-selection with API support (2 TagHelpers)

### Overlays & Modals (8 TagHelpers)
- **Modal** - Dialog windows with backdrop
- **Slide Over** - Side panel overlays (2 TagHelpers)
- **Tooltip** - Hover information display
- **Popover** - Click-activated content overlay

### Data Display (23 TagHelpers)
- **Card** - Content containers with header/body/footer (4 TagHelpers)
- **Table** - Data tables with sorting and responsive design (6 TagHelpers)
- **Data Table** - Advanced data tables with API, pagination, sorting (4 TagHelpers)
- **Pagination** - Page navigation with ranges
- **Copy to Clipboard** - Text copy functionality
- **Skeleton** - Loading state placeholders with animations (5 TagHelpers)

## 🎯 Examples

### Simple Alert
```html
<noundry-alert type="info" title="Information">
    This is an informational message.
</noundry-alert>
```

### Interactive Form
```html
<form asp-action="Submit">
    <noundry-text-input asp-for="Name" label="Full Name" />
    <noundry-switch asp-for="Subscribe" label="Subscribe to Newsletter" />
    <noundry-multi-select asp-for="Skills" label="Skills" color="green">
        <noundry-multi-select-option value="csharp">C#</noundry-multi-select-option>
        <noundry-multi-select-option value="javascript">JavaScript</noundry-multi-select-option>
        <noundry-multi-select-option value="python">Python</noundry-multi-select-option>
    </noundry-multi-select>
    <noundry-button type="submit" variant="primary">Submit</noundry-button>
</form>
```

### Complex Modal
```html
<noundry-modal title="User Profile" button-text="Edit Profile" max-width="sm:max-w-2xl">
    <div class="space-y-4">
        <noundry-text-input label="Name" placeholder="Enter your name" />
        <noundry-select label="Country" placeholder="Select country">
            <noundry-option value="us">United States</noundry-option>
            <noundry-option value="uk">United Kingdom</noundry-option>
        </noundry-select>
        <noundry-date-picker label="Birth Date" />
    </div>
</noundry-modal>
```

### Date Range Selection
```html
<!-- Basic date range picker -->
<noundry-date-range-picker label="Select Date Range" 
                           placeholder="Choose dates"
                           show-quick-select="true" />

<!-- Date range with model binding -->
<noundry-date-range-picker label="Project Duration"
                           start-date-property="Model.StartDate"
                           end-date-property="Model.EndDate"
                           show-quick-select="true"
                           show-days-count="true"
                           help-text="Select project start and end dates" />

<!-- Custom format and options -->
<noundry-date-range-picker format="YYYY-MM-DD"
                           quick-select-options="today,last7,last30"
                           width="w-80"
                           calendar-width="w-96" />
```

### Skeleton Loading States
```html
<!-- Basic skeleton -->
<noundry-skeleton variant="text" height="h-6" width="w-3/4" />

<!-- Avatar skeleton -->
<noundry-skeleton variant="avatar" size="lg" />

<!-- Card skeleton with multiple elements -->
<noundry-skeleton-card show-image="true" show-avatar="true" text-lines="3" />

<!-- Multi-line text skeleton -->
<noundry-skeleton-text lines="4" variable-width="true" />

<!-- Custom skeleton container -->
<noundry-skeleton-container container-class="flex items-center space-x-3">
    <noundry-skeleton variant="circle" width="3rem" height="h-12" />
    <noundry-skeleton variant="text" height="h-5" />
</noundry-skeleton-container>
```

### Advanced Data Table
```html
<!-- API-powered data table -->
<noundry-data-table title="Users" 
                   api-url="/api/users"
                   per-page="10"
                   server-pagination="true"
                   show-search="true">
    <noundry-data-table-column key="id" label="ID" sortable="true" />
    <noundry-data-table-column key="name" label="Name" sortable="true" href="/users/{id}" />
    <noundry-data-table-column key="email" label="Email" sortable="true" />
    <noundry-data-table-column key="role" label="Role" sortable="false" />
</noundry-data-table>

<!-- Model-bound data table -->
<noundry-data-table asp-for="Users" 
                   title="Team Members"
                   show-search="true"
                   show-pagination="true"
                   per-page="10">
    <noundry-data-table-column key="Id" label="ID" sortable="true" />
    <noundry-data-table-column key="Name" label="Name" sortable="true" href="/users/{Id}" />
    <noundry-data-table-column key="Email" label="Email" sortable="true" />
    <noundry-data-table-column key="Status" label="Status" sortable="true" />
</noundry-data-table>

<!-- Static data table with client-side features -->
<noundry-data-table title="Local Data" 
                   show-search="true"
                   show-pagination="true"
                   per-page="5">
    <noundry-data-table-column key="name" label="Name" sortable="true" />
    <noundry-data-table-column key="status" label="Status" sortable="true" />
    <noundry-data-table-column key="role" label="Role" sortable="false" />
    
    <noundry-data-table-row>
        <noundry-data-table-cell key="name">Alice Johnson</noundry-data-table-cell>
        <noundry-data-table-cell key="status">Active</noundry-data-table-cell>
        <noundry-data-table-cell key="role">Administrator</noundry-data-table-cell>
    </noundry-data-table-row>
    
    <noundry-data-table-row>
        <noundry-data-table-cell key="name">Bob Smith</noundry-data-table-cell>
        <noundry-data-table-cell key="status">Inactive</noundry-data-table-cell>
        <noundry-data-table-cell key="role">User</noundry-data-table-cell>
    </noundry-data-table-row>
</noundry-data-table>
```

## 🏗️ Project Structure

```
noundry.ui/
├── Noundry.UI/                 # Main TagHelper library
│   ├── Components/             # TagHelper components
│   │   ├── AlertTagHelper.cs
│   │   ├── ButtonTagHelper.cs
│   │   ├── ModalTagHelper.cs
│   │   └── ...
│   ├── Core/                   # Base classes and utilities
│   │   ├── NoundryTagHelperBase.cs
│   │   ├── AlpineDataBuilder.cs
│   │   └── Icons.cs
│   ├── Extensions/             # Service registration
│   └── README.md              # Library documentation
├── Noundry.UI.Demo/           # Demo application
│   ├── Controllers/
│   ├── Views/
│   └── Models/
├── GETTING_STARTED.md         # Quick start guide
└── README.md                  # This file
```

## 🛠️ Development

### Prerequisites
- .NET 8.0 SDK or later
- Alpine.js 3.x (included via CDN)
- Tailwind CSS 3.x (included via CDN)

### Building the Library

```bash
git clone https://github.com/plsft/noundry.ui.git
cd noundry.ui

# Build the library
dotnet build Noundry.UI/ --configuration Release

# Run tests (if any)
dotnet test

# Run the demo application
dotnet run --project Noundry.UI.Demo/
```

### Creating New Components

1. Create a new TagHelper class in `Noundry.UI/Components/`
2. Inherit from `NoundryTagHelperBase` or `NoundryFormTagHelperBase`
3. Implement the `ProcessAsync` method
4. Add examples to the demo application

Example:
```csharp
[HtmlTargetElement("noundry-my-component")]
public class MyComponentTagHelper : NoundryTagHelperBase
{
    public string Title { get; set; } = string.Empty;
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var content = await output.GetChildContentAsync();
        
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "my-component");
        
        var html = $"<h2>{Title}</h2><div>{content}</div>";
        output.Content.SetHtmlContent(html);
    }
}
```

## ⚙️ Configuration

Customize Noundry UI behavior:

```csharp
builder.Services.AddNoundryUI(options =>
{
    // Auto-include dependencies
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
    
    // Component defaults
    options.Defaults.ButtonVariant = "primary";
    options.Defaults.AlertType = "info";
    options.Defaults.ToastPosition = "top-right";
    
    // Custom CSS classes
    options.CustomClasses["button"] = "my-custom-button";
});
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### How to Contribute
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add tests for your changes
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## 🐛 Issues & Support

- **Bug Reports**: [GitHub Issues](https://github.com/plsft/noundry.ui/issues)
- **Feature Requests**: [GitHub Issues](https://github.com/plsft/noundry.ui/issues)
- **Discussions**: [GitHub Discussions](https://github.com/plsft/noundry.ui/discussions)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🙏 Acknowledgments

This project is based on and inspired by the excellent [Pines UI Library](https://devdojo.com/pines) by DevDojo. We thank the original authors for creating such a comprehensive and well-designed component library that served as the foundation for this ASP.NET TagHelper implementation.

**Original Pines UI Library**: https://github.com/thedevdojo/pines  
**Original Authors**: DevDojo Team  
**License**: MIT License

Additional thanks to:
- Powered by [Alpine.js](https://alpinejs.dev/) for lightweight interactivity
- Styled with [Tailwind CSS](https://tailwindcss.com/) for beautiful design
- Built for [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) developers

## 📈 Roadmap

- [ ] **Additional Components**: Data tables, file uploads, rich text editors
- [ ] **Themes**: Built-in theme support beyond Tailwind CSS
- [ ] **Accessibility**: Enhanced ARIA support and screen reader testing
- [ ] **Performance**: Server-side rendering optimizations
- [ ] **Documentation**: Interactive component playground

---

**Ready to build beautiful web applications? [Get started now!](GETTING_STARTED.md)**

[![Made with ❤️ by PLSFT](https://img.shields.io/badge/Made%20with%20%E2%9D%A4%EF%B8%8F%20by-PLSFT-red.svg)](https://github.com/plsft)
