// Time picker nasvatveni + spousteci kod

// Spousteci kod
$(document).ready(function () {
    $(".datetimepicker").datetimepicker({
        dateFormat: 'dd.mm.yy ',
        timeFormat: 'hh:mm',
        addSliderAccess: true,
        controlType: 'select',
        sliderAccessArgs: { touchonly: false },
        stepMinute: 5,
        is24HourView: true,
        hourMin: 0,
        hourMax: 24,
        minDate: "+1D",
    });
});

// Nastaveni
(function ($) {
    $.timepicker.regional['cs'] = {
        timeOnlyTitle: 'Vyberte čas',
        timeText: 'Čas',
        hourText: 'Hodiny',
        minuteText: 'Minuty',
        secondText: 'Vteřiny',
        millisecText: 'Milisekundy',
        timezoneText: 'Časové pásmo',
        currentText: 'Nyní',
        closeText: 'Ok',
        timeFormat: 'H:mm',
        amNames: ['dop.', 'AM', 'A'],
        pmNames: ['odp.', 'PM', 'P'],
        ampm: false
    };
    $.timepicker.setDefaults($.timepicker.regional['cs']);
})(jQuery);

function DateTimeNow() {
    var myDate = new Date();

    var prettyDate = (myDate.getMonth() + 1) + '/' + (myDate.getDate() + 1) + '/' + myDate.getFullYear();

    return prettyDate;
}

jQuery(function ($) {
    $.datepicker.regional['cs'] = {
        closeText: 'Ok',
        prevText: '&#x3c;Dříve',
        nextText: 'Později&#x3e;',
        currentText: 'Nyní',
        monthNames: ['leden', 'únor', 'březen', 'duben', 'květen', 'červen',
'červenec', 'srpen', 'září', 'říjen', 'listopad', 'prosinec'],
        monthNamesShort: ['led', 'úno', 'bře', 'dub', 'kvě', 'čer',
        'čvc', 'srp', 'zář', 'říj', 'lis', 'pro'],
        dayNames: ['neděle', 'pondělí', 'úterý', 'středa', 'čtvrtek', 'pátek', 'sobota'],
        dayNamesShort: ['ne', 'po', 'út', 'st', 'čt', 'pá', 'so'],
        dayNamesMin: ['ne', 'po', 'út', 'st', 'čt', 'pá', 'so'],
        weekHeader: 'Týd',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['cs']);
});