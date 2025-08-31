# Noundry UI - Usage Guide

This document provides comprehensive examples of how to use all the Noundry UI TagHelper components.

## Setup

1. Add the package reference to your project:
```xml
<PackageReference Include="Noundry.UI" Version="1.0.0" />
```

2. Register services in `Program.cs`:
```csharp
builder.Services.AddNoundryUI(options =>
{
    options.IncludeAlpineJS = true;
    options.IncludeTailwindCSS = true;
});
```

3. Add TagHelpers to `_ViewImports.cshtml`:
```csharp
@addTagHelper *, Noundry.UI
```

4. Include required scripts in your layout:
```html
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
```

## Components

### Alert
Display notification messages with different types and dismiss functionality.

```html
<!-- Basic alert -->
<noundry-alert type="info">This is an info alert</noundry-alert>

<!-- Alert with title and dismiss -->
<noundry-alert type="success" title="Success!" dismissible="true">
    Your action was completed successfully.
</noundry-alert>

<!-- All alert types -->
<noundry-alert type="info">Information</noundry-alert>
<noundry-alert type="success">Success</noundry-alert>
<noundry-alert type="warning">Warning</noundry-alert>
<noundry-alert type="error">Error</noundry-alert>
```

### Badge
Small status indicators and labels.

```html
<!-- Basic badges -->
<noundry-badge variant="default">Default</noundry-badge>
<noundry-badge variant="success">Success</noundry-badge>
<noundry-badge variant="warning">Warning</noundry-badge>
<noundry-badge variant="error">Error</noundry-badge>

<!-- Badge with icon -->
<noundry-badge variant="success" icon="check">Completed</noundry-badge>

<!-- Outlined badge -->
<noundry-badge variant="info" outline="true">Info</noundry-badge>

<!-- Pill style -->
<noundry-badge variant="success" pill="true">New</noundry-badge>

<!-- Different sizes -->
<noundry-badge size="sm">Small</noundry-badge>
<noundry-badge size="md">Medium</noundry-badge>
<noundry-badge size="lg">Large</noundry-badge>
```

### Button
Interactive buttons with various styles and states.

```html
<!-- Basic buttons -->
<noundry-button variant="primary">Primary</noundry-button>
<noundry-button variant="secondary">Secondary</noundry-button>
<noundry-button variant="success">Success</noundry-button>
<noundry-button variant="danger">Danger</noundry-button>

<!-- Button sizes -->
<noundry-button size="sm">Small</noundry-button>
<noundry-button size="md">Medium</noundry-button>
<noundry-button size="lg">Large</noundry-button>

<!-- Button states -->
<noundry-button disabled="true">Disabled</noundry-button>
<noundry-button loading="true" loading-text="Saving...">Save</noundry-button>

<!-- Form buttons -->
<noundry-button type="submit">Submit</noundry-button>
<noundry-button type="button">Cancel</noundry-button>
```

### Modal
Modal dialogs with backdrop and keyboard support.

```html
<!-- Basic modal -->
<noundry-modal title="Confirmation" button-text="Open Modal">
    <p>Are you sure you want to continue?</p>
    <div class="flex gap-2 mt-4">
        <noundry-button variant="primary">Confirm</noundry-button>
        <noundry-button variant="secondary">Cancel</noundry-button>
    </div>
</noundry-modal>

<!-- Modal configuration -->
<noundry-modal title="Settings" 
               button-text="Open Settings" 
               button-variant="secondary"
               max-width="sm:max-w-2xl"
               close-on-backdrop="false">
    <form>
        <noundry-text-input label="Name" placeholder="Enter your name" />
        <noundry-switch label="Enable notifications" />
    </form>
</noundry-modal>
```

### Accordion
Collapsible content sections.

```html
<noundry-accordion>
    <noundry-accordion-item title="Getting Started">
        Learn how to install and configure Noundry UI in your project.
    </noundry-accordion-item>
    <noundry-accordion-item title="Components" open="true">
        Explore all available components and their usage examples.
    </noundry-accordion-item>
    <noundry-accordion-item title="Advanced Features">
        Discover advanced features and customization options.
    </noundry-accordion-item>
</noundry-accordion>

<!-- Multiple items can be open -->
<noundry-accordion multiple="true">
    <noundry-accordion-item title="Section 1">Content 1</noundry-accordion-item>
    <noundry-accordion-item title="Section 2">Content 2</noundry-accordion-item>
</noundry-accordion>
```

### Tabs
Tab-based content switching.

```html
<noundry-tabs>
    <noundry-tab-item title="Profile" active="true">
        <div class="p-4">
            <h3 class="font-semibold">Profile Information</h3>
            <p>Update your profile details here.</p>
        </div>
    </noundry-tab-item>
    <noundry-tab-item title="Settings">
        <div class="p-4">
            <h3 class="font-semibold">Settings</h3>
            <p>Configure your preferences.</p>
        </div>
    </noundry-tab-item>
    <noundry-tab-item title="Security">
        <div class="p-4">
            <h3 class="font-semibold">Security</h3>
            <p>Manage security settings.</p>
        </div>
    </noundry-tab-item>
</noundry-tabs>
```

### Switch
Toggle switch for boolean values.

```html
<!-- Model binding -->
<noundry-switch asp-for="EnableNotifications" label="Enable Notifications" />

<!-- Manual configuration -->
<noundry-switch label="Dark Mode" checked="true" color="blue" />

<!-- Different sizes and colors -->
<noundry-switch label="Small Switch" size="sm" color="green" />
<noundry-switch label="Medium Switch" size="md" color="red" />
<noundry-switch label="Large Switch" size="lg" color="gray" />
```

### Text Input
Text input fields with validation support.

```html
<!-- Model binding with validation -->
<noundry-text-input asp-for="Name" label="Full Name" placeholder="Enter your name" />

<!-- Different input types -->
<noundry-text-input type="email" label="Email" placeholder="user@example.com" />
<noundry-text-input type="password" label="Password" />
<noundry-text-input type="number" label="Age" />

<!-- With icons and help text -->
<noundry-text-input label="Search" 
                    icon="search" 
                    placeholder="Search..." 
                    help-text="Enter keywords to search" />

<!-- Error state -->
<noundry-text-input label="Username" 
                    error-message="Username is required" 
                    required="true" />

<!-- Sizes -->
<noundry-text-input size="sm" placeholder="Small input" />
<noundry-text-input size="md" placeholder="Medium input" />
<noundry-text-input size="lg" placeholder="Large input" />
```

### Date Picker
Calendar-based date selection.

```html
<!-- Model binding -->
<noundry-date-picker asp-for="BirthDate" label="Birth Date" />

<!-- Custom configuration -->
<noundry-date-picker label="Event Date" 
                     format="dd/MM/yyyy"
                     min-date="DateTime.Now"
                     max-date="DateTime.Now.AddYears(1)"
                     culture="en-GB" />
```

### Date Range Picker
Advanced date range selection with quick presets and range validation.

```html
<!-- Basic date range picker -->
<noundry-date-range-picker label="Select Date Range" 
                           placeholder="Choose dates"
                           show-quick-select="true" />

<!-- Date range with model binding -->
<noundry-date-range-picker label="Project Duration"
                           start-date-property="ProjectStartDate"
                           end-date-property="ProjectEndDate"
                           show-quick-select="true"
                           show-days-count="true"
                           help-text="Select project start and end dates" />

<!-- Custom format and quick select options -->
<noundry-date-range-picker label="Reporting Period"
                           format="MM-DD-YYYY"
                           quick-select-options="today,yesterday,last7,last30,last90"
                           show-clear="true"
                           width="w-80"
                           calendar-width="w-96" />

<!-- Date range with validation -->
<noundry-date-range-picker label="Available Dates"
                           min-date="DateTime.Now"
                           max-date="DateTime.Now.AddMonths(6)"
                           required="true"
                           error-message="Please select a valid date range" />

<!-- Different date formats -->
<noundry-date-range-picker format="D d M, Y" />  <!-- Mon 15 Jan, 2024 -->
<noundry-date-range-picker format="YYYY-MM-DD" /> <!-- 2024-01-15 -->
<noundry-date-range-picker format="DD-MM-YYYY" /> <!-- 15-01-2024 -->

<!-- Localized date range picker -->
<noundry-date-range-picker culture="de-DE" 
                           format="DD-MM-YYYY"
                           label="Datumsbereich auswählen" />
```

### Select
Dropdown selection with search capability.

```html
<!-- Model binding -->
<noundry-select asp-for="Country" placeholder="Select country">
    <noundry-option value="">-- Select Country --</noundry-option>
    <noundry-option value="us">United States</noundry-option>
    <noundry-option value="uk">United Kingdom</noundry-option>
    <noundry-option value="ca">Canada</noundry-option>
</noundry-select>

<!-- Multiple selection -->
<noundry-select asp-for="Skills" multiple="true" placeholder="Select skills">
    <noundry-option value="csharp">C#</noundry-option>
    <noundry-option value="javascript">JavaScript</noundry-option>
    <noundry-option value="python">Python</noundry-option>
    <noundry-option value="sql">SQL</noundry-option>
</noundry-select>

<!-- Searchable select -->
<noundry-select searchable="true" 
                search-placeholder="Search options..." 
                no-results-text="No matches found">
    <noundry-option value="option1">Option 1</noundry-option>
    <noundry-option value="option2">Option 2</noundry-option>
</noundry-select>
```

### Data Table
Advanced data table with API support, pagination, sorting, and search.

```html
<!-- Basic API data table -->
<noundry-data-table title="Users" 
                   api-url="/api/users"
                   per-page="10"
                   show-search="true"
                   show-pagination="true">
    <noundry-data-table-column key="id" label="ID" sortable="true" />
    <noundry-data-table-column key="name" label="Name" sortable="true" />
    <noundry-data-table-column key="email" label="Email" sortable="true" />
</noundry-data-table>

<!-- Data table with links -->
<noundry-data-table title="Posts" 
                   api-url="https://jsonplaceholder.typicode.com/posts"
                   per-page="15"
                   server-pagination="true">
    <noundry-data-table-column key="id" label="ID" sortable="true" />
    <noundry-data-table-column key="title" label="Title" sortable="true" 
                              href="/posts/{id}" href-text="{title}" />
    <noundry-data-table-column key="body" label="Content" sortable="false" />
</noundry-data-table>

<!-- Custom styling and options -->
<noundry-data-table title="Data"
                   api-url="/api/data"
                   per-page="25"
                   per-page-options="10,25,50,100"
                   search-placeholder="Search data..."
                   no-results-message="No data found"
                   loading-message="Loading data..."
                   hoverable="true"
                   striped="false">
    <noundry-data-table-column key="name" label="Name" sortable="true" align="left" />
    <noundry-data-table-column key="status" label="Status" sortable="true" align="center" />
    <noundry-data-table-column key="created" label="Created" sortable="true" align="right" />
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
    <noundry-data-table-column key="Role" label="Role" sortable="false" />
</noundry-data-table>

<!-- Static inline data table -->
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

<!-- Minimal API configuration -->
<noundry-data-table api-url="/api/simple-data" show-search="false" show-pagination="false">
    <noundry-data-table-column key="name" label="Name" />
    <noundry-data-table-column key="value" label="Value" />
</noundry-data-table>
```

### Multi-Select
Advanced multi-selection component with API support and tag display.

```html
<!-- Basic multi-select -->
<noundry-multi-select label="Skills" 
                     placeholder="Select your skills"
                     color="blue">
    <noundry-multi-select-option value="csharp">C#</noundry-multi-select-option>
    <noundry-multi-select-option value="javascript">JavaScript</noundry-multi-select-option>
    <noundry-multi-select-option value="python">Python</noundry-multi-select-option>
    <noundry-multi-select-option value="sql">SQL</noundry-multi-select-option>
</noundry-multi-select>

<!-- Multi-select with model binding -->
<noundry-multi-select asp-for="SelectedServices" 
                     label="Services"
                     color="green"
                     show-remove-buttons="true">
    <noundry-multi-select-option value="consulting">Consulting</noundry-multi-select-option>
    <noundry-multi-select-option value="development">Development</noundry-multi-select-option>
    <noundry-multi-select-option value="design">Design</noundry-multi-select-option>
</noundry-multi-select>

<!-- Multi-select with API integration -->
<noundry-multi-select label="Team Members"
                     use-api="true"
                     api-url="/api/users"
                     api-id-property="id"
                     api-name-property="fullName"
                     loading-text="Loading team members..."
                     error-text="Failed to load team members"
                     color="blue" />

<!-- Different display styles -->
<noundry-multi-select display-style="count" 
                     max-display-items="3"
                     label="Technologies">
    <!-- Options... -->
</noundry-multi-select>

<!-- Custom styling -->
<noundry-multi-select color="red"
                     max-height="max-h-40"
                     show-remove-buttons="false"
                     help-text="Select multiple options from the list">
    <!-- Options... -->
</noundry-multi-select>
```

### Toast
Temporary notification messages.

```html
<!-- Different types -->
<noundry-toast type="info" title="Information" trigger-text="Show Info">
    This is an informational message.
</noundry-toast>

<noundry-toast type="success" title="Success" trigger-text="Show Success">
    Action completed successfully!
</noundry-toast>

<!-- Custom positioning and timing -->
<noundry-toast type="warning" 
               title="Warning" 
               position="bottom-right"
               delay="3000"
               auto-dismiss="true">
    This will auto-dismiss after 3 seconds.
</noundry-toast>

<!-- Non-dismissible toast -->
<noundry-toast type="error" 
               title="Error" 
               dismissible="false"
               auto-dismiss="false">
    This toast stays until page refresh.
</noundry-toast>
```

### Dropdown Menu
Context menus and user menus.

```html
<!-- User menu with avatar -->
<noundry-dropdown-menu button-text="John Doe" 
                       avatar-url="https://example.com/avatar.jpg"
                       user-name="John Doe" 
                       user-subtitle="john@example.com"
                       position="right">
    <noundry-dropdown-item text="Profile" icon="user" href="/profile" />
    <noundry-dropdown-item text="Settings" icon="settings" shortcut="⌘S" />
    <noundry-dropdown-item is-separator="true" />
    <noundry-dropdown-item text="Sign Out" icon="external-link" />
</noundry-dropdown-menu>

<!-- Simple dropdown -->
<noundry-dropdown-menu button-text="Actions">
    <noundry-dropdown-item text="Edit" icon="edit" />
    <noundry-dropdown-item text="Delete" icon="trash" />
    <noundry-dropdown-item text="Archive" disabled="true" />
</noundry-dropdown-menu>
```

## Form Integration

### Model Binding Example

```csharp
public class UserViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool EnableNotifications { get; set; }

    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }

    public string? Country { get; set; }
}

// Data table model binding example
public class PageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
{
    [BindProperty]
    public List<UserInfo> Users { get; set; } = new();
    
    public void OnGet()
    {
        Users = new List<UserInfo>
        {
            new UserInfo { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", Status = "Active", Role = "Admin" },
            new UserInfo { Id = 2, Name = "Bob Smith", Email = "bob@example.com", Status = "Inactive", Role = "User" },
            // ... more users
        };
    }
}

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
}
```

```html
<form asp-action="Create" method="post">
    <div class="space-y-4">
        <noundry-text-input asp-for="Name" />
        <noundry-text-input asp-for="Email" type="email" />
        <noundry-switch asp-for="EnableNotifications" />
        <noundry-date-picker asp-for="BirthDate" />
        
        <noundry-select asp-for="Country">
            <noundry-option value="">Select Country</noundry-option>
            <noundry-option value="us">United States</noundry-option>
            <noundry-option value="uk">United Kingdom</noundry-option>
        </noundry-select>
        
        <div class="flex gap-2">
            <noundry-button type="submit">Save</noundry-button>
            <noundry-button type="button" variant="secondary">Cancel</noundry-button>
        </div>
    </div>
</form>
```

## Advanced Usage

### Custom Styling
All components accept a `css-class` attribute for custom styling:

```html
<noundry-button css-class="my-custom-button">Custom Button</noundry-button>
<noundry-alert type="info" css-class="border-l-4 border-blue-500">Custom Alert</noundry-alert>
```

### Validation Integration
Components automatically integrate with ASP.NET model validation:

```html
<noundry-text-input asp-for="Email" />
<!-- Automatically shows validation errors from ModelState -->

<span asp-validation-for="Email" class="text-red-500 text-sm"></span>
```

### Skeleton
Loading state placeholders with animations.

```html
<!-- Basic skeleton -->
<noundry-skeleton variant="text" height="h-6" width="w-3/4" />

<!-- Avatar skeleton -->
<noundry-skeleton variant="avatar" size="lg" />

<!-- Circle skeleton -->
<noundry-skeleton variant="circle" width="4rem" height="h-16" />

<!-- Rectangle skeleton -->
<noundry-skeleton variant="rectangle" height="h-32" width="w-full" />

<!-- Multiple skeleton lines -->
<noundry-skeleton variant="text" lines="3" variable-width="true" />

<!-- Skeleton text component -->
<noundry-skeleton-text lines="4" variable-width="true" line-height="h-4" />

<!-- Avatar skeleton with sizes -->
<noundry-skeleton-avatar size="sm" />
<noundry-skeleton-avatar size="md" />
<noundry-skeleton-avatar size="lg" />
<noundry-skeleton-avatar size="xl" />

<!-- Card skeleton -->
<noundry-skeleton-card show-image="true" show-avatar="true" text-lines="3" />

<!-- Custom skeleton container -->
<noundry-skeleton-container container-class="flex items-center space-x-3" loading="true">
    <noundry-skeleton variant="avatar" size="md" />
    <div class="flex-1 space-y-2">
        <noundry-skeleton variant="text" height="h-5" width="w-3/4" />
        <noundry-skeleton variant="text" height="h-4" width="w-1/2" />
    </div>
</noundry-skeleton-container>

<!-- Conditional loading -->
<noundry-skeleton-container loading="@Model.IsLoading">
    <noundry-skeleton-text lines="3" />
</noundry-skeleton-container>
```

### Alpine.js Integration
All interactive components use Alpine.js for client-side behavior. You can extend functionality by adding custom Alpine.js directives:

```html
<div x-data="{ customData: 'hello' }">
    <noundry-button x-on:click="alert(customData)">Click Me</noundry-button>
</div>
```

## Component Properties Reference

### Alert (`noundry-alert`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `type` | string | "info" | Alert type: info, success, warning, error |
| `title` | string | null | Optional alert title |
| `dismissible` | bool | false | Whether the alert can be dismissed |
| `css-class` | string | null | Additional CSS classes |

### Badge (`noundry-badge`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `variant` | string | "default" | Badge variant: default, success, warning, error, info |
| `size` | string | "md" | Badge size: sm, md, lg |
| `icon` | string | null | Icon name (Heroicons) |
| `outline` | bool | false | Use outlined style |
| `pill` | bool | false | Use pill (rounded) style |
| `css-class` | string | null | Additional CSS classes |

### Button (`noundry-button`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `variant` | string | "primary" | Button variant: primary, secondary, success, danger, outline |
| `size` | string | "md" | Button size: sm, md, lg |
| `type` | string | "button" | Button type: button, submit, reset |
| `disabled` | bool | false | Disable the button |
| `loading` | bool | false | Show loading state |
| `loading-text` | string | null | Text to show when loading |
| `icon` | string | null | Icon name (Heroicons) |
| `icon-position` | string | "left" | Icon position: left, right |
| `css-class` | string | null | Additional CSS classes |

### Modal (`noundry-modal`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `title` | string | null | Modal title |
| `button-text` | string | "Open Modal" | Trigger button text |
| `button-variant` | string | "primary" | Trigger button variant |
| `max-width` | string | "sm:max-w-lg" | Modal max width classes |
| `close-on-backdrop` | bool | true | Close when clicking backdrop |
| `show-close-button` | bool | true | Show X close button |
| `css-class` | string | null | Additional CSS classes |

### Accordion (`noundry-accordion`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `multiple` | bool | false | Allow multiple items open |
| `css-class` | string | null | Additional CSS classes |

### Accordion Item (`noundry-accordion-item`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `title` | string | required | Accordion item title |
| `open` | bool | false | Initially open |
| `icon` | string | null | Icon name (Heroicons) |
| `css-class` | string | null | Additional CSS classes |

### Tabs (`noundry-tabs`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `variant` | string | "underline" | Tab style: underline, pills, bordered |
| `css-class` | string | null | Additional CSS classes |

### Tab Item (`noundry-tab-item`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `title` | string | required | Tab title |
| `active` | bool | false | Initially active |
| `icon` | string | null | Icon name (Heroicons) |
| `disabled` | bool | false | Disable the tab |
| `css-class` | string | null | Additional CSS classes |

### Switch (`noundry-switch`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `asp-for` | ModelExpression | null | Model binding expression |
| `label` | string | null | Switch label |
| `checked` | bool | false | Initially checked |
| `disabled` | bool | false | Disable the switch |
| `size` | string | "md" | Switch size: sm, md, lg |
| `color` | string | "blue" | Switch color theme |
| `css-class` | string | null | Additional CSS classes |

### Text Input (`noundry-text-input`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `asp-for` | ModelExpression | null | Model binding expression |
| `type` | string | "text" | Input type: text, email, password, number, tel, url |
| `label` | string | null | Input label |
| `placeholder` | string | null | Input placeholder |
| `value` | string | null | Input value |
| `required` | bool | false | Mark as required |
| `disabled` | bool | false | Disable the input |
| `readonly` | bool | false | Make input readonly |
| `size` | string | "md" | Input size: sm, md, lg |
| `icon` | string | null | Icon name (Heroicons) |
| `icon-position` | string | "left" | Icon position: left, right |
| `help-text` | string | null | Help text below input |
| `error-message` | string | null | Error message to display |
| `max-length` | int | null | Maximum character length |
| `css-class` | string | null | Additional CSS classes |

### Date Picker (`noundry-date-picker`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `asp-for` | ModelExpression | null | Model binding expression |
| `label` | string | null | Input label |
| `value` | DateTime? | null | Selected date |
| `format` | string | "yyyy-MM-dd" | Date format |
| `min-date` | DateTime? | null | Minimum selectable date |
| `max-date` | DateTime? | null | Maximum selectable date |
| `culture` | string | "en-US" | Culture for localization |
| `placeholder` | string | "Select date" | Input placeholder |
| `disabled` | bool | false | Disable the picker |
| `required` | bool | false | Mark as required |
| `css-class` | string | null | Additional CSS classes |

### Date Range Picker (`noundry-date-range-picker`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `label` | string | null | Label for the date range picker |
| `format` | string | "M d, Y" | Date format: M d, Y \| MM-DD-YYYY \| DD-MM-YYYY \| YYYY-MM-DD \| D d M, Y |
| `culture` | string | "en-US" | Culture for date formatting |
| `start-date-property` | string | null | Property name for start date model binding |
| `end-date-property` | string | null | Property name for end date model binding |
| `start-date` | DateTime? | null | Initial start date |
| `end-date` | DateTime? | null | Initial end date |
| `min-date` | DateTime? | null | Minimum selectable date |
| `max-date` | DateTime? | null | Maximum selectable date |
| `show-quick-select` | bool | true | Whether to show quick select buttons |
| `show-days-count` | bool | true | Whether to show days count display |
| `show-clear` | bool | true | Whether to show clear selection button |
| `quick-select-options` | string | "today,yesterday,last7,last30,last90" | Comma-separated quick select options |
| `width` | string | "w-[17rem]" | Width of the picker container |
| `calendar-width` | string | "w-[20rem]" | Width of the calendar dropdown |
| `placeholder` | string | "Select date range" | Input placeholder text |
| `help-text` | string | null | Help text displayed below picker |
| `error-message` | string | null | Error message to display |
| `disabled` | bool | false | Whether the picker is disabled |
| `required` | bool | false | Whether the picker is required |
| `css-class` | string | null | Additional CSS classes |

### Data Table (`noundry-data-table`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `asp-for` | ModelExpression | null | Model binding for collection data |
| `title` | string | null | Table title displayed in header |
| `api-url` | string | null | API endpoint URL for data loading |
| `per-page` | int | 10 | Number of items per page |
| `server-pagination` | bool | false | Whether to use server-side pagination |
| `show-search` | bool | true | Whether to show search input |
| `search-placeholder` | string | "Search..." | Search input placeholder text |
| `show-pagination` | bool | true | Whether to show pagination controls |
| `show-per-page-selector` | bool | true | Whether to show per-page selector |
| `per-page-options` | string | "5,10,25,50" | Comma-separated per-page options |
| `no-results-message` | string | "No results found" | Message when no results |
| `loading-message` | string | "Loading..." | Loading state message |
| `error-message` | string | "Error loading data" | Error message prefix |
| `hoverable` | bool | true | Whether rows are hoverable |
| `striped` | bool | false | Whether table has striped rows |
| `size` | string | "md" | Table size: sm, md, lg |
| `css-class` | string | null | Additional CSS classes |

### Data Table Column (`noundry-data-table-column`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `key` | string | required | Column key/property name |
| `label` | string | required | Column display label |
| `sortable` | bool | false | Whether this column is sortable |
| `href` | string | null | URL template for links (use {property} placeholders) |
| `href-text` | string | null | Text template for link display |
| `width` | string | null | Column width class |
| `align` | string | "left" | Text alignment: left, center, right |
| `hidden` | bool | false | Whether column is initially hidden |

### Data Table Row (`noundry-data-table-row`)
Used for static data in data tables. No properties - acts as a container for cells.

### Data Table Cell (`noundry-data-table-cell`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `key` | string | required | Column key this cell belongs to |

### Multi-Select (`noundry-multi-select`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `asp-for` | ModelExpression | null | Model binding expression |
| `label` | string | null | Label for the multi-select |
| `use-api` | bool | false | Whether to load options from API |
| `api-url` | string | null | API endpoint URL for loading options |
| `api-id-property` | string | "id" | Property name for option ID in API response |
| `api-name-property` | string | "name" | Property name for option name in API response |
| `loading-text` | string | "Loading options..." | Text shown while loading API data |
| `error-text` | string | "Failed to load options. Please try again." | Text shown when API fails |
| `select-placeholder` | string | "Select options..." | Placeholder when no options selected |
| `max-height` | string | "max-h-60" | Maximum height for dropdown |
| `show-remove-buttons` | bool | true | Whether to show remove buttons on selected items |
| `display-style` | string | "tags" | Display style: tags, list, count |
| `max-display-items` | int | 5 | Max items before showing count |
| `color` | string | "blue" | Color scheme: blue, green, red, gray |
| `help-text` | string | null | Help text displayed below |
| `error-message` | string | null | Error message to display |
| `disabled` | bool | false | Whether the multi-select is disabled |
| `required` | bool | false | Whether the multi-select is required |
| `css-class` | string | null | Additional CSS classes |

### Multi-Select Option (`noundry-multi-select-option`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `value` | string | required | Option value |
| `id` | string | null | Option ID (for API compatibility) |
| `selected` | bool | false | Whether initially selected |
| `disabled` | bool | false | Whether the option is disabled |

### Select (`noundry-select`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `asp-for` | ModelExpression | null | Model binding expression |
| `label` | string | null | Select label |
| `placeholder` | string | "Select option" | Placeholder text |
| `multiple` | bool | false | Allow multiple selection |
| `searchable` | bool | false | Enable search functionality |
| `search-placeholder` | string | "Search..." | Search input placeholder |
| `no-results-text` | string | "No results found" | Text when no matches |
| `disabled` | bool | false | Disable the select |
| `required` | bool | false | Mark as required |
| `size` | string | "md" | Select size: sm, md, lg |
| `css-class` | string | null | Additional CSS classes |

### Option (`noundry-option`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `value` | string | required | Option value |
| `selected` | bool | false | Initially selected |
| `disabled` | bool | false | Disable the option |

### Toast (`noundry-toast`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `type` | string | "info" | Toast type: info, success, warning, error |
| `title` | string | null | Toast title |
| `position` | string | "top-right" | Position: top-left, top-right, bottom-left, bottom-right |
| `trigger-text` | string | "Show Toast" | Trigger button text |
| `dismissible` | bool | true | Can be dismissed |
| `auto-dismiss` | bool | true | Auto dismiss after delay |
| `delay` | int | 5000 | Auto dismiss delay (ms) |
| `css-class` | string | null | Additional CSS classes |

### Dropdown Menu (`noundry-dropdown-menu`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `button-text` | string | "Menu" | Button text |
| `button-variant` | string | "secondary" | Button variant |
| `avatar-url` | string | null | User avatar URL |
| `user-name` | string | null | User name for header |
| `user-subtitle` | string | null | User subtitle (email, etc.) |
| `position` | string | "left" | Dropdown position: left, right |
| `css-class` | string | null | Additional CSS classes |

### Dropdown Item (`noundry-dropdown-item`)
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `text` | string | null | Item text |
| `icon` | string | null | Icon name (Heroicons) |
| `href` | string | null | Link URL |
| `shortcut` | string | null | Keyboard shortcut display |
| `disabled` | bool | false | Disable the item |
| `is-separator` | bool | false | Render as separator |
| `css-class` | string | null | Additional CSS classes |

## Browser Support
- Modern browsers supporting ES2015+
- Alpine.js 3.x
- Tailwind CSS 3.x

## Performance Considerations
- Components are rendered server-side for fast initial load
- Alpine.js provides lightweight client-side interactivity
- Use `x-cloak` to prevent flash of unstyled content
- Consider using `x-init` for component initialization when needed