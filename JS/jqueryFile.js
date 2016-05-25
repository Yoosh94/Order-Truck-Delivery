$(function () {
    //set the input with ID="datetimepicker" as a picker.
    $("#datepicker").datepicker({
        dateFormat: "dd-mm-yy"
    });
    $("#timepicker").timepicker({
        controlType: 'select',
        oneLine: true,
        timeFormat: 'H:mm'
    });

    

});