class CompUiValidation {
    initialise() {
        this.formValidationSelectorClassName = 'form.compui-validation';
        this.fieldErrorClassName = 'field-validation-error';
        this.fieldValidClassName = 'field-validation-valid';
        this.govukErrorSummaryClassName = "govuk-error-summary";
        this.govukErrorMessageClassName = "govuk-error-message";
        this.govukGroupErrorClassName = 'govuk-form-group--error';
        this.govukGroupClassName = 'govuk-form-group';
        this.govukInputClassName = 'govuk-input';
        this.govukInputErrorClassName = 'govuk-input--error';
        this.govukTextAreaErrorClassName = 'govuk-textarea--error';
        this.compUiValidationForDate = 'CompUiValidationForDate';
        this.compUiShellHide = 'dfc-composite-shell-hide';
        this.mainErrorSummaryId = 'compuiShell-ErrorSummary';

        if ($(this.formValidationSelectorClassName).length > 0) {
            this.CaptureAppErrorSummaryItems();
            this.InitialiseFieldValidationChangeCapture();
            this.InitialiseValidationMessageChangeCapture();
            this.InitialiseDateFieldValidation();
            this.ShowErrorInPageTitle(this.mainErrorSummaryId);
        }
    }

    CaptureAppErrorSummaryItems() {
        this.CaptureAppErrorSummaryItemsNonFieldErrors();
        this.CaptureErrorSummaryItemsFieldErrors();
    }

    CaptureAppErrorSummaryItemsNonFieldErrors() {
        var errorSummary = $('.' + this.govukErrorSummaryClassName);
        var mainErrorSummary = errorSummary[0];
        var mainErrorList = $(mainErrorSummary).find('UL');

        for (var i = 1; i < errorSummary.length; i++) {
            var appErrorSummary = errorSummary[i];
            var errorItems = $(appErrorSummary).find('LI');

            for (var j = 0; j < errorItems.length; j++) {
                var anchor = $(errorItems[j]).find('A');

                if (anchor.length === 0) {
                    $(mainErrorList).append($(errorItems[j]));
                }
                else {
                    errorItems[j].remove();
                }
            }

            $(appErrorSummary).addClass(this.compUiShellHide);
        }
    }

    CaptureErrorSummaryItemsFieldErrors() {
        var formValidationSelectors = this.formValidationSelectorClassName + ' .' + this.govukErrorMessageClassName;
        var outerThis = this;

        $(formValidationSelectors).each(function () {
            var formGroup = $(this).closest('.' + outerThis.govukGroupClassName);
            var valInputElementId = $('#' + this.id).data('valmsg-for');

            if (valInputElementId === undefined) {
                valInputElementId = this.id.replace('-error', '');
            }

            var inputElementId = '#' + valInputElementId.replace('.', '_');
            var errorMessage = $(this).text();

            outerThis.SyncErrorSummaryItem(formGroup, inputElementId, errorMessage);
        });
    }

    ShowErrorInPageTitle(errorSummaryId, setFocus = true) {
        var errorSummary = $('#' + errorSummaryId);
        var errorsVisible = this.AnyVisibleErrorSummaryErrors(errorSummary);

        if (errorsVisible) {
            $(errorSummary).removeClass(this.compUiShellHide);
            if (setFocus) {
                $(errorSummary).focus();
            }
        } else {
            $(errorSummary).addClass(this.compUiShellHide);
        }

        var errorStub = "Error: ";
        var titleBeginsWithError = document.title.indexOf(errorStub) === 0;
        if (titleBeginsWithError && !errorsVisible) {
            document.title = document.title.substring(errorStub.length);
        }
        else if (!titleBeginsWithError && errorsVisible) {
            document.title = errorStub + document.title;
        }
    }

    AnyVisibleErrorSummaryErrors(errorSummary) {
        var allItems = $(errorSummary).find('LI');

        for (var i = 0; i < allItems.length; i++) {
            if ($(allItems[i]).text() != '') {
                return true;
            }
        }

        return false;
    }

    InitialiseFieldValidationChangeCapture() {
        // override add/remove class to trigger a change event
        (function (func) {
            $.fn.addClass = function () {
                func.apply(this, arguments);
                this.trigger('classChanged');
                return this;
            }
        })($.fn.addClass); // pass the original function as an argument

        (function (func) {
            $.fn.removeClass = function () {
                func.apply(this, arguments);
                this.trigger('classChanged');
                return this;
            }
        })($.fn.removeClass);

        // trigger the add/remove class changes to add validation error class to the parent form group
        var formValidationSelectors = this.formValidationSelectorClassName + ' .' + this.govukErrorMessageClassName;
        var outerThis = this;

        $(formValidationSelectors).each(function () {
            outerThis.InitialiseFieldErrorClassChangeCapture(this);
        });

        $(formValidationSelectors).on('classChanged', function () {
            outerThis.InitialiseFieldErrorClassChangeCapture(this);
        });
    }

    InitialiseValidationMessageChangeCapture() {
        var formValidationSelectors = this.formValidationSelectorClassName + ' .' + this.govukErrorMessageClassName;
        var outerThis = this;

        $(formValidationSelectors).each(function () {
            outerThis.InitialiseValidationMessageChange(this);
        });
    }

    InitialiseValidationMessageChange(validMsg) {
        var outerThis = this;

        var observer = new MutationObserver(function (records) {
            records.forEach(function (record) {
                var errorMessage = '';
                var formGroup = $('#' + record.target.id).closest('.' + outerThis.govukGroupClassName);
                var valInputElementId = $('#' + record.target.id).data('valmsg-for');

                if (valInputElementId === undefined) {
                    valInputElementId = record.target.id.replace('-error', '');
                }

                var inputElementId = '#' + valInputElementId.replace('.', '_');

                if (record.addedNodes.length > 0) {
                    errorMessage = $(record.addedNodes[0]).text();
                }

                outerThis.SyncErrorSummaryItem(formGroup, inputElementId, errorMessage);
                outerThis.ShowErrorInPageTitle(outerThis.mainErrorSummaryId, false);
            });
        });

        observer.observe(validMsg, { childList: true, subtree: true });
    }

    InitialiseFieldErrorClassChangeCapture(validMsg) {
        var formGroup = $(validMsg).closest('.' + this.govukGroupClassName);
        var inputElementId = '#' + $(validMsg).data('valmsg-for').replace('.', '_');
        var errorCount = formGroup.find('.' + this.fieldErrorClassName).length;

        if (errorCount > 0) {
            formGroup.addClass(this.govukGroupErrorClassName);
        } else {
            formGroup.removeClass(this.govukGroupErrorClassName);
        }

        var inputElement = formGroup.find(inputElementId);
        var inputElementClassName = this.GetErrorClassForTag(inputElement.prop("tagName"));

        if (validMsg.classList.contains(this.fieldErrorClassName)) {
            inputElement.addClass(inputElementClassName);
        } else {
            inputElement.removeClass(inputElementClassName);
        }
    }

    GetErrorClassForTag(tagName) {
        return tagName === 'TEXTAREA' ? this.govukTextAreaErrorClassName : this.govukInputErrorClassName;
    }

    SyncErrorSummaryItem(formGroup, inputElementId, message) {
        var inputElement = $(inputElementId);
        if (inputElement.prop("tagName") != 'INPUT' && inputElement.prop("tagName") != 'TEXTAREA') {
            inputElementId = '#' + $(formGroup).find('INPUT,TEXTAREA')[0].id;
        }
        var errorSummary = $('.' + this.govukErrorSummaryClassName);

        if (errorSummary.length > 0) {
            var mainErrorSummary = errorSummary[0];
            var errorList = $(mainErrorSummary).find('UL')[0];
            var existingItems = $(errorList).find('LI A[href="' + inputElementId + '"]');

            if (existingItems.length > 0) {
                existingItems[0].text = message;
            }
            else {
                var errorItem = document.createElement("LI");
                var errorAnchor = document.createElement("A");
                var errorText = document.createTextNode(message);
                errorAnchor.setAttribute('href', inputElementId);
                errorAnchor.appendChild(errorText);
                errorItem.appendChild(errorAnchor);
                errorList.appendChild(errorItem);
            }
        }
    }

    InitialiseDateFieldValidation() {
        var outerThis = this;

        $.validator.unobtrusive.adapters.add(
            outerThis.compUiValidationForDate, ['properties'], function (options) {
                options.rules[outerThis.compUiValidationForDate] = options.params;
                options.messages[outerThis.compUiValidationForDate] = options.message;
            }
        );

        $.validator.addMethod(outerThis.compUiValidationForDate, function (value, element, params) {
            var result = outerThis.ValidateDateTime(value, element, params);

            $.validator.messages.dateValidation = result;

            return result === null;
        }, '');
    }

    ValidateDateTime(value, element, params) {
        var displayName = params.displayName;
        var minDate = CompUiUtilties.stringUtcToDate(params.minDate);
        var maxDate = CompUiUtilties.stringUtcToDate(params.maxDate);
        var dateRangeError = params.dateRangeError;
        var displayNameLowerCase = displayName.toLowerCase();
        var dateFormGroup = $(element).closest('.' + this.govukGroupClassName)[0];
        var validMsg = $(dateFormGroup).find('.' + this.govukErrorMessageClassName)[0];
        var inputFields = $(dateFormGroup).find('.' + this.govukInputClassName);
        var isForDateOnly = inputFields.length === 3;
        var dayString = $(inputFields[0]).val();
        var monthString = $(inputFields[1]).val();
        var yearString = $(inputFields[2]).val();
        var hourString = isForDateOnly ? '0' : $(inputFields[3]).val();
        var minuteString = isForDateOnly ? '0' : $(inputFields[4]).val();

        if (isForDateOnly && dayString === '' && monthString === '' && yearString === '') {
            return this.ValidationMessageShow(inputFields, null, validMsg, 'Enter ' + displayNameLowerCase);
        }
        if (!isForDateOnly && dayString === '' && monthString === '' && yearString === '' && hourString === '' && minuteString === '') {
            return this.ValidationMessageShow(inputFields, null, validMsg, 'Enter ' + displayNameLowerCase);
        }

        if (dayString === '') {
            return this.ValidationMessageShow(inputFields, [0], validMsg, displayName + ' must include a day');
        }
        if (monthString === '') {
            return this.ValidationMessageShow(inputFields, [1], validMsg, displayName + ' must include a month');
        }
        if (yearString === '' || yearString.length != 4) {
            return this.ValidationMessageShow(inputFields, [2], validMsg, displayName + ' must include a year');
        }

        if (!CompUiUtilties.isInt(dayString)) {
            return this.ValidationMessageShow(inputFields, [0], validMsg, displayName + ' requires numbers for the day');
        }
        if (!CompUiUtilties.isInt(monthString)) {
            return this.ValidationMessageShow(inputFields, [1], validMsg, displayName + ' requires numbers for the month');
        }
        if (!CompUiUtilties.isInt(yearString)) {
            return this.ValidationMessageShow(inputFields, [2], validMsg, displayName + ' requires numbers for the year');
        }

        var dayValue = parseInt(dayString);
        var monthValue = parseInt(monthString);
        var yearValue = parseInt(yearString);

        if (dayValue < 1 || dayValue > 31) {
            return this.ValidationMessageShow(inputFields, [1], validMsg, displayName + ' must be a real date');
        }
        if (monthValue < 1 || monthValue > 12) {
            return this.ValidationMessageShow(inputFields, [1], validMsg, displayName + ' must be a real date');
        }

        var daysInMonth = CompUiUtilties.getDaysInMonth(monthValue, yearValue);
        if (dayValue < 1 || dayValue > daysInMonth) {
            return this.ValidationMessageShow(inputFields, [0, 1], validMsg, displayName + ' must be a real date');
        }

        if (hourString === '') {
            return this.ValidationMessageShow(inputFields, [3], validMsg, displayName + ' must include an hour');
        }
        if (minuteString === '') {
            return this.ValidationMessageShow(inputFields, [4], validMsg, displayName + ' must include a minute');
        }
        if (!CompUiUtilties.isInt(hourString)) {
            return this.ValidationMessageShow(inputFields, [3], validMsg, displayName + ' requires numbers for the hour');
        }
        if (!CompUiUtilties.isInt(minuteString)) {
            return this.ValidationMessageShow(inputFields, [4], validMsg, displayName + ' requires numbers for the minute');
        }
        var hourValue = parseInt(hourString);
        var minuteValue = parseInt(minuteString);

        if (hourValue < 0 || hourValue > 23) {
            return this.ValidationMessageShow(inputFields, [3], validMsg, displayName + ' must be a real time');
        }
        if (minuteValue < 0 || minuteValue > 59) {
            return this.ValidationMessageShow(inputFields, [4], validMsg, displayName + ' must be a real time');
        }

        var dateObject = new Date(yearValue, monthValue - 1, dayValue, hourValue, minuteValue);

        if (CompUiUtilties.isValidDate(dateObject)) {
            if (dateObject < minDate || dateObject > maxDate) {
                return this.ValidationMessageShow(inputFields, [0, 1, 2], validMsg, dateRangeError);
            }

            this.ValidationMessageShow(inputFields, null, validMsg, '');
            return null;
        }

        return this.ValidationMessageShow(inputFields, null, validMsg, displayName + ' is not a valid date');
    }

    ValidationMessageShow(inputFields, errorFieldIndexes, validMsg, message) {
        var validMsgObj = $(validMsg);
        var showError = (message && message != "" ? true : false);

        validMsgObj.empty();

        if (showError) {
            validMsgObj.removeClass(this.fieldValidClassName);
            validMsgObj.addClass(this.fieldErrorClassName);

            var errorSpan = document.createElement('SPAN');
            errorSpan.innerHTML = message;
            validMsg.appendChild(errorSpan);
        } else {
            validMsgObj.removeClass(this.fieldErrorClassName);
            validMsgObj.addClass(this.fieldValidClassName);
        }

        this.ShowErrorHighlight(showError, inputFields, errorFieldIndexes)

        return message;
    }

    ShowErrorHighlight(showError, inputFields, errorFieldIndexes) {
        for (var i = 0; i < inputFields.length; i++) {
            var inputElementClassName = this.GetErrorClassForTag($(inputFields[i]).prop("tagName"));

            if (showError && (errorFieldIndexes === null || errorFieldIndexes.includes(i))) {
                $(inputFields[i]).addClass(inputElementClassName);
            } else if (!showError && errorFieldIndexes === null) {
                $(inputFields[i]).removeClass(inputElementClassName);
            }
        }
    }
}
