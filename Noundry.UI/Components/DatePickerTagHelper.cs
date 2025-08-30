using Microsoft.AspNetCore.Razor.TagHelpers;
using Noundry.UI.Core;
using Noundry.UI.Extensions;
using System.Globalization;

namespace Noundry.UI.Components;

[HtmlTargetElement("noundry-date-picker")]
public class DatePickerTagHelper : NoundryFormTagHelperBase
{
    /// <summary>
    /// Date format for display
    /// </summary>
    public string Format { get; set; } = "MM/dd/yyyy";

    /// <summary>
    /// Label text
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Minimum selectable date
    /// </summary>
    public DateTime? MinDate { get; set; }

    /// <summary>
    /// Maximum selectable date
    /// </summary>
    public DateTime? MaxDate { get; set; }

    /// <summary>
    /// Initially selected date
    /// </summary>
    public DateTime? SelectedDate { get; set; }

    /// <summary>
    /// First day of week (0 = Sunday, 1 = Monday)
    /// </summary>
    public int FirstDayOfWeek { get; set; } = 0;

    /// <summary>
    /// Culture for date formatting
    /// </summary>
    public string Culture { get; set; } = "en-US";

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        await output.GetChildContentAsync();

        var inputId = GetInputId();
        var inputName = GetInputName();
        var labelText = Label ?? GetDisplayName();
        
        var selectedDate = SelectedDate ?? (AspFor?.Model as DateTime?);
        var currentDate = selectedDate ?? DateTime.Now;

        output.TagName = "div";
        AddCssClasses(output, "relative", "w-full");

        if (!string.IsNullOrEmpty(CssClass))
        {
            output.Attributes.AppendInClassValue(CssClass);
        }

        var alpineData = BuildDatePickerData(currentDate);
        SetAlpineAttribute(output, "data", alpineData);

        var calendarIcon = CreateIcon(Icons.Calendar, "w-5 h-5 text-gray-400");
        var chevronLeft = CreateIcon(Icons.ArrowLeft, "w-4 h-4");
        var chevronRight = CreateIcon(Icons.ArrowRight, "w-4 h-4");

        var datePickerHtml = $@"
            {(!string.IsNullOrEmpty(labelText) ? $@"
            <label for=""{inputId}"" class=""block text-sm font-medium text-gray-700 mb-1"">
                {EscapeJavaScriptString(labelText)}{(Required ? " <span class=\"text-red-500\">*</span>" : "")}
            </label>
            " : "")}

            <div class=""relative"">
                <input 
                    type=""text""
                    id=""{inputId}""
                    name=""{inputName}""
                    :value=""selectedDateFormatted""
                    @click=""datePickerOpen = !datePickerOpen""
                    readonly
                    {(!string.IsNullOrEmpty(Placeholder) ? $@"placeholder=""{EscapeJavaScriptString(Placeholder)}"" " : "")}
                    class=""flex w-full h-10 px-3 py-2 pr-10 text-sm bg-white border rounded-md cursor-pointer border-neutral-300 ring-offset-background placeholder:text-neutral-400 focus:border-neutral-300 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-neutral-400 disabled:cursor-not-allowed disabled:opacity-50""
                    {(Disabled ? "disabled" : "")}
                    {(Required ? "required" : "")}
                />
                
                <div class=""absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none"">
                    {calendarIcon}
                </div>
                
                <div x-show=""datePickerOpen"" 
                     x-on:click.away=""datePickerOpen = false""
                     x-transition:enter=""ease-out duration-200""
                     x-transition:enter-start=""opacity-0 scale-95""
                     x-transition:enter-end=""opacity-100 scale-100""
                     x-transition:leave=""ease-in duration-75""
                     x-transition:leave-start=""opacity-100 scale-100""
                     x-transition:leave-end=""opacity-0 scale-95""
                     class=""absolute z-50 mt-1 bg-white border border-gray-200 rounded-lg shadow-lg w-80 p-4""
                     x-cloak>
                    
                    <div class=""flex items-center justify-between mb-4"">
                        <button @click=""previousMonth()"" type=""button"" class=""p-1 hover:bg-gray-100 rounded"">
                            {chevronLeft}
                        </button>
                        
                        <div class=""flex space-x-2"">
                            <select x-model=""datePickerMonth"" @change=""datePickerCalculateDays()"" 
                                    class=""text-sm border-0 bg-transparent font-semibold focus:outline-none"">
                                <template x-for=""(month, index) in monthNames"" :key=""index"">
                                    <option :value=""index"" x-text=""month"" :selected=""datePickerMonth == index""></option>
                                </template>
                            </select>
                            
                            <select x-model=""datePickerYear"" @change=""datePickerCalculateDays()""
                                    class=""text-sm border-0 bg-transparent font-semibold focus:outline-none"">
                                <template x-for=""year in yearRange"" :key=""year"">
                                    <option :value=""year"" x-text=""year"" :selected=""datePickerYear == year""></option>
                                </template>
                            </select>
                        </div>
                        
                        <button @click=""nextMonth()"" type=""button"" class=""p-1 hover:bg-gray-100 rounded"">
                            {chevronRight}
                        </button>
                    </div>
                    
                    <div class=""grid grid-cols-7 gap-1 mb-2"">
                        <template x-for=""dayName in dayNames"" :key=""dayName"">
                            <div class=""text-xs font-medium text-gray-500 text-center p-2"" x-text=""dayName""></div>
                        </template>
                    </div>
                    
                    <div class=""grid grid-cols-7 gap-1"">
                        <template x-for=""day in datePickerDaysInMonth"" :key=""day.date"">
                            <button @click=""selectDate(day)""
                                    :disabled=""day.disabled""
                                    :class=""{{ 'p-2 text-sm rounded hover:bg-blue-50 focus:outline-none focus:ring-2 focus:ring-blue-500': true, 'text-gray-400 cursor-not-allowed': day.disabled, 'text-gray-300': !day.isCurrentMonth && !day.disabled, 'bg-blue-600 text-white': day.isSelected && !day.disabled, 'bg-gray-100': day.isToday && !day.isSelected && !day.disabled }}""
                                    x-text=""day.day"">
                            </button>
                        </template>
                    </div>
                </div>
            </div>
            ";

        output.Content.SetHtmlContent(datePickerHtml);
    }

    private string BuildDatePickerData(DateTime currentDate)
    {
        var culture = new CultureInfo(Culture);
        var monthNames = culture.DateTimeFormat.MonthNames.Take(12).ToArray();
        var dayNames = culture.DateTimeFormat.AbbreviatedDayNames;
        
        // Adjust day names based on FirstDayOfWeek
        var adjustedDayNames = new string[7];
        for (int i = 0; i < 7; i++)
        {
            adjustedDayNames[i] = dayNames[(FirstDayOfWeek + i) % 7];
        }

        var currentYear = currentDate.Year;
        var minYear = MinDate?.Year ?? currentYear - 10;
        var maxYear = MaxDate?.Year ?? currentYear + 10;

        var builder = new AlpineDataBuilder()
            .AddBooleanProperty("datePickerOpen", false)
            .AddProperty("datePickerMonth", currentDate.Month - 1) // JS months are 0-based
            .AddProperty("datePickerYear", currentDate.Year)
            .AddProperty("selectedDate", SelectedDate?.ToString("yyyy-MM-dd") ?? "")
            .AddStringProperty("selectedDateFormatted", SelectedDate?.ToString(Format) ?? "")
            .AddProperty("monthNames", monthNames)
            .AddProperty("dayNames", adjustedDayNames)
            .AddProperty("yearRange", Enumerable.Range(minYear, maxYear - minYear + 1).ToArray())
            .AddProperty("datePickerDaysInMonth", new object[0])
            .AddProperty("firstDayOfWeek", FirstDayOfWeek);

        builder.AddMethod("datePickerCalculateDays() { " +
            "const year = this.datePickerYear; " +
            "const month = this.datePickerMonth; " +
            "const firstDay = new Date(year, month, 1); " +
            "const lastDay = new Date(year, month + 1, 0); " +
            "const daysInMonth = lastDay.getDate(); " +
            "const startingDayOfWeek = (firstDay.getDay() - this.firstDayOfWeek + 7) % 7; " +
            "const days = []; " +
            "for(let i = 0; i < startingDayOfWeek; i++) { " +
                "const prevMonthDay = new Date(year, month, -startingDayOfWeek + i + 1); " +
                "days.push({ day: prevMonthDay.getDate(), date: prevMonthDay.toISOString().split('T')[0], isCurrentMonth: false, disabled: false, isToday: false, isSelected: false }); " +
            "} " +
            "for(let day = 1; day <= daysInMonth; day++) { " +
                "const currentDay = new Date(year, month, day); " +
                "const dateStr = currentDay.toISOString().split('T')[0]; " +
                "const isToday = dateStr === new Date().toISOString().split('T')[0]; " +
                "const isSelected = dateStr === this.selectedDate; " +
                "days.push({ day: day, date: dateStr, isCurrentMonth: true, disabled: false, isToday: isToday, isSelected: isSelected }); " +
            "} " +
            "const totalCells = Math.ceil(days.length / 7) * 7; " +
            "for(let i = days.length; i < totalCells; i++) { " +
                "const nextMonthDay = new Date(year, month + 1, i - days.length + 1); " +
                "days.push({ day: nextMonthDay.getDate(), date: nextMonthDay.toISOString().split('T')[0], isCurrentMonth: false, disabled: false, isToday: false, isSelected: false }); " +
            "} " +
            "this.datePickerDaysInMonth = days; " +
            "}");

        builder.AddMethod("selectDate(day) { " +
            "if(day.disabled) return; " +
            "this.selectedDate = day.date; " +
            "this.selectedDateFormatted = new Date(day.date + 'T00:00:00').toLocaleDateString('" + Culture + "'); " +
            "this.datePickerOpen = false; " +
            "this.datePickerCalculateDays(); " +
            "}");

        builder.AddMethod("previousMonth() { " +
            "if(this.datePickerMonth === 0) { this.datePickerMonth = 11; this.datePickerYear--; } else { this.datePickerMonth--; } " +
            "this.datePickerCalculateDays(); " +
            "}");

        builder.AddMethod("nextMonth() { " +
            "if(this.datePickerMonth === 11) { this.datePickerMonth = 0; this.datePickerYear++; } else { this.datePickerMonth++; } " +
            "this.datePickerCalculateDays(); " +
            "}");

        builder.AddMethod("init() { this.datePickerCalculateDays(); }");

        return builder.Build();
    }
}