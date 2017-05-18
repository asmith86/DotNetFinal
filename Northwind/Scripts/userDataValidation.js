$(document).ready(function () {

    

    $('#userDataForm').validate({
        rules: {
            firstName: {
                required: true,
                lettersonly: true
                
            },
            lastName: {
                required: true,
                lettersonly: true
            },
            address: {
                required: true
                    
                
                
            },
            city: {
                required: true,
                lettersonly: true
            },
            state: {
                required: true,
                lettersonly: true,
                maxlength: 2
            },
            zip: {
                required: true,
                zipcodeUS: true
            }
        }

    });


    

});