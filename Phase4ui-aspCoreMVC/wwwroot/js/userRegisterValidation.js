

function validateUserForm() {
    var isValid = true;

    // Get form elements
    var userName = document.getElementById('userName');
    var userContact = document.getElementById('userContact');
    var userEmail = document.getElementById('userEmail');
    var userPassword = document.getElementById('userPassword');

    // Clear previous error messages
    document.getElementById('userNameError').textContent = '';
    document.getElementById('userContactError').textContent = '';
    document.getElementById('userEmailError').textContent = '';
    document.getElementById('userPasswordError').textContent = '';

    // Validate UserName
    if (!/^[A-Za-z\s]{1,20}$/.test(userName.value)) {
        document.getElementById('userNameError').textContent = 'Name should contain only letters and spaces, and be less than 20 characters long.';
        isValid = false;
    }

    // Validate UserContact
    if (!/^\d{10}$/.test(userContact.value)) {
        document.getElementById('userContactError').textContent = 'Contact number must be exactly 10 digits.';
        isValid = false;
    }

    // Validate UserEmail
    if (!/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test(userEmail.value)) {
        document.getElementById('userEmailError').textContent = 'Please enter a valid email address.';
        isValid = false;
    }

    // Validate UserPassword
    if (userPassword.value.trim() === '') {
        document.getElementById('userPasswordError').textContent = 'Password is required.';
        isValid = false;
    }

    // Prevent form submission if validation fails
    return isValid;
}
