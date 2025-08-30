using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;
using System.Globalization;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-date-range-picker")]
public class DateRangePickerTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Label text for the date range picker
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Date format for display (M d, Y | MM-DD-YYYY | DD-MM-YYYY | YYYY-MM-DD | D d M, Y)
    /// </summary>
    public string Format { get; set; } = "M d, Y";

    /// <summary>
    /// Culture for date formatting
    /// </summary>
    public string Culture { get; set; } = "en-US";

    /// <summary>
    /// Start date property name for model binding
    /// </summary>
    public string? StartDateProperty { get; set; }

    /// <summary>
    /// End date property name for model binding
    /// </summary>
    public string? EndDateProperty { get; set; }

    /// <summary>
    /// Initial start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Initial end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Minimum selectable date
    /// </summary>
    public DateTime? MinDate { get; set; }

    /// <summary>
    /// Maximum selectable date
    /// </summary>
    public DateTime? MaxDate { get; set; }

    /// <summary>
    /// Whether to show quick select buttons
    /// </summary>
    public bool ShowQuickSelect { get; set; } = true;

    /// <summary>
    /// Whether to show days count
    /// </summary>
    public bool ShowDaysCount { get; set; } = true;

    /// <summary>
    /// Whether to show clear button
    /// </summary>
    public bool ShowClear { get; set; } = true;

    /// <summary>
    /// Quick select options (comma-separated: today,yesterday,last7,last30,last90)
    /// </summary>
    public string QuickSelectOptions { get; set; } = "today,yesterday,last7,last30,last90";

    /// <summary>
    /// Width of the date picker container
    /// </summary>
    public string Width { get; set; } = "w-[17rem]";

    /// <summary>
    /// Width of the calendar dropdown
    /// </summary>
    public string CalendarWidth { get; set; } = "w-[20rem]";

    /// <summary>
    /// Help text displayed below the picker
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Error message to display
    /// </summary>
    public string? ErrorMessage { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        var pickerId = GetInputId();
        var labelText = Label ?? GetDisplayName();
        var culture = new CultureInfo(Culture);

        output.TagName = "div";
        AddCssClasses(output, "space-y-1");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var alpineData = BuildDateRangePickerData(culture);
        SetAlpineAttribute(output, "data", alpineData);
        SetAlpineAttribute(output, "init", "init()");
        output.Attributes.SetAttribute("x-cloak", "");

        var calendarIcon = CreateIcon(Icons.Calendar, "w-6 h-6");
        var chevronLeft = CreateIcon(Icons.ArrowLeft, "w-6 h-6 text-gray-400");
        var chevronRight = CreateIcon(Icons.ArrowRight, "w-6 h-6 text-gray-400");

        var quickSelectButtons = ShowQuickSelect ? BuildQuickSelectButtons() : "";
        var hasError = !string.IsNullOrEmpty(ErrorMessage);

        var dateRangePickerHtml = $@"
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <label for=""{pickerId}"" class=""block mb-1 text-sm font-medium text-neutral-500"">
                {EscapeJavaScriptString(labelText)}{(Required ? @" <span class=""text-red-500"">*</span>" : "")}
            </label>
            " : "")}

            <div class=""relative {Width}"">
                <!-- Display input (readonly) -->
                <input 
                    x-ref=""datePickerInput"" 
                    type=""text"" 
                    id=""{pickerId}""
                    @click=""toggleDatePicker()"" 
                    x-model=""displayValue"" 
                    x-on:keydown.escape=""isOpen = false"" 
                    class=""flex w-full h-10 px-3 py-2 text-sm bg-white border rounded-md text-neutral-600 {(hasError ? "border-red-300 focus:border-red-300 focus:ring-red-500" : "border-neutral-300 focus:border-neutral-300 focus:ring-neutral-400")} ring-offset-background placeholder:text-neutral-400 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"" 
                    placeholder=""{EscapeJavaScriptString(Placeholder ?? "Select date range")}"" 
                    {(Disabled ? "disabled" : "")}
                    {(Required ? "required" : "")}
                    readonly 
                />
                
                <!-- Hidden inputs for form submission -->
                <input type=""hidden"" name=""{StartDateProperty ?? (GetInputName() + "StartDate")}"" x-model=""startDate"" />
                <input type=""hidden"" name=""{EndDateProperty ?? (GetInputName() + "EndDate")}"" x-model=""endDate"" />
                
                <!-- Calendar icon -->
                <div 
                    @click=""toggleDatePicker(); if(isOpen){{ $refs.datePickerInput.focus() }}"" 
                    class=""absolute top-0 right-0 px-3 py-2 cursor-pointer text-neutral-400 hover:text-neutral-500""
                >
                    {calendarIcon}
                </div>
                
                <!-- Calendar dropdown -->
                <div  
                    x-show=""isOpen""
                    x-transition
                    @click.away=""isOpen = false"" 
                    class=""absolute top-0 left-0 max-w-lg p-4 mt-12 antialiased bg-white border rounded-lg shadow {CalendarWidth} border-neutral-200/70 z-50""
                >
                    {quickSelectButtons}
                    
                    <!-- Calendar header -->
                    <div class=""flex items-center justify-between mb-2"">
                        <div>
                            <span x-text=""monthNames[month]"" class=""text-lg font-bold text-gray-800""></span>
                            <span x-text=""year"" class=""ml-1 text-lg font-normal text-gray-600""></span>
                        </div>
                        <div>
                            <button @click=""previousMonth()"" type=""button"" class=""inline-flex p-1 transition duration-100 ease-in-out rounded-full cursor-pointer focus:outline-none focus:shadow-outline hover:bg-gray-100"">
                                {chevronLeft}
                            </button>
                            <button @click=""nextMonth()"" type=""button"" class=""inline-flex p-1 transition duration-100 ease-in-out rounded-full cursor-pointer focus:outline-none focus:shadow-outline hover:bg-gray-100"">
                                {chevronRight}
                            </button>
                        </div>
                    </div>
                    
                    <!-- Day names -->
                    <div class=""grid grid-cols-7 mb-3"">
                        <template x-for=""(day, index) in days"" :key=""index"">
                            <div class=""px-0.5"">
                                <div x-text=""day"" class=""text-xs font-medium text-center text-gray-800""></div>
                            </div>
                        </template>
                    </div>
                    
                    <!-- Calendar days -->
                    <div class=""grid grid-cols-7"">
                        <template x-for=""blankDay in blankDays"">
                            <div class=""p-1 text-sm text-center border border-transparent""></div>
                        </template>
                        <template x-for=""(day, dayIndex) in daysInMonth"" :key=""dayIndex"">
                            <div class=""px-0.5 mb-1 aspect-square"">
                                <div 
                                    x-text=""day""
                                    @click=""dayClicked(day)"" 
                                    :class=""{{
                                        'bg-neutral-200': isToday(day) && !isInSelectedRange(day), 
                                        'text-gray-600 hover:bg-neutral-200': !isToday(day) && !isInSelectedRange(day),
                                        'bg-neutral-800 text-white hover:bg-opacity-75': isStartDate(day) || isEndDate(day),
                                        'bg-neutral-200/70': isInSelectedRange(day) && !isStartDate(day) && !isEndDate(day)
                                    }}"" 
                                    class=""flex items-center justify-center text-sm leading-none text-center rounded-full cursor-pointer h-7 w-7""
                                ></div>
                            </div>
                        </template>
                    </div>
                    
                    <!-- Range selection info -->
                    <div class=""flex justify-between mt-2"">
                        <div class=""text-sm text-gray-600"" x-show=""startDateValue && !endDateValue"">
                            Select end date
                        </div>
                        <div class=""text-sm text-gray-600"" x-show=""!startDateValue"">
                            Select start date
                        </div>
                        <div class=""text-sm text-gray-600"" x-show=""startDateValue && endDateValue"">
                            {(ShowClear ? @"
                            <button @click=""clearSelection()"" class=""px-2 py-1 text-xs text-white bg-neutral-700 rounded hover:bg-neutral-600"">
                                Clear Selection
                            </button>
                            " : "")}
                        </div>
                        
                        {(ShowDaysCount ? @"
                        <!-- Selected range display -->
                        <div class=""text-xs text-gray-500"" x-show=""startDateValue && endDateValue"">
                            <span x-text=""getDaysBetween() + ' days'""></span>
                        </div>
                        " : "")}
                    </div>
                </div>
            </div>

            {(!string.IsNullOrEmpty(HelpText) && string.IsNullOrEmpty(ErrorMessage) ? $@"
            <p class=""text-sm text-gray-500"">
                {EscapeJavaScriptString(HelpText)}
            </p>
            " : "")}

            {(!string.IsNullOrEmpty(ErrorMessage) ? $@"
            <p class=""text-sm text-red-600"">
                {EscapeJavaScriptString(ErrorMessage)}
            </p>
            " : "")}";

        output.Content.SetHtmlContent(dateRangePickerHtml);
    }

    private string BuildDateRangePickerData(CultureInfo culture)
    {
        var monthNames = culture.DateTimeFormat.MonthNames.Take(12).ToArray();
        var dayNames = culture.DateTimeFormat.AbbreviatedDayNames.Select(d => d.Substring(0, 3)).ToArray();

        var startDateJs = StartDate?.ToString("yyyy-MM-dd") ?? "";
        var endDateJs = EndDate?.ToString("yyyy-MM-dd") ?? "";

        var builder = new AlpineDataBuilder()
            .AddBooleanProperty("isOpen", false)
            .AddStringProperty("month", "")
            .AddStringProperty("year", "")
            .AddStringProperty("day", "")
            .AddProperty("days", dayNames)
            .AddProperty("monthNames", monthNames)
            .AddProperty("blankDays", new int[0])
            .AddProperty("daysInMonth", new int[0])
            .AddStringProperty("startDateValue", startDateJs)
            .AddStringProperty("endDateValue", endDateJs)
            .AddStringProperty("displayValue", "")
            .AddStringProperty("selectionMode", "start")
            .AddStringProperty("format", Format);

        // Add computed properties
        builder.AddMethod("get startDate() { return this.startDateValue ? this.formatDateForSubmission(new Date(this.startDateValue)) : ''; }");
        builder.AddMethod("get endDate() { return this.endDateValue ? this.formatDateForSubmission(new Date(this.endDateValue)) : ''; }");

        // Add core methods
        builder.AddMethod("init() { let currentDate = new Date(); this.month = currentDate.getMonth(); this.year = currentDate.getFullYear(); this.calculateDays(); this.updateDisplayValue(); }");
        builder.AddMethod("toggleDatePicker() { this.isOpen = !this.isOpen; }");
        
        // Day selection logic
        builder.AddMethod(@"dayClicked(day) { 
            let selectedDate = new Date(this.year, this.month, day); 
            if (this.selectionMode === 'start' || (this.startDateValue && this.endDateValue)) { 
                this.startDateValue = selectedDate; 
                this.endDateValue = ''; 
                this.selectionMode = 'end'; 
            } else { 
                let startDate = new Date(this.startDateValue); 
                if (selectedDate < startDate) { 
                    this.endDateValue = this.startDateValue; 
                    this.startDateValue = selectedDate; 
                } else { 
                    this.endDateValue = selectedDate; 
                } 
                this.selectionMode = 'start'; 
                this.updateDisplayValue(); 
                this.isOpen = false; 
            } 
            this.updateDisplayValue(); 
        }");

        // Quick select methods
        builder.AddMethod(@"quickSelect(range) { 
            const today = new Date(); 
            today.setHours(0, 0, 0, 0); 
            let startDate, endDate; 
            switch(range) { 
                case 'today': startDate = new Date(today); endDate = new Date(today); break; 
                case 'yesterday': startDate = new Date(today); startDate.setDate(startDate.getDate() - 1); endDate = new Date(startDate); break; 
                case 'last7': endDate = new Date(today); startDate = new Date(today); startDate.setDate(startDate.getDate() - 6); break; 
                case 'last30': endDate = new Date(today); startDate = new Date(today); startDate.setDate(startDate.getDate() - 29); break; 
                case 'last90': endDate = new Date(today); startDate = new Date(today); startDate.setDate(startDate.getDate() - 89); break; 
            } 
            this.startDateValue = startDate; 
            this.endDateValue = endDate; 
            this.updateDisplayValue(); 
            this.month = startDate.getMonth(); 
            this.year = startDate.getFullYear(); 
            this.calculateDays(); 
            this.isOpen = false; 
        }");

        // Utility methods
        builder.AddMethod("getDaysBetween() { if (!this.startDateValue || !this.endDateValue) return 0; const start = new Date(this.startDateValue); const end = new Date(this.endDateValue); const diffTime = Math.abs(end - start); const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)); return diffDays + 1; }");
        
        builder.AddMethod(@"updateDisplayValue() { 
            if (this.startDateValue && this.endDateValue) { 
                let formattedStart = this.formatDate(new Date(this.startDateValue)); 
                let formattedEnd = this.formatDate(new Date(this.endDateValue)); 
                this.displayValue = `${formattedStart} - ${formattedEnd}`; 
            } else if (this.startDateValue) { 
                this.displayValue = this.formatDate(new Date(this.startDateValue)); 
            } else { 
                this.displayValue = ''; 
            } 
        }");

        builder.AddMethod("clearSelection() { this.startDateValue = ''; this.endDateValue = ''; this.displayValue = ''; this.selectionMode = 'start'; }");

        // Navigation methods
        builder.AddMethod("previousMonth() { if (this.month === 0) { this.year--; this.month = 11; } else { this.month--; } this.calculateDays(); }");
        builder.AddMethod("nextMonth() { if (this.month === 11) { this.month = 0; this.year++; } else { this.month++; } this.calculateDays(); }");

        // Date checking methods
        builder.AddMethod("isStartDate(day) { if (!this.startDateValue) return false; const d = new Date(this.year, this.month, day); const start = new Date(this.startDateValue); return d.toDateString() === start.toDateString(); }");
        builder.AddMethod("isEndDate(day) { if (!this.endDateValue) return false; const d = new Date(this.year, this.month, day); const end = new Date(this.endDateValue); return d.toDateString() === end.toDateString(); }");
        builder.AddMethod("isInSelectedRange(day) { if (!this.startDateValue || !this.endDateValue) return false; const d = new Date(this.year, this.month, day); const start = new Date(this.startDateValue); const end = new Date(this.endDateValue); return d >= start && d <= end; }");
        builder.AddMethod("isToday(day) { const today = new Date(); const d = new Date(this.year, this.month, day); return today.toDateString() === d.toDateString(); }");

        // Calendar calculation
        builder.AddMethod(@"calculateDays() { 
            let daysInMonth = new Date(this.year, this.month + 1, 0).getDate(); 
            let dayOfWeek = new Date(this.year, this.month).getDay(); 
            let blankdaysArray = []; 
            for (var i = 1; i <= dayOfWeek; i++) { 
                blankdaysArray.push(i); 
            } 
            let daysArray = []; 
            for (var i = 1; i <= daysInMonth; i++) { 
                daysArray.push(i); 
            } 
            this.blankDays = blankdaysArray; 
            this.daysInMonth = daysArray; 
        }");

        // Date formatting methods
        builder.AddMethod($@"formatDate(date) {{ 
            let formattedDay = this.days[date.getDay()]; 
            let formattedDate = ('0' + date.getDate()).slice(-2); 
            let formattedMonth = this.monthNames[date.getMonth()]; 
            let formattedMonthShortName = this.monthNames[date.getMonth()].substring(0, 3); 
            let formattedMonthInNumber = ('0' + (parseInt(date.getMonth()) + 1)).slice(-2); 
            let formattedYear = date.getFullYear(); 
            
            if (this.format === 'M d, Y') return `${{formattedMonthShortName}} ${{formattedDate}}, ${{formattedYear}}`; 
            if (this.format === 'MM-DD-YYYY') return `${{formattedMonthInNumber}}-${{formattedDate}}-${{formattedYear}}`; 
            if (this.format === 'DD-MM-YYYY') return `${{formattedDate}}-${{formattedMonthInNumber}}-${{formattedYear}}`; 
            if (this.format === 'YYYY-MM-DD') return `${{formattedYear}}-${{formattedMonthInNumber}}-${{formattedDate}}`; 
            if (this.format === 'D d M, Y') return `${{formattedDay}} ${{formattedDate}} ${{formattedMonthShortName}} ${{formattedYear}}`; 
            return `${{formattedMonth}} ${{formattedDate}}, ${{formattedYear}}`; 
        }}");

        builder.AddMethod("formatDateForSubmission(date) { let formattedDate = ('0' + date.getDate()).slice(-2); let formattedMonth = ('0' + (parseInt(date.getMonth()) + 1)).slice(-2); let formattedYear = date.getFullYear(); return `${formattedYear}-${formattedMonth}-${formattedDate}`; }");

        return builder.Build();
    }

    private string BuildQuickSelectButtons()
    {
        if (!ShowQuickSelect) return "";

        var options = QuickSelectOptions.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var buttons = new List<string>();

        var optionLabels = new Dictionary<string, string>
        {
            { "today", "Today" },
            { "yesterday", "Yesterday" },
            { "last7", "Last 7" },
            { "last30", "Last 30" },
            { "last90", "Last 90" }
        };

        foreach (var option in options)
        {
            var label = optionLabels.ContainsKey(option.Trim()) ? optionLabels[option.Trim()] : option.Trim();
            buttons.Add($@"
                <button 
                    @click=""quickSelect('{option.Trim()}')"" 
                    class=""px-2 py-1 text-xs bg-neutral-100 rounded hover:bg-neutral-200""
                >
                    {label}
                </button>");
        }

        return $@"
            <!-- Quick select buttons -->
            <div class=""flex flex-wrap gap-1 mb-3"">
                {string.Join("\n", buttons)}
            </div>";
    }
}