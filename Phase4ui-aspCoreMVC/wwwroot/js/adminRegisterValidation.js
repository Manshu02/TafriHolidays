

function validateForm() {
    var isValid = true;

    // Get form elements
    var adminName = document.getElementById('adminName');
    var adminContact = document.getElementById('adminContact');
    var adminEmail = document.getElementById('adminEmail');
    var adminPassword = document.getElementById('adminPassword');

    // Clear previous error messages
    document.getElementById('adminNameError').textContent = '';
    document.getElementById('adminContactError').textContent = '';
    document.getElementById('adminEmailError').textContent = '';
    document.getElementById('adminPasswordError').textContent = '';

    // Validate AdminName
    if (!/^[A-Za-z\s]{1,20}$/.test(adminName.value)) {
        document.getElementById('adminNameError').textContent = 'Name should contain only letters and spaces, and be less than 20 characters long.';
        isValid = false;
    }

    // Validate AdminContact
    if (!/^\d{10}$/.test(adminContact.value)) {
        document.getElementById('adminContactError').textContent = 'Phone number must be exactly 10 digits.';
        isValid = false;
    }

    // Validate AdminEmail
    if (!/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test(adminEmail.value)) {
        document.getElementById('adminEmailError').textContent = 'Please enter a valid email address.';
        isValid = false;
    }

    // Validate AdminPassword
    if (adminPassword.value.trim() === '') {
        document.getElementById('adminPasswordError').textContent = 'Password is required.';
        isValid = false;
    }

    // Prevent form submission if validation fails
    return isValid;
}
