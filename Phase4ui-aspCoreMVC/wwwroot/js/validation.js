document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('supplierForm').addEventListener('submit', function (event) {
        var isValid = true;

        // Get form elements
        var supplierName = document.getElementById('supplierName');
        var supplierContact = document.getElementById('supplierContact');
        var companyName = document.getElementById('companyName');
        var companyAddress = document.getElementById('companyAddress');
        var supplierEmail = document.getElementById('supplierEmail');
        var supplierPassword = document.getElementById('supplierPassword');

        // Clear previous error messages
        document.getElementById('supplierNameError').textContent = '';
        document.getElementById('supplierContactError').textContent = '';
        document.getElementById('companyNameError').textContent = '';
        document.getElementById('companyAddressError').textContent = '';
        document.getElementById('supplierEmailError').textContent = '';
        document.getElementById('supplierPasswordError').textContent = '';

        // Validate SupplierName
        if (!/^[A-Za-z\s]{1,20}$/.test(supplierName.value)) {
            document.getElementById('supplierNameError').textContent = 'Name should contain only letters and spaces, and be less than 20 characters long.';
            isValid = false;
        }

        // Validate SupplierContact
        if (!/^\d{10}$/.test(supplierContact.value)) {
            document.getElementById('supplierContactError').textContent = 'Phone number must be exactly 10 digits.';
            isValid = false;
        }

        // Validate CompanyName
        if (companyName.value.trim() === '') {
            document.getElementById('companyNameError').textContent = 'Company Name is required.';
            isValid = false;
        }

        // Validate CompanyAddress
        if (companyAddress.value.trim() === '') {
            document.getElementById('companyAddressError').textContent = 'Company Address is required.';
            isValid = false;
        }

        // Validate SupplierEmail
        if (!/^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/.test(supplierEmail.value)) {
            document.getElementById('supplierEmailError').textContent = 'Please enter a valid email address.';
            isValid = false;
        }

        // Validate SupplierPassword
        if (supplierPassword.value.trim() === '') {
            document.getElementById('supplierPasswordError').textContent = 'Password is required.';
            isValid = false;
        }

        // Prevent form submission if validation fails
        if (!isValid) {
            event.preventDefault();
        }
    });
});
